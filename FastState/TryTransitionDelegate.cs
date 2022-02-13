namespace FastState
{
    internal delegate bool TryTransitionDelegate<TState, TInput>(TState state, TInput input, out TState newState);
}
