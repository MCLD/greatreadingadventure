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
            var issues = ViewTemplates.CopyToShared();

            // now that we have logging present in our config, we must create the webhost
            using (IWebHost webhost = CreateWebHostBuilder(args).Build())
            {
                string instance = null;
                string site = null;
                string webRootPath = null;
                string runtimeCacheConfig = null;

                // perform initialization
                using (IServiceScope scope = webhost.Services.CreateScope())
                {
                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    runtimeCacheConfig = config[ConfigurationKey.RuntimeCacheConfiguration];
                    instance = config[ConfigurationKey.InstanceName] ?? "n/a";

                    webRootPath = scope.ServiceProvider
                        .GetRequiredService<IHostingEnvironment>().WebRootPath;

                    Log.Logger = LogConfig.Build(config).CreateLogger();
                    Log.Warning("GRA v{Version} instance {Instance} starting up in {WebRootPath}",
                        new Version().GetVersion(),
                        instance,
                        webRootPath);

                    foreach (string issue in issues)
                    {
                        Log.Error(issue);
                    }

                    Task.Run(() => new Web(scope).InitalizeAsync()).Wait();
                }

                // output the version and revision
                try
                {
                    Log.Information(runtimeCacheConfig);
                    webhost.Run();
                    return 0;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
                {
                    Log.Warning("GRA v{Version} {Instance} {Site} exited unexpectedly: {Message}",
                        new Version().GetVersion(),
                        instance,
                        site,
                        ex.Message);
                    return 1;
                }
#pragma warning restore CA1031 // Do not catch general exception types
                finally
                {
                    Log.Warning("GRA v{Version} {Instance} {Site} shutting down.",
                        new Version().GetVersion(),
                        instance,
                        site);
                    Log.CloseAndFlush();
                }
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
