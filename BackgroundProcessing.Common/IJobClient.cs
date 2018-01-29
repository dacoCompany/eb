using System;
using System.Linq.Expressions;

namespace BackgroundProcessing.Common
{
    public interface IJobClient
    {

        /// <summary>
        /// Creates fire and forget background job
        /// </summary>
        /// <param name="methodCall">The method call.</param>
        /// <returns>Id of job</returns>
        string Enqueue(Expression<Action> methodCall);

        bool Requeue(string jobId);

        string EnqueAfter(string parentJobId, Expression<Action> methodCall);

        string AddDelayedJob(Expression<Action> methodCall, TimeSpan delay);

        string AddDelayedJob(Expression<Action> methodCall, DateTimeOffset enqueueAt);

        string AddScheduledJob(Expression<Action> methodCall);

        bool DeleteJob(string jobId);
    }
}
