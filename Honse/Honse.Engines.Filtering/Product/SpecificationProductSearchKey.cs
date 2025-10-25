using Honse.Global.Specification;
using Honse.Resources.Interfaces;
using System.Linq.Expressions;

namespace Honse.Engines.Filtering.Product
{
    internal class SpecificationProductSearchKey : Specification<Resources.Interfaces.Entities.Product>
    {
        private readonly string searchKey;

        public SpecificationProductSearchKey(string searchKey)
        {
            this.searchKey = searchKey;
        }
        public override Expression<Func<Resources.Interfaces.Entities.Product, bool>> Expression =>
            (Resources.Interfaces.Entities.Product product) => product.Name.Contains(searchKey) || product.Description.Contains(searchKey);
    }
}
