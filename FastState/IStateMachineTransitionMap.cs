using System.Collections.Generic;

namespace FastState
{
    internal interface IStateMachineTransitionMap<TState, TInput> : IEnumerable<IStateTransitionMap<TState, TInput>>
    {
    }
}