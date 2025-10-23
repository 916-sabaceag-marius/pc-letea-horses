
namespace Honse.Managers.Interface
{
    public interface IProductManager
    {

    }

    public class Product
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal VAT { get; set; }

        public string Image { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
    }
}
