using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GRA.Web
{
    public class Program
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

                    if (!string.IsNullOrEmpty(config[ConfigurationKey.InstanceName]))
                    {
                        instance = config[ConfigurationKey.InstanceName];
                    }

                    Log.Logger = new LogConfig().Build(config, instance, args).CreateLogger();
                }
            }

            // now that we have logging present in our config, we must create the webhost
            IWebHost webhost = CreateWebHostBuilder(args).Build();

            using (IServiceScope scope = webhost.Services.CreateScope())
            {
                int stage = 10;
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<Data.Context>();

                    stage = 20;
                    var pending = dbContext.GetPendingMigrations();
                    if (pending != null && pending.Count() > 0)
                    {
                        Log.Warning("Applying {0} database migrations, last is: {1}",
                            pending.Count(),
                            pending.Last());
                    }

                    stage = 30;
                    dbContext.Migrate();

                    stage = 40;
                    Task.Run(() => scope
                        .ServiceProvider
                        .GetRequiredService<SiteLookupService>().GetDefaultSiteIdAsync()).Wait();

                    stage = 50;
                    Task.Run(() => scope
                        .ServiceProvider
                        .GetRequiredService<RoleService>().SyncPermissionsAsync()).Wait();

                    stage = 60;
                    scope.ServiceProvider.GetRequiredService<TemplateService>().SetupTemplates();

                    stage = 70;
                    IHostingEnvironment env
                        = scope.ServiceProvider.GetRequiredService<IHostingEnvironment>();
                    webRootPath = env.WebRootPath;
                }
                catch (Exception ex)
                {
                    bool critical = false;
                    string errorText = null;
                    switch (stage)
                    {
                        case 10:
                            critical = true;
                            errorText = "Error accessing data context: {Message}";
                            break;
                        case 20:
                            errorText = "Error looking up migrations to perform: {Message}";
                            break;
                        case 30:
                            errorText = "Error performing database migrations: {Message}";
                            break;
                        case 40:
                            errorText = "Error loading sites into cache: {Message}";
                            break;
                        case 50:
                            errorText = "Error synchronizing permissions: {Message}";
                            break;
                        case 60:
                            errorText = "Error copying templates to shared folder: {Message}";
                            break;
                        case 70:
                            errorText = "Error establishing WebRootPath: {Message}";
                            break;
                        default:
                            errorText = "Unknown error during application startup: {Message}";
                            break;
                    }
                    if (critical)
                    {
                        Log.Fatal(ex, errorText, ex.Message);
                        throw ex;
                    }
                    else
                    {
                        Log.Error(ex, errorText, ex.Message);
                    }
                }
            }

            string applicationName = instance.ToLower() != "gra" ? $"GRA {instance}" : "GRA";
            string version = "v" + Assembly.GetEntryAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

            try
            {
                Log.Warning("{0} {1} starting up in {2}",
                    applicationName,
                    version,
                    webRootPath);

                webhost.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Warning("{0} {1} exited unexpectedly: {2}",
                    applicationName,
                    version,
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
