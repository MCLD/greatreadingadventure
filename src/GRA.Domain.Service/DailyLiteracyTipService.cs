using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        public DailyLiteracyTipService(ILogger<DailyLiteracyTipService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IDailyLiteracyTipImageRepository dailyLiteracyTipImageRepository,
            IDailyLiteracyTipRepository dailyLiteracyTipRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageDailyLiteracyTips);
            _dailyLiteracyTipImageRepository = Require.IsNotNull(dailyLiteracyTipImageRepository,
                nameof(dailyLiteracyTipImageRepository));
            _dailyLiteracyTipRepository = Require.IsNotNull(dailyLiteracyTipRepository,
                nameof(dailyLiteracyTipRepository));
        }

        public async Task<IEnumerable<DailyLiteracyTip>> GetListAsync()
        {
            return await _dailyLiteracyTipRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<DataWithCount<ICollection<DailyLiteracyTip>>> GetPaginatedListAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<DailyLiteracyTip>>
            {
                Data = await _dailyLiteracyTipRepository.PageAsync(filter),
                Count = await _dailyLiteracyTipRepository.CountAsync(filter)
            };
        }

        public async Task<DailyLiteracyTip> AddAsync(DailyLiteracyTip dailyLiteracyTip)
        {
            VerifyManagementPermission();

            dailyLiteracyTip.SiteId = GetCurrentSiteId();

            return await _dailyLiteracyTipRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                dailyLiteracyTip);
        }

        public async Task UpdateAsync(DailyLiteracyTip dailyLiteracyTip)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var currentDailyLiteracyTip = await _dailyLiteracyTipRepository.GetByIdAsync(
                dailyLiteracyTip.Id);
            if (currentDailyLiteracyTip.SiteId != siteId)
            {
                _logger.LogError($"User {authId} cannot update point translation {currentDailyLiteracyTip.Id} for site {currentDailyLiteracyTip.SiteId}.");
                throw new GraException($"Permission denied - Daily Literacy Tip belongs to site id {currentDailyLiteracyTip.SiteId}");
            }

            currentDailyLiteracyTip.Message = dailyLiteracyTip.Message;
            currentDailyLiteracyTip.Name = dailyLiteracyTip.Name;

            await _dailyLiteracyTipRepository.UpdateSaveAsync(authId, currentDailyLiteracyTip);
        }

        public async Task RemoveAsync(int dailyLiteracyTipId)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var dailyLiteracyTip = await _dailyLiteracyTipRepository.GetByIdAsync(dailyLiteracyTipId);
            if (dailyLiteracyTip.SiteId != siteId)
            {
                _logger.LogError($"User {authId} cannot delete point translation {dailyLiteracyTipId} for site {dailyLiteracyTip.SiteId}.");
                throw new GraException($"Permission denied - point translation belongs to site id {dailyLiteracyTip.SiteId}.");
            }
            if (await _dailyLiteracyTipRepository.IsInUseAsync(dailyLiteracyTipId))
            {
                throw new GraException($"{dailyLiteracyTip.Name} is in use by programs and/or challenge tasks.");
            }
            await _dailyLiteracyTipRepository.RemoveSaveAsync(authId, dailyLiteracyTipId);
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

        public async Task<DailyLiteracyTipImage> AddImageAsync(DailyLiteracyTipImage image)
        {
            VerifyManagementPermission();
            var filter = new DailyImageFilter()
            {
                DailyLiteracyTipId = image.DailyLiteracyTipId
            };
            image.Day = await _dailyLiteracyTipImageRepository.CountAsync(filter);

            return await _dailyLiteracyTipImageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                image);
        }

        public async Task UpdateImageAsync(DailyLiteracyTipImage image)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var currentImage = await _dailyLiteracyTipImageRepository.GetByIdAsync(image.Id);
            if (currentImage.DailyLiteracyTip.SiteId != siteId)
            {
                _logger.LogError($"User {authId} cannot update daily image {currentImage.Id} for site {currentImage.DailyLiteracyTip.SiteId}.");
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

        public async Task RemoveImageAsync(int imageId)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var currentImage = await _dailyLiteracyTipImageRepository.GetByIdAsync(imageId);
            if (currentImage.DailyLiteracyTip.SiteId != siteId)
            {
                _logger.LogError($"User {authId} cannot remove daily image {currentImage.Id} for site {currentImage.DailyLiteracyTip.SiteId}.");
                throw new GraException($"Permission denied - Daily Literacy Tip image belongs to site id {currentImage.DailyLiteracyTip.SiteId}");
            }

            await _dailyLiteracyTipImageRepository.RemoveSaveAsync(authId, imageId);
        }

        public async Task<DailyLiteracyTipImage> GetImageByDayAsync(int dailyLiteracyTipId, int day)
        {
            return await _dailyLiteracyTipImageRepository.GetByDay(dailyLiteracyTipId, day);
        }
    }
}
