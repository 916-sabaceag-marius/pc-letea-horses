using Honse.Global.Specification;

namespace Honse.Engines.Filtering.Interfaces
{
    public interface IProductFilteringEngine
    {
        Specification<Resources.Interface.Product> ToSpecification(ProductFilterRequest filter);
    }

    public class ProductFilterRequest
    {
        public Guid UserId { get; set; }
        public string? SearchKey { get; set; }

        public string? CategoryName { get; set; }

        public bool? IsActive { get; set; }
    }
}
