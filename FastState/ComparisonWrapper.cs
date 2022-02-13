using System.Collections.Generic;

namespace FastState
{
    internal struct ComparisonWrapper<T>
    {
        public ComparisonWrapper(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public override bool Equals(object obj)
        {
            return obj is ComparisonWrapper<T> check &&
                   EqualityComparer<T>.Default.Equals(Value, check.Value);
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<T>.Default.GetHashCode(Value);
        }

        public static implicit operator T(ComparisonWrapper<T> obj)
        {
            return obj.Value;
        }

        public static bool operator ==(ComparisonWrapper<T> left, ComparisonWrapper<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ComparisonWrapper<T> left, ComparisonWrapper<T> right)
        {
            return !(left == right);
        }
    }
}