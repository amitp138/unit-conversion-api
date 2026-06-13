using System;
using UnitConversionApi.Exceptions;
using UnitConversionApi.Services;
using Xunit;

namespace UnitConversionApi.Tests
{
    public class ConversionServiceTests
    {
        private readonly IConversionService _service;

        public ConversionServiceTests()
        {
            _service = new ConversionService();
        }

        [Theory]
        [InlineData(100, "meter", "feet", 328.084)]
        [InlineData(328.084, "feet", "meter", 100)]
        [InlineData(1, "kilometer", "mile", 0.621371)]
        [InlineData(0.621371, "mile", "kilometer", 1)]
        public void Convert_LengthConversions_ReturnsExpectedValues(double value, string fromUnit, string toUnit, double expected)
        {
            // Act
            var result = _service.Convert(value, fromUnit, toUnit);

            // Assert
            Assert.Equal(fromUnit.ToLowerInvariant(), result.From);
            Assert.Equal(toUnit.ToLowerInvariant(), result.To);
            Assert.Equal(value, result.OriginalValue);
            Assert.Equal(expected, result.ConvertedValue, 3); // Assert with 3 decimal precision
        }

        [Theory]
        [InlineData(0, "celsius", "fahrenheit", 32.0)]
        [InlineData(100, "celsius", "fahrenheit", 212.0)]
        [InlineData(32, "fahrenheit", "celsius", 0.0)]
        [InlineData(212, "fahrenheit", "celsius", 100.0)]
        public void Convert_TemperatureConversions_ReturnsExpectedValues(double value, string fromUnit, string toUnit, double expected)
        {
            // Act
            var result = _service.Convert(value, fromUnit, toUnit);

            // Assert
            Assert.Equal(fromUnit.ToLowerInvariant(), result.From);
            Assert.Equal(toUnit.ToLowerInvariant(), result.To);
            Assert.Equal(value, result.OriginalValue);
            Assert.Equal(expected, result.ConvertedValue, 3);
        }

        [Theory]
        [InlineData(10, "kilogram", "pound", 22.0462)]
        [InlineData(22.0462, "pound", "kilogram", 10.0)]
        public void Convert_WeightConversions_ReturnsExpectedValues(double value, string fromUnit, string toUnit, double expected)
        {
            // Act
            var result = _service.Convert(value, fromUnit, toUnit);

            // Assert
            Assert.Equal(fromUnit.ToLowerInvariant(), result.From);
            Assert.Equal(toUnit.ToLowerInvariant(), result.To);
            Assert.Equal(value, result.OriginalValue);
            Assert.Equal(expected, result.ConvertedValue, 4);
        }

        [Theory]
        [InlineData(100, "meter", "meter")]
        [InlineData(37.5, "celsius", "celsius")]
        [InlineData(5, "kilogram", "kilogram")]
        public void Convert_SelfConversion_ReturnsOriginalValue(double value, string fromUnit, string toUnit)
        {
            // Act
            var result = _service.Convert(value, fromUnit, toUnit);

            // Assert
            Assert.Equal(value, result.ConvertedValue);
            Assert.Equal(value, result.OriginalValue);
        }

        [Theory]
        [InlineData("Meter", "fEeT")]
        [InlineData("  kilometer  ", "MILE")]
        [InlineData("Celsius", "FAHRENHEIT")]
        public void Convert_CaseInsensitiveAndWhitespace_Succeeds(string fromUnit, string toUnit)
        {
            // Act
            var result = _service.Convert(10, fromUnit, toUnit);

            // Assert
            Assert.Equal(fromUnit.Trim().ToLowerInvariant(), result.From);
            Assert.Equal(toUnit.Trim().ToLowerInvariant(), result.To);
        }

        [Theory]
        [InlineData("meter", "celsius")]
        [InlineData("kilogram", "feet")]
        [InlineData("fahrenheit", "mile")]
        public void Convert_MismatchedCategories_ThrowsInvalidConversionException(string fromUnit, string toUnit)
        {
            // Act & Assert
            var ex = Assert.Throws<InvalidConversionException>(() => _service.Convert(10, fromUnit, toUnit));
            Assert.Contains("must belong to the same category", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("stone", "kilogram")]
        [InlineData("meter", "yard")]
        [InlineData("celsius", "kelvin")]
        public void Convert_UnsupportedUnits_ThrowsInvalidConversionException(string fromUnit, string toUnit)
        {
            // Act & Assert
            var ex = Assert.Throws<InvalidConversionException>(() => _service.Convert(10, fromUnit, toUnit));
            Assert.Contains("not supported", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("", "feet")]
        [InlineData("meter", null)]
        [InlineData("   ", "   ")]
        public void Convert_NullOrWhitespaceUnits_ThrowsInvalidConversionException(string? fromUnit, string? toUnit)
        {
            // Act & Assert
            Assert.Throws<InvalidConversionException>(() => _service.Convert(10, fromUnit!, toUnit!));
        }

        [Fact]
        public void Convert_SameCategoryButNotSupportedDirectPair_ThrowsInvalidConversionException()
        {
            // Act & Assert
            // meter and kilometer are both Length, but we only support meter<->feet and kilometer<->mile directly
            var ex = Assert.Throws<InvalidConversionException>(() => _service.Convert(10, "meter", "kilometer"));
            Assert.Contains("not directly supported", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
