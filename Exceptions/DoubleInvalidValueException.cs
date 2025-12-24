using System;

namespace Exceptions
{
    public class DoubleInvalidValueException : Exception
    {
        public double ProvidedValue { get; }
        public double MinAllowed { get; }
        public double MaxAllowed { get; }

        public DoubleInvalidValueException(string message, double providedValue, double minAllowed, double maxAllowed)
            : base(message)
        {
            ProvidedValue = providedValue;
            MinAllowed = minAllowed;
            MaxAllowed = maxAllowed;
        }

        public DoubleInvalidValueException(string message, Exception inner)
            : base(message, inner) { }

        public override string ToString()
        {
            return $"{base.ToString()}\nProvided: {ProvidedValue}, Expected: {MinAllowed}–{MaxAllowed}";
        }
    }
}
