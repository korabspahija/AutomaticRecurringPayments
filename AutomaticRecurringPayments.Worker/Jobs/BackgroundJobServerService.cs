using AutomaticRecurringPayments.Core.Interfaces;
using AutomaticRecurringPayments.Core.Services;
using Hangfire;
using Hangfire.SqlServer;
using System.Data.SqlClient;


namespace AutomaticRecurringPayments.Worker.Jobs
{
    public class BackgroundJobServerService : BackgroundService
    {
        private IServiceProvider Services { get; }
        private BackgroundJobServer BackgroundJobServer { get; set; }

        public string Status { get; set; } = "Stopped";

        public BackgroundJobServerService(
            IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {

                var options = new BackgroundJobServerOptions
                {
                    WorkerCount = 3,
                    Queues = new[] {
                        "schedule_recurrent_transaction",
                        "create_transaction"
                    }
                };

                if (BackgroundJobServer != null)
                    BackgroundJobServer.Dispose();

                var jobStorage = HangfireSetting.GetDefaultBackgroundJobStorage();

                BackgroundJobServer = new BackgroundJobServer(options, jobStorage);
                Status = "Running";

                var manager = new RecurringJobManager(jobStorage);

                manager.AddOrUpdate<IBraintreeTransactionJob>("schedule_recurrent_transaction", x => x.ScheduleRecurrentTransaction(null), cronExpression: "0 */12 * * *", queue: "schedule_recurrent_transaction");
            }

            return Task.CompletedTask;
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (BackgroundJobServer != null)
                BackgroundJobServer.Dispose();
            Status = "Stopped";
            return base.StopAsync(cancellationToken);
        }
    }
}
