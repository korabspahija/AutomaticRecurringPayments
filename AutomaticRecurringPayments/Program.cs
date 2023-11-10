
using AutomaticRecurringPayments.Core.DatabaseContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace AutomaticRecurringPayments
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    await db.Database.MigrateAsync();
                    await DatabaseContextInitializer.InitializeAsync();
                }
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Done");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}