
using Honse.Engines.Filtering.Interfaces;
using Honse.Global.Specification;

namespace Honse.Engines.Filtering.Product
{
    public class ProductFilteringEngine : IProductFilteringEngine
    {
        public Specification<Resources.Interfaces.Entities.Product> GetSpecification(ProductFilterRequest filter)
        {
            Specification<Resources.Interfaces.Entities.Product> specification = new SpecificationProductHasUser(filter.UserId);

            if (filter.SearchKey != null)
                specification.And(new SpecificationProductSearchKey(filter.SearchKey));

            //TODO: Add more filters

            return specification;
        }
    }
}
