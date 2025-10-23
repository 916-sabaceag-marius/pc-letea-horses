using System.Linq.Expressions;

namespace Honse.Global.Specification;

public abstract class Specification<TEntity>
{
    public abstract Expression<Func<TEntity, bool>> Expr { get; }

}