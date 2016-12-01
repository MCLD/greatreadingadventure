using System;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace GRA.Domain.Service
{
    public class SiteService : Abstract.BaseService<SiteService>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IBranchRepository _branchRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISystemRepository _systemRepository;

        public SiteService(ILogger<SiteService> logger,
            IMemoryCache memoryCache,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            ISiteRepository siteRepository,
            ISystemRepository systemRepository)
            : base(logger)
        {
            _memoryCache = Require.IsNotNull(memoryCache, nameof(memoryCache));
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _systemRepository = Require.IsNotNull(systemRepository, nameof(systemRepository));
        }

        private async Task<IEnumerable<Site>> InsertInitialSite()
        {
            var site = new Site
            {
                IsDefault = true,
                Name = "Great Reading Adventure",
                Path = "gra",
                Footer = "This site is running the open source <a href=\"http://www.greatreadingadventure.com/\">Great Reading Adventure</a> software developed by the <a href=\"https://mcldaz.org/\">Maricopa County Library District</a> with support by the <a href=\"http://www.azlibrary.gov/\">Arizona State Library, Archives and Public Records</a>, a division of the Secretary of State, and with federal funds from the <a href=\"http://www.imls.gov/\">Institute of Museum and Library Services</a>."
            };
            site = await _siteRepository.AddSaveAsync(-1, site);
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