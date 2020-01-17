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
                string webRootPath = null;

                // perform initialization
                using (IServiceScope scope = webhost.Services.CreateScope())
                {
                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    instance = config[ConfigurationKey.InstanceName] ?? "n/a";

                    webRootPath = scope.ServiceProvider
                        .GetRequiredService<IHostingEnvironment>().WebRootPath;

                    Log.Logger = LogConfig.Build(config).CreateLogger();
                    Log.Information("GRA v{Version} instance {Instance} starting up in {WebRootPath}",
                        new Version().GetVersion(),
                        instance,
                        webRootPath);

                    if (!string.IsNullOrEmpty(config["DOTNET_RUNNING_IN_CONTAINER"]))
                    {
                        Log.Information("Containerized: commit {ContainerCommit} created on {ContainerDate} image {ContainerImageVersion}",
                            config["org.opencontainers.image.revision"] ?? "unknown",
                            config["org.opencontainers.image.created"] ?? "unknown",
                            config["org.opencontainers.image.version"] ?? "unknown");
                    }

                    foreach (string issue in issues)
                    {
                        Log.Error(issue);
                    }

                    switch (config[ConfigurationKey.DistributedCache]?.ToLower(Culture.DefaultCulture))
                    {
                        case "redis":
                            Log.Information("Cache: Redis config {RedisConfig} discriminator {RedisDiscriminator}",
                                config[ConfigurationKey.RuntimeCacheRedisConfiguration],
                                config[ConfigurationKey.RuntimeCacheRedisInstance]);
                            break;
                        case "sqlserver":
                            Log.Information("Cache: SQL Server in table {SQLCacheTable}",
                                config[ConfigurationKey.RuntimeCacheSqlConfiguration]);
                            break;
                        default:
                            Log.Information("Cache: in-memory");
                            break;
                    }

                    Task.Run(() => new Web(scope).InitalizeAsync()).Wait();
                }

                // output the version and revision
                try
                {
                    webhost.Run();
                    return 0;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
                {
                    Log.Warning("GRA v{Version} {Instance} exited unexpectedly: {Message}",
                        new Version().GetVersion(),
                        instance,
                        ex.Message);
                    return 1;
                }
#pragma warning restore CA1031 // Do not catch general exception types
                finally
                {
                    Log.Warning("GRA v{Version} {Instance} shutting down.",
                        new Version().GetVersion(),
                        instance);
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
