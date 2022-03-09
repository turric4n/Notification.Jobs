using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Notification.Jobs.Commons;

namespace Notification.Jobs.TestJob
{
    public class TestJob : JobBase

    {
        private const bool Enabled = true;

        private const string JobName = "TestJob";

        public TestJob(ILogger<TestJob> logger) : base(logger)
        {
        }

        [AutomaticRetry(Attempts = 0)]
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

            IsRunning = true;

            await Task.Run(() =>
            {
                try
                {
                    string correlationId = Guid.NewGuid().ToString();

                    _logger.LogInformation($"Job Started - {JobName}");

                    _logger.LogInformation($"Job Stopped - {JobName}");
                }

                finally
                {
                    IsRunning = false;
                }

            }, cancellationToken);
        }
    }
}
