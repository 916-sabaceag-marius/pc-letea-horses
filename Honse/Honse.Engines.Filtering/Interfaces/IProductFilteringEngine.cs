using Honse.Global.Specification;

namespace Honse.Engines.Filtering.Interfaces
{
    public interface IProductFilteringEngine
    {
        Specification<Resources.Interfaces.Entities.Product> GetSpecification(ProductFilterRequest filter);
    }

    public class ProductFilterRequest
    {
        public Guid UserId { get; set; }
        public string? SearchKey { get; set; }

        public string? CategoryName { get; set; }

        public bool? IsActive { get; set; }
    }
}
