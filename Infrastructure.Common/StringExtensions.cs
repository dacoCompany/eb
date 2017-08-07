using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
