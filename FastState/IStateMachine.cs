namespace FastState
{
    internal interface IStateMachine<TState, TInput>
    {
        bool HasState(TState state);
        bool TryTransition(TState state, TInput input, out TState newState);
        bool TryGetDefaultForState(TState state, out TState newState);
    }
}