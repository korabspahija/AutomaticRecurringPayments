using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using AutomaticRecurringPayments.Core.Services;
using AutomaticRecurringPayments.Worker.Jobs;
using Hangfire;

namespace AutomaticRecurringPayments.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            GlobalConfiguration.Configuration.UseStorage(HangfireSetting.GetDefaultBackgroundJobStorage());
            builder.Services.StartHangfire(); 
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();


            app.Run();
        }
    }

    public static class StartupExtension
    {
        public static void StartHangfire(this IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(services);
            var lifetimescope = containerBuilder.Build();
            services.AddSingleton<IContainer>(x => lifetimescope);
            GlobalConfiguration.Configuration.UseAutofacActivator(lifetimescope);


            services.AddSingleton<BackgroundJobServerService>();
            services.AddHostedService(s => s.GetRequiredService<BackgroundJobServerService>());
        }
    }
}