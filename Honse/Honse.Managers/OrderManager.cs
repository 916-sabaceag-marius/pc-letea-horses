using Honse.Managers.Interfaces;
using Honse.Resources.Interfaces;
using Honse.Resources.Interfaces.Entities;
using Honse.Global.Extensions;
using Honse.Global;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Honse.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly IOrderResource orderResource;
        private readonly Engines.Filtering.Interfaces.IOrderFilteringEngine orderFilteringEngine;
        private readonly Resources.Interfaces.IRestaurantResource restaurantResource;
        private readonly Resources.Interfaces.IProductResource productResource;
        private readonly Resources.Interfaces.IOrderConfirmationTokenResource orderConfirmationTokenResource;
        private readonly IEmailSender emailSender;

        public OrderManager(
            IOrderResource orderResource,
            Engines.Filtering.Interfaces.IOrderFilteringEngine orderFilteringEngine,
            Resources.Interfaces.IRestaurantResource restaurantResource,
            Resources.Interfaces.IProductResource productResource,
            Resources.Interfaces.IOrderConfirmationTokenResource orderConfirmationTokenResource,
            IEmailSender emailSender)
        {
            this.orderResource = orderResource;
            this.orderFilteringEngine = orderFilteringEngine;
            this.restaurantResource = restaurantResource;
            this.productResource = productResource;
            this.orderConfirmationTokenResource = orderConfirmationTokenResource;
            this.emailSender = emailSender;
        }

        public async Task<Order?> GetOrderById(Guid id, Guid? userId)
        {
            var order = userId != null ? await orderResource.GetById(id, userId.Value) : await orderResource.GetByIdPublic(id);
            return order;
        }

        //TODO: Move to OrderProcessorEngine
        public async Task<Order> ProcessOrder(OrderProcessRequest request)
        {
            var order = await orderResource.GetById(request.Id, request.UserId)
                ?? throw new InvalidOperationException("Order not found");

            // Verify the order belongs to the restaurant
            if (order.RestaurantId != request.RestaurantId)
                throw new UnauthorizedAccessException("Order does not belong to this restaurant");

            // Update status
            var history = System.Text.Json.JsonSerializer.Deserialize<List<Global.Order.OrderStatusHistoryEntry>>(order.StatusHistory) 
                ?? new List<Global.Order.OrderStatusHistoryEntry>();
            
            history.Add(new Global.Order.OrderStatusHistoryEntry
            {
                Status = request.NewStatus,
                Timestamp = DateTime.UtcNow,
                Notes = request.StatusNotes
            });

            order.OrderStatus = request.NewStatus;
            order.StatusHistory = System.Text.Json.JsonSerializer.Serialize(history);

            // Update preparation time when status becomes Accepted
            if (request.NewStatus == Global.Order.OrderStatus.Accepted && !order.PreparationTime.HasValue)
            {
                order.PreparationTime = DateTime.UtcNow;
            }

            // Update delivery time when status becomes Finished
            if (request.NewStatus == Global.Order.OrderStatus.Finished && !order.DeliveryTime.HasValue)
            {
                order.DeliveryTime = DateTime.UtcNow;
            }

            return (await orderResource.Update(order.Id, order.UserId, order))!;
        }

        /// <summary>
        /// This function is used for both the client and the company endpoints
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// 
        public async Task CancelOrder(Guid id, Guid? userId)
        {
            Order order = (userId != null ? await orderResource.GetById(id, userId.Value) : await orderResource.GetByIdPublic(id))
                ?? throw new InvalidOperationException("Order not found");

            // Check if order can be cancelled (only if not finished or already cancelled)
            if (order.OrderStatus == Global.Order.OrderStatus.Finished ||
                order.OrderStatus == Global.Order.OrderStatus.Cancelled)
            {
                throw new InvalidOperationException($"Cannot cancel order with status: {order.OrderStatus}");
            }

            // Mark as cancelled
            var history = System.Text.Json.JsonSerializer.Deserialize<List<Global.Order.OrderStatusHistoryEntry>>(order.StatusHistory) 
                ?? new List<Global.Order.OrderStatusHistoryEntry>();
            
            history.Add(new Global.Order.OrderStatusHistoryEntry
            {
                Status = Global.Order.OrderStatus.Cancelled,
                Timestamp = DateTime.UtcNow,
                Notes = "Order cancelled by customer"
            });

            order.OrderStatus = Global.Order.OrderStatus.Cancelled;
            order.StatusHistory = System.Text.Json.JsonSerializer.Serialize(history);

            await orderResource.Update(order.Id, order.UserId, order);
        }

        public async Task<List<Order>> GetAllOrdersByRestaurant(Guid restaurantId, Guid userId)
        {
            var orders = await orderResource.GetByRestaurantId(restaurantId);
            return orders.ToList();
        }

        public async Task<Global.PaginatedResult<Order>> FilterOrders(OrderFilterRequest request)
        {
            var specification = orderFilteringEngine.GetSpecification(request.DeepCopyTo<Engines.Filtering.Interfaces.OrderFilterRequest>());

            return await orderResource.Filter(specification, request.PageSize, request.PageNumber);
        }

        public async Task<ValidationResult> ValidateOrder(PlaceOrderRequest request)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate restaurant exists and is enabled
            var restaurant = await restaurantResource.GetByIdPublic(request.RestaurantId);
            if (restaurant == null)
            {
                result.IsValid = false;
                result.Errors.Add("Restaurant not found");
                return result;
            }

            if (!restaurant.IsEnabled)
            {
                result.IsValid = false;
                result.Errors.Add("Restaurant is currently unavailable");
                return result;
            }

            // Validate restaurant schedule (check if currently open)
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            if (currentTime < restaurant.OpeningTime || currentTime > restaurant.ClosingTime)
            {
                result.IsValid = false;
                result.Errors.Add($"Restaurant is closed. Open hours: {restaurant.OpeningTime:HH:mm} - {restaurant.ClosingTime:HH:mm}");
                return result;
            }

            // Validate products
            if (request.Products == null || !request.Products.Any())
            {
                result.IsValid = false;
                result.Errors.Add("Order must contain at least one product");
                return result;
            }

            foreach (var orderProduct in request.Products)
            {
                var product = await productResource.GetById(orderProduct.ProductId, restaurant.UserId);
                if (product == null)
                {
                    result.IsValid = false;
                    result.Errors.Add($"Product with ID {orderProduct.ProductId} not found");
                    continue;
                }

                if (!product.IsEnabled)
                {
                    result.IsValid = false;
                    result.Errors.Add($"Product '{product.Name}' is currently unavailable");
                    continue;
                }

                // Verify product belongs to the restaurant (through userId)
                if (product.UserId != restaurant.UserId)
                {
                    result.IsValid = false;
                    result.Errors.Add($"Product '{product.Name}' does not belong to this restaurant");
                }
            }

            return result;
        }

        public async Task<Guid> PlaceOrder(PlaceOrderRequest request)
        {
            // First, validate the order
            var validation = await ValidateOrder(request);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException($"Order validation failed: {string.Join(", ", validation.Errors)}");
            }

            // Get restaurant for userId
            var restaurant = await restaurantResource.GetByIdPublic(request.RestaurantId);
            if (restaurant == null)
                throw new InvalidOperationException("Restaurant not found");

            // Create OrderConfirmationToken
            var token = new Resources.Interfaces.Entities.OrderConfirmationToken
            {
                Id = Guid.NewGuid(),
                UserId = restaurant.UserId,
                RestaurantId = request.RestaurantId,
                ClientName = request.CustomerName,
                ClientEmail = request.CustomerEmail,
                DeliveryAddress = request.DeliveryAddress,
                Products = request.Products.Select(p => new Global.Order.OrderProduct
                {
                    Name = p.Name,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    VAT = p.VAT,
                    Total = p.Total,
                    Image = p.Image
                }).ToList(),
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                Used = false
            };

            await orderConfirmationTokenResource.Add(token);

            // Send confirmation email
            var frontendLink = $"https://localhost:2000/{token.Id}";
            var backendLink = $"https://localhost:2000/api/public/orders/confirm/{token.Id}";
            
            var emailBody = $@"
                <h2>Order Confirmation</h2>
                <p>Dear {request.CustomerName},</p>
                <p>Thank you for your order! Please click the link below to confirm your order:</p>
                <p><a href=""{frontendLink}"">Confirm Order</a></p>
                <p>This link will expire in 24 hours.</p>
                <p>Best regards,<br/>Honse Team</p>
            ";

            await emailSender.SendEmailAsync(request.CustomerEmail, "Confirm Your Order", emailBody);

            return token.Id;
        }

        public async Task<Order> ConfirmOrder(Guid tokenId)
        {
            // Retrieve the token - we need to get it without userId since it's public
            var token = await orderConfirmationTokenResource.GetByIdPublic(tokenId);
            if (token == null)
            {
                throw new InvalidOperationException("Confirmation token not found");
            }

            // Check if token is already used
            if (token.Used)
            {
                throw new InvalidOperationException("This confirmation link has already been used");
            }

            // Check if token has expired
            if (token.ExpiresAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException("This confirmation link has expired");
            }

            // Create the order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = token.UserId,
                RestaurantId = token.RestaurantId,
                ClientName = token.ClientName,
                ClientEmail = token.ClientEmail,
                DeliveryAddress = System.Text.Json.JsonSerializer.Serialize(token.DeliveryAddress),
                Products = System.Text.Json.JsonSerializer.Serialize(token.Products),
                OrderStatus = Global.Order.OrderStatus.New,
                StatusHistory = System.Text.Json.JsonSerializer.Serialize(new List<Global.Order.OrderStatusHistoryEntry>
                {
                    new Global.Order.OrderStatusHistoryEntry
                    {
                        Status = Global.Order.OrderStatus.New,
                        Timestamp = DateTime.UtcNow,
                        Notes = "Order confirmed by customer"
                    }
                }),
                Timestamp = DateTime.UtcNow,
                Total = token.Products.Sum(p => p.Total),
                OrderNo = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}"
            };

            await orderResource.Add(order);

            // Mark token as used
            token.Used = true;
            await orderConfirmationTokenResource.Update(token.Id, token.UserId, token);

            return order;
        }
    }
}
