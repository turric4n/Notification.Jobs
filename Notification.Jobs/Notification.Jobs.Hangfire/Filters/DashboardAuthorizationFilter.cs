using Hangfire.Dashboard;

namespace Notification.Jobs.Hangfire.Filters
{
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext dashboardContext)
        {
            // Do something with context, up to you
            return true;
        }
    }
}
