using MvcThrottle;
using WebAPIFactory.Logging.Core;
using WebAPIFactory.Logging.Core.Diagnostics;

namespace Web.eBado
{
    public class EbadoThrottleLogger : IThrottleLogger
    {
        public void Log(ThrottleLogEntry entry)
        {
            string message = $"{entry.LogDate} Request {entry.RequestId} from {entry.ClientIp} has been blocked, quota {entry.RateLimit}/{entry.RateLimitPeriod} exceeded by {entry.TotalRequests}";
            EntlibLogger.LogInfo("Filter", "Throttled request", message, DiagnosticsLogging.Create("Global", nameof(EbadoThrottleFilter)));
        }
    }
}