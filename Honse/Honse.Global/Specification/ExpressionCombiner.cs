using System.Linq.Expressions;

namespace Honse.Global.Specification;

public static class ExpressionCombiner
{
    public static Expression<Func<TEntity, bool>> CombineAnd<TEntity>(List<Expression<Func<TEntity, bool>>> expressions)
    {
        if (expressions == null || expressions.Count == 0)
        {
            throw new ArgumentException("The list of expressions cannot be null or empty.");
        }

        // Start with the first expression
        var combinedExpression = expressions[0];

        // Combine each expression using AndAlso
        foreach (var expression in expressions.Skip(1))
        {
            combinedExpression = CombineTwoExpressions(combinedExpression, expression);
        }

        return combinedExpression;
    }

    private static Expression<Func<TEntity, bool>> CombineTwoExpressions<TEntity>(
        Expression<Func<TEntity, bool>> first,
        Expression<Func<TEntity, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");

        var firstBody = ReplaceParameter(first.Body, first.Parameters[0], parameter);
        var secondBody = ReplaceParameter(second.Body, second.Parameters[0], parameter);

        var combinedBody = Expression.OrElse(firstBody, secondBody);
        return Expression.Lambda<Func<TEntity, bool>>(combinedBody, parameter);
    }

    private static Expression ReplaceParameter(Expression body, ParameterExpression toReplace, ParameterExpression newParameter)
    {
        return new ParameterReplacer(toReplace, newParameter).Visit(body);
    }

    private class ParameterReplacer : ExpressionVisitor
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
}
