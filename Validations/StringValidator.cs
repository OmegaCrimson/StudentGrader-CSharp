using Exceptions;

namespace Validations
{
    public static class StringValidator
    {
        public static void ValidateLength(string value, int min, int max, string label)
        {
            int length = value?.Length ?? 0;

            if (length < min || length > max)
            {
                throw new StringLengthOutOfRangeException($"Invalid {label} length.", length, min, max);
            }
        }

        public static void ValidateRequired(string value, string label)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new StringLengthOutOfRangeException($"{label} is required.", 0, 1, int.MaxValue);
            }
        }
    }
}
