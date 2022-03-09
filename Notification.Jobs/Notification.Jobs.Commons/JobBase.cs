using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Notification.Jobs.Commons.Logger;

namespace Notification.Jobs.Commons
{
    public abstract class JobBase : IJob
    {
        protected readonly ILogger _logger;

        protected JobBase(ILogger logger)
        {
            _logger = logger;
            _logger = new HangfireLogger(_logger);
        }

        protected bool Enabled { get; set; }

        protected static bool IsRunning = false;

        protected void Init(PerformContext context)
        {

            ((IHangfireLogger)_logger).AddHangfireContext(context);
        }

        public abstract Task ExecuteAsync(CancellationToken cancellationToken, PerformContext context);

        public string GetName()
        {
            return JobName;
        }

        protected string JobName { get; set; }
    }
}
