using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FastState
{
    internal static class StateMachineTransitionMapExpressionFactory<TState, TInput>
    {
        public static Expression<TryTransitionDelegate<TState, TInput>> BuildTryTransitionExpression(IStateMachineTransitionMap<TState, TInput> map)
        {
            ParameterExpression stateParam = Expression.Parameter(typeof(TState));
            ParameterExpression inputParam = Expression.Parameter(typeof(TInput));
            ParameterExpression outNewStateParam = Expression.Parameter(typeof(TState).MakeByRefType());

            LabelTarget returnTarget = Expression.Label(typeof(bool));

            bool isTuple = TupleHelpers.IsTuple(stateParam.Type);

            var currentStateDecisionExpressions = new List<Expression>();

            if (map.Any())
            {
                var switchCaseExpressions = map
                    .Select(tm =>
                    {
                        ConstantExpression constantExpression = isTuple
                            ? Expression.Constant(new ComparisonWrapper<TState>(tm.State))
                            : Expression.Constant(tm.State);

                        return Expression.SwitchCase(
                            StateTransitionMapExpressionFactory<TState, TInput>.BuildTryGetValueExpression(tm, inputParam, outNewStateParam, returnTarget),
                            constantExpression);
                    })
                    .ToArray();

                Expression switchValue = isTuple
                    ? ExpressionHelpers.CreateNewComparisonWrapperExpression<TState>(stateParam)
                    : stateParam;

                currentStateDecisionExpressions.Add(Expression.Switch(switchValue, switchCaseExpressions));
            }

            currentStateDecisionExpressions.Add(Expression.Throw(Expression.Constant(new ArgumentException("Invalid state"))));
            currentStateDecisionExpressions.Add(Expression.Label(returnTarget, Expression.Constant(false)));
            
            BlockExpression body = Expression.Block(typeof(bool), currentStateDecisionExpressions);

            return Expression.Lambda<TryTransitionDelegate<TState, TInput>>(body, stateParam, inputParam, outNewStateParam);
        }

        public static Expression<TryGetDefaultDelegate<TState, TInput>> BuildTryGetDefaultExpression(IStateMachineTransitionMap<TState, TInput> map)
        {
            ParameterExpression stateParam = Expression.Parameter(typeof(TState));
            ParameterExpression outNewStateParam = Expression.Parameter(typeof(TState).MakeByRefType());
            LabelTarget returnTarget = Expression.Label(typeof(bool));

            var currentStateDecisionExpressions = new List<Expression>();

            if (TupleHelpers.IsTuple<TState>())
            {
                currentStateDecisionExpressions.AddRange(
                    map.Select(tm =>
                        Expression.IfThen(
                            ExpressionHelpers.BuildEqualityCheckExpression<TState>(stateParam, Expression.Constant(tm.State, typeof(TState)), (input, tupleConstant)
                                => EqualityComparer<TState>.Default.Equals(input, tupleConstant)),
                            StateTransitionMapExpressionFactory<TState, TInput>.BuildGetDefaultExpression(tm, outNewStateParam, returnTarget)
                        )
                    )
                );
            }
            else
            {
                var switchCaseExpressions = map
                    .Select(tm =>
                        Expression.SwitchCase(
                            StateTransitionMapExpressionFactory<TState, TInput>.BuildGetDefaultExpression(tm, outNewStateParam, returnTarget),
                            Expression.Constant(tm.State)
                        )
                    )
                    .ToArray();

                currentStateDecisionExpressions.Add(Expression.Switch(stateParam, switchCaseExpressions));
            }

            currentStateDecisionExpressions.Add(Expression.Throw(Expression.Constant(new ArgumentException("Invalid state"))));
            currentStateDecisionExpressions.Add(Expression.Label(returnTarget, Expression.Constant(false)));

            BlockExpression body = Expression.Block(typeof(bool), currentStateDecisionExpressions);

            return Expression.Lambda<TryGetDefaultDelegate<TState, TInput>>(body, stateParam, outNewStateParam);
        }
    }
}
