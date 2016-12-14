using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class BadgeService : Abstract.BaseUserService<BadgeService>
    {
        const string BadgePath = "badges";
        private readonly IConfigurationRoot _config;
        private readonly IBadgeRepository _badgeRepository;
        public BadgeService(ILogger<BadgeService> logger,
            IUserContextProvider userContextProvider,
            IConfigurationRoot config,
            IBadgeRepository badgeRepository) : base(logger, userContextProvider)
        {
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _config = Require.IsNotNull(config, nameof(config));
        }

        private string GetPath()
        {
            string contentDir = _config[ConfigurationKey.ContentDirectory];
            if (!contentDir.EndsWith("/"))
            {
                contentDir += "/";
            }
            contentDir += $"site{GetClaimId(ClaimType.SiteId)}/{BadgePath}";
            if (!System.IO.Directory.Exists(contentDir))
            {
                System.IO.Directory.CreateDirectory(contentDir);
            }
            return contentDir;
        }

        private string WriteBadgeFile(Badge badge, byte[] imageFile)
        {
            string extension = System.IO.Path.GetExtension(badge.Filename).ToLower();
            string filename = $"{GetPath()}/badge{badge.Id}{extension}";
            System.IO.File.WriteAllBytes(filename, imageFile);
            return filename;
        }

        public async Task<Badge> AddBadgeAsync(Badge badge, byte[] imageFile)
        {
            var result = await _badgeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), badge);
           
            result.Filename = WriteBadgeFile(badge, imageFile);
            result = await _badgeRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), result);
            return result;
        }

        public async Task<Badge> UpdateBadgeAsync(Badge badge)
        {
            return await _badgeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), badge);
        }

        public async Task<Badge> ReplaceBadgeFileAsync(Badge badge, byte[] imageFile)
        {
            var existingBadge = await _badgeRepository.GetByIdAsync(badge.Id);
            if (System.IO.File.Exists(existingBadge.Filename))
            {
                System.IO.File.Delete(existingBadge.Filename);
            }
            badge.Filename = WriteBadgeFile(badge, imageFile);
            return await _badgeRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                badge);
        }

        public async Task<Badge> GetByIdAsync(int badgeId)
        {
            return await _badgeRepository.GetByIdAsync(badgeId);
        }
    }
}
