using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GRA.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHost webhost = CreateWebHostBuilder(args).Build();

            using (IServiceScope scope = webhost.Services.CreateScope())
            {
                Data.Context dbContext = scope.ServiceProvider.GetRequiredService<Data.Context>();
                try
                {
                    System.Collections.Generic.IEnumerable<string> pending = dbContext.GetPendingMigrations();
                    if (pending != null && pending.Count() > 0)
                    {
                        //Log.Logger.Warning($"Applying {pending.Count()} database migrations, last is: {pending.Last()}");
                    }
                }
                catch (Exception ex)
                {
                    //Log.Logger.Error($"Error looking up migrations to perform: {ex.Message}");
                }
                dbContext.Migrate();
                Task.Run(() => scope
                    .ServiceProvider
                    .GetRequiredService<SiteLookupService>().GetDefaultSiteIdAsync()).Wait();
                Task.Run(() => scope
                    .ServiceProvider
                    .GetRequiredService<RoleService>().SyncPermissionsAsync()).Wait();
                scope.ServiceProvider.GetRequiredService<TemplateService>().SetupTemplates();
            }

            webhost.Run();
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
                .UseStartup<Startup>();
    }
}
