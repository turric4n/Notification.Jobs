namespace Notification.Jobs.Hangfire.Settings
{
    public class Job
    {
        public string Name {  get; set; }
        public string Description {  get; set; }
        public string CronExpression { get; set; }
        public string TimezoneName { get; set; }
    }
}
