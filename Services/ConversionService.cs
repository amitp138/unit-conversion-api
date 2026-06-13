using System;
using System.Collections.Generic;
using UnitConversionApi.Exceptions;
using UnitConversionApi.Models;

namespace UnitConversionApi.Services
{
    /// <summary>
    /// Service implementation for handling unit conversion logic.
    /// </summary>
    public class ConversionService : IConversionService
    {
        private delegate double ConversionFunc(double value);

        // Registry of supported conversion formulas (case-insensitive keys)
        private static readonly Dictionary<string, ConversionFunc> Conversions = new(StringComparer.OrdinalIgnoreCase)
        {
            { "meter->feet", val => val * 3.28084 },
            { "feet->meter", val => val / 3.28084 },
            { "kilometer->mile", val => val * 0.621371 },
            { "mile->kilometer", val => val / 0.621371 },
            { "celsius->fahrenheit", val => (val * 9.0 / 5.0) + 32.0 },
            { "fahrenheit->celsius", val => (val - 32.0) * 5.0 / 9.0 },
            { "kilogram->pound", val => val * 2.20462 },
            { "pound->kilogram", val => val / 2.20462 }
        };

        // Registry mapping units to their measurement category for validation and messaging
        private static readonly Dictionary<string, string> UnitCategories = new(StringComparer.OrdinalIgnoreCase)
        {
            { "meter", "Length" },
            { "feet", "Length" },
            { "kilometer", "Length" },
            { "mile", "Length" },
            { "celsius", "Temperature" },
            { "fahrenheit", "Temperature" },
            { "kilogram", "Weight" },
            { "pound", "Weight" }
        };

        /// <inheritdoc />
        public ConversionResponse Convert(double value, string fromUnit, string toUnit)
        {
            if (string.IsNullOrWhiteSpace(fromUnit))
            {
                throw new InvalidConversionException("The 'from' unit parameter is required.");
            }

            if (string.IsNullOrWhiteSpace(toUnit))
            {
                throw new InvalidConversionException("The 'to' unit parameter is required.");
            }

            fromUnit = fromUnit.Trim();
            toUnit = toUnit.Trim();

            // Self-conversion is trivially supported if the unit exists
            if (string.Equals(fromUnit, toUnit, StringComparison.OrdinalIgnoreCase))
            {
                if (!UnitCategories.ContainsKey(fromUnit))
                {
                    throw new InvalidConversionException($"Unit '{fromUnit}' is not supported.");
                }

                return new ConversionResponse
                {
                    OriginalValue = value,
                    From = fromUnit.ToLowerInvariant(),
                    To = toUnit.ToLowerInvariant(),
                    ConvertedValue = value
                };
            }

            bool fromExists = UnitCategories.TryGetValue(fromUnit, out var fromCategory);
            bool toExists = UnitCategories.TryGetValue(toUnit, out var toCategory);

            if (!fromExists && !toExists)
            {
                throw new InvalidConversionException($"Units '{fromUnit}' and '{toUnit}' are not supported.");
            }

            if (!fromExists)
            {
                throw new InvalidConversionException($"Unit '{fromUnit}' is not supported.");
            }

            if (!toExists)
            {
                throw new InvalidConversionException($"Unit '{toUnit}' is not supported.");
            }

            if (fromCategory != toCategory)
            {
                throw new InvalidConversionException($"Cannot convert '{fromUnit}' ({fromCategory}) to '{toUnit}' ({toCategory}). Units must belong to the same category.");
            }

            string conversionKey = $"{fromUnit}->{toUnit}";
            if (Conversions.TryGetValue(conversionKey, out var conversionFunc))
            {
                double rawResult = conversionFunc(value);
                
                // Rounding to 6 decimal places to prevent floating-point inaccuracies
                // while keeping high precision for technical usage.
                double roundedResult = Math.Round(rawResult, 6);

                return new ConversionResponse
                {
                    OriginalValue = value,
                    From = fromUnit.ToLowerInvariant(),
                    To = toUnit.ToLowerInvariant(),
                    ConvertedValue = roundedResult
                };
            }

            // Units are in the same category, but are not a supported direct pair (e.g. meter <-> mile)
            throw new InvalidConversionException($"Conversion from '{fromUnit}' to '{toUnit}' is not directly supported. Supported conversion pairs in the '{fromCategory}' category are: " +
                GetSupportedPairsForCategory(fromCategory ?? string.Empty));
        }

        private static string GetSupportedPairsForCategory(string category)
        {
            return category switch
            {
                "Length" => "meter <-> feet, kilometer <-> mile",
                "Temperature" => "celsius <-> fahrenheit",
                "Weight" => "kilogram <-> pound",
                _ => string.Empty
            };
        }
    }
}
