namespace FastState
{
    internal delegate bool TryGetDefaultDelegate<TState, TInput>(TState state, out TState newState);
}
