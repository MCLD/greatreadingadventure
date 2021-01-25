using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace GRA.Domain.Service
{
    public class BadgeService : Abstract.BaseUserService<BadgeService>
    {
        public static readonly int DefaultMaxDimension = 400;
        public static readonly int KBSize = 1024;

        private const string BadgePath = "badges";
        private readonly IBadgeRepository _badgeRepository;
        private readonly IPathResolver _pathResolver;
        private readonly SiteLookupService _siteLookupService;
        public BadgeService(ILogger<BadgeService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IBadgeRepository badgeRepository,
            IPathResolver pathResolver,
            SiteLookupService siteLookupService) 
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _badgeRepository = badgeRepository 
                ?? throw new ArgumentNullException(nameof(badgeRepository));
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
        }

        private string GetFilePath(string filename)
        {
            string contentDir = _pathResolver.ResolveContentFilePath();
            contentDir = System.IO.Path.Combine(contentDir,
                    $"site{GetCurrentSiteId()}",
                    BadgePath);

            if (!Directory.Exists(contentDir))
            {
                Directory.CreateDirectory(contentDir);
            }
            return Path.Combine(contentDir, filename);
        }

        private string GetUrlPath(string filename)
        {
            return $"site{GetCurrentSiteId()}/{BadgePath}/{filename}";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Filename extension is ok to be lowercase")]
        private async Task<string> WriteBadgeFileAsync(Badge badge,
            byte[] imageFile,
            ImageType? imageType)
        {
            string extension = imageType.HasValue
                ? "." + imageType.ToString().ToLowerInvariant()
                : Path.GetExtension(badge.Filename).ToLowerInvariant();

            string filename = $"badge{badge.Id}{extension}";
            string fullFilePath = GetFilePath(filename);

            using (var image = Image.Load(imageFile))
            {
                var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingIntAsync(
                    GetCurrentSiteId(),
                    SiteSettingKey.Badges.MaxDimension);

                int maxDimension = IsSet
                    ? SetValue
                    : DefaultMaxDimension;

                if (image.Width > maxDimension || image.Height > maxDimension)
                {
                    _logger.LogInformation("Resizing badge file {BadgeFile}", fullFilePath);
                    var sw = Stopwatch.StartNew();
                    image.Mutate(_ => _.Resize(maxDimension,
                        maxDimension,
                        KnownResamplers.Lanczos3));

                    if (image.Metadata != null)
                    {
                        image.Metadata.ExifProfile = null;
                        image.Metadata.IccProfile = null;
                        image.Metadata.IptcProfile = null;
                    }
                    switch (extension)
                    {
                        case "jpg":
                        case "jpeg":
                            await image.SaveAsJpegAsync(fullFilePath, new JpegEncoder
                            {
                                Quality = 77
                            });
                            break;
                        default:
                            await image.SaveAsPngAsync(fullFilePath, new PngEncoder
                            {
                                CompressionLevel = PngCompressionLevel.BestCompression,
                                IgnoreMetadata = true
                            });
                            break;
                    }
                    _logger.LogInformation("Image resize and save of {Filename} took {Elapsed} ms",
                        filename,
                        sw.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogDebug("Writing out badge file {BadgeFile}", fullFilePath);
                    File.WriteAllBytes(fullFilePath, imageFile);
                }
                return GetUrlPath(filename);
            }
        }

        public async Task<Badge> AddBadgeAsync(Badge badge, byte[] imageFile)
        {
            badge.SiteId = GetCurrentSiteId();
            var result = await _badgeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), badge);
            result.Filename = await WriteBadgeFileAsync(result, imageFile, imageType: null);
            result.AltText = badge.AltText?.Trim();
            return await _badgeRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), result);
        }

        public async Task<Badge> ReplaceBadgeFileAsync(Badge badge,
            byte[] imageFile,
            string uploadFilename)
        {
            var existingBadge = await _badgeRepository.GetByIdAsync(badge.Id);

            if (imageFile != null)
            {
                if (File.Exists(existingBadge.Filename))
                {
                    File.Delete(existingBadge.Filename);
                }

                var imageType = ImageType.Png;

                if (!string.IsNullOrEmpty(uploadFilename))
                {
                    var uploadExtension = Path
                        .GetExtension(uploadFilename)
                        .ToLowerInvariant();

                    if (uploadExtension.EndsWith("jpg", StringComparison.OrdinalIgnoreCase)
                        || uploadExtension.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        imageType = ImageType.Jpg;
                    }
                }

                badge.Filename = await WriteBadgeFileAsync(existingBadge, imageFile, imageType);
            }
            badge.AltText = badge.AltText?.Trim();
            return await _badgeRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), badge);
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
            var (IsSet, SetValue) = await _siteLookupService
                    .GetSiteSettingIntAsync(GetCurrentSiteId(), SiteSettingKey.Badges.MaxFileSize);
            if (IsSet && badgeImage != null && badgeImage.Length > SetValue * KBSize)
            {
                throw new GraException($"File size exceeds the maximum of {SetValue}KB");
            }

            using (var image = Image.Load(badgeImage))
            {
                if (image.Height != image.Width)
                {
                    throw new GraException("Badge images must be square.");
                }
            }
        }
    }
}
