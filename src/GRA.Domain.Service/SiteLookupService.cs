using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class SiteLookupService : BaseService<SiteLookupService>
    {
        private readonly IConfigurationRoot _config;
        private readonly IMemoryCache _memoryCache;
        private readonly ISiteRepository _siteRepository;
        private readonly InitialSetupService _initialSetupService;

        public SiteLookupService(ILogger<SiteLookupService> logger,
            IConfigurationRoot config,
            IMemoryCache memoryCache,
            ISiteRepository siteRepository,
            InitialSetupService initialSetupService) : base(logger)
        {

            _memoryCache = Require.IsNotNull(memoryCache, nameof(memoryCache));
            _config = Require.IsNotNull(config, nameof(config));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(_siteRepository));
            _initialSetupService = Require.IsNotNull(initialSetupService,
                nameof(initialSetupService));
        }
        private async Task<IEnumerable<Site>> GetSitesFromCache()
        {
            var sites = _memoryCache.Get<IEnumerable<Site>>(CacheKey.SitePaths);
            if (sites == null)
            {
                sites = await _siteRepository.GetAllAsync();
                if (sites.Count() == 0)
                {
                    sites = await InsertInitialSite();
                }
                _memoryCache.Set(CacheKey.SitePaths, sites);
            }
            return sites;
        }

        public async Task<int> GetDefaultSiteId()
        {
            var sites = await GetSitesFromCache();
            return sites.Where(_ => _.IsDefault).Select(_ => _.Id).FirstOrDefault();
        }

        public async Task<Site> GetSiteByPath(string sitePath)
        {
            var sites = await GetSitesFromCache();
            return sites.Where(_ => _.Path == sitePath).SingleOrDefault();
        }

        public async Task<Site> GetById(int siteId)
        {
            var sites = await GetSitesFromCache();
            return sites.Where(_ => _.Id == siteId).FirstOrDefault();
        }

        public async Task<IEnumerable<string>> GetSitePaths()
        {
            var sites = await GetSitesFromCache();
            return sites.Select(_ => _.Path);
        }

        private async Task<IEnumerable<Site>> InsertInitialSite()
        {
            int? outgoingMailPort = null;
            if(!string.IsNullOrEmpty(_config[ConfigurationKey.DefaultOutgoingMailPort]))
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
                RegistrationOpens = DateTime.Now,
                ProgramStarts = DateTime.Now,
                ProgramEnds = DateTime.Now.AddDays(60),
                AccessClosed = DateTime.Now.AddDays(90)
            };
            site = await _siteRepository.AddSaveAsync(-1, site);
            _memoryCache.Remove(CacheKey.SitePaths);

            await _initialSetupService.Insert(site.Id,
                _config[ConfigurationKey.InitialAuthorizationCode]);

            return new List<Site>
            {
                site
            };
        }
    }
}
