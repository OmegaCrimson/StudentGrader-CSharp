using System;
using System.Collections.Generic;

namespace Exceptions
{
    public class StringInvalidValueException : Exception
    {
        public string ActualValue { get; }
        public List<string> ValidValues { get; }

        public StringInvalidValueException(string message, string actualValue, List<string> validValues)
            : base(message)
        {
            ActualValue = actualValue;
            ValidValues = validValues;
        }

        public StringInvalidValueException(string message, Exception inner)
            : base(message, inner)
        {
            ActualValue = string.Empty;
            ValidValues = new List<string>();
        }

        public override string ToString()
        {
            string validValuesStr = string.Join(", ", ValidValues);
            return $"{base.ToString()}\nActual Value: '{ActualValue}', Valid Values: [{validValuesStr}]";
        }
    }
}