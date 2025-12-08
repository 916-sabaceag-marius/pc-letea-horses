using System.ComponentModel.DataAnnotations;

namespace Honse.Engines.Common
{
    public class PlaceOrder
    {
        public Guid RestaurantId { get; set; }

        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerPhone { get; set; } = string.Empty;

        public Global.Address DeliveryAddress { get; set; } = new Global.Address();

        public List<OrderItemData> Items { get; set; } = new List<OrderItemData>();
    }

    public class OrderItemData
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
