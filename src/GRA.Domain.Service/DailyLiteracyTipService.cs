﻿using System;
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
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class DailyLiteracyTipService : BaseUserService<DailyLiteracyTipService>
    {
        private readonly IDailyLiteracyTipImageRepository _dailyLiteracyTipImageRepository;
        private readonly IDailyLiteracyTipRepository _dailyLiteracyTipRepository;
        private readonly IPathResolver _pathResolver;

        public DailyLiteracyTipService(ILogger<DailyLiteracyTipService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
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

        public async Task<DailyLiteracyTipImage> AddImageAsync(DailyLiteracyTipImage image)
        {
            VerifyManagementPermission();
            if (image == null)
            {
                throw new GraException("Unable to add empty image.");
            }
            var filter = new DailyImageFilter()
            {
                DailyLiteracyTipId = image.DailyLiteracyTipId
            };
            image.Day = await _dailyLiteracyTipImageRepository.CountAsync(filter);

            return await _dailyLiteracyTipImageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                image);
        }

        public Task<(int, IList<string>)> AddImagesZipAsync(int dailyLiteracyTipId,
            ZipArchive archive)
        {
            VerifyManagementPermission();
            ArgumentNullException.ThrowIfNull(archive);
            return AddImagesZipInternalAsync(dailyLiteracyTipId, archive);
        }

        public async Task<DailyLiteracyTip> GetByIdAsync(int id)
        {
            return await _dailyLiteracyTipRepository.GetByIdAsync(id);
        }

        public async Task<DailyLiteracyTipImage> GetImageByDayAsync(int dailyLiteracyTipId, int day)
        {
            return await _dailyLiteracyTipImageRepository.GetByDay(dailyLiteracyTipId, day);
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

        public async Task RemoveAsync(int dailyLiteracyTipId)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var dailyLiteracyTip = await _dailyLiteracyTipRepository.GetByIdAsync(dailyLiteracyTipId);
            if (dailyLiteracyTip.SiteId != siteId)
            {
                _logger.LogError("User {AuthId} cannot delete point translation {DailyLiteracyTipId} for site {SiteId}.",
                    authId,
                    dailyLiteracyTipId,
                    dailyLiteracyTip.SiteId);
                throw new GraException($"Permission denied - point translation belongs to site id {dailyLiteracyTip.SiteId}.");
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
            if (currentImage.DailyLiteracyTip.SiteId != siteId)
            {
                _logger.LogError("User {AuthId} cannot remove daily image {CurrentImageId} for site {SiteId}.",
                    authId,
                    currentImage.Id,
                    currentImage.DailyLiteracyTip.SiteId);
                throw new GraException($"Permission denied - Daily Literacy Tip image belongs to site id {currentImage.DailyLiteracyTip.SiteId}");
            }

            await _dailyLiteracyTipImageRepository.RemoveSaveAsync(authId, imageId);
        }

        public async Task UpdateAsync(DailyLiteracyTip dailyLiteracyTip)
        {
            VerifyManagementPermission();
            if (dailyLiteracyTip == null)
            {
                throw new GraException("Unable to update empty daily literacy tip");
            }
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var currentDailyLiteracyTip = await _dailyLiteracyTipRepository.GetByIdAsync(
                dailyLiteracyTip.Id);
            if (currentDailyLiteracyTip.SiteId != siteId)
            {
                _logger.LogError("User {AuthId} cannot update point translation {TipId} for site {SiteId}.",
                    authId,
                    currentDailyLiteracyTip.Id,
                    currentDailyLiteracyTip.SiteId);
                throw new GraException($"Permission denied - Daily Literacy Tip belongs to site id {currentDailyLiteracyTip.SiteId}");
            }

            currentDailyLiteracyTip.Message = dailyLiteracyTip.Message;
            currentDailyLiteracyTip.Name = dailyLiteracyTip.Name;

            await _dailyLiteracyTipRepository.UpdateSaveAsync(authId, currentDailyLiteracyTip);
        }

        public async Task UpdateImageAsync(DailyLiteracyTipImage image)
        {
            VerifyManagementPermission();
            if (image == null)
            {
                throw new GraException("Unable to update empty daily literacy tip image");
            }
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var currentImage = await _dailyLiteracyTipImageRepository.GetByIdAsync(image.Id);
            if (currentImage.DailyLiteracyTip.SiteId != siteId)
            {
                _logger.LogError("User {AuthId} cannot update daily image {ImageId} for site {SiteId}.",
                    authId,
                    currentImage.Id,
                    currentImage.DailyLiteracyTip.SiteId);
                throw new GraException($"Permission denied - daily literacy tip image belongs to site id {currentImage.DailyLiteracyTip.SiteId}");
            }

            currentImage.Name = image.Name;
            currentImage.Extension = image.Extension;

            if (image.Day != currentImage.Day)
            {
                await _dailyLiteracyTipImageRepository.UpdateSaveAsync(authId, currentImage,
                    image.Day);
            }
            else
            {
                await _dailyLiteracyTipImageRepository.UpdateSaveAsync(authId, currentImage);
            }
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
                try
                {
                    string fullPath = Path.Combine(tipDirectoryInfo.FullName, file.Name);
                    int dupeCheck = 1;
                    while (File.Exists(fullPath))
                    {
                        fullPath = Path.Combine(tipDirectoryInfo.FullName,
                            Path.GetFileNameWithoutExtension(file.Name)
                                + $"-{dupeCheck++}"
                                + Path.GetExtension(file.Name));
                    }
                    file.ExtractToFile(fullPath);

                    await _dailyLiteracyTipImageRepository
                        .AddSaveAsync(GetClaimId(ClaimType.UserId),
                            new DailyLiteracyTipImage
                            {
                                DailyLiteracyTipId = dailyLiteracyTipId,
                                Day = dayCounter++,
                                Extension = Path.GetExtension(fullPath),
                                Name = Path.GetFileNameWithoutExtension(fullPath)
                            });

                    added++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Issue adding daily tip file {FilePath}: {ErrorMessage}",
                        file.FullName,
                        ex.Message);
                    issues.Add($"Issue adding {file.Name}: {ex.Message}");
                }
            }

            return (added, issues);
        }
    }
}
