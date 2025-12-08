using Honse.Global;

namespace Honse.Managers.Interfaces
{
    public interface IOrderManager
    {
        Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request);
    }

    public class OrderProcessRequest
    {
        public Guid OrderId { get; set; }

        public Guid RestaurantId { get; set; }

        public Guid UserId { get; set; }

        public Global.Order.OrderStatus OrderStatus { get; set; }

        public string Data { get; set; } = string.Empty; // This is a Serialized object, that will be different, depending on NextStatus
    }

    public class PlaceOrderRequest
    {
        public Guid RestaurantId { get; set; }

        public string CustomerEmail { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerPhone { get; set; } = string.Empty;

        public Address DeliveryAddress { get; set; } = new Address();

        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class PlaceOrderResponse
    {
        public Guid OrderId { get; set; }

        public Guid ConfirmationToken { get; set; }

        public string Message { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
    }
}
