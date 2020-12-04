﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog;

namespace GRA.Web
{
    public static class Program
    {
        private const string EnvRunningInContainer = "DOTNET_RUNNING_IN_CONTAINER";
        private const string EnvAspNetCoreEnv = "ASPNETCORE_ENVIRONMENT";
        private const string DevEnvironment = "Development";

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
                new Version().GetVersion(),
                instance,
                config[EnvAspNetCoreEnv],
                webHostEnvironment.WebRootPath,
                config[ConfigurationKey.InternalContentPath]);

            if (!string.IsNullOrEmpty(config[EnvRunningInContainer]))
            {
                Log.Information("Containerized: commit {ContainerCommit} created on {ContainerDate} image {ContainerImageVersion}",
                    config["org.opencontainers.image.revision"] ?? "unknown",
                    config["org.opencontainers.image.created"] ?? "unknown",
                    config["org.opencontainers.image.version"] ?? "unknown");
            }

            var issues = ViewTemplates.CopyToShared(config[ConfigurationKey.InternalContentPath]);

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
            try
            {
                Task.Run(() => new Web(scope).InitalizeAsync()).Wait();
            }
            catch (StackExchange.Redis.RedisConnectionException ex)
            {
                Log.Fatal("Redis is configured for {RedisConfig} but failed: {ErrorMessage}",
                    config[ConfigurationKey.RuntimeCacheRedisConfiguration],
                    ex.Message);
                throw;
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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var contentRoot
                = Environment.GetEnvironmentVariable(EnvAspNetCoreEnv) == DevEnvironment
                    ? System.IO.Directory.GetCurrentDirectory()
                    : PlatformServices.Default.Application.ApplicationBasePath;

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
    }
}
