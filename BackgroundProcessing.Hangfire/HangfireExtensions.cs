using Hangfire;
using Owin;

namespace BackgroundProcessing.Hangfire
{
    public static class HangfireExtensions
    {
        public static IAppBuilder UseHangfireJobServer(this IAppBuilder builder, BackgroundJobServerOptions options)
        {
            return builder.UseHangfireServer(options);
        }

        public static IAppBuilder UseHangfireJobDashboard(this IAppBuilder builder, string path, DashboardOptions options)
        {
            return builder.UseHangfireDashboard(path, options);
        }
    }
}
