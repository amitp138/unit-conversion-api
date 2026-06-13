using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Controllers;
using UnitConversionApi.Exceptions;
using UnitConversionApi.Models;
using UnitConversionApi.Services;
using Xunit;

namespace UnitConversionApi.Tests
{
    public class ConversionControllerTests
    {
        private readonly MockConversionService _mockService;
        private readonly ConversionController _controller;

        public ConversionControllerTests()
        {
            _mockService = new MockConversionService();
            _controller = new ConversionController(_mockService);
        }

        [Fact]
        public void GetConversion_ValidRequest_CallsServiceAndReturnsOk()
        {
            // Arrange
            double testValue = 100;
            string testFrom = "meter";
            string testTo = "feet";
            var mockResponse = new ConversionResponse
            {
                OriginalValue = testValue,
                From = testFrom,
                To = testTo,
                ConvertedValue = 328.084
            };
            _mockService.ResponseToReturn = mockResponse;

            // Act
            var actionResult = _controller.GetConversion(testValue, testFrom, testTo);

            // Assert
            Assert.True(_mockService.ConvertCalled);
            Assert.Equal(testValue, _mockService.LastValue);
            Assert.Equal(testFrom, _mockService.LastFromUnit);
            Assert.Equal(testTo, _mockService.LastToUnit);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<ConversionResponse>(okResult.Value);
            Assert.Equal(mockResponse.ConvertedValue, response.ConvertedValue);
        }

        [Fact]
        public void GetConversion_MissingValue_ThrowsInvalidConversionException()
        {
            // Act & Assert
            var ex = Assert.Throws<InvalidConversionException>(() => _controller.GetConversion(null, "meter", "feet"));
            Assert.Contains("'value' parameter is required", ex.Message, System.StringComparison.OrdinalIgnoreCase);
            Assert.False(_mockService.ConvertCalled);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void GetConversion_MissingFromUnit_ThrowsInvalidConversionException(string? fromUnit)
        {
            // Act & Assert
            var ex = Assert.Throws<InvalidConversionException>(() => _controller.GetConversion(100, fromUnit, "feet"));
            Assert.Contains("'from' parameter is required", ex.Message, System.StringComparison.OrdinalIgnoreCase);
            Assert.False(_mockService.ConvertCalled);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void GetConversion_MissingToUnit_ThrowsInvalidConversionException(string? toUnit)
        {
            // Act & Assert
            var ex = Assert.Throws<InvalidConversionException>(() => _controller.GetConversion(100, "meter", toUnit));
            Assert.Contains("'to' parameter is required", ex.Message, System.StringComparison.OrdinalIgnoreCase);
            Assert.False(_mockService.ConvertCalled);
        }
    }

    /// <summary>
    /// A clean, handwritten mock of <see cref="IConversionService"/> to test the controller in isolation.
    /// </summary>
    public class MockConversionService : IConversionService
    {
        public bool ConvertCalled { get; private set; }
        public double LastValue { get; private set; }
        public string LastFromUnit { get; private set; } = string.Empty;
        public string LastToUnit { get; private set; } = string.Empty;
        public ConversionResponse ResponseToReturn { get; set; } = new();

        public ConversionResponse Convert(double value, string fromUnit, string toUnit)
        {
            ConvertCalled = true;
            LastValue = value;
            LastFromUnit = fromUnit;
            LastToUnit = toUnit;
            return ResponseToReturn;
        }
    }
}
