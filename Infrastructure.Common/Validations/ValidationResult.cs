namespace Infrastructure.Common.Validations
{
    public class ValidationResult
    {
        /// <summary>
        /// Gets or sets the name of parameter.
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the description of validation result.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        public override string ToString()
        {
            return string.Format("ParameterName: {0}, ErrorMessage: {1}", ParameterName, ErrorMessage);
        }
    }
}
