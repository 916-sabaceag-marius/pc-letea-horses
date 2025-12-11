using Honse.Managers.Interfaces;
using Honse.Resources.Interfaces;
using Honse.Resources.Interfaces.Entities;
using Honse.Global.Extensions;
using Honse.Engines.Processing.Interfaces;

namespace Honse.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly IOrderResource orderResource;
        private readonly Engines.Filtering.Interfaces.IOrderFilteringEngine orderFilteringEngine;
        private readonly IOrderProcessorEngine orderProcessorEngine;
        private readonly Resources.Interfaces.IRestaurantResource restaurantResource;
        private readonly Resources.Interfaces.IProductResource productResource;

        public OrderManager(
            IOrderResource orderResource,
            Engines.Filtering.Interfaces.IOrderFilteringEngine orderFilteringEngine,
            Engines.Processing.Interfaces.IOrderProcessorEngine orderProcessorEngine,
            Resources.Interfaces.IRestaurantResource restaurantResource,
            Resources.Interfaces.IProductResource productResource)
        {
            this.orderResource = orderResource;
            this.orderFilteringEngine = orderFilteringEngine;
            this.orderProcessorEngine = orderProcessorEngine;
            this.restaurantResource = restaurantResource;
            this.productResource = productResource;
        }

        public async Task<Order?> GetOrderById(Guid id, Guid? userId)
        {
            var order = userId != null ? await orderResource.GetById(id, userId.Value) : await orderResource.GetByIdPublic(id);
            return order;
        }

        public async Task<Order> ProcessOrder(OrderProcessRequest request)
        {
            var order = await orderResource.GetById(request.Id, request.UserId)
                ?? throw new InvalidOperationException("Order not found");

            // Verify the order belongs to the restaurant
            if (order.RestaurantId != request.RestaurantId)
                throw new UnauthorizedAccessException("Order does not belong to this restaurant");

            order = orderProcessorEngine.ProcessOrder(order.DeepCopyTo<Global.Order.Order>(), request.NextStatus, request.PreparationTimeMinutes, request.StatusNotes).DeepCopyTo<Order>();

            return (await orderResource.Update(order.Id, order.UserId, order))!;

            //SignalR
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

            order = orderProcessorEngine.CancelOrder(order.DeepCopyTo<Global.Order.Order>()).DeepCopyTo<Order>();

            await orderResource.Update(order.Id, order.UserId, order);

            // SignalR
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
    }
}
