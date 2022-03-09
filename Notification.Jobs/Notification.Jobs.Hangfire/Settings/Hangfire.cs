using System.Collections.Generic;
using Hangfire.Common;

namespace Notification.Jobs.Hangfire.Settings
{
    public class Hangfire
    {
        public List<Job> Jobs { get; set; }
        public string RedisHost { get; set; }
        public int? RedisDb { get; set; }
        public string RunningEnvironment { get; set; }
    }
}
