// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;

using FastState;
using FastState.Benchmarks;

public class TupleStateMachine : BenchmarkBase<(string, string)>
{
    public TupleStateMachine()
        : base(i => (i.ToString(), i.ToString())) { }
}