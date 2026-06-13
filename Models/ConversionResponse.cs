namespace UnitConversionApi.Models
{
    /// <summary>
    /// Represents the result of a unit conversion operation.
    /// </summary>
    public class ConversionResponse
    {
        /// <summary>
        /// The original value that was converted.
        /// </summary>
        public double OriginalValue { get; set; }

        /// <summary>
        /// The unit from which conversion was performed.
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// The unit to which conversion was performed.
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// The result of the unit conversion.
        /// </summary>
        public double ConvertedValue { get; set; }
    }
}
