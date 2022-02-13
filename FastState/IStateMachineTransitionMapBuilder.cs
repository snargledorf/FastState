namespace FastState
{
    public interface IStateMachineTransitionMapBuilder<TState, TInput>
    {        
        IStateTransitionMapBuilder<TState, TInput> From(TState state);
    }
}