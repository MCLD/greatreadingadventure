using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace GRA.Domain.Service
{
    public class SiteLookupService : BaseService<SiteLookupService>
    {
        private readonly IConfigurationRoot _config;
        private readonly IMemoryCache _memoryCache;
        private readonly ISiteRepository _siteRepository;
        private readonly ISiteSettingRepository _siteSettingRepository;
        private readonly IInitialSetupService _initialSetupService;

        public SiteLookupService(ILogger<SiteLookupService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IConfigurationRoot config,
            IMemoryCache memoryCache,
            ISiteRepository siteRepository,
            ISiteSettingRepository siteSettingRepository,
            IInitialSetupService initialSetupService) : base(logger, dateTimeProvider)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _siteRepository = siteRepository
                ?? throw new ArgumentNullException(nameof(siteRepository));
            _siteSettingRepository = siteSettingRepository
                ?? throw new ArgumentNullException(nameof(siteSettingRepository));
            _initialSetupService = initialSetupService
                ?? throw new ArgumentNullException(nameof(initialSetupService));
        }
        private async Task<IEnumerable<Site>> GetSitesFromCacheAsync()
        {
            var sites = _memoryCache.Get<IEnumerable<Site>>(CacheKey.SitePaths);
            if (sites == null)
            {
                sites = await _siteRepository.GetAllAsync();
                if (sites.Count() == 0)
                {
                    sites = await InsertInitialSiteAsync();
                }
                foreach (var site in sites)
                {
                    site.Settings = await _siteSettingRepository.GetBySiteIdAsync(site.Id);
                }
                _memoryCache.Set(CacheKey.SitePaths, sites);
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
            return sites.Where(_ => _.Path == sitePath).SingleOrDefault();
        }

        public async Task<Site> GetByIdAsync(int siteId)
        {
            var sites = await GetSitesFromCacheAsync();
            return sites.Where(_ => _.Id == siteId).FirstOrDefault();
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
            _memoryCache.Remove(CacheKey.SitePaths);

            await _initialSetupService.InsertAsync(site.Id,
                _config[ConfigurationKey.InitialAuthorizationCode]);

            return new List<Site>
            {
                site
            };
        }

        public async Task<IEnumerable<Site>> ReloadSiteCacheAsync()
        {
            _memoryCache.Remove(CacheKey.SitePaths);
            return await GetSitesFromCacheAsync();
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
                throw new Exception($"Invalid key: {key}");
            }
            else if (settingDefinition.Format == SiteSettingFormat.Integer)
            {
                throw new Exception($"Invalid format for key: {key}");
            }

            var site = (await GetSitesFromCacheAsync())
                .Where(_ => _.Id == siteId)
                .SingleOrDefault();
            return site.Settings
                .Where(_ => _.Key == key)
                .FirstOrDefault()?
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
                throw new Exception($"Invalid key: {key}");
            }
            else if (settingDefinition.Format == SiteSettingFormat.Boolean)
            {
                throw new Exception($"Invalid format for key: {key}");
            }

            var site = (await GetSitesFromCacheAsync())
                .Where(_ => _.Id == siteId)
                .SingleOrDefault();
            var settingValueString = site.Settings
                .Where(_ => _.Key == key)
                .FirstOrDefault()?
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
    }
}
