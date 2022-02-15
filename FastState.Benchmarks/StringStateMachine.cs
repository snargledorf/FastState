// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;

using FastState;

public class StringStateMachine
{
    readonly StateMachine<string, string> Machine = new(builder =>
    {
        builder.From("State1")

            .When((input) => input == "Input1", "State2")
            .When(input => input.StartsWith("Input1"), "State3")

            .When(input => input.StartsWith("Input2"), "State2")
            .When(input => input == "Input2", "State3")

            .When(input => input == "Input3", "State2")
            .When("Input3", "State3")

            .Default("State4");

        builder.From("State2");
    });

    [Benchmark]
    public string TryTransitionHitConstant()
    {
        Machine.TryTransition("State1", "Input3", out string newState);
        return newState;
    }

    [Benchmark]
    public string TryTransitionHitExpression()
    {
        Machine.TryTransition("State1", "Input1", out string newState);
        return newState;
    }

    [Benchmark]
    public string TryTransitionDefault()
    {
        Machine.TryTransition("State1", "Input4", out string newState);
        return newState;
    }

    [Benchmark]
    public string TryGetDefaultForStateHit()
    {
        Machine.TryGetDefaultForState("State1", out string newState);
        return newState;
    }

    [Benchmark]
    public string TryGetDefaultForStateMiss()
    {
        Machine.TryGetDefaultForState("State2", out string newState);
        return newState;
    }
}