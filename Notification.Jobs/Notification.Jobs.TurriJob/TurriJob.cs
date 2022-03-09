using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Notification.Jobs.Commons;

namespace Notification.Jobs.TurriJob
{
    public class TurriJob : JobBase
    {
        private readonly ILogger _logger;
        private readonly PerformContext _context;

        private const bool Enabled = true;

        private const string JobName = "TurriJob in antoher project!";

        public TurriJob(ILogger<TurriJob> logger) : base(logger)
        {
            base.JobName = JobName;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 0)]
        [DisableConcurrentExecution(60)]
        public override async Task ExecuteAsync(CancellationToken cancellationToken, PerformContext context)
        {
            base.Init(context);

            if (!Enabled)
            {
                _logger.LogInformation($"Job Execution is disabled - {JobName} - ");
                return;
            }

            if (IsRunning)
            {
                _logger.LogInformation($"Job Another instance is executing. Bye bye!  {JobName}");
                return;
            }

            lock (this)
            {
                IsRunning = true;
            }

            await Task.Run(() =>
            {
                try
                {
                    string correlationId = Guid.NewGuid().ToString();

                    Thread.Sleep(5000);

                    _logger.LogInformation($"Job Started - {JobName}");

                    _logger.LogInformation($"Job Stopped - {JobName}");
                }

                finally
                {
                    lock (this)
                    {
                        IsRunning = false;
                    }
                }

            }, cancellationToken);
        }
    }
}
