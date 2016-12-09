using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfSettings.Utils
{
    internal static class EmptyChecker
    {
        public static bool IsEmpty(this object value)
        {
            return value == null || Empty((dynamic) value);
        }

        private static bool Empty(object value)
        {
            return value == null;
        }

        private static bool Empty(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        private static bool Empty(bool value)
        {
            return !value;
        }

        private static bool Empty(int value)
        {
            return value == 0;
        }

        private static bool Empty(short value)
        {
            return value == 0;
        }

        private static bool Empty(double value)
        {
            return Math.Abs(value) < double.Epsilon;
        }

        private static bool Empty(float value)
        {
            return Math.Abs(value) < float.Epsilon;
        }

        private static bool Empty<T>(IEnumerable<T> value)
        {
            return value == null || !value.Any();
        }
    }
}
