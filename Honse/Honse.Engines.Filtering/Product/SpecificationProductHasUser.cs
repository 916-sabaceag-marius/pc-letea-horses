
using Honse.Global.Specification;
using System.Linq.Expressions;

namespace Honse.Engines.Filtering.Product
{
    internal class SpecificationProductHasUser : Specification<Common.Product>
    {
        private readonly Guid userId;

        public SpecificationProductHasUser(Guid userId)
        {
            this.userId = userId;
        }

        public override Expression<Func<Common.Product, bool>> Expression =>
            (Common.Product product) => product.UserId == userId;
    }
}