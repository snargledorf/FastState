using System.Linq.Expressions;

using BenchmarkDotNet.Attributes;

namespace FastState.Benchmarks
{
    public abstract class ExperimentalBenchmarks
    {
        const int numberOfStatesAndInputs = 5;
        const int delStateOffset = 50;

        protected static readonly IEnumerable<int> states = Enumerable.Range(1, numberOfStatesAndInputs);

        private delegate int GetNewStateDel(int state, int input);

        private GetNewStateDel? getNewState;

        [GlobalSetup]
        public void Setup()
        {
            CompileGetNewState();
        }

        private void CompileGetNewState()
        {
            ParameterExpression stateParam = Expression.Parameter(typeof(int));
            ParameterExpression inputParam = Expression.Parameter(typeof(int));
            LabelTarget returnTarget = Expression.Label(typeof(int));

            var currentStateFlowControlExpressions = new List<Expression>();

            SwitchCase[] inputSwitchCases = states.Select(state1 => Expression.SwitchCase(Expression.Return(returnTarget, Expression.Constant(state1)), Expression.Constant(state1))).ToArray();

            var stateSwitchCase = Expression.SwitchCase(
                Expression.Switch(inputParam, inputSwitchCases),
                states.Select(state => Expression.Constant(state)).ToArray()
            );

            currentStateFlowControlExpressions.Add(Expression.Switch(stateParam, stateSwitchCase));

            currentStateFlowControlExpressions.Add(Expression.Throw(Expression.Constant(new ArgumentException("Invalid state"))));
            currentStateFlowControlExpressions.Add(Expression.Label(returnTarget, Expression.Constant(0, typeof(int))));

            BlockExpression body = Expression.Block(currentStateFlowControlExpressions);

            getNewState = Expression.Lambda<GetNewStateDel>(body, stateParam, inputParam).Compile();
        }

        [Benchmark]
        public int TryTransitionHitConstantDelegate()
        {
            int result = default;

            foreach (var state in states)
            {
                result = getNewState!(state, state);
                if (result != state)
                    throw new Exception();
            }

            return result;
        }

        [Benchmark]
        public int TryTransitionHitConstantDirectCall()
        {
            int result = default;

            foreach (var state in states)
            {
                result = GetNewState(state, state);
                if (result != state)
                    throw new Exception();
            }

            return result;
        }

        private static int GetNewState(int state, int input)
        {
            switch (state)
            {
                case 1:
                case 2:
                case 3:
                case 4:
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

            throw new Exception($"Unexpected state {state}");
        }

        private int Func_1(int input)
        {
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

            throw new Exception($"Unexpected input {input}");
        }
    }
}
