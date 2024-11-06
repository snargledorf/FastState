using System.Collections.Generic;
using System.Linq;

namespace FastState
{
    internal class StateMachineTransitionMapBuilder<TState, TInput> : IStateMachineTransitionMapBuilder<TState, TInput>
    {
        private readonly Dictionary<TState, StateTransitionMapBuilder<TState, TInput>> _transitionMapBuilders =
            new Dictionary<TState, StateTransitionMapBuilder<TState, TInput>>();

        public IStateMachineTransitionMap<TState, TInput> Build()
        {
            return new StateMachineTransitionMap<TState, TInput>(_transitionMapBuilders.Values.Select(mp => mp.Build()).ToArray());
        }

        public IStateTransitionMapBuilder<TState, TInput> From(TState state)
        {
            if (_transitionMapBuilders.TryGetValue(state, out StateTransitionMapBuilder<TState, TInput> builder))
                return builder;
            
            return _transitionMapBuilders[state] = new StateTransitionMapBuilder<TState, TInput>(state, this);
        }
    }
}