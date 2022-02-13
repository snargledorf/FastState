﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FastState
{
    internal class StateTransitionMapBuilder<TState, TInput> : IStateTransitionMapBuilder<TState, TInput>
    {
        internal readonly List<Transition<TState, TInput>> transitions = new List<Transition<TState, TInput>>();

        public StateTransitionMapBuilder(TState state, IStateMachineTransitionMapBuilder<TState, TInput> stateMachineTransitionMapBuilder)
        {
            State = DefaultTransitionState = state;
            StateMachineTransitionMapBuilder = stateMachineTransitionMapBuilder;
        }

        public TState State { get; }

        public TState DefaultTransitionState { get; private set; }

        public bool HasDefaultTransitionState => !EqualityComparer<TState>.Default.Equals(DefaultTransitionState, State);

        public IStateMachineTransitionMapBuilder<TState, TInput> StateMachineTransitionMapBuilder { get; }

        public IStateTransitionMapBuilder<TState, TInput> When(Expression<Func<TInput, bool>> condition, TState newState)
        {
            transitions.Add(new Transition<TState, TInput>(condition, newState));
            return this;
        }

        public IStateTransitionMapBuilder<TState, TInput> When(TInput input, TState newState)
        {
            ConstantExpression constantExpression;
            if (TupleHelpers.IsTuple<TInput>())
                constantExpression = Expression.Constant(new ComparisonWrapper<TInput>(input));
            else
                constantExpression = Expression.Constant(input);

            transitions.Add(new Transition<TState, TInput>(constantExpression, newState));
            return this;
        }

        public IStateTransitionMapBuilder<TState, TInput> GotoWhen(TState newState, params TInput[] inputs)
        {
            foreach (var input in inputs)
                When(input, newState);

            return this;
        }

        public IStateTransitionMapBuilder<TState, TInput> Default(TState newState)
        {
            DefaultTransitionState = newState;
            return this;
        }

        public StateTransitionMap<TState, TInput> Build()
        {
            return new StateTransitionMap<TState, TInput>(State, transitions, DefaultTransitionState);
        }
    }
}