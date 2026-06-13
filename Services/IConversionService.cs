using UnitConversionApi.Models;

namespace UnitConversionApi.Services
{
    /// <summary>
    /// Service interface for handling unit conversion business logic.
    /// </summary>
    public interface IConversionService
    {
        /// <summary>
        /// Converts a value from one unit to another.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <param name="fromUnit">The unit to convert from.</param>
        /// <param name="toUnit">The unit to convert to.</param>
        /// <returns>A conversion response detailing the original and converted values.</returns>
        /// <exception cref="Exceptions.InvalidConversionException">Thrown when unit conversion is invalid or unsupported.</exception>
        ConversionResponse Convert(double value, string fromUnit, string toUnit);
    }
}
