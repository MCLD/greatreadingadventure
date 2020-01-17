﻿using System;
using System.Collections.Generic;
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
    public class SiteFilterAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ILogger<SiteFilterAttribute> _logger;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _config;
        private readonly IPathResolver _pathResolver;
        private readonly SiteLookupService _siteLookupService;
        private readonly IUserContextProvider _userContextProvider;
        private readonly IOptions<RequestLocalizationOptions> _l10nOptions;
        private readonly UserService _userService;

        public SiteFilterAttribute(ILogger<SiteFilterAttribute> logger,
            IDistributedCache cache,
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
            var httpContext = context.HttpContext;
            await httpContext.Session.LoadAsync();
            // if we've already fetched it on this request it's present in Items
            int? siteId = null;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                // if the user is authenticated, that is their site
                try
                {
                    siteId = _userContextProvider.GetId(httpContext.User, ClaimType.SiteId);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unable to get SiteId claim for user {Name}: {Message}",
                        httpContext.User.Identity.Name,
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
                    siteId = httpContext.Session.GetInt32(SessionKey.SiteId);
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

            bool showChallenges = true;
            bool showEvents = true;

            if (siteStage == SiteStage.BeforeRegistration)
            {
                // we might need to hide challenges and/or events since we're pre-registration
                // hide means that if the value is set then we want showChallenges to be false
                // hence == null rather than != null
                showChallenges = site.Settings
                    .FirstOrDefault(_ => _.Key == SiteSettingKey
                        .Challenges
                        .HideUntilRegistrationOpen)?
                    .Value == null;

                showEvents = site.Settings
                    .FirstOrDefault(_ => _.Key == SiteSettingKey
                        .Events
                        .HideUntilRegistrationOpen)?
                    .Value == null;
            }

            httpContext.Items[ItemKey.GoogleAnalytics] = site.GoogleAnalyticsTrackingId;
            httpContext.Items[ItemKey.RouteId] = context.RouteData.Values["id"];
            httpContext.Items[ItemKey.SiteName] = site?.Name
                ?? _config[ConfigurationKey.DefaultSiteName];
            httpContext.Items[ItemKey.SiteStage] = siteStage;
            httpContext.Session.SetInt32(SessionKey.SiteId, (int)siteId);
            httpContext.Items[ItemKey.SiteId] = (int)siteId;
            httpContext.Items[ItemKey.ShowChallenges] = showChallenges;
            httpContext.Items[ItemKey.ShowEvents] = showEvents;
            httpContext.Items[ItemKey.WebScheme] = site.IsHttpsForced ? "https" : "http";

            // only check if the site.css and site.js have changed periodically by default and
            // cache the last modification time
            string siteCssCacheKey = $"s{siteId}.{CacheKey.SiteCss}";
            string siteJsCacheKey = $"s{siteId}.{CacheKey.SiteJs}";

            var cssLastModified = await _cache.GetStringAsync(siteCssCacheKey);
            var jsLastModified = await _cache.GetStringAsync(siteJsCacheKey);

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
                    httpContext.Items[ItemKey.SiteCss] = $"{contentPath}/site{siteId}/styles/site.css?v={cssLastModified}";
                }
                if (!string.IsNullOrEmpty(jsLastModified))
                {
                    httpContext.Items[ItemKey.SiteJs] = $"{contentPath}/site{siteId}/scripts/site.js?v={jsLastModified}";
                }
            }

            httpContext.Items[ItemKey.HouseholdTitle]
                = string.IsNullOrEmpty(httpContext.Session.GetString(SessionKey.CallItGroup))
                ? Annotations.Interface.Family
                : Annotations.Interface.Group;

            if (!string.IsNullOrWhiteSpace(site.ExternalEventListUrl))
            {
                httpContext.Items[ItemKey.ExternalEventListUrl] = site.ExternalEventListUrl;
            }

            var currentCulture = _userContextProvider.GetCurrentCulture();
            httpContext.Items[ItemKey.ISOLanguageName] = currentCulture.TwoLetterISOLanguageName;

            int? activeUserId = _userContextProvider.GetContext().ActiveUserId;

            if (_l10nOptions.Value?.SupportedCultures.Count > 1)
            {
                var cookieCulture = httpContext
                    .Request
                    .Cookies[CookieRequestCultureProvider.DefaultCookieName];

                if (currentCulture.Name == Culture.DefaultName)
                {
                    if (cookieCulture != null)
                    {
                        httpContext
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
                    httpContext.Response.Cookies.Append(
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
                httpContext.Items[ItemKey.HrefLang] = cultureHrefLang;
                httpContext.Items[ItemKey.L10n] = cultureList.OrderBy(_ => _.Text);
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
                updatedLastModified = File.GetLastWriteTime(file).ToString("yyMMddHHmmss");
                if (cacheMinutes > 0)
                {
                    await _cache.SetStringAsync(cacheKey,
                        updatedLastModified,
                        new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheMinutes)));
                }
            }
            return updatedLastModified;
        }
    }
}
