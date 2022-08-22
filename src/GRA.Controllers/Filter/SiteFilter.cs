using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace GRA.Controllers.Filter
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class SiteFilterAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly IGraCache _cache;
        private readonly IConfiguration _config;
        private readonly IOptions<RequestLocalizationOptions> _l10nOptions;
        private readonly ILogger<SiteFilterAttribute> _logger;
        private readonly IPathResolver _pathResolver;
        private readonly SiteLookupService _siteLookupService;
        private readonly IUserContextProvider _userContextProvider;
        private readonly UserService _userService;

        public SiteFilterAttribute(ILogger<SiteFilterAttribute> logger,
            IGraCache cache,
            IConfiguration config,
            IPathResolver pathResolver,
            SiteLookupService siteLookupService,
            IUserContextProvider userContextProvider,
            IOptions<RequestLocalizationOptions> l10nOptions,
            UserService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _userContextProvider = userContextProvider
                ?? throw new ArgumentNullException(nameof(userContextProvider));
            _l10nOptions = l10nOptions ?? throw new ArgumentNullException(nameof(l10nOptions));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
            ResourceExecutionDelegate next)
        {
            Site site = null;
            await context.HttpContext.Session.LoadAsync();
            // if we've already fetched it on this request it's present in Items
            int? siteId = null;
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                // if the user is authenticated, that is their site
                try
                {
                    siteId = _userContextProvider.GetId(context.HttpContext.User,
                        ClaimType.SiteId);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unable to get SiteId claim for user {Name}: {Message}",
                        context.HttpContext.User.Identity.Name,
                        ex.Message);
                }
            }

            if (siteId == null)
            {
                string sitePath = context.RouteData.Values["sitePath"]?.ToString();
                // first check, did they use a sitePath giving them a specific site
                if (!string.IsNullOrEmpty(sitePath))
                {
                    site = await _siteLookupService.GetSiteByPathAsync(sitePath);
                    if (site != null)
                    {
                        siteId = site.Id;
                    }
                }
                // if not check if they already have one in their session
                if (siteId == null)
                {
                    siteId = context.HttpContext.Session.GetInt32(SessionKey.SiteId);
                }
                // if not then resort to the default
                if (siteId == null)
                {
                    siteId = await _siteLookupService.GetDefaultSiteIdAsync();
                }
            }
            if (site == null)
            {
                site = await _siteLookupService.GetByIdAsync((int)siteId);
            }

            var siteStage = _siteLookupService.GetSiteStage(site);

            // we might need to hide challenges and/or events based on system settings
            // the setting is to hide so the logic is == null rather than != null
            bool showChallenges = true;
            bool showEvents = true;

            if (siteStage == SiteStage.RegistrationOpen
                || siteStage == SiteStage.BeforeRegistration)
            {
                showEvents = site.Settings
                    .FirstOrDefault(_ => _.Key == SiteSettingKey
                        .Events
                        .HideUntilProgramOpen)?
                    .Value == null;
            }

            if (siteStage == SiteStage.BeforeRegistration)
            {
                showChallenges = site.Settings
                    .FirstOrDefault(_ => _.Key == SiteSettingKey
                        .Challenges
                        .HideUntilRegistrationOpen)?
                    .Value == null;

                if (!showEvents)
                {
                    // they might be hidden above
                    showEvents = site.Settings
                        .FirstOrDefault(_ => _.Key == SiteSettingKey
                            .Events
                            .HideUntilRegistrationOpen)?
                        .Value == null;
                }
            }

            context.HttpContext.Items[ItemKey.GoogleAnalytics] = site.GoogleAnalyticsTrackingId;
            context.HttpContext.Items[ItemKey.RouteId] = context.RouteData.Values["id"];
            context.HttpContext.Items[ItemKey.SiteName] = site?.Name
                ?? _config[ConfigurationKey.DefaultSiteName];
            context.HttpContext.Items[ItemKey.SiteStage] = siteStage;
            context.HttpContext.Session.SetInt32(SessionKey.SiteId, (int)siteId);
            context.HttpContext.Items[ItemKey.SiteId] = (int)siteId;
            context.HttpContext.Items[ItemKey.ShowChallenges] = showChallenges;
            context.HttpContext.Items[ItemKey.ShowEvents] = showEvents;
            context.HttpContext.Items[ItemKey.WebScheme] = site.IsHttpsForced ? "https" : "http";
            context.HttpContext.Items[ItemKey.ShowMail] = site.Settings
                .FirstOrDefault(_ => _.Key == SiteSettingKey.Mail.Disable)?
                .Value == null;

            // only check if the site.css and site.js have changed periodically by default and
            // cache the last modification time
            string siteCssCacheKey = $"s{siteId}.{CacheKey.SiteCss}";
            string siteJsCacheKey = $"s{siteId}.{CacheKey.SiteJs}";

            var cssLastModified = await _cache.GetStringFromCache(siteCssCacheKey);
            var jsLastModified = await _cache.GetStringFromCache(siteJsCacheKey);

            // compute the appropriate cache time in minutes, default to 60 if not provided
            int cacheMinutes = 60;

            var cacheSiteCustomizationsMinutes = site.Settings
                .FirstOrDefault(_ => _.Key == SiteSettingKey.Web.CacheSiteCustomizationsMinutes)?
                .Value;

            if (cacheSiteCustomizationsMinutes != null
                && !int.TryParse(cacheSiteCustomizationsMinutes, out cacheMinutes))
            {
                _logger.LogError("Could not convert cache site customizations value to a number: {cacheSiteCustomizationsMinutes}", cacheSiteCustomizationsMinutes);
            }

            // kill any cached values if the cache value was set to 0
            if (cacheMinutes == 0)
            {
                if (!string.IsNullOrEmpty(cssLastModified))
                {
                    await _cache.RemoveAsync(siteCssCacheKey);
                }
                if (!string.IsNullOrEmpty(jsLastModified))
                {
                    await _cache.RemoveAsync(siteJsCacheKey);
                }
            }

            // check for a cache miss and compute the appropriate last modification date
            if (string.IsNullOrEmpty(cssLastModified)
                || string.IsNullOrEmpty(jsLastModified))
            {
                // file path to the site.css file
                string file = _pathResolver.ResolveContentFilePath(
                    Path.Combine($"site{siteId}", "styles", "site.css"));

                cssLastModified
                    = await UpdateCacheAsync(file, cacheMinutes, cssLastModified, siteCssCacheKey);

                // file path to the site.js file
                file = _pathResolver.ResolveContentFilePath(
                    Path.Combine($"site{siteId}", "scripts", "site.js"));

                jsLastModified
                    = await UpdateCacheAsync(file, cacheMinutes, jsLastModified, siteJsCacheKey);
            }

            // if we have values then set appropriate HttpContext.Items
            if (!string.IsNullOrEmpty(cssLastModified)
                || !string.IsNullOrEmpty(jsLastModified))
            {
                string contentPath = _config[ConfigurationKey.ContentPath].StartsWith("/")
                    ? _config[ConfigurationKey.ContentPath]
                    : $"/{_config[ConfigurationKey.ContentPath]}";

                if (!string.IsNullOrEmpty(cssLastModified))
                {
                    context.HttpContext.Items[ItemKey.SiteCss]
                        = $"{contentPath}/site{siteId}/styles/site.css?v={cssLastModified}";
                }
                if (!string.IsNullOrEmpty(jsLastModified))
                {
                    context.HttpContext.Items[ItemKey.SiteJs]
                        = $"{contentPath}/site{siteId}/scripts/site.js?v={jsLastModified}";
                }
            }

            context.HttpContext.Items[ItemKey.HouseholdTitle]
                = string.IsNullOrEmpty(context
                    .HttpContext
                    .Session
                    .GetString(SessionKey.CallItGroup))
                ? Annotations.Interface.Family
                : Annotations.Interface.Group;

            if (!string.IsNullOrWhiteSpace(site.ExternalEventListUrl))
            {
                context.HttpContext.Items[ItemKey.ExternalEventListUrl]
                    = site.ExternalEventListUrl;
            }

            var currentCulture = _userContextProvider.GetCurrentCulture();
            context.HttpContext.Items[ItemKey.ISOLanguageName]
                = currentCulture.TwoLetterISOLanguageName;

            int? activeUserId = _userContextProvider.GetContext().ActiveUserId;

            if (_l10nOptions.Value?.SupportedCultures.Count > 1)
            {
                var cookieCulture = context
                    .HttpContext
                    .Request
                    .Cookies[CookieRequestCultureProvider.DefaultCookieName];

                if (currentCulture.Name == Culture.DefaultName)
                {
                    if (cookieCulture != null)
                    {
                        context
                            .HttpContext
                            .Response
                            .Cookies
                            .Delete(CookieRequestCultureProvider.DefaultCookieName);
                        if (activeUserId != null)
                        {
                            await _userService.UpdateCulture(null);
                        }
                    }
                }
                else
                {
                    // no cookie or new culture selected, reset cookie
                    context.HttpContext.Response.Cookies.Append(
                        CookieRequestCultureProvider.DefaultCookieName,
                        CookieRequestCultureProvider
                            .MakeCookieValue(new RequestCulture(currentCulture.Name)),
                        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(14) }
                    );
                    if (activeUserId != null)
                    {
                        await _userService.UpdateCulture(currentCulture.Name);
                    }
                }

                // generate list for drop-down
                var cultureList = new List<SelectListItem>();
                var cultureHrefLang = new Dictionary<string, string>
                {
                    { "x-default", Culture.DefaultName }
                };
                foreach (var culture in _l10nOptions.Value.SupportedCultures)
                {
                    var text = culture.Parent != null
                        ? culture.Parent.NativeName
                        : culture.NativeName;
                    cultureList.Add(new SelectListItem(text, culture.Name));
                    if (!cultureHrefLang.Keys.Contains(culture.Name))
                    {
                        cultureHrefLang.Add(culture.Name, culture.Name);
                        if (culture.Parent != null
                            && !cultureHrefLang.Keys.Contains(culture.Parent.Name))
                        {
                            cultureHrefLang.Add(culture.Parent.Name, culture.Parent.Name);
                        }
                    }
                }
                context.HttpContext.Items[ItemKey.HrefLang] = cultureHrefLang;
                context.HttpContext.Items[ItemKey.L10n] = cultureList.OrderBy(_ => _.Text);
            }

            using (LogContext.PushProperty(LoggingEnrichment.ActiveUserId, activeUserId))
            using (LogContext.PushProperty(LoggingEnrichment.LanguageId,
                currentCulture?.TwoLetterISOLanguageName))
            using (LogContext.PushProperty(LoggingEnrichment.RouteAction,
                context.RouteData?.Values["action"]))
            using (LogContext.PushProperty(LoggingEnrichment.RouteArea,
                context.RouteData?.Values["area"]))
            using (LogContext.PushProperty(LoggingEnrichment.RouteController,
                context.RouteData?.Values["controller"]))
            using (LogContext.PushProperty(LoggingEnrichment.RouteId,
                context.RouteData?.Values["id"]))
            using (LogContext.PushProperty(LoggingEnrichment.SiteStage, siteStage))
            {
                await next();
            }
        }

        private async Task<string> UpdateCacheAsync(string file,
            int cacheMinutes,
            string lastModified,
            string cacheKey)
        {
            var updatedLastModified = lastModified;
            if (string.IsNullOrEmpty(lastModified) && File.Exists(file))
            {
                updatedLastModified = File
                    .GetLastWriteTime(file)
                    .ToString("yyMMddHHmmss", CultureInfo.InvariantCulture);
                if (cacheMinutes > 0)
                {
                    await _cache.SaveToCacheAsync(cacheKey,
                        updatedLastModified,
                        TimeSpan.FromMinutes(cacheMinutes));
                }
            }
            return updatedLastModified;
        }
    }
}
