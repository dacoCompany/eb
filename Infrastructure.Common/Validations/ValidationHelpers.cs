namespace Infrastructure.Common.Validations
{
    public class ValidationHelpers
    {
        public void AddValidationResult(ValidationResultCollection collection, string parameter, string description)
        {
            collection.Add(new ValidationResult
            {
                Name = parameter,
                Description = description,
            });
        }
    }
}
