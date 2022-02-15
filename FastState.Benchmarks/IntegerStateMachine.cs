// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;

using FastState;
using FastState.Benchmarks;

public class IntegerStateMachine : BenchmarkBase<int>
{
    public IntegerStateMachine()
        : base(i => i)
    {
    }
}