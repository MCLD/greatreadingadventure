using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GRA.Web
{
    public static class Program
    {
        private const string DevEnvironment = "Development";
        private const string EnvAspNetCoreEnv = "ASPNETCORE_ENVIRONMENT";
        private const string EnvRunningInContainer = "DOTNET_RUNNING_IN_CONTAINER";

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var contentRoot
                = Environment.GetEnvironmentVariable(EnvAspNetCoreEnv) == DevEnvironment
                    ? System.IO.Directory.GetCurrentDirectory()
                    : System.AppContext.BaseDirectory;

            return Host.CreateDefaultBuilder(args)
                       .ConfigureWebHostDefaults(webBuilder =>
                       {
                           webBuilder.UseContentRoot(contentRoot);
                           webBuilder.ConfigureAppConfiguration((_, config) =>
                           {
                               config.AddJsonFile("shared/appsettings.json",
                                   optional: true,
                                   reloadOnChange: true)
                               .AddInMemoryCollection(new Dictionary<string, string>
                               {
                                    { ConfigurationKey.InternalContentPath, contentRoot }
                               })
                               .AddEnvironmentVariables();
                           });
                           webBuilder.ConfigureKestrel(_ => { }).UseStartup<Startup>();
                       })
                       .UseSerilog();
        }

        public static int Main(string[] args)
        {
            using var webhost = CreateHostBuilder(args).Build();

            using var scope = webhost.Services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var instance = config[ConfigurationKey.InstanceName] ?? "n/a";

            var webHostEnvironment = scope.ServiceProvider
                .GetRequiredService<IWebHostEnvironment>();

            Log.Logger = LogConfig.Build(config).CreateLogger();
            Log.Information("GRA v{Version} instance {Instance} environment {Environment} in {WebRootPath} with content root {ContentRoot}",
                Version.GetVersion(),
                instance,
                config[EnvAspNetCoreEnv] ?? "Production",
                webHostEnvironment.WebRootPath,
                config[ConfigurationKey.InternalContentPath]);

            if (!string.IsNullOrEmpty(config[EnvRunningInContainer]))
            {
                Log.Information("Containerized: commit {ContainerCommit} created on {ContainerDate} image {ContainerImageVersion}",
                    config["org.opencontainers.image.revision"] ?? "unknown",
                    config["org.opencontainers.image.created"] ?? "unknown",
                    config["org.opencontainers.image.version"] ?? "unknown");
            }

            ViewTemplates.CopyToShared(config[ConfigurationKey.InternalContentPath], Log.Logger);

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
            try
            {
                Task.Run(() => new WebStartup(scope).InitalizeAsync()).Wait();
            }
            catch (StackExchange.Redis.RedisConnectionException ex)
            {
                Log.Fatal("Redis is configured for {RedisConfig} but failed: {ErrorMessage}",
                    config[ConfigurationKey.RuntimeCacheRedisConfiguration],
                    ex.Message);
                throw;
            }

            if (!string.IsNullOrEmpty(config[ConfigurationKey.WorkerThreads])
                || !string.IsNullOrEmpty(config[ConfigurationKey.CompletionPortThreads]))
            {
                ThreadPool.GetMinThreads(out int minThreads, out int minCompletionPortThreads);
                var setThreads = minThreads;
                var setCompletionPortThreads = minCompletionPortThreads;
                if (!string.IsNullOrEmpty(config[ConfigurationKey.WorkerThreads]))
                {
                    if (int.TryParse(config[ConfigurationKey.WorkerThreads], out int threads))
                    {
                        if (threads > minThreads)
                        {
                            setThreads = threads;
                        }
                        else
                        {
                            Log.Error("Configured {SettingName} to value {Value} would place it below the minimum {Minimum}",
                                ConfigurationKey.WorkerThreads,
                                config[ConfigurationKey.WorkerThreads],
                                minThreads);
                        }
                    }
                    else
                    {
                        Log.Error("Unable to parse configuration {SettingName} value: {Value}",
                            ConfigurationKey.WorkerThreads,
                            config[ConfigurationKey.WorkerThreads]);
                    }
                }

                if (!string.IsNullOrEmpty(config[ConfigurationKey.CompletionPortThreads]))
                {
                    if (int.TryParse(config[ConfigurationKey.CompletionPortThreads], out int threads))
                    {
                        if (threads > minCompletionPortThreads)
                        {
                            setCompletionPortThreads = threads;
                        }
                        else
                        {
                            Log.Error("Configured {SettingName} to value {Value} would place it below the minimum {Minimum}",
                                ConfigurationKey.CompletionPortThreads,
                                config[ConfigurationKey.CompletionPortThreads],
                                minCompletionPortThreads);
                        }
                    }
                    else
                    {
                        Log.Error("Unable to parse configuration {SettingName} value: {Value}",
                            ConfigurationKey.CompletionPortThreads,
                            config[ConfigurationKey.CompletionPortThreads]);
                    }
                }

                if (minThreads != setThreads 
                    || minCompletionPortThreads != setCompletionPortThreads)
                {
                    if (ThreadPool.SetMinThreads(setThreads, setCompletionPortThreads)) {
                        Log.Information("Set minimum thread counts to {SetThreads} threads, {SetCompletionPortThreads} completion port threads",
                            setThreads,
                            setCompletionPortThreads);
                    }
                    else
                    {
                        Log.Error("Unable to set minimum thread counts to {SetThreads} threads, {SetCompletionPortThreads} completion port threads",
                            setThreads,
                            setCompletionPortThreads);
                    }
                }
            }

            // output the version and revision
            try
            {
                webhost.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Warning("GRA v{Version} {Instance} exited unexpectedly: {Message}",
                    Version.GetVersion(),
                    instance,
                    ex.Message);
                Environment.ExitCode = 1;
                throw;
            }
            finally
            {
                Log.Warning("GRA v{Version} {Instance} shutting down.",
                    Version.GetVersion(),
                    instance);
                Log.CloseAndFlush();
            }
        }
    }
}
