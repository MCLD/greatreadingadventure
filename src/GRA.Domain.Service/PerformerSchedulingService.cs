using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class PerformerSchedulingService : BaseUserService<PerformerSchedulingService>
    {
        private readonly Regex AlphanumericRegex = new Regex("[^a-zA-Z0-9 -]");

        private const string PerformerImagePath = "performerimage";
        private const string ReferencesPath = "performerreferences";

        public IPathResolver _pathResolver;
        public IPsBlackoutDateRepository _psBlackoutDateRepository;
        public IPsDatesRepository _psDatesRepository;
        public IPsPerformerImageRepository _psPerformerImageRepository;
        public IPsPerformerRepository _psPerformerRepository;
        public PerformerSchedulingService(ILogger<PerformerSchedulingService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IPathResolver pathResolver,
            IPsBlackoutDateRepository psBlackoutDateRepository,
            IPsDatesRepository psDatesRepository,
            IPsPerformerImageRepository psPerformerImageRepository,
            IPsPerformerRepository psPerformerRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
            _psBlackoutDateRepository = psBlackoutDateRepository
                ?? throw new ArgumentNullException(nameof(psBlackoutDateRepository));
            _psDatesRepository = psDatesRepository
                ?? throw new ArgumentNullException(nameof(psDatesRepository));
            _psPerformerImageRepository = psPerformerImageRepository
                ?? throw new ArgumentNullException(nameof(psPerformerImageRepository));
            _psPerformerRepository = psPerformerRepository
                ?? throw new ArgumentNullException(nameof(psPerformerRepository));
        }

        public PsSchedulingStage GetSchedulingStage(PsDates dates)
        {
            if (dates?.RegistrationClosed == null
                || dates?.RegistrationOpen == null
                || dates?.SchedulePosted == null
                || dates?.SchedulingClosed == null
                || dates?.SchedulingOpen == null
                || dates?.SchedulingPreview == null
                || dates?.ScheduleStartDate == null
                || dates?.ScheduleEndDate == null)
            {
                return PsSchedulingStage.Unavailable;
            }

            if (_dateTimeProvider.Now >= dates.SchedulePosted)
            {
                return PsSchedulingStage.SchedulePosted;
            }
            if (_dateTimeProvider.Now >= dates.SchedulingClosed)
            {
                return PsSchedulingStage.SchedulingClosed;
            }
            if (_dateTimeProvider.Now >= dates.SchedulingOpen)
            {
                return PsSchedulingStage.SchedulingOpen;
            }
            if (_dateTimeProvider.Now >= dates.SchedulingPreview)
            {
                return PsSchedulingStage.SchedulingPreview;
            }
            if (_dateTimeProvider.Now >= dates.RegistrationClosed)
            {
                return PsSchedulingStage.PerformerClosed;
            }
            if (_dateTimeProvider.Now >= dates.RegistrationOpen)
            {
                return PsSchedulingStage.PerformerOpen;
            }

            return PsSchedulingStage.BeforeRegistration;
        }

        public async Task<PsDates> GetDatesAsync()
        {
            var siteId = GetCurrentSiteId();
            return await _psDatesRepository.GetBySiteIdAsync(siteId);
        }

        public async Task UpdateDatesAsync(PsDates dates)
        {
            var authId = GetClaimId(ClaimType.UserId);
            var currentDates = await GetDatesAsync();
            if (currentDates == null)
            {
                dates.SiteId = GetCurrentSiteId();
                await _psDatesRepository.AddSaveAsync(authId, dates);
            }
            else
            {
                currentDates.RegistrationOpen = dates.RegistrationOpen;
                currentDates.RegistrationClosed = dates.RegistrationClosed;
                currentDates.SchedulingPreview = dates.SchedulingPreview;
                currentDates.SchedulingOpen = dates.SchedulingOpen;
                currentDates.SchedulingClosed = dates.SchedulingClosed;
                currentDates.SchedulePosted = dates.SchedulePosted;
                currentDates.ScheduleStartDate = dates.ScheduleStartDate;
                currentDates.ScheduleEndDate = dates.ScheduleEndDate;

                await _psDatesRepository.UpdateSaveAsync(authId, currentDates);
            }
        }

        public async Task<IEnumerable<PsBlackoutDate>> GetAllBlackoutDatesAsync()
        {
            return await _psBlackoutDateRepository.GetAllAsync();
        }

        public async Task<DataWithCount<ICollection<PsBlackoutDate>>> GetPaginatedBlackoutDatesAsync(
            BaseFilter filter)
        {
            return await _psBlackoutDateRepository.GetPaginatedListAsync(filter);
        }

        public async Task<PsBlackoutDate> AddBlackoutDateAsync(PsBlackoutDate blackoutDate)
        {
            blackoutDate.Reason = blackoutDate.Reason?.Trim();

            return await _psBlackoutDateRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                blackoutDate);
        }

        public async Task RemoveBlackoutDateAsync(int blackoutDateId)
        {
            await _psBlackoutDateRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId),
                blackoutDateId);
        }

        public async Task<PsPerformer> GetPerformerByUserIdAsync(int userId, 
            bool includeBranches = false)
        {
            var authId = GetClaimId(ClaimType.UserId);
            if (authId != userId)
            {
                _logger.LogError($"User {authId} doesn't have permission to view performer information for user {userId}.");
                throw new GraException("Permission denied.");
            }

            return await _psPerformerRepository.GetByUserIdAsync(userId);
        }

        public async Task<PsPerformer> AddPerformerAsync(PsPerformer performer,
            List<int> branchAvailability)
        {
            var authId = GetClaimId(ClaimType.UserId);

            performer.BillingAddress = performer.BillingAddress.Trim();
            performer.Name = performer.Name.Trim();
            performer.Phone = performer.Phone.Trim();
            performer.UserId = authId;
            performer.VendorId = performer.VendorId.Trim();
            performer.Website = performer.Website?.Trim();

            performer = await _psPerformerRepository.AddSaveAsync(authId, performer);

            if (performer.AllBranches == false)
            {
                await _psPerformerRepository.AddPerformerBranchesAsync(performer.Id,
                    branchAvailability);
            }

            return performer;
        }

        public async Task<PsPerformer> EditPerformerAsync(PsPerformer performer,
            List<int> branchAvailability)
        {
            var authId = GetClaimId(ClaimType.UserId);

            var currentPerformer = await _psPerformerRepository.GetByUserIdAsync(authId);
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

            if (currentPerformer.AllBranches == false && performer.AllBranches == true)
            {
                var currentBranches = currentPerformer.Branches.Select(_ => _.Id).ToList();
                await _psPerformerRepository.RemovePerformerBranchesAync(currentPerformer.Id,
                    currentBranches);
            }

            currentPerformer.AllBranches = performer.AllBranches;

            currentPerformer = await _psPerformerRepository.UpdateSaveAsync(authId,
                currentPerformer);
            return currentPerformer;
        }

        public async Task AddPerformerReferencesAsync(int performerId, byte[] referencesBytes,
            string fileExtension)
        {
            var performer = await _psPerformerRepository.GetByIdAsync(performerId);

            if (!string.IsNullOrWhiteSpace(performer.ReferencesFilename))
            {
                var filePath = _pathResolver.ResolveContentFilePath(performer.ReferencesFilename);
                System.IO.File.WriteAllBytes(filePath, referencesBytes);
            }
            else
            {
                var siteId = GetCurrentSiteId();

                var referencesDirectory = _pathResolver.ResolveContentFilePath(
                        Path.Combine($"site{siteId}", ReferencesPath));
                Directory.CreateDirectory(referencesDirectory);

                var performerFilename = AlphanumericRegex.Replace(performer.Name, "");
                var referencesFilename = $"{performerFilename}_references{fileExtension}";

                while (System.IO.File.Exists(Path.Combine(referencesDirectory, referencesFilename)))
                {
                    referencesFilename = $"{performerFilename}_references" +
                        $"_{Path.GetRandomFileName().Replace(".", "")}{fileExtension}";
                }

                performer.ReferencesFilename = Path.Combine($"site{siteId}", ReferencesPath,
                    referencesFilename);
                var filePath = _pathResolver.ResolveContentFilePath(performer.ReferencesFilename);
                System.IO.File.WriteAllBytes(filePath, referencesBytes);

                await _psPerformerRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), performer);
            }
        }

        public async Task AddPerformerImageAsync(int performerId, byte[] imageBytes,
            string fileExtension)
        {
            var siteId = GetCurrentSiteId();

            var performerImageDirectory = _pathResolver.ResolveContentFilePath(
                        Path.Combine($"site{siteId}", PerformerImagePath));
            Directory.CreateDirectory(performerImageDirectory);

            var performer = await _psPerformerRepository.GetByIdAsync(performerId);

            var performerFilename = AlphanumericRegex.Replace(performer.Name, "");
            var imageFilename = $"{performerFilename}{fileExtension}";

            while (System.IO.File.Exists(Path.Combine(performerImageDirectory, imageFilename)))
            {
                imageFilename = $"{performerFilename}" +
                    $"_{Path.GetRandomFileName().Replace(".", "")}{fileExtension}";
            }

            var filePath = _pathResolver.ResolveContentFilePath(
                Path.Combine($"site{siteId}", PerformerImagePath, imageFilename));
            System.IO.File.WriteAllBytes(filePath, imageBytes);

            var performerImage = new PsPerformerImage()
            {
                PerformerId = performer.Id,
                Filename = imageFilename
            };

            await _psPerformerImageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                performerImage);
        }
    }
}
