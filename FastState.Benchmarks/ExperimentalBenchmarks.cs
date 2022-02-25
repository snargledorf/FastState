
using BenchmarkDotNet.Attributes;

namespace FastState.Benchmarks
{
    public abstract class ExperimentalBenchmarks
    {
        const int numberOfStatesAndInputs = 5;
        const int delStateOffset = 50;

        protected static readonly IEnumerable<int> stateInts = Enumerable.Range(1, numberOfStatesAndInputs);

        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark]
        public int TryTransitionHitConstant()
        {
            int result = default;

            foreach (var state in stateInts)
            {
                result = GetNewState(state, state);
            }

            return result;
        }

        private int GetNewState(int state, int input)
        {
            switch (state)
            {
                case 1:
                    switch (input)
                    {
                        case 1:
                            return 1;
                        case 2:
                            return 2;
                        case 3:
                            return 3;
                        case 4:
                            return 4;
                        case 5:
                            return 5;
                    }
                    break;
                case 2:
                    switch (input)
                    {
                        case 1:
                            return 1;
                        case 2:
                            return 2;
                        case 3:
                            return 3;
                        case 4:
                            return 4;
                        case 5:
                            return 5;
                    }
                    break;
                case 3:
                    switch (input)
                    {
                        case 1:
                            return 1;
                        case 2:
                            return 2;
                        case 3:
                            return 3;
                        case 4:
                            return 4;
                        case 5:
                            return 5;
                    }
                    break;
                case 4:
                    switch (input)
                    {
                        case 1:
                            return 1;
                        case 2:
                            return 2;
                        case 3:
                            return 3;
                        case 4:
                            return 4;
                        case 5:
                            return 5;
                    }
                    break;
                case 5:
                    switch (input)
                    {
                        case 1:
                            return 1;
                        case 2:
                            return 2;
                        case 3:
                            return 3;
                        case 4:
                            return 4;
                        case 5:
                            return 5;
                    }
                    break;
            }

            throw new Exception();
        }
    }
}
