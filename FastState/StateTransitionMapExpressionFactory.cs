using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FastState
{
    internal static class StateTransitionMapExpressionFactory<TState, TInput>
    {
        internal static BlockExpression BuildTryGetValueExpression(IStateTransitionMap<TState, TInput> map, ParameterExpression inputParam, ParameterExpression outNewStateParam, LabelTarget returnTarget)
        {
            var transitionExpressions = new List<Expression>();

            transitionExpressions.AddRange(BuildConstantChecks(map, inputParam, outNewStateParam, returnTarget));
            transitionExpressions.AddRange(BuildDelegateChecks(map, inputParam, outNewStateParam, returnTarget));
            transitionExpressions.Add(BuildGetDefaultExpression(map, outNewStateParam, returnTarget));

            return Expression.Block(transitionExpressions);
        }

        private static IEnumerable<Expression> BuildDelegateChecks(IStateTransitionMap<TState, TInput> map, ParameterExpression inputParam, ParameterExpression outNewStateParam, LabelTarget returnTarget)
        {
            return map
                .Where(t => t.Condition is Expression<Func<TInput, bool>>)
                .Select(t =>
                    Expression.IfThen(
                        Expression.Invoke(t.Condition, inputParam),
                        Expression.Block(
                            Expression.Assign(outNewStateParam, Expression.Constant(t.NewState)),
                            Expression.Return(returnTarget, Expression.Constant(true))
                        )
                    )
                );
        }

        private static IEnumerable<Expression> BuildConstantChecks(IStateTransitionMap<TState, TInput> map, ParameterExpression inputParam, ParameterExpression outNewStateParam, LabelTarget returnTarget)
        {
            var constants = map.Where(t => t.Condition.NodeType == ExpressionType.Constant);
            if (constants.Any())
            {
                bool inputIsTuple = TupleHelpers.IsTuple(inputParam.Type);

                SwitchCase[] switchCases = constants.Select(t => {
                     var condition = t.Condition;

                     if (inputIsTuple)
                     {
                         ConstantExpression constantExpression = (ConstantExpression)condition;
                         condition = Expression.Constant(new ComparisonWrapper<TInput>((TInput)constantExpression.Value));
                     }

                     return Expression.SwitchCase(
                         Expression.Block(
                             Expression.Assign(outNewStateParam, Expression.Constant(t.NewState)),
                             Expression.Return(returnTarget, Expression.Constant(true))
                         ),
                         condition
                     );
                 }).ToArray();

                Expression input = inputIsTuple
                    ? ExpressionHelpers.CreateNewComparisonWrapperExpression<TInput>(inputParam)
                    : inputParam;

                yield return Expression.Switch(input, switchCases);
            }
        }

        internal static BlockExpression BuildGetDefaultExpression(IStateTransitionMap<TState, TInput> map, ParameterExpression outNewStateParam, LabelTarget returnTarget)
        {
            BinaryExpression assignExp;
            GotoExpression returnExp;
            if (map.HasDefaultTransitionState)
            {
                assignExp = Expression.Assign(outNewStateParam, Expression.Constant(map.DefaultTransitionState));
                returnExp = Expression.Return(returnTarget, Expression.Constant(true));
            }
            else
            {
                assignExp = Expression.Assign(outNewStateParam, Expression.Constant(default(TState), typeof(TState)));
                returnExp = Expression.Return(returnTarget, Expression.Constant(false));
            }

            return Expression.Block(
                assignExp,
                returnExp
            );
        }
    }
}
