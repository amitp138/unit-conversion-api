using System;

namespace UnitConversionApi.Exceptions
{
    /// <summary>
    /// Exception thrown when a unit conversion request is invalid (e.g., unsupported units or incompatible conversion types).
    /// </summary>
    public class InvalidConversionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConversionException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidConversionException(string message) : base(message)
        {
        }
    }
}
