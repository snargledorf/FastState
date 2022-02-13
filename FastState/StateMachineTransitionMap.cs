using System.Collections;
using System.Collections.Generic;

namespace FastState
{
    internal class StateMachineTransitionMap<TState, TInput> : IStateMachineTransitionMap<TState, TInput>
    {
        private readonly IEnumerable<IStateTransitionMap<TState, TInput>> transitionMaps;

        public StateMachineTransitionMap(IEnumerable<IStateTransitionMap<TState, TInput>> enumerable)
        {
            transitionMaps = enumerable;
        }

        public IEnumerator<IStateTransitionMap<TState, TInput>> GetEnumerator()
        {
            return transitionMaps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}