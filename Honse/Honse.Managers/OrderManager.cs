using Honse.Global.Extensions;
using Honse.Managers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Honse.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly Resources.Interfaces.IOrderResource orderResource;
        private readonly Resources.Interfaces.IRestaurantResource restaurantResource;
        private readonly Resources.Interfaces.IProductResource productResource;
        private readonly Engines.Validation.Interfaces.IOrderValidationEngine orderValidationEngine;

        public OrderManager(
            Resources.Interfaces.IOrderResource orderResource,
            Resources.Interfaces.IRestaurantResource restaurantResource,
            Resources.Interfaces.IProductResource productResource,
            Engines.Validation.Interfaces.IOrderValidationEngine orderValidationEngine)
        {
            this.orderResource = orderResource;
            this.restaurantResource = restaurantResource;
            this.productResource = productResource;
            this.orderValidationEngine = orderValidationEngine;
        }

        public async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request)
        {
            // 1. Validate the request
            orderValidationEngine.ValidatePlaceOrder(request.DeepCopyTo<Engines.Common.PlaceOrder>());

            // 2. Validate restaurant exists and is available (enabled + open)
            var restaurant = await restaurantResource.GetByIdPublic(request.RestaurantId);
            if (restaurant == null)
                throw new ValidationException("Restaurant not found!");

            // Validate restaurant availability
            if (!restaurant.IsEnabled)
                throw new ValidationException("Restaurant is currently disabled!");

            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            bool isOpen = currentTime >= restaurant.OpeningTime && currentTime <= restaurant.ClosingTime;

            if (!isOpen)
                throw new ValidationException($"Restaurant is currently closed. Opens at {restaurant.OpeningTime} and closes at {restaurant.ClosingTime}.");

            // 3. Validate products and calculate total
            decimal totalAmount = 0;
            var orderItems = new List<Resources.Interfaces.Entities.OrderItem>();

            foreach (var item in request.Items)
            {
                var product = await productResource.GetProductByIdPublic(item.ProductId);
                if (product == null)
                    throw new ValidationException($"Product with ID {item.ProductId} not found!");

                // Validate product belongs to this restaurant
                if (product.Category.RestaurantId != request.RestaurantId)
                    throw new ValidationException($"Product '{product.Name}' does not belong to the selected restaurant!");

                // Validate product is available
                if (!product.IsEnabled)
                    throw new ValidationException($"Product '{product.Name}' is currently unavailable!");

                // Calculate subtotal
                decimal subtotal = product.Price * item.Quantity;
                totalAmount += subtotal;

                // Create order item
                orderItems.Add(new Resources.Interfaces.Entities.OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    Subtotal = subtotal
                });
            }

            // 4. Create the order
            var order = new Resources.Interfaces.Entities.Order
            {
                Id = Guid.NewGuid(),
                RestaurantId = request.RestaurantId,
                CustomerEmail = request.CustomerEmail,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                DeliveryAddress = System.Text.Json.JsonSerializer.Serialize(request.DeliveryAddress),
                OrderStatus = Global.Order.OrderStatus.Unconfirmed,
                TotalAmount = totalAmount,
                ConfirmationToken = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            // 5. Save to database
            await orderResource.Add(order);

            // 6. Return response
            return new PlaceOrderResponse
            {
                OrderId = order.Id,
                ConfirmationToken = order.ConfirmationToken,
                TotalAmount = totalAmount,
                Message = "Order placed successfully! Please check your email for confirmation."
            };
        }
    }
}
