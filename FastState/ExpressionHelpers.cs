using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FastState
{
    internal static class ExpressionHelpers
    {
        public static InvocationExpression BuildEqualityCheckExpression<T>(Expression left, Expression right) 
            => BuildEqualityCheckExpression<T>(left, right, (leftInput, rightInput) => EqualityComparer<T>.Default.Equals(leftInput, rightInput));

        public static InvocationExpression BuildEqualityCheckExpression<T>(Expression left, Expression right, Expression<Func<T, T, bool>> equalityCheck)
            => Expression.Invoke(equalityCheck, left, right);
    }
}
