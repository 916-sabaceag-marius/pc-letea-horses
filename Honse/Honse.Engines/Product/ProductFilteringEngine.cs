
using Honse.Engines.Interface;
using Honse.Global;
using Honse.Global.Specification;
using System.Linq.Expressions;

namespace Honse.Engines.Product
{
    public class ProductFilteringEngine : IProductFilteringEngine
    {
        public async Task<Specification<Product>> ToSpecification(ProductFilterRequest filter)
        {
            Specification<Product> specification = new SpecificationProductHasUser(filter.UserId);

            if (filter.SearchKey != null)
                specification.And(new SpecificationProductNameOrDescriptionSearchKey(filter.SearchKey));

            //TODO: Add more filters

            return specification;
        }

        internal class SpecificationProductHasUser : Specification<Product>
        {
            private readonly Guid userId;

            public SpecificationProductHasUser(Guid userId)
            {
                this.userId = userId;
            }

            public override Expression<Func<Product, bool>> Expression =>
                (Product product) => product.UserId == userId;
        }

        internal class SpecificationProductNameOrDescriptionSearchKey : Specification<Product>
        {
            private readonly string searchKey;

            public SpecificationProductNameOrDescriptionSearchKey(string searchKey)
            {
                this.searchKey = searchKey;
            }
            public override Expression<Func<Product, bool>> Expression =>
                (Product product) => product.Name.Contains(searchKey) || product.Description.Contains(searchKey);
        }
    }
}
