
using Honse.Global.Specification;
using System.Linq.Expressions;

namespace Honse.Engines.Filtering.Product
{
    internal class SpecificationProductHasUser : Specification<Resources.Interfaces.Entities.Product>
    {
        private readonly Guid userId;

        public SpecificationProductHasUser(Guid userId)
        {
            this.userId = userId;
        }

        public override Expression<Func<Resources.Interfaces.Entities.Product, bool>> Expression =>
            (Resources.Interfaces.Entities.Product product) => product.UserId == userId;
    }
}