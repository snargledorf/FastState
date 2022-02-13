using System;
using System.Linq.Expressions;

namespace FastState
{
    public interface IStateTransitionMapBuilder<TState, TInput>
    {
        TState State { get; }

        IStateMachineTransitionMapBuilder<TState, TInput> StateMachineTransitionMapBuilder { get; }

        IStateTransitionMapBuilder<TState, TInput> When(Expression<Func<TInput, bool>> condition, TState newState);

        IStateTransitionMapBuilder<TState, TInput> When(TInput input, TState newState);

        IStateTransitionMapBuilder<TState, TInput> GotoWhen(TState newState, params TInput[] input);

        IStateTransitionMapBuilder<TState, TInput> Default(TState state);
    }
}