using System.Collections.Generic;
using System.Linq;

namespace FastState
{
    internal class StateMachineTransitionMapBuilder<TState, TInput> : IStateMachineTransitionMapBuilder<TState, TInput>
    {
        private readonly Dictionary<TState, StateTransitionMapBuilder<TState, TInput>> transitionMapBuilders = new Dictionary<TState, StateTransitionMapBuilder<TState, TInput>>();

        public IStateMachineTransitionMap<TState, TInput> Build()
        {
            return new StateMachineTransitionMap<TState, TInput>(transitionMapBuilders.Values.Select(mp => mp.Build()));
        }

        public IStateTransitionMapBuilder<TState, TInput> From(TState state)
        {
            return transitionMapBuilders.ContainsKey(state)
                ? transitionMapBuilders[state]
                : transitionMapBuilders[state] = new StateTransitionMapBuilder<TState, TInput>(state, this);
        }
    }
}