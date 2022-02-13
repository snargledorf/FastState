using System.Collections.Generic;
using System.Linq.Expressions;

namespace FastState
{
    internal interface IStateTransitionMap<TState, TInput> : IEnumerable<Transition<TState, TInput>>
    {
        bool HasDefaultTransitionState { get; }
        TState DefaultTransitionState { get; }
        TState State { get; }
    }
}