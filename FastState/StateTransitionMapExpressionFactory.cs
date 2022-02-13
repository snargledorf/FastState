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
            var constants = map
                .Where(t => t.Condition.NodeType == ExpressionType.Constant)
                .Select(t => {
                    var condition = t.Condition;;

                    return Expression.SwitchCase(
                        Expression.Block(
                            Expression.Assign(outNewStateParam, Expression.Constant(t.NewState)),
                            Expression.Return(returnTarget, Expression.Constant(true))
                        ),
                        condition
                    );
                }).ToArray();

            if (constants.Length > 0)
            {
                Expression input = inputParam;
                if (TupleHelpers.IsTuple(input.Type))
                    input = Expression.New(typeof(ComparisonWrapper<TInput>).GetConstructor(new[] { typeof(TInput) }), inputParam);
                yield return Expression.Switch(input, constants);
            }
            /*
// Check tuple constants
var constantTupleConditions = constants.Where(t => TupleHelpers.IsTuple(t.Condition.Type))
   .Select(t =>
       Expression.IfThen(
           TupleHelpers.BuildTupleCheckExpression<TInput>(inputParam, t.Condition, (input, tupleConstant) => EqualityComparer<TInput>.Default.Equals(input, tupleConstant)),
           Expression.Block(
               Expression.Assign(outNewStateParam, Expression.Constant(t.NewState)),
               Expression.Return(returnTarget, Expression.Constant(true))
           )
       )
   );

foreach (Expression expression in constantTupleConditions)
   yield return expression;*/


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
