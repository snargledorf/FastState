
using BenchmarkDotNet.Attributes;

namespace FastState.Benchmarks
{
    public abstract class BenchmarkBase
    {
        protected const int NumberOfStatesAndInputs = 5;
        protected const int DelStateOffset = 50;

        protected static readonly IEnumerable<int> StateInts = Enumerable.Range(1, NumberOfStatesAndInputs);
    }
    
    [MemoryDiagnoser]
    public abstract class BenchmarkBase<T> : BenchmarkBase
    {
        private readonly Func<int, T?> _intToT;

        private StateMachine<T?, T?> _machine = null!;

        protected BenchmarkBase(Func<int, T> intToT)
        {
            _intToT = intToT;
        }

        [GlobalSetup]
        public void Setup()
        {
            _machine = new StateMachine<T?, T?>(builder =>
            {
                foreach (int state in StateInts)
                {
                    IStateTransitionMapBuilder<T?, T?> stateBuilder = builder.From(_intToT(state));

                    foreach (int constInput in Enumerable.Range(state, NumberOfStatesAndInputs))
                    {
                        T? input = _intToT(constInput);
                        stateBuilder.When(input, input);
                    }

                    foreach (int delInput in Enumerable.Range(state + DelStateOffset, NumberOfStatesAndInputs))
                    {
                        T? input = _intToT(delInput);
                        stateBuilder.When(i => EqualityComparer<T>.Default.Equals(i, input), input);
                    }

                    stateBuilder.Default(_intToT(state + 1));
                }

                builder.From(_intToT(DelStateOffset));
            });
        }

        [Benchmark]
        public T? TryTransitionHitConstant()
        {
            T? newState = default;

            foreach (int state in StateInts)
            {
                foreach (int inputInt in Enumerable.Range(state, NumberOfStatesAndInputs))
                {
                    T? input = _intToT(inputInt);
                    if (_machine.TryTransition(_intToT(state), input, out newState) 
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

            foreach (int state in StateInts)
            {
                foreach (int inputInt in Enumerable.Range(state + DelStateOffset, NumberOfStatesAndInputs))
                {
                    T? input = _intToT(inputInt);
                    if (_machine.TryTransition(_intToT(state), input, out newState)
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

            foreach (int state in StateInts)
            {
                if (_machine.TryTransition(_intToT(state), default, out newState)
                    && !EqualityComparer<T>.Default.Equals(newState, _intToT(state + 1)))
                    throw new Exception();
            }

            return newState;
        }

        [Benchmark]
        public T? TryGetDefaultForStateHit()
        {
            T? newState = default;

            foreach (int state in StateInts)
            {
                if (_machine.TryGetDefaultForState(_intToT(state), out newState)
                    && !EqualityComparer<T>.Default.Equals(newState, _intToT(state + 1)))
                    throw new Exception();
            }

            return newState;
        }

        [Benchmark]
        public T? TryGetDefaultForStateMiss()
        {
            _machine.TryGetDefaultForState(_intToT(DelStateOffset), out T? newState);
            return newState;
        }
    }
}
