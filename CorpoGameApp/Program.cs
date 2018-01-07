using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Serilog;
using Serilog.Events;

namespace CorpoGameApp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "logs/LOG_.log",
                    rollingInterval: RollingInterval.Day, 
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 5242880, // 5 MB
                    retainedFileCountLimit: 7)
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                BuildWebHost(args).Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .UseSerilog()
                    .Build();

    }
}
