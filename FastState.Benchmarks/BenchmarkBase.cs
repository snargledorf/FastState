
using BenchmarkDotNet.Attributes;

namespace FastState.Benchmarks
{
    [MemoryDiagnoser]
    public abstract class BenchmarkBase<T>
    {
        const int numberOfStatesAndInputs = 5;
        const int delStateOffset = 50;

        protected static readonly IEnumerable<int> stateInts = Enumerable.Range(1, numberOfStatesAndInputs);
        
        private readonly Func<int, T> intToT;
        
        protected StateMachine<T, T> Machine = null!;

        protected BenchmarkBase(Func<int, T> intToT)
        {
            this.intToT = intToT;
        }

        [GlobalSetup]
        public void Setup()
        {
            Machine = new(builder =>
            {
                foreach (var state in BenchmarkBase<T>.stateInts)
                {
                    IStateTransitionMapBuilder<T, T> stateBuilder = builder.From(intToT(state));

                    foreach (var constInput in Enumerable.Range(state, BenchmarkBase<T>.numberOfStatesAndInputs))
                    {
                        T? input = intToT(constInput);
                        stateBuilder.When(input, input);
                    }

                    foreach (var delInput in Enumerable.Range(state + BenchmarkBase<T>.delStateOffset, BenchmarkBase<T>.numberOfStatesAndInputs))
                    {
                        T input = intToT(delInput);
                        stateBuilder.When(i => EqualityComparer<T>.Default.Equals(i, input), input);
                    }

                    stateBuilder.Default(intToT(state + 1));
                }

                builder.From(intToT(delStateOffset));
            });
        }

        [Benchmark]
        public T? TryTransitionHitConstant()
        {
            T? newState = default;

            foreach (var state in stateInts)
            {
                foreach (var inputInt in Enumerable.Range(state, numberOfStatesAndInputs))
                {
                    T input = intToT(inputInt);
                    if (Machine.TryTransition(intToT(state), input, out newState) 
                        && !EqualityComparer<T>.Default.Equals(newState, input))
                        throw new Exception();
                }
            }

            return newState;
        }

        [Benchmark]
        public T? TryTransitionHitExpression()
        {
            T? newState = default;

            foreach (var state in stateInts)
            {
                foreach (var inputInt in Enumerable.Range(state + delStateOffset, numberOfStatesAndInputs))
                {
                    T input = intToT(inputInt);
                    if (Machine.TryTransition(intToT(state), input, out newState)
                        && !EqualityComparer<T>.Default.Equals(newState, input))
                        throw new Exception();
                }
            }

            return newState;
        }

        [Benchmark]
        public T? TryTransitionDefault()
        {
            T? newState = default;

            foreach (var state in stateInts)
            {
                if (Machine.TryTransition(intToT(state), default, out newState)
                    && !EqualityComparer<T>.Default.Equals(newState, intToT(state + 1)))
                    throw new Exception();
            }

            return newState;
        }

        [Benchmark]
        public T? TryGetDefaultForStateHit()
        {
            T? newState = default;

            foreach (var state in stateInts)
            {
                if (Machine.TryGetDefaultForState(intToT(state), out newState)
                    && !EqualityComparer<T>.Default.Equals(newState, intToT(state + 1)))
                    throw new Exception();
            }

            return newState;
        }

        [Benchmark]
        public T? TryGetDefaultForStateMiss()
        {
            Machine.TryGetDefaultForState(intToT(delStateOffset), out T newState);
            return newState;
        }
    }
}
