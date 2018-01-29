using BackgroundProcessing.Common;
using Hangfire;
using System;
using System.Linq.Expressions;

namespace BackgroundProcessing.Hangfire
{
    public class HangfireJobClient : IJobClient
    {
        public string AddDelayedJob(Expression<Action> methodCall, TimeSpan delay)
        {
            return BackgroundJob.Schedule(methodCall, delay);
        }

        public string AddDelayedJob(Expression<Action> methodCall, DateTimeOffset enqueueAt)
        {
            return BackgroundJob.Schedule(methodCall, enqueueAt);
        }

        public string AddScheduledJob(Expression<Action> methodCall)
        {
            throw new NotImplementedException();
        }

        public bool DeleteJob(string jobId)
        {
            return BackgroundJob.Delete(jobId);
        }

        public string EnqueAfter(string parentJobId, Expression<Action> methodCall)
        {
            return BackgroundJob.ContinueWith(parentJobId, methodCall);
        }

        public string Enqueue(Expression<Action> methodCall)
        {
            return BackgroundJob.Enqueue(methodCall);
        }

        public bool Requeue(string jobId)
        {
            return BackgroundJob.Requeue(jobId);
        }
    }
}
