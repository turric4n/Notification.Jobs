using Conditions;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.JobsLogger;
// using Hangfire.Pro.Redis;
using Hangfire.QuickJump;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Jobs.Commons;
using Notification.Jobs.Hangfire.Filters;
using Notification.Jobs.Hangfire.Reflection;
using QuickLogger.Extensions.NetCore;
using System;
using System.Linq;
using System.Threading;
using Hangfire.MemoryStorage;


namespace Notification.Jobs.Hangfire
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private Settings.Hangfire hangfireOptions;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddQuickLogger();

            hangfireOptions = Configuration.GetSection("Hangfire")
                .Get<Settings.Hangfire>();

            hangfireOptions
                .Requires(nameof(hangfireOptions))
                .IsNotNull()
                .Requires(nameof(hangfireOptions.RedisHost))
                .IsNotNull();

            var hangfireRunningEnv = hangfireOptions?.RunningEnvironment;

            hangfireRunningEnv ??= "Development";

            services
                .AddHangfire(configuration => configuration
                    .UseQuickJump()
                    .UseRecommendedSerializerSettings()
                    .UseJobsLogger()
                    .UseDashboardMetric(DashboardMetrics.AwaitingCount)
                    .UseDashboardMetric(DashboardMetrics.FailedCount)
                    .UseDashboardMetric(DashboardMetrics.SucceededCount)
                    .UseMemoryStorage());

                    //Redis storage if you have Hangfire.Pro license.
                    //.UseRedisStorage(
                    //    hangfireOptions?.RedisHost,
                    //    new RedisStorageOptions
                    //    {
                    //        Prefix = $"Notification:Jobs:Hangfire:{hangfireRunningEnv}:",
                    //        Database = hangfireOptions.RedisDb,
                    //    }));

            services.AddControllers();

            services.AddHangfireServer(options =>
            {
                options.ServerName = "Notification.Jobs";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var dashboardOptions =
                new DashboardOptions
                {
                    IgnoreAntiforgeryToken = true,
                    Authorization = new[] { new DashboardAuthorizationFilter() }
                };

            app.UseHangfireDashboard("", dashboardOptions);

            var assemblies = ReflectiveEnumerator
                .LoadAllBinDirectoryAssemblies("Notification.Jobs.*");

            assemblies.ForEach(a =>
            {
                var types = a
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(JobBase)) && !t.IsAbstract);

                foreach (var type in types)
                {
                    var instance = ActivatorUtilities.CreateInstance(app.ApplicationServices, type) as IJob;

                    var jobOptions = hangfireOptions.Jobs.
                        FirstOrDefault(x => x.Name == instance?.GetName());

                    if (jobOptions == null) return;

                    jobOptions.TimezoneName ??= "Romance Standard Time";

                    var timezone = TimeZoneInfo.FindSystemTimeZoneById(jobOptions.TimezoneName);

                    // That expression points to a very far expression in the future
                    jobOptions.CronExpression ??= "59 59 23 31 12 ? 2099";

                    var humanExpressionDescription =
                        CronExpressionDescriptor.ExpressionDescriptor.GetDescription(jobOptions.CronExpression);

                    Console.WriteLine($"Setting up job : {jobOptions.Name} - timeZone : {jobOptions.TimezoneName} - CronDescription : {humanExpressionDescription}");

                    RecurringJob.AddOrUpdate(jobOptions.Name,
                        () => instance.ExecuteAsync(CancellationToken.None, null), jobOptions.CronExpression, timezone);
                }
            });
        }
    }
}
