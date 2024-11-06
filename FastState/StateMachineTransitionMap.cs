using System.Collections;
using System.Collections.Generic;

namespace FastState
{
    internal class StateMachineTransitionMap<TState, TInput> : IStateMachineTransitionMap<TState, TInput>
    {
        private readonly IReadOnlyCollection<IStateTransitionMap<TState, TInput>> _transitionMaps;

        public StateMachineTransitionMap(IReadOnlyCollection<IStateTransitionMap<TState, TInput>> transitionMaps)
        {
            _transitionMaps = transitionMaps;
        }

        public IEnumerator<IStateTransitionMap<TState, TInput>> GetEnumerator()
        {
            return _transitionMaps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}