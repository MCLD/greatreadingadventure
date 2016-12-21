using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class StaticAvatarService : Abstract.BaseUserService<StaticAvatarService>
    {
        private readonly IStaticAvatarRepository _staticAvatarRepository;
        public StaticAvatarService(ILogger<StaticAvatarService> logger,
            IStaticAvatarRepository staticAvatarRepository,
            IUserContextProvider userContextProvider) : base(logger, userContextProvider)
        {
            _staticAvatarRepository = Require.IsNotNull(staticAvatarRepository,
                nameof(staticAvatarRepository));
        }

        public async Task<IEnumerable<StaticAvatar>> GetAvartarListAsync()
        {
            int siteId = GetClaimId(ClaimType.SiteId);
            return await _staticAvatarRepository.GetAvartarListAsync(siteId);
        }

        public async Task<StaticAvatar> GetByIdAsync(int id)
        {
            int siteId = GetClaimId(ClaimType.SiteId);
            var avatar = await _staticAvatarRepository.GetByIdAsync(siteId, id);
            if (avatar == null)
            {
                _logger.LogError($"User {GetClaimId(ClaimType.UserId)} cannot change avatar to avatar {id}");
                throw new GraException("Avatar does not exist");
            }
            return avatar;
        }
    }
}
