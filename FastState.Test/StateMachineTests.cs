using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastState.Test
{
    [TestClass]
    public class StateMachineTests
    {
        [TestMethod]
        public void NewStateMachine()
        {
            var sm = new StateMachine<string, string>(_ => { });
        }

        [TestMethod]
        public void SimpleStateTransition()
        {
            var sm = new StateMachine<string, string>(builder =>
            {
                builder.From("State1")
                    .When("Input1", "State2");
            });

            Assert.IsTrue(sm.TryTransition("State1", "Input1", out string newState));
            Assert.AreEqual("State2", newState);
        }

        [TestMethod]
        public void MultipleStates()
        {
            var sm = new StateMachine<string, string>(builder =>
            {
                builder.From("State1")
                    .When("Input1", "State2");

                builder.From("State2")
                    .When("Input2", "State1");
            });

            Assert.IsTrue(sm.TryTransition("State1", "Input1", out string newState));
            Assert.AreEqual("State2", newState);

            Assert.IsTrue(sm.TryTransition("State2", "Input2", out newState));
            Assert.AreEqual("State1", newState);
        }

        [TestMethod]
        public void InputWithoutTransition()
        {
            var sm = new StateMachine<string, string>(builder =>
            {
                builder.From("State1")
                    .When("Input1", "State2");
            });

            Assert.IsFalse(sm.TryTransition("State1", "Input2", out string newState));
            Assert.IsNull(newState);
        }

        [TestMethod]
        public void OrderOfTransitions()
        {
            var sm = new StateMachine<string, string>(builder =>
            {
                builder.From("State1")

                    .When((input) => input == "Input1", "State2")
                    .When(input => input.StartsWith("Input1"), "State3")

                    .When(input => input.StartsWith("Input2"), "State2")
                    .When(input => input == "Input2", "State3")

                    .When(input => input == "Input3", "State2")
                    .When("Input3", "State3");
            });

            Assert.IsTrue(sm.TryTransition("State1", "Input1", out string newState));
            Assert.AreEqual("State2", newState);

            Assert.IsTrue(sm.TryTransition("State1", "Input1Start", out newState));
            Assert.AreEqual("State3", newState);

            Assert.IsTrue(sm.TryTransition("State1", "Input2", out newState));
            Assert.AreEqual("State2", newState);

            Assert.IsTrue(sm.TryTransition("State1", "Input3", out newState));
            Assert.AreEqual("State3", newState);
        }

        [TestMethod]
        public void TupleInput()
        {
            var sm = new StateMachine<string, (string, string)>(builder =>
            {
                builder.From("State1")

                    .When(input => input.Item1 == "Input1", "State2")
                    .When(input => input.Item2 == "Input1", "State3")

                    .When(input => input.Item2 == "Input2", "State2")
                    .When(input => input.Item1 == "Input2", "State3")

                    .When(input => input.Item1 == "Input3" && input.Item2 == "Input3", "State2")
                    .When(("Input3", "Input3"), "State3");
            });

            Assert.IsTrue(sm.TryTransition("State1", ("Input1", "Input1"), out string newState));
            Assert.AreEqual("State2", newState);

            Assert.IsTrue(sm.TryTransition("State1", ("Input2", "Input2"), out newState));
            Assert.AreEqual("State2", newState);

            Assert.IsTrue(sm.TryTransition("State1", ("Input3", "Input3"), out newState));
            Assert.AreEqual("State3", newState);
        }

        [TestMethod]
        public void TupleState()
        {
            var sm = new StateMachine<(string, string), string>(builder =>
            {
                builder.From(("State1", "State1"))
                    .When("Input1", ("State2", "State2"));

                builder.From(("State2", "State2"))
                    .When("Input1", ("State1", "State1"));
            });

            Assert.IsTrue(sm.TryTransition(("State1", "State1"), "Input1", out (string, string) newState));
            Assert.AreEqual(("State2", "State2"), newState);

            Assert.IsTrue(sm.TryTransition(("State2", "State2"), "Input1", out newState));
            Assert.AreEqual(("State1", "State1"), newState);
        }

        [TestMethod]
        public void DateTimeInput()
        {
            var sm = new StateMachine<string, DateTime>(builder =>
            {
                builder.From("State1")

                    .When(DateTime.Today, "Today")
                    .When(DateTime.Today.AddDays(1), "Tomorrow")
                    .When(input => input < DateTime.Today, "Before Today");
            });

            Assert.IsTrue(sm.TryTransition("State1", DateTime.Today, out string newState));
            Assert.AreEqual("Today", newState);

            Assert.IsTrue(sm.TryTransition("State1", DateTime.Today.AddDays(1), out newState));
            Assert.AreEqual("Tomorrow", newState);

            Assert.IsTrue(sm.TryTransition("State1", DateTime.Today.AddDays(-1), out newState));
            Assert.AreEqual("Before Today", newState);
        }

        [TestMethod]
        public void DateTimeState()
        {
            var sm = new StateMachine<DateTime, string>(builder =>
            {
                builder.From(DateTime.Today)
                    .When("AddDay", DateTime.Today.AddDays(1));

                builder.From(DateTime.Today.AddDays(1))
                    .When("RemoveDay", DateTime.Today);
            });

            Assert.IsTrue(sm.TryTransition(DateTime.Today, "AddDay", out DateTime newState));
            Assert.AreEqual(DateTime.Today.AddDays(1), newState);

            Assert.IsTrue(sm.TryTransition(DateTime.Today.AddDays(1), "RemoveDay", out newState));
            Assert.AreEqual(DateTime.Today, newState);
        }

        [TestMethod]
        public void StructInput()
        {
            var sm = new StateMachine<string, TestStruct>(builder =>
            {
                builder.From("State1")
                    .When(new TestStruct("Input1"), "State2")
                    .When(new TestStruct("Input2"), "State3");
            });

            Assert.IsTrue(sm.TryTransition("State1", new TestStruct("Input1"), out string newState));
            Assert.AreEqual("State2", newState);

            Assert.IsTrue(sm.TryTransition("State1", new TestStruct("Input2"), out newState));
            Assert.AreEqual("State3", newState);
        }

        [TestMethod]
        public void StructState()
        {
            var sm = new StateMachine<TestStruct, string>(builder =>
            {
                builder.From(new TestStruct("State1"))
                    .When("Input1", new TestStruct("State2"));
                builder.From(new TestStruct("State2"))
                    .When("Input1", new TestStruct("State1"));
            });

            Assert.IsTrue(sm.TryTransition(new TestStruct("State1"), "Input1", out TestStruct newState));
            Assert.AreEqual(new TestStruct("State2"), newState);

            Assert.IsTrue(sm.TryTransition(new TestStruct("State2"), "Input1", out newState));
            Assert.AreEqual(new TestStruct("State1"), newState);
        }

        [TestMethod]
        public void ClassInput()
        {
            var sm = new StateMachine<string, TestClass>(builder =>
            {
                builder.From("State1")
                    .When(new TestClass("Input1"), "State2")
                    .When(new TestClass("Input2"), "State3");
            });

            Assert.IsTrue(sm.TryTransition("State1", new TestClass("Input1"), out string newState));
            Assert.AreEqual("State2", newState);

            Assert.IsTrue(sm.TryTransition("State1", new TestClass("Input2"), out newState));
            Assert.AreEqual("State3", newState);
        }

        [TestMethod]
        public void ClassState()
        {
            var sm = new StateMachine<TestClass, string>(builder =>
            {
                builder.From(new TestClass("State1"))
                    .When("Input1", new TestClass("State2"));
                builder.From(new TestClass("State2"))
                    .When("Input1", new TestClass("State1"));
            });

            Assert.IsTrue(sm.TryTransition(new TestClass("State1"), "Input1", out TestClass newState));
            Assert.AreEqual(new TestClass("State2"), newState);

            Assert.IsTrue(sm.TryTransition(new TestClass("State2"), "Input1", out newState));
            Assert.AreEqual(new TestClass("State1"), newState);
        }

        [TestMethod]
        public void SimpleDefault()
        {
            var sm = new StateMachine<string, string>(builder =>
            {
                builder.From("State1")
                    .Default("State2");
            });

            Assert.IsTrue(sm.TryTransition("State1", "Input1", out string newState));
            Assert.AreEqual("State2", newState);
        }

        [TestMethod]
        public void DefaultWithOtherTransitions()
        {
            var sm = new StateMachine<string, string>(builder =>
            {
                builder.From("State1")
                    .When("Input1", "State2")
                    .Default("State3");
            });

            Assert.IsTrue(sm.TryTransition("State1", "Input1", out string newState));
            Assert.AreEqual("State2", newState);
            Assert.IsTrue(sm.TryTransition("State1", "Input2", out newState));
            Assert.AreEqual("State3", newState);
        }

        [TestMethod]
        public void GetDefault()
        {
            var sm = new StateMachine<string, string>(builder =>
            {
                builder.From("State1")
                    .Default("State2");
            });

            Assert.IsTrue(sm.TryGetDefaultForState("State1", out string newState));
            Assert.AreEqual("State2", newState);
        }

        [TestMethod]
        public void GetDefaultWithNoDefault()
        {
            var sm = new StateMachine<string, string>(builder =>
            {
                builder.From("State1");
            });

            Assert.IsFalse(sm.TryGetDefaultForState("State1", out string newState));
            Assert.IsNull(newState);
        }

        [TestMethod]
        public void GetDefaultWithOtherTransitions()
        {
            var sm = new StateMachine<string, string>(builder =>
            {
                builder.From("State1")
                    .When("Input1", "State2")
                    .Default("State3");
            });

            Assert.IsTrue(sm.TryGetDefaultForState("State1", out string newState));
            Assert.AreEqual("State3", newState);
        }

        [TestMethod]
        public void GetDefaultWithMultipleStates()
        {
            var sm = new StateMachine<string, string>(builder =>
            {
                builder.From("State1")
                    .Default("State3");

                builder.From("State2");

                builder.From("State3")
                    .When("Input2", "State2")
                    .Default("State1");
            });

            Assert.IsTrue(sm.TryGetDefaultForState("State1", out string newState));
            Assert.AreEqual("State3", newState);
            Assert.IsFalse(sm.TryGetDefaultForState("State2", out newState));
            Assert.IsNull(newState);
            Assert.IsTrue(sm.TryGetDefaultForState("State3", out newState));
            Assert.AreEqual("State1", newState);
        }

        [TestMethod]
        public void HasState()
        {
            var sm = new StateMachine<string, string>(builder => 
            {
                builder.From("State1");
            });

            Assert.IsTrue(sm.HasState("State1"));
            Assert.IsFalse(sm.HasState("State2"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetDefaultInvalidState()
        {
            var sm = new StateMachine<string, string>(_ => { });
            sm.TryGetDefaultForState(string.Empty, out _);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TransitionInvalidState()
        {
            var sm = new StateMachine<string, string>(_ => { });
            sm.TryTransition(string.Empty, string.Empty, out _);
        }
    }
}