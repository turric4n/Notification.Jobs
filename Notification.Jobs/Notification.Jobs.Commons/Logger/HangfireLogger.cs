using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Server;
using Hangfire.JobsLogger;
using Microsoft.Extensions.Logging;

namespace Notification.Jobs.Commons.Logger
{
    public class HangfireLogger : ILogger, IHangfireLogger
    {
        private readonly ILogger _logger;
        private PerformContext _context;

        public HangfireLogger(ILogger logger, PerformContext context = null)
        {
            _logger = logger;
            _context = context;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel,eventId,state,exception,formatter);
            _context.Log(logLevel, state.ToString());
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public void AddHangfireContext(PerformContext context)
        {
            _context = context;
        }
    }
}
