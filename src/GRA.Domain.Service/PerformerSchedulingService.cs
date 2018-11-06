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
        private static readonly Regex AlphanumericRegex = new Regex("[^a-zA-Z0-9 -]");

        public static readonly string KitImagePath = "kitimages";
        public static readonly string PerformerImagePath = "performerimages";
        public static readonly string ProgramImagePath = "programimages";
        public static readonly string ReferencesPath = "performerreferences";

        public IPathResolver _pathResolver;
        public IPsAgeGroupRepository _psAgeGroupRepository;
        public IPsBlackoutDateRepository _psBlackoutDateRepository;
        public IPsSettingsRepository _psSettingsRepository;
        public IPsPerformerImageRepository _psPerformerImageRepository;
        public IPsPerformerRepository _psPerformerRepository;
        public IPsPerformerScheduleRepository _psPerformerScheduleRepository;
        public IPsProgramRepository _psProgramRepository;
        public IPsProgramImageRepository _psProgramImageRepository;
        public PerformerSchedulingService(ILogger<PerformerSchedulingService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IPathResolver pathResolver,
            IPsAgeGroupRepository psAgeGroupRepository,
            IPsBlackoutDateRepository psBlackoutDateRepository,
            IPsSettingsRepository psSettingsRepository,
            IPsPerformerImageRepository psPerformerImageRepository,
            IPsPerformerRepository psPerformerRepository,
            IPsPerformerScheduleRepository psPerformerScheduleRepository,
            IPsProgramRepository psProgramRepository,
            IPsProgramImageRepository psProgramImageRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
            _psAgeGroupRepository = psAgeGroupRepository
                ?? throw new ArgumentNullException(nameof(psAgeGroupRepository));
            _psBlackoutDateRepository = psBlackoutDateRepository
                ?? throw new ArgumentNullException(nameof(psBlackoutDateRepository));
            _psSettingsRepository = psSettingsRepository
                ?? throw new ArgumentNullException(nameof(psSettingsRepository));
            _psPerformerImageRepository = psPerformerImageRepository
                ?? throw new ArgumentNullException(nameof(psPerformerImageRepository));
            _psPerformerRepository = psPerformerRepository
                ?? throw new ArgumentNullException(nameof(psPerformerRepository));
            _psPerformerScheduleRepository = psPerformerScheduleRepository
                ?? throw new ArgumentNullException(nameof(psPerformerScheduleRepository));
            _psProgramRepository = psProgramRepository
                ?? throw new ArgumentNullException(nameof(psProgramRepository));
            _psProgramImageRepository = psProgramImageRepository
                ?? throw new ArgumentNullException(nameof(psProgramImageRepository));
        }

        public PsSchedulingStage GetSchedulingStage(PsSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings?.ContactEmail)
                || settings?.RegistrationClosed == null
                || settings?.RegistrationOpen == null
                || settings?.SchedulePosted == null
                || settings?.SchedulingClosed == null
                || settings?.SchedulingOpen == null
                || settings?.SchedulingPreview == null
                || settings?.ScheduleStartDate == null
                || settings?.ScheduleEndDate == null)
            {
                return PsSchedulingStage.Unavailable;
            }

            if (_dateTimeProvider.Now >= settings.SchedulePosted)
            {
                return PsSchedulingStage.SchedulePosted;
            }
            if (_dateTimeProvider.Now >= settings.SchedulingClosed)
            {
                return PsSchedulingStage.SchedulingClosed;
            }
            if (_dateTimeProvider.Now >= settings.SchedulingOpen)
            {
                return PsSchedulingStage.SchedulingOpen;
            }
            if (_dateTimeProvider.Now >= settings.SchedulingPreview)
            {
                return PsSchedulingStage.SchedulingPreview;
            }
            if (_dateTimeProvider.Now >= settings.RegistrationClosed)
            {
                return PsSchedulingStage.RegistrationClosed;
            }
            if (_dateTimeProvider.Now >= settings.RegistrationOpen)
            {
                return PsSchedulingStage.RegistrationOpen;
            }

            return PsSchedulingStage.BeforeRegistration;
        }

        public async Task<PsSettings> GetSettingsAsync()
        {
            var siteId = GetCurrentSiteId();
            return await _psSettingsRepository.GetBySiteIdAsync(siteId);
        }

        public async Task UpdateSettingsAsync(PsSettings dates)
        {
            var authId = GetClaimId(ClaimType.UserId);
            var currentSettings = await GetSettingsAsync();
            if (currentSettings == null)
            {
                dates.SiteId = GetCurrentSiteId();
                await _psSettingsRepository.AddSaveAsync(authId, dates);
            }
            else
            {
                currentSettings.ContactEmail = dates.ContactEmail?.Trim();
                currentSettings.RegistrationOpen = dates.RegistrationOpen;
                currentSettings.RegistrationClosed = dates.RegistrationClosed;
                currentSettings.SchedulingPreview = dates.SchedulingPreview;
                currentSettings.SchedulingOpen = dates.SchedulingOpen;
                currentSettings.SchedulingClosed = dates.SchedulingClosed;
                currentSettings.SchedulePosted = dates.SchedulePosted;
                currentSettings.ScheduleStartDate = dates.ScheduleStartDate;
                currentSettings.ScheduleEndDate = dates.ScheduleEndDate;

                await _psSettingsRepository.UpdateSaveAsync(authId, currentSettings);
            }
        }

        public async Task<IEnumerable<PsBlackoutDate>> GetBlackoutDatesAsync()
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

        public async Task<IEnumerable<PsAgeGroup>> GetAgeGroupsAsync()
        {
            return await _psAgeGroupRepository.GetAllAsync();
        }

        public async Task<DataWithCount<ICollection<PsAgeGroup>>> GetPaginatedAgeGroupsAsync(
            BaseFilter filter)
        {
            return await _psAgeGroupRepository.GetPaginatedListAsync(filter);
        }

        public async Task<PsAgeGroup> AddAgeGroupAsync(PsAgeGroup ageGroup)
        {
            ageGroup.Name = ageGroup.Name?.Trim();

            return await _psAgeGroupRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                ageGroup);
        }

        public async Task RemoveAgeGroupAsync(int ageGroupId)
        {
            await _psAgeGroupRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId),
                ageGroupId);
        }

        public async Task<PsPerformer> GetPerformerByUserIdAsync(int userId,
            bool includeBranches = false,
            bool includePrograms = false,
            bool includeSchedule = false)
        {
            var authId = GetClaimId(ClaimType.UserId);
            if (authId != userId)
            {
                _logger.LogError($"User {authId} doesn't have permission to view performer information for user {userId}.");
                throw new GraException("Permission denied.");
            }

            var performer = await _psPerformerRepository.GetByUserIdAsync(userId);
            if (performer != null)
            {
                if (includeBranches)
                {
                    performer.Branches = await _psPerformerRepository.GetPerformerBranchesAsync(
                        performer.Id);
                }
                if (includePrograms)
                {
                    performer.Programs = await _psProgramRepository.GetByPerformerIdAsync(
                        performer.Id);
                }
                if (includeSchedule)
                {
                    performer.Schedule = await _psPerformerScheduleRepository
                        .GetByPerformerIdAsync(performer.Id);
                }
            }

            return performer;
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

            if (currentPerformer.AllBranches == false || performer.AllBranches == false)
            {
                var branchesToAdd = new List<int>();
                var branchesToRemove = new List<int>();

                if (currentPerformer.AllBranches == true)
                {
                    branchesToAdd = branchAvailability;
                }
                else
                {
                    var currentBranches = (await _psPerformerRepository
                    .GetPerformerBranchesAsync(currentPerformer.Id))
                    .Select(_ => _.Id)
                    .ToList();

                    if (performer.AllBranches == true)
                    {
                        branchesToRemove = currentBranches;
                    }
                    else
                    {
                        branchesToAdd = branchAvailability.Except(currentBranches).ToList();
                        branchesToRemove = currentBranches.Except(branchAvailability).ToList();
                    }
                }

                if (branchesToAdd.Count > 0)
                {
                    await _psPerformerRepository.AddPerformerBranchesAsync(currentPerformer.Id,
                        branchesToAdd);
                }
                if (branchesToRemove.Count > 0)
                {
                    await _psPerformerRepository.RemovePerformerBranchesAsync(currentPerformer.Id,
                        branchesToRemove);
                }
            }

            currentPerformer.AllBranches = performer.AllBranches;

            currentPerformer = await _psPerformerRepository.UpdateSaveAsync(authId,
                currentPerformer);
            return currentPerformer;
        }

        public async Task SetPerformerRegistrationCompelted(int performerId)
        {
            var authId = GetClaimId(ClaimType.UserId);

            var performer = await _psPerformerRepository.GetByIdAsync(performerId);
            performer.RegistrationCompleted = true;
            await _psPerformerRepository.UpdateSaveAsync(authId, performer);
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

            var imagePath = Path.Combine($"site{siteId}", PerformerImagePath, imageFilename);
            var filePath = _pathResolver.ResolveContentFilePath(imagePath);
            System.IO.File.WriteAllBytes(filePath, imageBytes);

            var performerImage = new PsPerformerImage()
            {
                PerformerId = performer.Id,
                Filename = imagePath
            };

            await _psPerformerImageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                performerImage);
        }

        public async Task RemovePerformerImageAsync(int imageId)
        {
            var authId = GetClaimId(ClaimType.UserId);
            var image = await _psPerformerImageRepository.GetByIdAsync(imageId);

            await _psPerformerImageRepository.RemoveSaveAsync(authId, imageId);
            var file = _pathResolver.ResolveContentFilePath(image.Filename);
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
        }

        public async Task EditPerformerScheduleAsync(int performerId, List<PsScheduleDate> schedule)
        {
            var authId = GetClaimId(ClaimType.UserId);
            var settings = await GetSettingsAsync();
            var blackoutDates = await GetBlackoutDatesAsync();

            var blackoutCount = blackoutDates
                    .Select(_ => _.Date.Date)
                    .Where(_ => _.Date >= settings.ScheduleStartDate.Value
                        && _.Date <= settings.ScheduleEndDate.Value)
                    .Distinct()
                    .Count();

            var totalDates = (settings.ScheduleEndDate.Value - settings.ScheduleStartDate.Value).Days
                + 1 - blackoutCount;

            if (schedule.Count != totalDates)
            {
                _logger.LogError($"Invalid schedule date count for user {authId}:" +
                    $"{schedule.Count} instead of {totalDates}");
                throw new GraException("Invalid date count.");
            }
            var invalidDates = schedule
                .Where(_ => _.ParsedDate < settings.ScheduleStartDate.Value.Date
                    || _.ParsedDate > settings.ScheduleEndDate.Value.Date
                    || blackoutDates.Any(b => b.Date.Date == _.ParsedDate))
                .ToList();

            if (invalidDates.Any())
            {
                _logger.LogError($"Invalid schedule dates for user {authId}: "
                    + string.Join(", ", invalidDates.Select(_ => _.ParsedDate)));
                throw new GraException("Invalid dates selected.");
            }

            var unavailableDates = schedule
                         .Where(_ => _.Availability == PsScheduleDateStatus.Unavailable.ToString())
                         .Select(_ => new PsPerformerSchedule()
                         {
                             PerformerId = performerId,
                             Date = _.ParsedDate
                         });
            var restrictedDates = schedule
                        .Where(_ => _.Availability == PsScheduleDateStatus.Time.ToString())
                        .Select(_ => new PsPerformerSchedule()
                        {
                            PerformerId = performerId,
                            Date = _.ParsedDate,
                            StartTime = DateTime.Parse(_.Time[0]),
                            EndTime = DateTime.Parse(_.Time[1])
                        });

            var performerSchedule = unavailableDates.Concat(restrictedDates).ToList();

            await _psPerformerScheduleRepository.SetPerformerScheduleAsync(performerId,
                performerSchedule);

            var performer = await _psPerformerRepository.GetByIdAsync(performerId);

            if (performer.SetSchedule == false)
            {
                performer.SetSchedule = true;
                await _psPerformerRepository.UpdateSaveAsync(authId, performer);
            }
        }

        public async Task<PsProgram> GetProgramByIdAsync(int id)
        {
            var authId = GetClaimId(ClaimType.UserId);
            var program = await _psProgramRepository.GetByIdAsync(id); ;
            if (!HasPermission(Permission.ManagePerformers)
                || !HasPermission(Permission.SchedulePerformers)
                || !HasPermission(Permission.ViewPerformerDetails))
            {
                var performer = await _psPerformerRepository.GetByIdAsync(program.PerformerId);
                if (performer.UserId != authId)
                {
                    _logger.LogError($"User id {authId} does not have persmission to view program id {id}.");
                    throw new GraException("Permission denied.");
                }
            }

            return program;
        }

        public async Task<PsProgram> AddProgramAsync(PsProgram program, List<int> ageSelection)
        {
            var authId = GetClaimId(ClaimType.UserId);

            program.Description = program.Description?.Trim();
            program.Setup = program.Setup?.Trim();
            program.Title = program.Title?.Trim();

            program = await _psProgramRepository.AddSaveAsync(authId, program);

            await _psProgramRepository.AddProgramAgeGroupsAsync(program.Id, ageSelection);

            return program;
        }

        public async Task<PsProgram> UpdateProgramAsync(PsProgram program, List<int> ageSelection)
        {
            var authId = GetClaimId(ClaimType.UserId);

            var currentProgram = await _psProgramRepository.GetByIdAsync(program.Id);
            currentProgram.AllowArchiving = program.AllowArchiving;
            currentProgram.AllowStreaming = program.AllowStreaming;
            currentProgram.BackToBackMinutes = program.BackToBackMinutes;
            currentProgram.BreakdownTimeMinutes = program.BreakdownTimeMinutes;
            currentProgram.Cost = program.Cost;
            currentProgram.Description = program.Description?.Trim();
            currentProgram.MaximumCapacity = program.MaximumCapacity;
            currentProgram.MinimumCapacity = program.MinimumCapacity;
            currentProgram.ProgramLengthMinutes = program.ProgramLengthMinutes;
            currentProgram.Setup = program.Setup?.Trim();
            currentProgram.SetupTimeMinutes = program.SetupTimeMinutes;
            currentProgram.Title = program.Title?.Trim();

            currentProgram = await _psProgramRepository.UpdateSaveAsync(authId, currentProgram);

            var agesToAdd = ageSelection.Except(currentProgram.AgeGroups.Select(_ => _.Id)).ToList();
            var agesToRemove = currentProgram.AgeGroups.Select(_ => _.Id).Except(ageSelection).ToList();

            await _psProgramRepository.AddProgramAgeGroupsAsync(currentProgram.Id, agesToAdd);
            await _psProgramRepository.RemoveProgramAgeGroupsAsync(currentProgram.Id, agesToRemove);

            return currentProgram;
        }

        public async Task RemoveProgramAsync(int programId)
        {
            var authId = GetClaimId(ClaimType.UserId);

            var program = await _psProgramRepository.GetByIdAsync(programId);

            var ageGroupIds = program.AgeGroups.Select(_ => _.Id).ToList();
            await _psProgramRepository.RemoveProgramAgeGroupsAsync(programId, ageGroupIds);

            var imageIds = program.ProgramImages.Select(_ => _.Id);
            foreach (var imageId in imageIds)
            {
                await RemoveProgramImageAsync(imageId);
            }

            await _psProgramRepository.RemoveSaveAsync(authId, programId);
        }

        public async Task AddProgramImageAsync(int programId, byte[] imageBytes,
            string fileExtension)
        {
            var siteId = GetCurrentSiteId();

            var programImageDirectory = _pathResolver.ResolveContentFilePath(
                        Path.Combine($"site{siteId}", ProgramImagePath));
            Directory.CreateDirectory(programImageDirectory);

            var program = await _psProgramRepository.GetByIdAsync(programId);

            var programFilename = AlphanumericRegex.Replace(program.Title, "");
            var imageFilename = $"{programFilename}{fileExtension}";

            while (System.IO.File.Exists(Path.Combine(programImageDirectory, imageFilename)))
            {
                imageFilename = $"{programFilename}" +
                    $"_{Path.GetRandomFileName().Replace(".", "")}{fileExtension}";
            }

            var imagePath = Path.Combine($"site{siteId}", ProgramImagePath, imageFilename);
            var filePath = _pathResolver.ResolveContentFilePath(imagePath);
            System.IO.File.WriteAllBytes(filePath, imageBytes);

            var programImage = new PsProgramImage()
            {
                ProgramId = program.Id,
                Filename = imagePath
            };

            await _psProgramImageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                programImage);
        }

        public async Task RemoveProgramImageAsync(int imageId)
        {
            var authId = GetClaimId(ClaimType.UserId);
            var image = await _psProgramImageRepository.GetByIdAsync(imageId);

            await _psPerformerImageRepository.RemoveSaveAsync(authId, imageId);
            var file = _pathResolver.ResolveContentFilePath(image.Filename);
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
        }
    }
}
