using System;

namespace Exceptions
{
    public class IntInvalidValueException : Exception
    {
        public int ProvidedValue { get; }
        public int MinAllowed { get; }
        public int MaxAllowed { get; }

        public IntInvalidValueException(string message, int providedValue, int minAllowed, int maxAllowed)
            : base(message)
        {
            ProvidedValue = providedValue;
            MinAllowed = minAllowed;
            MaxAllowed = maxAllowed;
        }

        public IntInvalidValueException(string message, Exception inner)
            : base(message, inner) { }

        public override string ToString()
        {
            return $"{base.ToString()}\nProvided: {ProvidedValue}, Expected: {MinAllowed}–{MaxAllowed}";
        }
    }
}
