using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GRA.Web
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            string instance = null;
            string webRootPath = null;

            // create a webhost to read configuration values for logging setup
            using (IWebHost configWebhost = CreateWebHostBuilder(args).Build())
            {
                using (IServiceScope scope = configWebhost.Services.CreateScope())
                {
                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    Log.Logger = new LogConfig().Build(config).CreateLogger();
                    if (!string.IsNullOrEmpty(config[ConfigurationKey.InstanceName]))
                    {
                        instance = " instance " + config[ConfigurationKey.InstanceName];
                    }
                }
            }

            // now that we have logging present in our config, we must create the webhost
            IWebHost webhost = CreateWebHostBuilder(args).Build();

            // perform initialization
            using (IServiceScope scope = webhost.Services.CreateScope())
            {
                var web = new Web(scope);
                Task.Run(() => web.InitalizeAsync()).Wait();
                webRootPath = scope.ServiceProvider
                    .GetRequiredService<IHostingEnvironment>().WebRootPath;
            }

            string appDetails = string.Format("GRA {0}{1}", new Version().GetVersion(), instance);

            // output the version and revision
            try
            {
                Log.Warning(appDetails + " starting up in {webRootPath}", webRootPath);
                webhost.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Warning(appDetails + " exited unexpectedly: {Message}", ex.Message);
                return 1;
            }
            finally
            {
                Log.Warning(appDetails + " shutting down.");
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, config) =>
                {
                    config.AddJsonFile("shared/appsettings.json",
                        optional: true,
                        reloadOnChange: true)
                    .AddEnvironmentVariables();
                })
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
