using System;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

namespace GRA.Domain.Service
{
    public class SiteService : Abstract.BaseService<SiteService>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISystemRepository _systemRepository;

        public SiteService(ILogger<SiteService> logger,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            ISiteRepository siteRepository,
            ISystemRepository systemRepository)
            : base(logger)
        {
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _systemRepository = Require.IsNotNull(systemRepository, nameof(systemRepository));
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