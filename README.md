# Notifications and Scheduled Jobs framework for .NET
Fresh Hangfire framework to manage Scheduled jobs with some adventages :

- Put jobs in another projects inside the same solution
- Import .NET jobs from another projects
- Not only hangfire (future)
- Cron expressions

# Getting Started

Add job base instance in a new project inside same solution (Warning! Namespace of the project need to be : Notification.Jobs) as :

    Notification.Jobs.TestJob.cprog
    
    public class TurriJob : JobBase
    {
        private readonly ILogger _logger;
        private readonly PerformContext _context;

        private const bool Enabled = true;

        private const string JobName = "Test Job";

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

                    _logger.LogInformation($"Job Started - {JobName}");                                       
                    
                    // Put JOB code here!

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
    
Update your settings file as :

    Jobs:    
      ###########################################
    - Name: Test Job
      Description : Only for readability purposes    
      CronExpression: "* * * * *" # every minute
      TimezoneName : Romance Standard Time
      ###########################################
      
Run the program
