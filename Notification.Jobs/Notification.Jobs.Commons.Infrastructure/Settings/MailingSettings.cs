namespace Notification.Jobs.Commons.Infrastructure.Settings
{
    public class MailingSettings
    {
        public string From { get; set; }
        public string DisplayName { get; set; }
        public bool CanSendEmailsOutSide { get; set; }
        public string CarbonCopy { get; set; }
        public string EmailApiHost { get; set; }
        public string OverrideDestinationMailbox { get; set; }
        public string[] NeverOverrideMailboxes { get; set; }
    }
}
