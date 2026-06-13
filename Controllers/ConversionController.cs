using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Exceptions;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers
{
    /// <summary>
    /// Controller for unit conversion operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ConversionController : ControllerBase
    {
        private readonly IConversionService _conversionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConversionController"/> class.
        /// </summary>
        /// <param name="conversionService">The conversion service implementation.</param>
        public ConversionController(IConversionService conversionService)
        {
            _conversionService = conversionService;
        }

        /// <summary>
        /// Converts a value from one unit to another.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <param name="from">The unit to convert from (e.g., meter, feet, kilometer, mile, celsius, fahrenheit, kilogram, pound).</param>
        /// <param name="to">The unit to convert to (e.g., meter, feet, kilometer, mile, celsius, fahrenheit, kilogram, pound).</param>
        /// <returns>A conversion response detailing the original and converted values.</returns>
        /// <response code="200">The unit conversion was successful.</response>
        /// <response code="400">The conversion request was invalid (e.g., missing parameters, unsupported units, or mismatched categories).</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<ConversionResponse> GetConversion(
            [FromQuery] double? value, 
            [FromQuery] string? from, 
            [FromQuery] string? to)
        {
            if (!value.HasValue)
            {
                throw new InvalidConversionException("The 'value' parameter is required and must be a valid number.");
            }

            if (string.IsNullOrWhiteSpace(from))
            {
                throw new InvalidConversionException("The 'from' parameter is required and cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(to))
            {
                throw new InvalidConversionException("The 'to' parameter is required and cannot be empty.");
            }

            var response = _conversionService.Convert(value.Value, from, to);
            return Ok(response);
        }
    }
}
