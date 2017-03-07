using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class SystemInformationService : BaseUserService<SystemInformationService>
    {
        private readonly ISystemInformationRepository _systemInformationRepository;
        public SystemInformationService(ILogger<SystemInformationService> logger,
            IUserContextProvider userContextProvider,
            ISystemInformationRepository systemInformationRepository)
            : base(logger, userContextProvider)
        {
            _systemInformationRepository = Require.IsNotNull(systemInformationRepository,
                nameof(systemInformationRepository));
        }

        public async Task<string> GetCurrentMigrationAsync()
        {
            return await _systemInformationRepository.GetCurrentMigrationAsync();
        }
    }
}
