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

            var issues = ViewTemplates.CopyToShared();

            // create a webhost to read configuration values for logging setup
            using (IWebHost configWebhost = CreateWebHostBuilder(args).Build())
            {
                using (IServiceScope scope = configWebhost.Services.CreateScope())
                {
                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    Log.Logger = LogConfig.Build(config).CreateLogger();
                    if (!string.IsNullOrEmpty(config[ConfigurationKey.InstanceName]))
                    {
                        instance = " instance " + config[ConfigurationKey.InstanceName];
                    }
                }
            }

            // now that we have logging present in our config, we must create the webhost
            IWebHost webhost = CreateWebHostBuilder(args).Build();

            foreach (string issue in issues)
            {
                Log.Error(issue);
            }

            // perform initialization
            using (IServiceScope scope = webhost.Services.CreateScope())
            {
                var web = new Web(scope);
                Task.Run(() => web.InitalizeAsync()).Wait();
                webRootPath = scope.ServiceProvider
                    .GetRequiredService<IHostingEnvironment>().WebRootPath;
            }

            string appDetails = $"{new Version().GetVersion()}{instance}";

            // output the version and revision
            try
            {
                Log.Warning("GRA {AppDetails} starting up in {webRootPath}",
                    appDetails,
                    webRootPath);
                webhost.Run();
                return 0;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                Log.Warning("GRA {AppDetails} exited unexpectedly: {Message}",
                    appDetails,
                    ex.Message);
                return 1;
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
                Log.Warning("GRA {AppDetails} shutting down.",
                    appDetails);
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
