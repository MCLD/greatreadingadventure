using GRA.Domain.Repository;
using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using GRA.Domain.Service.Abstract;

namespace GRA.Domain.Service
{
    public class SiteService : BaseUserService<SiteService>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISystemRepository _systemRepository;

        public SiteService(ILogger<SiteService> logger,
            IUserContextProvider userContextProvider,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            ISiteRepository siteRepository,
            ISystemRepository systemRepository)
            : base(logger, userContextProvider)
        {
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _systemRepository = Require.IsNotNull(systemRepository, nameof(systemRepository));
        }

        public async Task<IEnumerable<Model.System>> GetSystemList()
        {
            
            return await _systemRepository.GetAllAsync(await GetCurrentSiteId());
        }

        public async Task<IEnumerable<Branch>> GetBranches(int systemId)
        {
            return await _branchRepository.GetAllAsync(systemId);
        }

        public async Task<IEnumerable<Program>> GetProgramList()
        {
            return await _programRepository.GetAllAsync(await GetCurrentSiteId());
        }
    }
}