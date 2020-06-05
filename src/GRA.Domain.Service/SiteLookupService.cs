﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class SiteLookupService : BaseService<SiteLookupService>
    {
        private readonly IConfiguration _config;
        private readonly IDistributedCache _cache;
        private readonly ISiteRepository _siteRepository;
        private readonly ISiteSettingRepository _siteSettingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IInitialSetupService _initialSetupService;

        public SiteLookupService(ILogger<SiteLookupService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IConfiguration config,
            IDistributedCache cache,
            ISiteRepository siteRepository,
            ISiteSettingRepository siteSettingRepository,
            IUserRepository userRepository,
            IInitialSetupService initialSetupService) : base(logger, dateTimeProvider)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _siteRepository = siteRepository
                ?? throw new ArgumentNullException(nameof(siteRepository));
            _siteSettingRepository = siteSettingRepository
                ?? throw new ArgumentNullException(nameof(siteSettingRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _initialSetupService = initialSetupService
                ?? throw new ArgumentNullException(nameof(initialSetupService));
        }

        public async Task<int> GetSystemUserId()
        {
            return await _userRepository.GetSystemUserId();
        }

        private async Task<IEnumerable<Site>> GetSitesFromCacheAsync()
        {
            IEnumerable<Site> sites = null;
            var cachedSites = _cache.GetString(CacheKey.Sites);
            if (cachedSites == null)
            {
                sites = await _siteRepository.GetAllAsync();
                if (!sites.Any())
                {
                    sites = await InsertInitialSiteAsync();
                }
                _cache.SetString(CacheKey.Sites, JsonConvert.SerializeObject(sites));
                _logger.LogTrace("Cache miss on sites: {Count} loaded", sites.Count());
            }
            else
            {
                sites = JsonConvert.DeserializeObject<IEnumerable<Site>>(cachedSites);
            }

            foreach (var site in sites)
            {
                string key = $"s{site.Id}.{CacheKey.SiteSettings}";
                var cachedSiteSettings = _cache.GetString(key);

                if (cachedSiteSettings == null)
                {
                    site.Settings = await _siteSettingRepository.GetBySiteIdAsync(site.Id);
                    _cache.SetString(key, JsonConvert.SerializeObject(site.Settings));
                    _logger.LogTrace("Cache miss on site settings for site id {Id}, {Count} loaded",
                        site.Id,
                        site.Settings.Count);
                }
                else
                {
                    site.Settings = JsonConvert.DeserializeObject<ICollection<SiteSetting>>(cachedSiteSettings);
                }
            }

            return sites;
        }

        public async Task<int> GetDefaultSiteIdAsync()
        {
            var sites = await GetSitesFromCacheAsync();
            return sites.Where(_ => _.IsDefault).Select(_ => _.Id).FirstOrDefault();
        }

        public async Task<Site> GetSiteByPathAsync(string sitePath)
        {
            var sites = await GetSitesFromCacheAsync();
            return sites.SingleOrDefault(_ => _.Path == sitePath);
        }

        public async Task<Site> GetByIdAsync(int siteId)
        {
            var sites = await GetSitesFromCacheAsync();
            return sites.FirstOrDefault(_ => _.Id == siteId);
        }

        public async Task<IEnumerable<string>> GetSitePathsAsync()
        {
            var sites = await GetSitesFromCacheAsync();
            return sites.Select(_ => _.Path);
        }

        public SiteStage GetSiteStage(Site site)
        {
            if (site.AccessClosed == null
                && site.ProgramEnds == null
                && site.ProgramStarts == null
                && site.RegistrationOpens == null)
            {
                return SiteStage.ProgramOpen;
            }

            if (site.AccessClosed != null && _dateTimeProvider.Now >= site.AccessClosed)
            {
                return SiteStage.AccessClosed;
            }
            if (site.ProgramEnds != null && _dateTimeProvider.Now >= site.ProgramEnds)
            {
                return SiteStage.ProgramEnded;
            }
            if (site.ProgramStarts != null && _dateTimeProvider.Now >= site.ProgramStarts)
            {
                return SiteStage.ProgramOpen;
            }
            if (site.RegistrationOpens != null && _dateTimeProvider.Now >= site.RegistrationOpens)
            {
                return SiteStage.RegistrationOpen;
            }
            return SiteStage.BeforeRegistration;
        }

        public int? GetSiteDay(Site site)
        {
            if (site.ProgramStarts.HasValue)
            {
                var daysElapsed = (_dateTimeProvider.Now.Date - site.ProgramStarts.Value.Date).Days;
                return daysElapsed + 1;
            }
            else
            {
                return null;
            }
        }

        private async Task<IEnumerable<Site>> InsertInitialSiteAsync()
        {
            int? outgoingMailPort = null;
            if (!string.IsNullOrEmpty(_config[ConfigurationKey.DefaultOutgoingMailPort]))
            {
                outgoingMailPort = int.Parse(_config[ConfigurationKey.DefaultOutgoingMailPort]);
            }

            var site = new Site
            {
                IsDefault = true,
                Name = _config[ConfigurationKey.DefaultSiteName],
                PageTitle = _config[ConfigurationKey.DefaultPageTitle],
                Path = _config[ConfigurationKey.DefaultSitePath],
                Footer = _config[ConfigurationKey.DefaultFooter],
                OutgoingMailHost = _config[ConfigurationKey.DefaultOutgoingMailHost],
                OutgoingMailLogin = _config[ConfigurationKey.DefaultOutgoingMailLogin],
                OutgoingMailPassword = _config[ConfigurationKey.DefaultOutgoingMailPassword],
                OutgoingMailPort = outgoingMailPort,
                RegistrationOpens = _dateTimeProvider.Now,
                ProgramStarts = _dateTimeProvider.Now,
                ProgramEnds = _dateTimeProvider.Now.AddDays(60),
                AccessClosed = _dateTimeProvider.Now.AddDays(90)
            };
            site = await _siteRepository.AddSaveAsync(-1, site);
            _cache.Remove(CacheKey.Sites);

            await _initialSetupService.InsertAsync(site.Id,
                _config[ConfigurationKey.InitialAuthorizationCode]);

            return new List<Site>
            {
                site
            };
        }

        public async Task<IEnumerable<Site>> ReloadSiteCacheAsync()
        {
            var sites = await _siteRepository.GetAllAsync();
            foreach(var site in sites)
            {
                _cache.Remove($"s{site.Id}.{CacheKey.SiteSettings}");
                _cache.Remove($"s{site.Id}.{CacheKey.SiteCss}");
            }
            _cache.Remove(CacheKey.Sites);
            return await GetSitesFromCacheAsync();
        }

        /// <summary>
        /// Look up if a site setting is set by site id and key.
        /// </summary>
        /// <param name="siteId">Site id that the setting is associated with</param>
        /// <param name="key">The site setting key value (a string, up to 255 characters)</param>
        /// <returns>True if the value is set in the database, false if the key is not present or
        /// set to NULL.</returns>
        public async Task<bool> IsSiteSettingSetAsync(int siteId, string key)
        {
            var settingDefinition = SiteSettingDefinitions.DefinitionDictionary[key];

            if (settingDefinition == null)
            {
                throw new GraException($"Invalid key: {key}");
            }

            var site = (await GetSitesFromCacheAsync())
                .SingleOrDefault(_ => _.Id == siteId);
            return site.Settings
                .FirstOrDefault(_ => _.Key == key)?
                .Value != null;
        }

        /// <summary>
        /// Look up a boolean site setting by site id and key.
        /// </summary>
        /// <param name="siteId">Site id that the setting is associated with</param>
        /// <param name="key">The site setting key value (a string, up to 255 characters)</param>
        /// <returns>True if the value is set in the database, false if the key is not present or
        /// set to NULL.</returns>
        public async Task<bool> GetSiteSettingBoolAsync(int siteId, string key)
        {
            var settingDefinition = SiteSettingDefinitions.DefinitionDictionary[key];

            if (settingDefinition == null)
            {
                throw new GraException($"Invalid key: {key}");
            }
            else if (settingDefinition.Format == SiteSettingFormat.Integer
                || settingDefinition.Format == SiteSettingFormat.String)
            {
                throw new GraException($"Invalid format for key: {key}");
            }

            var site = (await GetSitesFromCacheAsync())
                .SingleOrDefault(_ => _.Id == siteId);
            return site.Settings
                .FirstOrDefault(_ => _.Key == key)?
                .Value != null;
        }

        /// <summary>
        /// Look up an integer site setting by site id and key.
        /// </summary>
        /// <param name="siteId">Site id that the setting is associated with</param>
        /// <param name="key">The site setting key value (a string, up to 255 characters)</param>
        /// <returns>A tuple, the bool is true if the setting is present and a number with the
        /// value being the number. The bool is false if the setting is not set or is not a parsable
        /// integer.</returns>
        public async Task<(bool IsSet, int SetValue)> GetSiteSettingIntAsync(int siteId, string key)
        {
            var settingDefinition = SiteSettingDefinitions.DefinitionDictionary[key];

            if (settingDefinition == null)
            {
                throw new GraException($"Invalid key: {key}");
            }
            else if (settingDefinition.Format == SiteSettingFormat.Boolean
                || settingDefinition.Format == SiteSettingFormat.String)
            {
                throw new GraException($"Invalid format for key: {key}");
            }

            var site = (await GetSitesFromCacheAsync())
                .SingleOrDefault(_ => _.Id == siteId);
            var settingValueString = site.Settings
                .FirstOrDefault(_ => _.Key == key)?
                .Value;

            if (!string.IsNullOrEmpty(settingValueString))
            {
                if (int.TryParse(settingValueString, out int value))
                {
                    return (IsSet: true, SetValue: value);
                }
            }
            return (IsSet: false, SetValue: default(int));
        }

        /// <summary>
        /// Look up a string site setting by site id and key.
        /// </summary>
        /// <param name="siteId">Site id that the setting is associated with</param>
        /// <param name="key">The site setting key value (a string, up to 255 characters)</param>
        /// <returns>A tuple, the bool is true if the setting is present and a string with the
        /// value. The bool is false if the setting is not set.</returns>
        public async Task<(bool IsSet, string SetValue)> GetSiteSettingStringAsync(
            int siteId, string key)
        {
            var settingDefinition = SiteSettingDefinitions.DefinitionDictionary[key];

            if (settingDefinition == null)
            {
                throw new GraException($"Invalid key: {key}");
            }
            else if (settingDefinition.Format == SiteSettingFormat.Boolean
                || settingDefinition.Format == SiteSettingFormat.Integer)
            {
                throw new GraException($"Invalid format for key: {key}");
            }

            var site = (await GetSitesFromCacheAsync())
                .SingleOrDefault(_ => _.Id == siteId);
            var settingValueString = site.Settings
                .FirstOrDefault(_ => _.Key == key)?
                .Value;

            if (!string.IsNullOrEmpty(settingValueString))
            {
                return (IsSet: true, SetValue: settingValueString);
            }
            return (IsSet: false, SetValue: string.Empty);
        }
    }
}
