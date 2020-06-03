using System;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace GRA.Domain.Service
{
    public class BadgeService : Abstract.BaseUserService<BadgeService>
    {
        public static readonly int KBSize = 1024;

        private const string BadgePath = "badges";
        private readonly IConfiguration _config;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IPathResolver _pathResolver;
        private readonly SiteLookupService _siteLookupService;
        public BadgeService(ILogger<BadgeService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IConfiguration config,
            IBadgeRepository badgeRepository,
            IPathResolver pathResolver,
            SiteLookupService siteLookupService) : base(logger, dateTimeProvider, userContextProvider)
        {
            _config = Require.IsNotNull(config, nameof(config));
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _pathResolver = Require.IsNotNull(pathResolver, nameof(pathResolver));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
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
            _logger.LogDebug("Writing out badge file {BadgeFile}", fullFilePath);
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

        public async Task<string> GetBadgeFilenameAsync(int id)
        {
            return await _badgeRepository.GetBadgeFileNameAsync(id);
        }

        public async Task ValidateBadgeImageAsync(byte[] badgeImage)
        {
            var maxUploadSize = await _siteLookupService
                    .GetSiteSettingIntAsync(GetCurrentSiteId(), SiteSettingKey.Badges.MaxFileSize);
            if (maxUploadSize.IsSet && badgeImage.Length > maxUploadSize.SetValue * KBSize)
            {
                throw new GraException($"File size exceeds the maximum of {maxUploadSize.SetValue}KB");
            }

            using (var image = Image.Load(badgeImage))
            {
                if (image.Height != image.Width || image.Height > 400 || image.Width > 400)
                {
                    throw new GraException("Image height and width must be the same and cannot exceed 400px");
                }
            }
        }
    }
}
