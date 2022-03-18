using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Appointment.Infrastructure;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;

namespace Appointment.Infrastructure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(config)
                            .CreateLogger();


            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            try
            {
                Log.Information("Starting host...");
                var webHost = CreateHostBuilder(args).Build();
                Log.Information("seeding endpoint rate limit rule");
                using (var scope = webHost.Services.CreateScope())
                {
                    var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();
                    clientPolicyStore.SeedAsync().GetAwaiter().GetResult();
                }

                webHost.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                    .UseSerilog()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();

                    });
    }
}
