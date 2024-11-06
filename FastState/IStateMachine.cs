namespace FastState
{
    internal interface IStateMachine<TState, TInput>
    {
        bool HasState(TState state);
        bool StateHasInputTransitions(TState state);
        bool StateHasDefaultTransition(TState state);
        bool TryTransition(TState state, TInput input, out TState newState);
        bool TryGetDefaultForState(TState state, out TState newState);
    }
}