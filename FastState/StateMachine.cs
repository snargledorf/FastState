using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FastState
{
    public sealed class StateMachine<TState, TInput> : IStateMachine<TState, TInput>
    {
        private readonly IStateMachineTransitionMap<TState, TInput> stateMachineTransitionMap;

        private readonly TryTransitionDelegate<TState, TInput> tryTransitions;
        private readonly TryGetDefaultDelegate<TState, TInput> tryGetDefault;

        public StateMachine(Action<IStateMachineTransitionMapBuilder<TState, TInput>> buildStates)
        {
            var stateMachineConfig = new StateMachineTransitionMapBuilder<TState, TInput>();
            buildStates(stateMachineConfig);
            stateMachineTransitionMap = stateMachineConfig.Build();

            tryTransitions = StateMachineTransitionMapExpressionFactory<TState, TInput>.BuildTryTransitionExpression(stateMachineTransitionMap).Compile();
            tryGetDefault = StateMachineTransitionMapExpressionFactory<TState, TInput>.BuildTryGetDefaultExpression(stateMachineTransitionMap).Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryTransition(TState state, TInput input, out TState newState)
        {
            return tryTransitions(state, input, out newState);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetDefaultForState(TState state, out TState newState)
        {
            return tryGetDefault(state, out newState);
        }

        public bool HasState(TState state)
        {
            return stateMachineTransitionMap.Any(map => EqualityComparer<TState>.Default.Equals(state, map.State));
        }
    }
}
