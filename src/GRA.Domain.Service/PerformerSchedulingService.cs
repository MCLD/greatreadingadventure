using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class PerformerSchedulingService : BaseUserService<PerformerSchedulingService>
    {
        private readonly Regex AlphanumericRegex = new Regex("[^a-zA-Z0-9 -]");

        private const string ReferencesPath = "references";

        public IPathResolver _pathResolver;
        public IPsDatesRepository _psDatesRepository;
        public IPsPerformerRepository _psPerformerRepository;
        public PerformerSchedulingService(ILogger<PerformerSchedulingService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IPathResolver pathResolver,
            IPsDatesRepository psDatesRepository,
            IPsPerformerRepository psPerformerRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
            _psDatesRepository = psDatesRepository
                ?? throw new ArgumentNullException(nameof(psDatesRepository));
            _psPerformerRepository = psPerformerRepository
                ?? throw new ArgumentNullException(nameof(psPerformerRepository));
        }

        public async Task<PsDates> GetDates()
        {
            var siteId = GetCurrentSiteId();
            return await _psDatesRepository.GetBySiteIdAsync(siteId);
        }

        public PsSchedulingStage GetSchedulingStage(PsDates dates)
        {
            if (dates.PerformerRegistrationClosed == null
                && dates.PerformerRegistrationOpen == null
                && dates.PerformerSchedulePosted == null
                && dates.PerformerSchedulingClosed == null
                && dates.PerformerSchedulingOpen == null
                && dates.PerformerSchedulingPreview == null)
            {
                return PsSchedulingStage.Unavailable;
            }

            if (dates.PerformerSchedulePosted != null
                && _dateTimeProvider.Now >= dates.PerformerSchedulePosted)
            {
                return PsSchedulingStage.SchedulePosted;
            }
            if (dates.PerformerSchedulingClosed != null
                && _dateTimeProvider.Now >= dates.PerformerSchedulingClosed)
            {
                return PsSchedulingStage.SchedulingClosed;
            }
            if (dates.PerformerSchedulingOpen != null
                && _dateTimeProvider.Now >= dates.PerformerSchedulingOpen)
            {
                return PsSchedulingStage.SchedulingOpen;
            }
            if (dates.PerformerSchedulingPreview != null
                && _dateTimeProvider.Now >= dates.PerformerSchedulingPreview)
            {
                return PsSchedulingStage.SchedulingPreview;
            }
            if (dates.PerformerRegistrationClosed != null
                && _dateTimeProvider.Now >= dates.PerformerRegistrationClosed)
            {
                return PsSchedulingStage.PerformerClosed;
            }
            if (dates.PerformerRegistrationOpen != null
                && _dateTimeProvider.Now >= dates.PerformerRegistrationOpen)
            {
                return PsSchedulingStage.PerformerOpen;
            }

            return PsSchedulingStage.BeforeRegistration;
        }

        public async Task<PsPerformer> GetPerformerByUserIdAsync(int userId)
        {
            var authId = GetClaimId(ClaimType.UserId);
            if (authId != userId)
            {
                _logger.LogError($"User {authId} doesn't have permission to view performer information for user {userId}.");
                throw new GraException("Permission denied.");
            }

            return await _psPerformerRepository.GetByUserIdAsync(userId);
        }

        public async Task<PsPerformer> AddPerformerAsync(PsPerformer performer)
        {
            var authId = GetClaimId(ClaimType.UserId);

            performer.BillingAddress = performer.BillingAddress.Trim();
            performer.Name = performer.Name.Trim();
            performer.Phone = performer.Phone.Trim();
            performer.UserId = authId;
            performer.VendorId = performer.VendorId.Trim();
            performer.Website = performer.Website?.Trim();

            await _psPerformerRepository.AddSaveAsync(authId, performer);
            return performer;
        }

        public async Task<PsPerformer> EditPerformerAsync(PsPerformer performer)
        {
            var authId = GetClaimId(ClaimType.UserId);

            var currentPerformer = await _psPerformerRepository.GetByIdAsync(authId);
            if (currentPerformer == null)
            {
                throw new GraException("Performer not found.");
            }

            currentPerformer.BillingAddress = performer.BillingAddress.Trim();
            currentPerformer.HasFingerprintCard = performer.HasFingerprintCard;
            currentPerformer.Name = performer.Name.Trim();
            currentPerformer.Phone = performer.Phone.Trim();
            currentPerformer.PhonePreferred = performer.PhonePreferred;
            currentPerformer.VendorId = performer.VendorId.Trim();
            currentPerformer.Website = performer.Website?.Trim();

            await _psPerformerRepository.UpdateSaveAsync(authId, currentPerformer);
            return currentPerformer;
        }

        public async Task AddPerformerReferencesAsync(int performerId, byte[] referenceBytes,
            string fileExtension)
        {
            var performer = await _psPerformerRepository.GetByIdAsync(performerId);

            if (!string.IsNullOrWhiteSpace(performer.ReferencesFilename))
            {
                var filePath = _pathResolver.ResolveContentFilePath(performer.ReferencesFilename);
                System.IO.File.WriteAllBytes(filePath, referenceBytes);
            }
            else
            {
                var siteId = GetCurrentSiteId();

                var performerFilename = AlphanumericRegex.Replace(performer.Name, "");
                var referencesFilename = $"{performerFilename}_references.{fileExtension}";

                while (System.IO.File.Exists(_pathResolver.ResolveContentFilePath(
                        Path.Combine($"site{siteId}", ReferencesPath, referencesFilename))))
                {
                    referencesFilename = $"{performerFilename}_references" +
                        $"_{Path.GetRandomFileName().Replace(".", "")}{fileExtension}";
                }

                performer.ReferencesFilename = Path.Combine($"site{siteId}", ReferencesPath,
                    referencesFilename);
                await 
            }
        }
    }
}
