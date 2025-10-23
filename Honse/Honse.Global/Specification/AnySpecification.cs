using System.Linq.Expressions;

namespace Honse.Global.Specification;

public class AnySpecification<TEntity> : Specification<TEntity>
{
    public override Expression<Func<TEntity, bool>> Expr
        => entity => true;
}