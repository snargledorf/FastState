// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;

using BenchmarkDotNet.Attributes;

using FastState;
using FastState.Benchmarks;

public class StringStateMachine : BenchmarkBase<string>
{
    public StringStateMachine()
        : base(i => i.ToString())
    {
    }
}