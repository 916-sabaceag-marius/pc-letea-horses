using System.ComponentModel.DataAnnotations.Schema;

namespace Honse.Resources.Interfaces.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        [ForeignKey("Restaurant")]
        public Guid RestaurantId { get; set; }

        public string CustomerEmail { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerPhone { get; set; } = string.Empty;

        public string DeliveryAddress { get; set; } = string.Empty; // JSON format

        public Global.Order.OrderStatus OrderStatus { get; set; } = Global.Order.OrderStatus.Unconfirmed;

        public decimal TotalAmount { get; set; }

        public Guid ConfirmationToken { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Restaurant? Restaurant { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
