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
        private readonly IInitialSetupService _initialSetupService;

        public SiteLookupService(ILogger<SiteLookupService> logger,
            IConfigurationRoot config,
            IMemoryCache memoryCache,
            ISiteRepository siteRepository,
            IInitialSetupService initialSetupService) : base(logger)
        {

            _memoryCache = Require.IsNotNull(memoryCache, nameof(memoryCache));
            _config = Require.IsNotNull(config, nameof(config));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(_siteRepository));
            _initialSetupService = Require.IsNotNull(initialSetupService,
                nameof(initialSetupService));
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

        public SiteStage GetSiteStageAsync(Site site)
        {
            if (site.AccessClosed == null 
                && site.ProgramEnds == null 
                && site.ProgramStarts == null 
                && site.RegistrationOpens == null)
            {
                return SiteStage.ProgramOpen;
            }

            if (site.AccessClosed != null && DateTime.Now >= site.AccessClosed)
            {
                return SiteStage.AccessClosed;
            }
            if(site.ProgramEnds != null && DateTime.Now >= site.ProgramEnds)
            {
                return SiteStage.ProgramEnded;
            }
            if(site.ProgramStarts != null && DateTime.Now >= site.ProgramStarts)
            {
                return SiteStage.ProgramOpen;
            }
            if(site.RegistrationOpens != null && DateTime.Now >= site.RegistrationOpens)
            {
                return SiteStage.RegistrationOpen;
            }
            return SiteStage.BeforeRegistration;
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
                RegistrationOpens = DateTime.Now,
                ProgramStarts = DateTime.Now,
                ProgramEnds = DateTime.Now.AddDays(60),
                AccessClosed = DateTime.Now.AddDays(90),
                UseDynamicAvatars = true
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
    }
}
