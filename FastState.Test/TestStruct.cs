using System;

namespace FastState.Test
{
    internal struct TestStruct
    {
        public TestStruct(string v)
        {
            Value = v;
        }

        public string Value { get; }

        public override bool Equals(object? obj)
        {
            return obj is TestStruct @struct &&
                   Value == @struct.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(TestStruct left, TestStruct right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TestStruct left, TestStruct right)
        {
            return !(left == right);
        }
    }
}