using System.ComponentModel.DataAnnotations.Schema;

namespace Honse.Resources.Interfaces.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty; // Snapshot

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; } // Snapshot

        public decimal Subtotal { get; set; }

        // Navigation properties
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
