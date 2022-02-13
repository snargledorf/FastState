using System.Linq.Expressions;

namespace FastState
{
    internal class Transition<TState, TInput>
    {
        public Transition(Expression conditions, TState newState)
        {
            Condition = conditions;
            NewState = newState;
        }

        public Expression Condition { get; }

        public TState NewState { get; set; }
    }
}