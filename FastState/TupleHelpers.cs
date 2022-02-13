using System;

namespace FastState
{
    internal static class TupleHelpers
    {   
        public static bool IsTuple<T>() => IsTuple(typeof(T));

        public static bool IsTuple(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var openType = type.GetGenericTypeDefinition();
            return openType == typeof(ValueTuple<>)
                || openType == typeof(ValueTuple<,>)
                || openType == typeof(ValueTuple<,,>)
                || openType == typeof(ValueTuple<,,,>)
                || openType == typeof(ValueTuple<,,,,>)
                || openType == typeof(ValueTuple<,,,,,>)
                || openType == typeof(ValueTuple<,,,,,,>)
                || (openType == typeof(ValueTuple<,,,,,,,>) && IsTuple(type.GetGenericArguments()[7]));
        }
    }
}
