using BenchmarkDotNet.Running;
using FastState.Benchmarks;

BenchmarkRunner.Run<ExperimentalBenchmarks>();

//var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
//Console.WriteLine(summary);