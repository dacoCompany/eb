using System.Configuration;

namespace Infrastructure.Configuration
{
    public class HangfireConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("serverConfiguration")]
        public KeyValueConfigurationCollection ServerConfiguration
        {
            get { return (KeyValueConfigurationCollection)this["serverConfiguration"]; }
        }

        [ConfigurationProperty("storageConfiguration")]
        public KeyValueConfigurationCollection StorageConfiguration
        {
            get { return (KeyValueConfigurationCollection)this["storageConfiguration"]; }
        }

        [ConfigurationProperty("dashboardConfiguration")]
        public KeyValueConfigurationCollection DashboardConfiguration
        {
            get { return (KeyValueConfigurationCollection)this["dashboardConfiguration"]; }
        }

        [ConfigurationProperty("queues")]
        public KeyValueConfigurationCollection Queues
        {
            get { return (KeyValueConfigurationCollection)this["queues"]; }
        }
    }
}
