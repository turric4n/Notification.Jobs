using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Server;

namespace Notification.Jobs.Commons.Logger
{
    public interface IHangfireLogger
    {
        void AddHangfireContext(PerformContext context);
    }
}
