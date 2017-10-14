namespace Infrastructure.Common.Validations
{
    public static class ValidationHelpers
    {
        public static void AddValidationResult(ValidationResultCollection collection, string parameter, string description)
        {
            collection.Add(new ValidationResult
            {
                ParameterName = parameter,
                ErrorMessage = description,
            });
        }
    }
}
