using System.Collections;
using System.Collections.Generic;

namespace FastState
{
    internal class StateTransitionMap<TState, TInput> : IStateTransitionMap<TState, TInput>
    {
        private readonly IEnumerable<Transition<TState, TInput>> transitions;

        public StateTransitionMap(
            TState state,
            IEnumerable<Transition<TState, TInput>> transitions,
            TState defaultTransitionState)
        {
            this.transitions = transitions;

            State = state;
            DefaultTransitionState = defaultTransitionState;
        }

        public bool HasDefaultTransitionState => !EqualityComparer<TState>.Default.Equals(DefaultTransitionState, State);

        public TState State { get; }

        public TState DefaultTransitionState { get; }

        public IEnumerator<Transition<TState, TInput>> GetEnumerator()
        {
            return transitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}