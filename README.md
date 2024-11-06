# FastState

[![NuGet Version](https://img.shields.io/nuget/v/FastState?link=https%3A%2F%2Fgithub.com%2Fsnargledorf%2FFastState)](https://www.nuget.org/packages/FastState/)

```c#
var sm = new StateMachine<string, string>(builder =>
{
    builder.From("State1")
        .When("Input1", "State2");
});
 
Assert.IsTrue(sm.TryTransition("State1", "Input1", out string newState));
Assert.AreEqual("State2", newState);
```
