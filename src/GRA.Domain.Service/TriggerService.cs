﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class TriggerService : BaseUserService<TriggerService>
    {
        private readonly IAvatarBundleRepository _avatarBundleRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IProgramRepository _programRepository;
        private readonly SiteLookupService _siteLookupService;
        private readonly ISystemRepository _systemRepository;
        private readonly ITriggerRepository _triggerRepository;

        public TriggerService(ILogger<TriggerService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IAvatarBundleRepository avatarBundleRepository,
            IBranchRepository branchRepository,
            IEventRepository eventRepository,
            IProgramRepository programRepository,
            ISystemRepository systemRepository,
            ITriggerRepository triggerRepository,
            SiteLookupService siteLookupService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageTriggers);
            _avatarBundleRepository = avatarBundleRepository
                ?? throw new ArgumentNullException(
                nameof(avatarBundleRepository));
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _eventRepository = eventRepository
                ?? throw new ArgumentNullException(nameof(eventRepository));
            _programRepository = programRepository
                ?? throw new ArgumentNullException(nameof(programRepository));
            _systemRepository = systemRepository
                ?? throw new ArgumentNullException(nameof(systemRepository));
            _triggerRepository = triggerRepository
                ?? throw new ArgumentNullException(nameof(triggerRepository));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
        }

        public async Task<Trigger> AddAsync(Trigger trigger)
        {
            VerifyManagementPermission();
            trigger.SiteId = GetCurrentSiteId();
            trigger.RelatedBranchId = GetClaimId(ClaimType.BranchId);
            trigger.RelatedSystemId = GetClaimId(ClaimType.SystemId);
            if (!HasPermission(Permission.ManageVendorCodes))
            {
                trigger.AwardVendorCodeTypeId = null;
            }
            if (!HasPermission(Permission.ManageAvatars))
            {
                trigger.AwardAvatarBundleId = null;
            }
            if (!HasPermission(Permission.ManageTriggerMail))
            {
                trigger.AwardMail = null;
                trigger.AwardMailSubject = null;
            }
            if (!HasPermission(Permission.TriggerAttachments))
            {
                trigger.AwardAttachmentId = null;
            }
            await ValidateTriggerAsync(trigger);
            return await _triggerRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                trigger);
        }

        public async Task<bool> CodeExistsAsync(string secretCode, int? triggerId = null)
        {
            return await _triggerRepository.CodeExistsAsync(
                GetCurrentSiteId(), secretCode.Trim().ToLower(), triggerId);
        }

        public async Task<Trigger> GetByBadgeIdAsync(int badgeId)
        {
            return await _triggerRepository.GetByBadgeIdAsync(badgeId);
        }

        public async Task<Trigger> GetByIdAsync(int id)
        {
            VerifyManagementPermission();
            return await _triggerRepository.GetByIdAsync(id);
        }

        public async Task<ICollection<Trigger>> GetDependentsAsync(int triggerId)
        {
            return await _triggerRepository.GetTriggerDependentsAsync(triggerId);
        }

        public async Task<int?> GetMaximumAllowedPointsAsync(int siteId)
        {
            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingIntAsync(siteId,
                SiteSettingKey.Triggers.MaxPointsPerTrigger);

            return IsSet ? SetValue : (int?)null;
        }

        public async Task<DataWithCount<ICollection<Trigger>>> GetPaginatedListAsync(TriggerFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<Trigger>>
            {
                Data = await _triggerRepository.PageAsync(filter),
                Count = await _triggerRepository.CountAsync(filter)
            };
        }

        public async Task<ICollection<TriggerRequirement>> GetRequirementsByIdsAsync(
            List<int> badgeIds, List<int> challengeIds)
        {
            VerifyManagementPermission();
            var trigger = new Trigger()
            {
                BadgeIds = badgeIds,
                ChallengeIds = challengeIds
            };
            return await _triggerRepository.GetTriggerRequirmentsAsync(trigger);
        }

        public async Task<string> GetTriggerPrizeNameAsync(int id)
        {
            VerifyPermission(Permission.ViewUserPrizes);

            var trigger = await _triggerRepository.GetByIdAsync(id);

            return trigger?.AwardPrizeName;
        }

        public async Task<ICollection<TriggerRequirement>> GetTriggerRequirementsAsync(Trigger trigger)
        {
            VerifyManagementPermission();
            return await _triggerRepository.GetTriggerRequirmentsAsync(trigger);
        }

        public async Task<ICollection<Trigger>> GetTriggersAwardingPrizesAsync()
        {
            VerifyPermission(Permission.ViewAllReporting);
            return await _triggerRepository.GetTriggersAwardingPrizesAsync(GetCurrentSiteId());
        }

        public async Task<DataWithCount<ICollection<TriggerRequirement>>> PageRequirementAsync(BaseFilter filter)
        {
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<TriggerRequirement>>()
            {
                Data = await _triggerRepository.PageRequirementsAsync(filter),
                Count = await _triggerRepository.CountRequirementsAsync(filter)
            };
        }

        public async Task RemoveAsync(int triggerId)
        {
            VerifyManagementPermission();
            var dependentTriggers = await _triggerRepository.DependentTriggers(triggerId);
            if (dependentTriggers?.Count > 0)
            {
                var ex = new GraException("Trigger has dependencies");
                foreach (var dependentTrigger in dependentTriggers)
                {
                    ex.Data.Add(dependentTrigger.Key, dependentTrigger.Value);
                }
                throw ex;
            }
            var trigger = await _triggerRepository.GetByIdAsync(triggerId);
            trigger.IsDeleted = true;
            trigger.SecretCode = null;
            trigger.BadgeIds = new List<int>();
            trigger.ChallengeIds = new List<int>();

            await _triggerRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                trigger);

            var requireSecretCode = await _siteLookupService.GetSiteSettingBoolAsync(
                GetCurrentSiteId(), SiteSettingKey.Events.RequireBadge);

            if (!requireSecretCode)
            {
                await _eventRepository.DetachRelatedTrigger(triggerId);
            }
        }

        public async Task<bool> SecretCodeInUseAsync(string username)
        {
            string trimmedUsername = username.Trim();
            return await _triggerRepository.SecretCodeInUseAsync(GetCurrentSiteId(), trimmedUsername);
        }

        public async Task<Trigger> UpdateAsync(Trigger trigger)
        {
            VerifyManagementPermission();
            trigger.SiteId = GetCurrentSiteId();
            await ValidateTriggerAsync(trigger);
            if (!HasPermission(Permission.ManageVendorCodes)
                || !HasPermission(Permission.ManageTriggerMail)
                || !HasPermission(Permission.ManageAvatars))
            {
                var currentTrigger = await _triggerRepository.GetByIdAsync(trigger.Id);
                if (!HasPermission(Permission.ManageVendorCodes))
                {
                    trigger.AwardVendorCodeTypeId = currentTrigger.AwardVendorCodeTypeId;
                }
                if (!HasPermission(Permission.ManageAvatars))
                {
                    trigger.AwardAvatarBundleId = currentTrigger.AwardAvatarBundleId;
                }
                if (!HasPermission(Permission.ManageTriggerMail))
                {
                    trigger.AwardMail = currentTrigger.AwardMail;
                    trigger.AwardMailSubject = currentTrigger.AwardMailSubject;
                }
            }
            return await _triggerRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                trigger);
        }

        private async Task ValidateTriggerAsync(Trigger trigger)
        {
            if (trigger.LimitToSystemId.HasValue)
            {
                if (!(await _systemRepository.ValidateAsync(
                    trigger.LimitToSystemId.Value, trigger.SiteId)))
                {
                    throw new GraException("Invalid System selection.");
                }
                if (trigger.LimitToBranchId.HasValue && !(await _branchRepository.ValidateAsync(
                        trigger.LimitToBranchId.Value, trigger.LimitToSystemId.Value)))
                {
                    throw new GraException("Invalid Branch selection.");
                }
            }
            else if (trigger.LimitToBranchId.HasValue
                && !await _branchRepository.ValidateBySiteAsync(
                    trigger.LimitToBranchId.Value, trigger.SiteId))
            {
                throw new GraException("Invalid Branch selection.");
            }

            if (trigger.LimitToProgramId.HasValue && !(await _programRepository.ValidateAsync(
                    trigger.LimitToProgramId.Value, trigger.SiteId)))
            {
                throw new GraException("Invalid Program selection.");
            }

            if (!string.IsNullOrWhiteSpace(trigger.SecretCode))
            {
                if (await _triggerRepository.CodeExistsAsync(
                    trigger.SiteId, trigger.SecretCode, trigger.Id))
                {
                    throw new GraException("Secret code already in use.");
                }
            }
            else if (trigger.ItemsRequired > 0)
            {
                // To Do
            }
            else if (!(trigger.Points > 0))
            {
                throw new GraException("No criteria selected.");
            }

            if (trigger.AwardAvatarBundleId.HasValue)
            {
                var bundle = await _avatarBundleRepository
                    .GetByIdAsync(trigger.AwardAvatarBundleId.Value, false);

                if (bundle?.CanBeUnlocked != true)
                {
                    throw new GraException("Invalid Avatar Bundle selection.");
                }
            }

            var maxPointLimit = await GetMaximumAllowedPointsAsync(GetCurrentSiteId());
            if (maxPointLimit.HasValue && !HasPermission(Permission.IgnorePointLimits))
            {
                var currentTrigger = await _triggerRepository.GetByIdAsync(trigger.Id);
                if (currentTrigger?.AwardPoints > maxPointLimit)
                {
                    throw new GraException("Permission denied.");
                }
                if (trigger.AwardPoints > maxPointLimit)
                {
                    throw new GraException($"A trigger may award a maximum of {maxPointLimit} points.");
                }
            }
        }
    }
}
