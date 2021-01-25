﻿using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GRA.Web
{
    public class WebStartup
    {
        private readonly ILogger<WebStartup> _log;
        private readonly IServiceScope _scope;

        public WebStartup(IServiceScope scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            _log = scope.ServiceProvider.GetRequiredService<ILogger<WebStartup>>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Many startup exception possibilities")]
        public async Task InitalizeAsync()
        {
            try
            {
                var cache = _scope.ServiceProvider.GetRequiredService<IDistributedCache>();
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
                await cache.SetStringAsync("Startup",
                    DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    options);
            }
            catch (Exception ex)
            {
                _log.LogCritical("Startup error utilizing distributed cache: {ErrorMessage}",
                    ex.Message);
                throw;
            }

            Data.Context dbContext;

            try
            {
                dbContext = _scope.ServiceProvider.GetRequiredService<Data.Context>();
            }
            catch (Exception ex)
            {
                _log.LogCritical("Startup error accessing data context: {ErrorMessage}",
                    ex.Message);
                throw;
            }

            try
            {
                var pending = dbContext.GetPendingMigrations();
                if (pending?.Count() > 0)
                {
                    _log.LogWarning("Applying {0} database migrations, last is: {1}",
                        pending.Count(),
                        pending.Last());
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical("Startup error getting list of pending database migrations: {ErrorMessage}",
                    ex.Message);
                throw;
            }

            try
            {
                dbContext.Migrate();
            }
            catch (Exception ex)
            {
                _log.LogError("Startup error applying database migrations: {ErrorMessage}",
                    ex.Message);
            }

            try
            {
                var siteLookupService = _scope.ServiceProvider
                    .GetRequiredService<SiteLookupService>();
                await siteLookupService.ReloadSiteCacheAsync();
                await siteLookupService.GetDefaultSiteIdAsync();
            }
            catch (Exception ex)
            {
                _log.LogError("Startup error loading sites: {ErrorMessage}",
                    ex.Message);
            }

            try
            {
                var languageService = _scope.ServiceProvider.GetRequiredService<LanguageService>();
                await languageService.SyncLanguagesAsync();
            }
            catch (Exception ex)
            {
                _log.LogError("Startup error loading languages: {ErrorMessage}",
                    ex.Message);
            }

            try
            {
                await _scope
                    .ServiceProvider
                    .GetRequiredService<RoleService>().SyncPermissionsAsync();
            }
            catch (Exception ex)
            {
                _log.LogError("Startup error synchronizing permissions: {ErrorMessage}",
                    ex.Message);
            }

            try
            {
                await _scope
               .ServiceProvider.GetRequiredService<NewsService>()
               .EnsureDefaultCategoryAsync();
            }
            catch (Exception ex)
            {
                _log.LogError("Startup error ensuring default news category: {ErrorMessage}",
                    ex.Message);
            }

            try
            {
                await _scope
                .ServiceProvider.GetRequiredService<UserService>()
                .EnsureUserUnsubscribeTokensAsync();
            }
            catch (Exception ex)
            {
                _log.LogError("Startup error ensuring users have unsubscribe tokens: {ErrorMessage}",
                    ex.Message);
            }
        }
    }
}
