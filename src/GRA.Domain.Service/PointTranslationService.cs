using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class PointTranslationService : BaseUserService<PointTranslationService>
    {
        private readonly IPointTranslationRepository _pointTranslationRepository;
        private readonly IUserLogRepository _userLogRepository;

        public PointTranslationService(ILogger<PointTranslationService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IPointTranslationRepository pointTranslationRepository,
            IUserLogRepository userLogRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManagePrograms);

            ArgumentNullException.ThrowIfNull(pointTranslationRepository);
            ArgumentNullException.ThrowIfNull(userLogRepository);

            _pointTranslationRepository = pointTranslationRepository;
            _userLogRepository = userLogRepository;
        }

        public async Task<PointTranslation> AddAsync(PointTranslation pointTranslation)
        {
            ArgumentNullException.ThrowIfNull(pointTranslation);

            VerifyManagementPermission();
            pointTranslation.SiteId = GetCurrentSiteId();
            return await _pointTranslationRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                pointTranslation);
        }

        public async Task<PointTranslation> GetByIdAsync(int id)
        {
            VerifyManagementPermission();
            return await _pointTranslationRepository.GetByIdAsync(id);
        }

        public async Task<PointTranslation> GetByProgramIdAsync(int id, bool titleCase = false)
        {
            var pointTranslation = await _pointTranslationRepository.GetByProgramIdAsync(id);

            if (titleCase)
            {
                if (!string.IsNullOrWhiteSpace(pointTranslation.ActivityDescription))
                {
                    pointTranslation.ActivityDescription = pointTranslation.ActivityDescription[0]
                        .ToString()
                        .ToUpper(System.Globalization.CultureInfo.CurrentCulture)
                        + pointTranslation.ActivityDescription[1..];
                }
                if (!string.IsNullOrWhiteSpace(pointTranslation.ActivityDescriptionPlural))
                {
                    pointTranslation.ActivityDescriptionPlural =
                        pointTranslation.ActivityDescriptionPlural[0]
                        .ToString()
                        .ToUpper(System.Globalization.CultureInfo.CurrentCulture)
                        + pointTranslation.ActivityDescriptionPlural[1..];
                }
                if (!string.IsNullOrWhiteSpace(pointTranslation.TranslationDescriptionPastTense))
                {
                    pointTranslation.TranslationDescriptionPastTense =
                        pointTranslation.TranslationDescriptionPastTense[0]
                        .ToString()
                        .ToUpper(System.Globalization.CultureInfo.CurrentCulture)
                        + pointTranslation.TranslationDescriptionPastTense[1..];
                }
                if (!string.IsNullOrWhiteSpace(pointTranslation.TranslationDescriptionPresentTense))
                {
                    pointTranslation.TranslationDescriptionPresentTense =
                        pointTranslation.TranslationDescriptionPresentTense[0]
                        .ToString()
                        .ToUpper(System.Globalization.CultureInfo.CurrentCulture)
                        + pointTranslation.TranslationDescriptionPresentTense[1..];
                }
            }
            return pointTranslation;
        }

        public async Task<IEnumerable<PointTranslation>> GetListAsync()
        {
            return await _pointTranslationRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<DataWithCount<ICollection<PointTranslation>>>
            GetPaginatedListAsync(Model.Filters.BaseFilter filter)
        {
            filter ??= new Model.Filters.BaseFilter();
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<PointTranslation>>
            {
                Data = await _pointTranslationRepository.PageAsync(filter),
                Count = await _pointTranslationRepository.CountAsync(filter)
            };
        }

        public async Task<bool> HasBeenUsedAsync(int id)
        {
            VerifyManagementPermission();
            return await _userLogRepository.PointTranslationHasBeenUsedAsync(id);
        }

        public async Task RemoveAsync(int pointTranslationId)
        {
            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var pointTranslation
                = await _pointTranslationRepository.GetByIdAsync(pointTranslationId);

            if (pointTranslation.SiteId != siteId)
            {
                _logger.LogError("User {AuthenticatedUserId} cannot update point translation {PointTranslationId} for site {PointTranslationiSiteId}.",
                    authId,
                    pointTranslationId,
                    pointTranslation.SiteId);
                throw new GraException($"Permission denied - point translation belongs to site id {pointTranslation.SiteId}.");
            }
            if (await _pointTranslationRepository.IsInUseAsync(pointTranslationId))
            {
                throw new GraException($"{pointTranslation.TranslationName} is in use by programs and/or challenge tasks.");
            }
            await _pointTranslationRepository.RemoveSaveAsync(authId, pointTranslationId);
        }

        public async Task UpdateAsync(PointTranslation pointTranslation)
        {
            ArgumentNullException.ThrowIfNull(pointTranslation);

            VerifyManagementPermission();
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var currentPointTranslation = await _pointTranslationRepository.GetByIdAsync(
                pointTranslation.Id);
            if (currentPointTranslation.SiteId != siteId)
            {
                _logger.LogError("User {AuthenticatedUserId} cannot update point translation {PointTranslationId} for site {PointTranslationiSiteId}.",
                    authId,
                    currentPointTranslation.Id,
                    currentPointTranslation.SiteId);
                throw new GraException($"Permission denied - point translation belongs to site id {currentPointTranslation.SiteId}.");
            }

            currentPointTranslation.ActivityDescription = pointTranslation.ActivityDescription;
            currentPointTranslation.ActivityDescriptionPlural
                = pointTranslation.ActivityDescriptionPlural;
            currentPointTranslation.TranslationDescriptionPastTense
                = pointTranslation.TranslationDescriptionPastTense;
            currentPointTranslation.TranslationDescriptionPresentTense
                = pointTranslation.TranslationDescriptionPresentTense;
            currentPointTranslation.TranslationName = pointTranslation.TranslationName;

            var hasBeenUsed = await HasBeenUsedAsync(pointTranslation.Id);

            if (!hasBeenUsed)
            {
                currentPointTranslation.ActivityAmount = pointTranslation.ActivityAmount;
                currentPointTranslation.IsSingleEvent = pointTranslation.IsSingleEvent;
                currentPointTranslation.PointsEarned = pointTranslation.PointsEarned;
            }

            await _pointTranslationRepository.UpdateSaveAsync(authId, currentPointTranslation);
        }
    }
}
