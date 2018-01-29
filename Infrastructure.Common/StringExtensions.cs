using System.Linq;

namespace Infrastructure.Common
{
    public static class StringExtensions
    {
        public static bool IsAnyOf(this string input, params string[] values)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return values.Any(value => input == value);
            }

            return false;
        }

        public static string IsNullOrEmptyWithDefault(this string input, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return defaultValue;
            }

            return input;
        }
    }
}
