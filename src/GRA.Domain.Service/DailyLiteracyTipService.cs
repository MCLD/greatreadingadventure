using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class DailyLiteracyTipService : BaseUserService<DailyLiteracyTipService>
    {
        private const string noValidImagesMessage = "No valid image files were found in the archive.";

        private readonly IDailyLiteracyTipImageRepository _dailyLiteracyTipImageRepository;
        private readonly IDailyLiteracyTipRepository _dailyLiteracyTipRepository;
        private readonly IPathResolver _pathResolver;

        public DailyLiteracyTipService(ILogger<DailyLiteracyTipService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IDailyLiteracyTipImageRepository dailyLiteracyTipImageRepository,
            IDailyLiteracyTipRepository dailyLiteracyTipRepository,
            IPathResolver pathResolver)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            ArgumentNullException.ThrowIfNull(dailyLiteracyTipImageRepository);
            ArgumentNullException.ThrowIfNull(dailyLiteracyTipRepository);
            ArgumentNullException.ThrowIfNull(pathResolver);

            _dailyLiteracyTipImageRepository = dailyLiteracyTipImageRepository;
            _dailyLiteracyTipRepository = dailyLiteracyTipRepository;
            _pathResolver = pathResolver;

            SetManagementPermission(Permission.ManageDailyLiteracyTips);
        }

        public async Task<DailyLiteracyTip> AddAsync(DailyLiteracyTip dailyLiteracyTip)
        {
            VerifyManagementPermission();
            if (dailyLiteracyTip == null)
            {
                throw new GraException("Unable to add empty daily literacy tip.");
            }
            dailyLiteracyTip.SiteId = GetCurrentSiteId();

            return await _dailyLiteracyTipRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                dailyLiteracyTip);
        }

        public async Task AddImageAsync(DailyLiteracyTipImage image, IFormFile file)
        {
            VerifyManagementPermission();

            if (image == null || file == null)
            {
                throw new GraException("Image or file is missing.");
            }

            var latestDay = await _dailyLiteracyTipImageRepository
                .GetLatestDayAsync(image.DailyLiteracyTipId);
            image.Day = latestDay + 1;

            await _dailyLiteracyTipImageRepository
                .AddSaveAsync(GetClaimId(ClaimType.UserId), image);

            var siteId = GetCurrentSiteId();
            var filePath = _pathResolver.ResolveContentFilePath($"site{siteId}/dailyimages/dailyliteracytip{image.DailyLiteracyTipId}/{image.Name}{image.Extension}");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        public Task<(int, IList<string>)> AddImagesZipAsync(int dailyLiteracyTipId,
            ZipArchive archive)
        {
            VerifyManagementPermission();
            return archive == null
                ? throw new ArgumentNullException(nameof(archive))
                : AddImagesZipInternalAsync(dailyLiteracyTipId, archive);
        }

        public async Task<DailyLiteracyTip> GetByIdAsync(int id)
        {
            return await _dailyLiteracyTipRepository.GetByIdAsync(id);
        }

        public async Task<Tuple<int, int>> GetFirstLastDayAsync(int dailyTipId)
        {
            return await _dailyLiteracyTipImageRepository.GetFirstLastDayAsync(dailyTipId);
        }

        public async Task<DailyLiteracyTipImage> GetImageByDayAsync(int dailyLiteracyTipId, int day)
        {
            return await _dailyLiteracyTipImageRepository.GetByDay(dailyLiteracyTipId, day);
        }

        public async Task<DailyLiteracyTipImage> GetImageByIdAsync(int imageId)
        {
            return await _dailyLiteracyTipImageRepository.GetByIdAsync(imageId);
        }

        public async Task<int> GetImagesByTipIdAsync(int dailyLiteracyTipId)
        {
            return await _dailyLiteracyTipImageRepository.CountAsync(new DailyImageFilter
            {
                DailyLiteracyTipId = dailyLiteracyTipId
            });
        }

        public async Task<IEnumerable<DailyLiteracyTip>> GetListAsync()
        {
            return await _dailyLiteracyTipRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<DataWithCount<ICollection<DailyLiteracyTipImage>>>
            GetPaginatedImageListAsync(DailyImageFilter filter)
        {
            VerifyManagementPermission();
            return new DataWithCount<ICollection<DailyLiteracyTipImage>>
            {
                Data = await _dailyLiteracyTipImageRepository.PageAsync(filter),
                Count = await _dailyLiteracyTipImageRepository.CountAsync(filter)
            };
        }

        public async Task<DataWithCount<ICollection<DailyLiteracyTip>>> GetPaginatedImageListAsync(
                    BaseFilter filter)
        {
            VerifyManagementPermission();
            filter ??= new BaseFilter();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<DailyLiteracyTip>>
            {
                Data = await _dailyLiteracyTipRepository.PageAsync(filter),
                Count = await _dailyLiteracyTipRepository.CountAsync(filter)
            };
        }

        public async Task<DataWithCount<ICollection<DailyLiteracyTip>>> GetPaginatedListAsync(
                            BaseFilter filter)
        {
            VerifyManagementPermission();
            filter ??= new BaseFilter();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<DailyLiteracyTip>>
            {
                Data = await _dailyLiteracyTipRepository.PageAsync(filter),
                Count = await _dailyLiteracyTipRepository.CountAsync(filter)
            };
        }

        public async Task<bool> ImageNameExistsAsync(int tipId, string name, string extension)
        {
            return await _dailyLiteracyTipImageRepository
                .ImageNameExistsAsync(tipId, name, extension);
        }

        public async Task MoveImageDownAsync(int imageId)
        {
            var siteId = GetCurrentSiteId();
            await _dailyLiteracyTipImageRepository.IncreaseDayAsync(imageId, siteId);
        }

        public async Task MoveImageUpAsync(int imageId)
        {
            var siteId = GetCurrentSiteId();
            await _dailyLiteracyTipImageRepository.DecreaseDayAsync(imageId, siteId);
        }

        public async Task RemoveAsync(int dailyLiteracyTipId)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var dailyLiteracyTip = await _dailyLiteracyTipRepository
              .GetByIdAsync(dailyLiteracyTipId);

            if (dailyLiteracyTip.SiteId != siteId)
            {
                _logger.LogError("User {AuthId} cannot delete Daily Literacy Tip {TipId} for site {TipSiteId}.",
                    authId,
                    dailyLiteracyTipId,
                    dailyLiteracyTip.SiteId);
                throw new GraException($"Permission denied - Daily Literacy Tip belongs to site id {dailyLiteracyTip.SiteId}.");
            }
            if (await _dailyLiteracyTipRepository.IsInUseAsync(dailyLiteracyTipId))
            {
                throw new GraException($"{dailyLiteracyTip.Name} is in use by programs and/or challenge tasks.");
            }
            await _dailyLiteracyTipRepository.RemoveSaveAsync(authId, dailyLiteracyTipId);
        }

        public async Task RemoveImageAsync(int imageId)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var currentImage = await _dailyLiteracyTipImageRepository.GetByIdAsync(imageId);
            var tip = await _dailyLiteracyTipRepository
              .GetByIdAsync(currentImage.DailyLiteracyTipId);

            await _dailyLiteracyTipImageRepository.RemoveSaveAsync(authId, imageId);

            if (tip.SiteId != siteId)
            {
                _logger.LogError("User {AuthId} cannot remove Daily Literacy Tip image {ImageId} for site {TipSiteId}.",
                    authId,
                    currentImage.Id,
                    tip.SiteId);
                throw new GraException($"Permission denied - Daily Literacy Tip image belongs to site id {tip.SiteId}");
            }

            var filePath = _pathResolver.ResolveContentFilePath($"site{siteId}/dailyimages/dailyliteracytip{tip.Id}/{currentImage.Name}{currentImage.Extension}");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task SetImageDayAsync(int imageId, int newDay)
        {
            VerifyManagementPermission();

            var image = await _dailyLiteracyTipImageRepository.GetByIdAsync(imageId)
                ?? throw new GraException("Image not found.");
            var tipId = image.DailyLiteracyTipId;
            var count = await _dailyLiteracyTipImageRepository
              .CountAsync(new DailyImageFilter { DailyLiteracyTipId = tipId });

            newDay = Math.Max(1, Math.Min(newDay, count));

            if (newDay == image.Day)
            {
                return;
            }

            await _dailyLiteracyTipImageRepository
                .UpdateDayAndShiftOthersAsync(imageId, newDay, GetCurrentSiteId());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
          "CA1031:Do not catch general exception types",
          Justification = "Don't fail the entire import on issues with single images")]
        private async Task<(int, IList<string>)> AddImagesZipInternalAsync(int dailyLiteracyTipId,
            ZipArchive archive)
        {
            VerifyManagementPermission();
            var issues = new List<string>();

            var rootPath = _pathResolver.ResolveContentFilePath($"site{GetCurrentSiteId()}");

            var tipDirectoryInfo = Directory.CreateDirectory(Path.Combine(rootPath,
              "dailyimages",
              $"dailyliteracytip{dailyLiteracyTipId}"));

            int dayCounter = await _dailyLiteracyTipImageRepository
              .GetLatestDayAsync(dailyLiteracyTipId);
            dayCounter++;

            int added = 0;

            foreach (var file in archive.Entries.OrderBy(_ => _.Name))
            {
                if (string.IsNullOrWhiteSpace(file.Name))
                {
                    continue;
                }

                try
                {
                    var extension = Path.GetExtension(file.Name);
                    if (!ValidFiles.ImageExtensions.Contains(extension,
                      StringComparer.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Skipped file {FileName} due to invalid extension: {Extension}",
                            file.Name,
                            extension);
                        issues.Add($"Skipped {file.Name}: unsupported image file type.");
                        continue;
                    }

                    var originalName = Path.GetFileNameWithoutExtension(file.Name);
                    var safeName = originalName;
                    int dupeCheck = 1;
                    string fullPath = Path
                        .Combine(tipDirectoryInfo.FullName, $"{safeName}{extension}");

                    while (File.Exists(fullPath))
                    {
                        safeName = $"{originalName}-{dupeCheck++}";
                        fullPath = Path
                            .Combine(tipDirectoryInfo.FullName, $"{safeName}{extension}");
                    }

                    file.ExtractToFile(fullPath);

                    await _dailyLiteracyTipImageRepository
                        .AddSaveAsync(GetClaimId(ClaimType.UserId),
                              new DailyLiteracyTipImage
                              {
                                  DailyLiteracyTipId = dailyLiteracyTipId,
                                  Day = dayCounter++,
                                  Extension = extension,
                                  Name = safeName
                              });

                    added++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Issue adding Daily Literacy Tip image {EntryName}: {ErrorMessage}",
                        file.Name,
                        ex.Message);
                    issues.Add($"Error adding {file.Name}: {ex.Message}");
                }
            }
            if (added == 0)
            {
                _logger.LogWarning(noValidImagesMessage);
                issues.Add(noValidImagesMessage);
            }

            return (added, issues);
        }
    }
}
