using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Store.Common.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if(first == null)
                return Expression.Lambda<Func<T, bool>>(second.Body, second.Parameters[0]);

            if(second == null)
                return Expression.Lambda<Func<T, bool>>(first.Body, first.Parameters[0]);

            ParameterExpression parameter = first.Parameters[0];

            SubstituteExpressionVisitor visitor = new SubstituteExpressionVisitor
            {
                Substitute = {[second.Parameters[0]] = parameter}
            };

            Expression body = Expression.AndAlso(first.Body, visitor.Visit(second.Body));

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
                return Expression.Lambda<Func<T, bool>>(second.Body, second.Parameters[0]);

            if (second == null)
                return Expression.Lambda<Func<T, bool>>(first.Body, first.Parameters[0]);

            ParameterExpression parameter = first.Parameters[0];

            SubstituteExpressionVisitor visitor = new SubstituteExpressionVisitor
            {
                Substitute = {[second.Parameters[0]] = parameter}
            };

            Expression body = Expression.OrElse(first.Body, visitor.Visit(second.Body));

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }

    internal class SubstituteExpressionVisitor : ExpressionVisitor
    {
        public Dictionary<Expression, Expression> Substitute = new Dictionary<Expression, Expression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return Substitute.TryGetValue(node, out Expression newValue) ? newValue : node;
        }
    }
}