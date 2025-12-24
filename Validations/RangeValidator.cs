using Exceptions;

namespace Validations
{
    public static class RangeValidator
    {
        public static void ValidateInt(int value, int min, int max, string label)
        {
            if (value < min || value > max)
            {
                throw new IntInvalidValueException($"Invalid {label}.", value, min, max);
            }
        }

        public static void ValidateDouble(double value, double min, double max, string label)
        {
            if (value < min || value > max)
            {
                throw new DoubleInvalidValueException($"Invalid {label}.", value, min, max);
            }
        }
    }
}