using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
            string instance = "gra";
            string webRootPath = Path.GetDirectoryName(".");

            // create a webhost to read configuration values for logging setup
            using (IWebHost configWebhost = CreateWebHostBuilder(args).Build())
            {
                using (IServiceScope scope = configWebhost.Services.CreateScope())
                {
                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    Log.Logger = new LogConfig().Build(config).CreateLogger();
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

            // run the application
            string applicationName
                = instance.ToLowerInvariant() != "gra" ? $"GRA {instance}" : "GRA";

            // output the version and revision
            try
            {
                Log.Warning("{0} {1} starting up in {2}",
                    applicationName,
                    new Version().GetVersion(),
                    webRootPath);

                webhost.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Warning("{0} {1} exited unexpectedly: {2}",
                    applicationName,
                    new Version().GetVersion(),
                    ex.Message);
                return 1;
            }
            finally
            {
                Log.Warning("{0} shutting down.", applicationName);
                Log.CloseAndFlush();
            }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
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
