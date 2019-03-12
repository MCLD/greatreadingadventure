using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GRA.Web
{
    public class Web
    {
        private readonly ILogger<Web> _log;
        private readonly IServiceScope _scope;

        public Web(IServiceScope scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            _log = scope.ServiceProvider.GetRequiredService<ILogger<Web>>();
        }

        public async Task InitalizeAsync()
        {
            int stage = 10;
            try
            {
                var cache = _scope.ServiceProvider.GetRequiredService<IDistributedCache>();
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
                await cache.SetStringAsync("Startup", DateTime.Now.ToString(), options);

                stage = 15;
                var dbContext = _scope.ServiceProvider.GetRequiredService<Data.Context>();

                stage = 20;
                var pending = dbContext.GetPendingMigrations();
                if (pending?.Count() > 0)
                {
                    _log.LogWarning("Applying {0} database migrations, last is: {1}",
                        pending.Count(),
                        pending.Last());
                }

                stage = 30;
                dbContext.Migrate();

                stage = 40;
                var siteLookupService = _scope.ServiceProvider
                    .GetRequiredService<SiteLookupService>();
                await siteLookupService.ReloadSiteCacheAsync();

                stage = 50;
                await siteLookupService.GetDefaultSiteIdAsync();

                stage = 55;
                var languageService = _scope.ServiceProvider.GetRequiredService<LanguageService>();
                await languageService.SyncLanguagesAsync();

                stage = 60;
                await _scope
                    .ServiceProvider
                    .GetRequiredService<RoleService>().SyncPermissionsAsync();

                stage = 65;
                await _scope
                    .ServiceProvider.GetRequiredService<NewsService>()
                    .EnsureDefaultCategoryAsync();

                stage = 70;
                _scope.ServiceProvider.GetRequiredService<TemplateService>().SetupTemplates();
            }
            catch (Exception ex)
            {
                bool critical = false;
                string errorText = null;
                switch (stage)
                {
                    case 10:
                        critical = true;
                        errorText = "Error utilizing distributed cache: {Message}";
                        break;
                    case 15:
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
                        errorText = "Error clearing sites from cache: {Message}";
                        break;
                    case 50:
                        errorText = "Error loading sites into cache: {Message}";
                        break;
                    case 55:
                        errorText = "Error syncing available languages with database: {Message}";
                        break;
                    case 60:
                        errorText = "Error synchronizing permissions: {Message}";
                        break;
                    case 65:
                        errorText = "Error ensuring default news category: {Message}";
                        break;
                    case 70:
                        errorText = "Error copying templates to shared folder: {Message}";
                        break;
                    default:
                        errorText = "Unknown error during application startup: {Message}";
                        break;
                }

                if (critical)
                {
                    _log.LogCritical(ex, errorText, ex.Message);
                    throw;
                }
                else
                {
                    _log.LogCritical(ex, errorText, ex.Message);
                }
            }
        }
    }
}
