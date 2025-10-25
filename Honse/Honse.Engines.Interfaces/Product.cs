
namespace Honse.Engines.Common
{
    public class CreateProduct
    {
        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal VAT { get; set; }

        public string Image { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }
    }
}
