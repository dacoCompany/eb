using Hangfire;
using Hangfire.SqlServer;
using Infrastructure.Common;
using Infrastructure.Configuration;
using Ninject;
using System;
using System.Configuration;

namespace BackgroundProcessing.Hangfire
{
    public class HangfireConfiguration
    {
        public const string DefaultPrepareSchemeIfNecessary = "true";
        public const string DefaultScheme = "Hangfire";
        public const string DefaultDashboardJobListLimit = "10000";
        public const string DefaultQueuePollInterval = "15";
        public const string DefaultJobExpirationCheckInterval = "1800";
        public const string DefaultQueue = "default";
        public const string DefaultAppPath = "/";
        public readonly string DefaultWorkerCount = (Environment.ProcessorCount * 5).ToString();

        private SqlServerStorageOptions storageOptions;
        private BackgroundJobServerOptions serverOptions;
        private DashboardOptions dashboardOptions;

        public SqlServerStorageOptions StorageOptions { get { return storageOptions; } }
        public BackgroundJobServerOptions ServerOptions { get { return serverOptions; } }
        public DashboardOptions DashboardOptions { get { return dashboardOptions; } }


        public HangfireConfiguration(IKernel kernel)
        {
            storageOptions = new SqlServerStorageOptions();
            serverOptions = new BackgroundJobServerOptions();
            dashboardOptions = new DashboardOptions();
            Configure(kernel);
        }

        private void Configure(IKernel kernel)
        {
            var configuration = ConfigurationManager.GetSection("hangfire") as HangfireConfigurationSection;

            // configuration of job storage
            storageOptions.DashboardJobListLimit = int.Parse(configuration.StorageConfiguration["DashboardJobListLimit"].Value?.IsNullOrEmptyWithDefault(DefaultDashboardJobListLimit));
            storageOptions.JobExpirationCheckInterval = TimeSpan.FromSeconds(double.Parse(configuration.StorageConfiguration["JobExpirationCheckInterval"].Value?.IsNullOrEmptyWithDefault(DefaultJobExpirationCheckInterval)));
            storageOptions.QueuePollInterval = TimeSpan.FromSeconds(double.Parse(configuration.StorageConfiguration["QueuePollInterval"].Value?.IsNullOrEmptyWithDefault(DefaultQueuePollInterval)));
            storageOptions.PrepareSchemaIfNecessary = bool.Parse(configuration.StorageConfiguration["PrepareSchemeIfNecessary"].Value?.IsNullOrEmptyWithDefault(DefaultPrepareSchemeIfNecessary));
            storageOptions.SchemaName = configuration.StorageConfiguration["DefaultScheme"].Value?.IsNullOrEmptyWithDefault(DefaultScheme);

            // configuration of background job server
            serverOptions.WorkerCount = int.Parse(configuration.ServerConfiguration["WorkerCount"].Value?.IsNullOrEmptyWithDefault(DefaultWorkerCount));
            serverOptions.Queues = configuration.Queues.AllKeys ?? new string[] { DefaultQueue };

            // configuration of dashboard
            // TODO: authorization
            // dashboardOptions.Authorization = null;
            dashboardOptions.AppPath = configuration.DashboardConfiguration["AppPath"].Value?.IsNullOrEmptyWithDefault(DefaultAppPath);
            // dashboardOptions.StatsPollingInterval = ;

            string connectionString = ConfigurationManager.ConnectionStrings["dbsEbadoMaster"].ConnectionString;

            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString, storageOptions);
            GlobalConfiguration.Configuration.UseNinjectActivator(kernel);
            GlobalConfiguration.Configuration.UseEntLibLogProvider();
        }
    }
}
