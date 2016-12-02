using GRA.Domain.Repository;
using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace GRA.Domain.Service
{
    public class SiteService : Abstract.BaseService<SiteService>
    {
        private readonly IConfigurationRoot _config;
        private readonly IMemoryCache _memoryCache;
        private readonly IBranchRepository _branchRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly ConfigurationService _configurationService;

        public SiteService(ILogger<SiteService> logger,
            IConfigurationRoot config,
            IMemoryCache memoryCache,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            ISiteRepository siteRepository,
            ISystemRepository systemRepository,
            ConfigurationService configurationService)
            : base(logger)
        {
            _config = Require.IsNotNull(config, nameof(config));
            _memoryCache = Require.IsNotNull(memoryCache, nameof(memoryCache));
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _systemRepository = Require.IsNotNull(systemRepository, nameof(systemRepository));
            _configurationService = Require.IsNotNull(configurationService, nameof(configurationService));
        }

        private async Task<IEnumerable<Site>> InsertInitialSite()
        {
            var site = new Site
            {
                IsDefault = true,
                Name = _config[ConfigurationKey.DefaultSiteName],
                PageTitle = _config[ConfigurationKey.DefaultPageTitle],
                Path = _config[ConfigurationKey.DefaultSitePath],
                Footer = _config[ConfigurationKey.DefaultFooter]
            };
            site = await _siteRepository.AddSaveAsync(-1, site);
            _memoryCache.Remove(CacheKey.SitePaths);

            await _configurationService.InsertSetupData(site);

            return new List<Site>
            {
                site
            };
        }

        private async Task<IEnumerable<Site>> GetSitesFromCache()
        {
            var sites = _memoryCache.Get<IEnumerable<Site>>(CacheKey.SitePaths);
            if (sites == null)
            {
                sites = await _siteRepository.GetAllAsync();
                if(sites.Count() == 0)
                {
                    sites = await InsertInitialSite();
                }
                _memoryCache.Set(CacheKey.SitePaths, sites);
            }
            return sites;
        }

        public async Task<Site> GetById(int siteId)
        {
            var sites = await GetSitesFromCache();
            return sites.Where(_ => _.Id == siteId).FirstOrDefault();
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

        public async Task<IEnumerable<string>> GetSitePaths()
        {
            var sites = await GetSitesFromCache();
            return sites.Select(_ => _.Path);
        }

        public async Task<IEnumerable<Model.System>> GetSystemList(ClaimsPrincipal user)
        {
            var siteId = GetId(user, ClaimType.SiteId);
            return await _systemRepository.GetAllAsync(siteId);
        }

        public async Task<IEnumerable<Branch>> GetBranches(ClaimsPrincipal user,
            int? systemId = null)
        {
            return await _branchRepository.GetAllAsync(systemId ?? GetId(user, ClaimType.SystemId));
        }

        public async Task<IEnumerable<Program>> GetProgramList(ClaimsPrincipal user)
        {
            var siteId = GetId(user, ClaimType.SiteId);
            return await _programRepository.GetAllAsync(siteId);
        }
    }
}