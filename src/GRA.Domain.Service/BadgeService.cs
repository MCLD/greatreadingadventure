using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class BadgeService : Abstract.BaseUserService<BadgeService>
    {
        private const string BadgePath = "badges";
        private readonly IConfigurationRoot _config;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IPathResolver _pathResolver;
        public BadgeService(ILogger<BadgeService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IConfigurationRoot config,
            IBadgeRepository badgeRepository,
            IPathResolver pathResolver) : base(logger, dateTimeProvider, userContextProvider)
        {
            _config = Require.IsNotNull(config, nameof(config));
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _pathResolver = Require.IsNotNull(pathResolver, nameof(pathResolver));
        }

        private string GetFilePath(string filename)
        {
            string contentDir = _pathResolver.ResolveContentFilePath();
            contentDir = System.IO.Path.Combine(contentDir,
                    $"site{GetCurrentSiteId()}",
                    BadgePath);

            if (!System.IO.Directory.Exists(contentDir))
            {
                System.IO.Directory.CreateDirectory(contentDir);
            }
            return System.IO.Path.Combine(contentDir, filename);
        }

        private string GetUrlPath(string filename)
        {
            return $"site{GetCurrentSiteId()}/{BadgePath}/{filename}";
        }

        private string WriteBadgeFile(Badge badge, byte[] imageFile)
        {
            string extension = System.IO.Path.GetExtension(badge.Filename).ToLower();
            string filename = $"badge{badge.Id}{extension}";
            string fullFilePath = GetFilePath(filename);
            _logger.LogInformation($"Writing out badge file {fullFilePath}...");
            System.IO.File.WriteAllBytes(fullFilePath, imageFile);
            return GetUrlPath(filename);
        }

        public async Task<Badge> AddBadgeAsync(Badge badge, byte[] imageFile)
        {
            badge.SiteId = GetCurrentSiteId();
            var result = await _badgeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), badge);

            result.Filename = WriteBadgeFile(result, imageFile);
            result = await _badgeRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), result);
            return result;
        }

        public async Task<Badge> UpdateBadgeAsync(Badge badge)
        {
            badge.SiteId = GetCurrentSiteId();
            return await _badgeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), badge);
        }

        public async Task<Badge> ReplaceBadgeFileAsync(Badge badge, byte[] imageFile)
        {
            var existingBadge = await _badgeRepository.GetByIdAsync(badge.Id);
            if (System.IO.File.Exists(existingBadge.Filename))
            {
                System.IO.File.Delete(existingBadge.Filename);
            }
            badge.Filename = WriteBadgeFile(existingBadge, imageFile);
            return await _badgeRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                badge);
        }

        public async Task<Badge> GetByIdAsync(int badgeId)
        {
            return await _badgeRepository.GetByIdAsync(badgeId);
        }
    }
}
