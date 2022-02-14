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

            bool stateIsTuple = TupleHelpers.IsTuple(stateParam.Type);

            var currentStateFlowControlExpressions = new List<Expression>();

            if (map.Any())
            {
                if (stateIsTuple)
                {
                    IEnumerable<Expression> ifStatements = map.Select(tm =>
                        Expression.IfThen(
                            ExpressionHelpers.BuildEqualityCheckExpression<TState>(stateParam, Expression.Constant(tm.State)),
                            StateTransitionMapExpressionFactory<TState, TInput>.BuildTryGetValueExpression(tm, inputParam, outNewStateParam, returnTarget)
                        )
                    );

                    currentStateFlowControlExpressions.AddRange(ifStatements);
                }
                else
                {
                    var switchCaseExpressions = map
                        .Select(tm =>
                            Expression.SwitchCase(
                                StateTransitionMapExpressionFactory<TState, TInput>.BuildTryGetValueExpression(tm, inputParam, outNewStateParam, returnTarget),
                                Expression.Constant(tm.State)
                            )
                        )
                        .ToArray();

                    currentStateFlowControlExpressions.Add(Expression.Switch(stateParam, switchCaseExpressions));
                }
            }

            currentStateFlowControlExpressions.Add(Expression.Throw(Expression.Constant(new ArgumentException("Invalid state"))));
            currentStateFlowControlExpressions.Add(Expression.Label(returnTarget, Expression.Constant(false)));
            
            BlockExpression body = Expression.Block(typeof(bool), currentStateFlowControlExpressions);

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
                            ExpressionHelpers.BuildEqualityCheckExpression<TState>(stateParam, Expression.Constant(tm.State)),
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
