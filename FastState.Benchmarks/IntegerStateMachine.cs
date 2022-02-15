// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;

using FastState;

public class IntegerStateMachine
{
    readonly StateMachine<int, int> Machine = new(builder =>
    {
        builder.From(1)

            .When(input => input == 1, 2)
            .When(input => input<1, 3)

            .When(input => input<2, 2)
            .When(input => input == 2, 3)

            .When(input => input == 3, 2)
            .When(3, 3)

            .Default(4);

        builder.From(2);
    });

    [Benchmark]
    public int TryTransitionHitConstant()
    {
        Machine.TryTransition(1, 3, out int newState);
        return newState;
    }

    [Benchmark]
    public int TryTransitionHitExpression()
    {
        Machine.TryTransition(1, 1, out int newState);
        return newState;
    }

    [Benchmark]
    public int TryTransitionDefault()
    {
        Machine.TryTransition(1, 4, out int newState);
        return newState;
    }

    [Benchmark]
    public int TryGetDefaultForStateHit()
    {
        Machine.TryGetDefaultForState(1, out int newState);
        return newState;
    }

    [Benchmark]
    public int TryGetDefaultForStateMiss()
    {
        Machine.TryGetDefaultForState(2, out int newState);
        return newState;
    }
}