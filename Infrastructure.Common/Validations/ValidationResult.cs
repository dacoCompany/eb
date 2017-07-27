namespace Infrastructure.Common.Validations
{
    public class ValidationResult
    {
        /// <summary>
        /// Gets or sets the name of parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of validation result.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Parameter: {0}, Description: {1}", Name, Description);
        }
    }
}
