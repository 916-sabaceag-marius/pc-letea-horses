using System.Linq.Expressions;

namespace Honse.Global.Specification;

public sealed class EmptySpecification<TEntity> : Specification<TEntity>
{
    public EmptySpecification(Expression<Func<TEntity,bool>> expr)
    {
        Expression = expr;
    }

    public override Expression<Func<TEntity, bool>> Expression { get; }
}