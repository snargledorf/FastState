﻿// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;

using FastState;

public class TupleStateMachine
{
    readonly StateMachine<(string, string), (string, string)> tupleStateMachine = new(builder =>
    {
        builder.From(("State1", "State1"))

            .When(input => input.Item1 == "Input1" && input.Item2 == "Input1", ("State2", "State2"))
            .When(input => input.Item1 == "Input1", ("State3", "State3"))

            .When(input => input.Item1 == "Input2", ("State2", "State2"))
            .When(input => input.Item1 == "Input2" && input.Item2 == "Input2", ("State3", "State3"))

            .When(input => input.Item1 == "Input3" && input.Item2 == "Input3", ("State2", "State2"))
            .When(("Input3", "Input3"), ("State3", "State3"))

            .Default(("State4", "State4"));

        builder.From(("State2", "State2"));
    });

    [Benchmark]
    public (string, string) TryTransitionTupleStateHitConstant()
    {
        tupleStateMachine.TryTransition(("State1", "State1"), ("Input3", "Input3"), out (string, string) newState);
        return newState;
    }

    [Benchmark]
    public (string, string) TryTransitionTupleStateHitExpression()
    {
        tupleStateMachine.TryTransition(("State1", "State1"), ("Input1", "Input1"), out (string, string) newState);
        return newState;
    }

    [Benchmark]
    public (string, string) TryTransitionTupleStateDefault()
    {
        tupleStateMachine.TryTransition(("State1", "State1"), ("Input4", "Input4"), out (string, string) newState);
        return newState;
    }

    [Benchmark]
    public (string, string) TryGetDefaultForStateTupleStateHit()
    {
        tupleStateMachine.TryGetDefaultForState(("State1", "State1"), out (string, string) newState);
        return newState;
    }

    [Benchmark]
    public (string, string) TryGetDefaultForStateTupleStateMiss()
    {
        tupleStateMachine.TryGetDefaultForState(("State2", "State2"), out (string, string) newState);
        return newState;
    }
}