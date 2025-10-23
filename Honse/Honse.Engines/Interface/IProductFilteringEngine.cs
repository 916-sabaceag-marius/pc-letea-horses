using Honse.Engines.Product;
using Honse.Global;
using Honse.Global.Specification;

namespace Honse.Engines.Interface
{
    public interface IProductFilteringEngine
    {
        Task<Specification<Product.Product>> ToSpecification(ProductFilterRequest filter);
    }

    public class ProductFilterRequest
    {
        public Guid UserId { get; set; }
        public string? SearchKey { get; set; }

        public string? CategoryName { get; set; }

        public bool? IsActive { get; set; }
    }
}
