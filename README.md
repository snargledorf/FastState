```
var sm = new StateMachine<string, string>(builder =>
{
    builder.From("State1")
        .When("Input1", "State2");
});
 
Assert.IsTrue(sm.TryTransition("State1", "Input1", out string newState));
Assert.AreEqual("State2", newState);
```