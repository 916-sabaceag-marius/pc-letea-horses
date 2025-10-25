using Honse.Global.Specification;
using System.Linq.Expressions;

namespace Honse.Engines.Filtering.Product
{
    internal class SpecificationProductSearchKey : Specification<Common.Product>
    {
        private readonly string searchKey;

        public SpecificationProductSearchKey(string searchKey)
        {
            this.searchKey = searchKey;
        }
        public override Expression<Func<Common.Product, bool>> Expression =>
            (Common.Product product) => product.Name.Contains(searchKey) || product.Description.Contains(searchKey);
    }
}
