using System.ComponentModel.DataAnnotations.Schema;

namespace Honse.Resources.Interface
{
    public interface IProductResource : IResource<Product>
    {

    }

    public class Product : Entity
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal VAT { get; set; }

        public string Image { get; set; } = string.Empty;

        //[ForeignKey("Category")]
        public Guid CategoryId { get; set; }
    }
}
