using System.Collections.Specialized;
using System.Configuration;

namespace Infrastructure.Configuration
{
    public static class GlobalConfiguration
    {
        public static NameValueCollection EmailConfiguration = ConfigurationManager.GetSection(ConfigurationSections.EmailConfiguration) as NameValueCollection;
        public static NameValueCollection SmsConfiguration = ConfigurationManager.GetSection(ConfigurationSections.SmsConfiguration) as NameValueCollection;
    }
}
