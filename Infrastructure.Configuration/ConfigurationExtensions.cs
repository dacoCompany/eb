using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Infrastructure.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Get value from configuration by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurationSection">The configuration section.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Value converted to specified value type
        /// </returns>
        /// <exception cref="System.Exception"></exception>
        public static T Get<T>(this NameValueCollection configurationSection, string key) where T : struct
        {
            var value = configurationSection.Get(key);

            /// TODO: create exception
            if (string.IsNullOrWhiteSpace(value)) throw new Exception(key);
            var converter = TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFromInvariantString(value);
        }
    }
}
