using System.Collections.Specialized;
using System.Configuration;

namespace Infrastructure.Configuration
{
    public static class EbadoConfiguration
    {

        public static NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }
    }
}
