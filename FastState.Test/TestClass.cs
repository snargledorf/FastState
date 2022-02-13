using System;
using System.Collections.Generic;

namespace FastState.Test
{
    internal class TestClass
    {
        public TestClass(string v)
        {
            Value = v;
        }

        public string Value { get; }

        public override bool Equals(object? obj)
        {
            return obj is TestClass @class &&
                   Value == @class.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(TestClass? left, TestClass? right)
        {
            return EqualityComparer<TestClass>.Default.Equals(left, right);
        }

        public static bool operator !=(TestClass? left, TestClass? right)
        {
            return !(left == right);
        }
    }
}