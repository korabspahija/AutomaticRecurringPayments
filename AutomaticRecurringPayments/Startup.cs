using AutomaticRecurringPayments.Core.DatabaseContexts;
using AutomaticRecurringPayments.Extensions.DependencyInjections;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace AutomaticRecurringPayments
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            CurrentEnvironment = environment;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowTrackingControllers", builder =>
                {
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowAnyOrigin();
                });
            });

            services.AddHealthChecks();

            services.AddMappingWithProfiles();
            
            services.AddConfigurations(Configuration);

            services.AddDbContext<DatabaseContext>(builder =>
            {
                if (!builder.IsConfigured)
                {
                    builder.UseSqlServer(Configuration.GetSection("DbContextSettings").GetSection("ConnectionString").Value);
                }
            });

            services.AddHttpContextAccessor();
            services.AddMediatR();
            services.AddServices();
            services.RegisterHangfire();
            services.RegisterControllers();
            services.AddSwagger(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IServiceProvider provider)
        {
            app.UseCors("AllowTrackingControllers");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerDI();
            }
            else
            {
                app.UseExceptionHandler(c => c.Run(async context =>
                {
                    var exception = context.Features
                        .Get<IExceptionHandlerPathFeature>()
                        .Error;
                    var response = new { error = "Internal Server Error!" };
                    await context.Response.WriteAsJsonAsync(response);
                }));
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();

            app.UseHangfireDefaultDashboard();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.Redirect("/swagger", permanent: false);
                    await Task.CompletedTask;
                });
                endpoints.MapHealthChecks("/healthcheck");
                endpoints.MapControllers();
            });

        }
    }
}
