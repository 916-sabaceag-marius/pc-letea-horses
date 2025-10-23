using System.Linq.Expressions;

namespace Honse.Global.Specification;

public static class SpecificationExtension
{
    public static Specification<TEntity> And<TEntity>(this Specification<TEntity> one, Specification<TEntity> other)    
    {
        var expression = CombineTwoExpressions(one.Expr, other.Expr, Expression.AndAlso);
        return new EmptySpecification<TEntity>(expression);
    }

    public static Specification<TEntity> Or<TEntity>(this Specification<TEntity> one, Specification<TEntity> other)    
    {
        var expression = CombineTwoExpressions(one.Expr, other.Expr, Expression.OrElse);

        return new EmptySpecification<TEntity>(expression);
    }

    //TODO fix it
    public static Specification<TEntity> Not<TEntity>(this Specification<TEntity> one) 
    {
        var param = one.Expr.Parameters;
        var body = Expression.Not(one.Expr.Body);

        return new EmptySpecification<TEntity>(Expression.Lambda<Func<TEntity, bool>>(body, param));
    }

    public static Specification<TEntity> CombineListAnd<TEntity>(this List<Specification<TEntity>> specifications)
    {
        if (specifications.Count == 0) return new AnySpecification<TEntity>();

        Specification<TEntity> resultSpecification = new AnySpecification<TEntity>();

        foreach (var specification in specifications)
        {
            resultSpecification = resultSpecification.And(specification);
        }

        return resultSpecification;
    }

    public static Specification<TEntity> CombineListOr<TEntity>(this List<Specification<TEntity>> specifications)
    {
        if (specifications.Count == 0) return new AnySpecification<TEntity>();

        Specification<TEntity> resultSpecification = new NoneSpecification<TEntity>();

        foreach (var specification in specifications)
        {
            resultSpecification = resultSpecification.Or(specification);
        }

        return resultSpecification;
    }

    private static Expression<Func<TEntity, bool>> CombineTwoExpressions<TEntity>(
        Expression<Func<TEntity, bool>> first,
        Expression<Func<TEntity, bool>> second,
        Func<Expression, Expression, Expression> merge)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");

        var firstBody = ReplaceParameter(first.Body, first.Parameters[0], parameter);
        var secondBody = ReplaceParameter(second.Body, second.Parameters[0], parameter);

        var combinedBody = merge(firstBody, secondBody);
        return Expression.Lambda<Func<TEntity, bool>>(combinedBody, parameter);
    }

    private static Expression ReplaceParameter(Expression body, ParameterExpression toReplace, ParameterExpression newParameter)
    {
        return new ParameterReplacer(toReplace, newParameter).Visit(body);
    }
}

internal class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _toReplace;
    private readonly ParameterExpression _replacement;

    public ParameterReplacer(ParameterExpression toReplace, ParameterExpression replacement)
    {
        _toReplace = toReplace;
        _replacement = replacement;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _toReplace ? _replacement : base.VisitParameter(node);
    }
}