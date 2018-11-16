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

        private static readonly DateTime DefaultPerformerScheduleStartTime = DateTime.Parse("8:00 AM");
        private static readonly DateTime DefaultPerformerScheduleEndTime = DateTime.Parse("8:00 PM");

        public static readonly string KitImagePath = "kitimages";
        public static readonly string PerformerImagePath = "performerimages";
        public static readonly string ProgramImagePath = "programimages";
        public static readonly string ReferencesPath = "performerreferences";

        public IPathResolver _pathResolver;
        public IPsAgeGroupRepository _psAgeGroupRepository;
        public IPsBlackoutDateRepository _psBlackoutDateRepository;
        public IPsBranchSelectionRepository _psBranchSelectionRepository;
        public IPsKitRepository _psKitRepository;
        public IPsKitImageRepository _psKitImageRepository;
        public IPsPerformerImageRepository _psPerformerImageRepository;
        public IPsPerformerRepository _psPerformerRepository;
        public IPsPerformerScheduleRepository _psPerformerScheduleRepository;
        public IPsProgramRepository _psProgramRepository;
        public IPsProgramImageRepository _psProgramImageRepository;
        public IPsSettingsRepository _psSettingsRepository;
        public ITriggerRepository _triggerRepository;
        public PerformerSchedulingService(ILogger<PerformerSchedulingService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IPathResolver pathResolver,
            IPsAgeGroupRepository psAgeGroupRepository,
            IPsBlackoutDateRepository psBlackoutDateRepository,
            IPsBranchSelectionRepository psBranchSelectionRepository,
            IPsKitRepository psKitRepository,
            IPsKitImageRepository psKitImageRepository,
            IPsPerformerImageRepository psPerformerImageRepository,
            IPsPerformerRepository psPerformerRepository,
            IPsPerformerScheduleRepository psPerformerScheduleRepository,
            IPsProgramRepository psProgramRepository,
            IPsProgramImageRepository psProgramImageRepository,
            IPsSettingsRepository psSettingsRepository,
            ITriggerRepository triggerRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
            _psAgeGroupRepository = psAgeGroupRepository
                ?? throw new ArgumentNullException(nameof(psAgeGroupRepository));
            _psBlackoutDateRepository = psBlackoutDateRepository
                ?? throw new ArgumentNullException(nameof(psBlackoutDateRepository));
            _psBranchSelectionRepository = psBranchSelectionRepository
                ?? throw new ArgumentNullException(nameof(psBranchSelectionRepository));
            _psKitRepository = psKitRepository
                ?? throw new ArgumentNullException(nameof(psKitRepository));
            _psKitImageRepository = psKitImageRepository
                ?? throw new ArgumentNullException(nameof(psKitImageRepository));
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
            _psSettingsRepository = psSettingsRepository
                ?? throw new ArgumentNullException(nameof(psSettingsRepository));
            _triggerRepository = triggerRepository
                ?? throw new ArgumentNullException(nameof(triggerRepository));
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

        public async Task<DataWithCount<ICollection<PsPerformer>>> GetPaginatedPerformerList(
            BaseFilter filter)
        {
            return await _psPerformerRepository.PageAsync(filter);
        }

        public async Task<List<int>> GetPerformerIndexListAsync()
        {
            return await _psPerformerRepository.GetIndexListAsync();
        }

        public async Task<ICollection<PsBlackoutDate>> GetBlackoutDatesAsync()
        {
            return await _psBlackoutDateRepository.GetAllAsync();
        }

        public async Task<DataWithCount<ICollection<PsBlackoutDate>>> GetPaginatedBlackoutDatesAsync(
            BaseFilter filter)
        {
            return await _psBlackoutDateRepository.PageAsync(filter);
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
            return await _psAgeGroupRepository.PageAsync(filter);
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

        public async Task<PsPerformer> GetPerformerByIdAsync(int id,
            bool includeBranches = false,
            bool includePrograms = false,
            bool includeSchedule = false)
        {
            var performer = await _psPerformerRepository.GetByUserIdAsync(id);
            if (performer == null)
            {
                throw new GraException("The requested performer could not be accessed or does not exist.");
            }
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

            return performer;
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

        public async Task<int> GetPerformerProgramCountAsync(int performerId)
        {
            return await _psProgramRepository.GetCountByPerformerAsync(performerId);
        }

        public async Task<PsPerformerSchedule> GetPerformerDateScheduleAsync(int performerId,
            DateTime date)
        {
            return await _psPerformerScheduleRepository.GetPerformerDateScheduleAsync(performerId,
                date);
        }

        public async Task<int> GetPerformerSelectionCountAsync(int performerId)
        {
            return await _psBranchSelectionRepository.GetCountByPerformerIdAsync(performerId);
        }

        public async Task<ICollection<PsBranchSelection>> GetPerformerBranchSelectionsAsync(
            int performerId, DateTime? date = null)
        {
            return await _psBranchSelectionRepository.GetByPerformerIdAsync(performerId, date);
        }

        public async Task<PsBranchSelection> GetBranchSelectionAsync(int branchSelectionId)
        {
            return await _psBranchSelectionRepository.GetByIdAsync(branchSelectionId);
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
                await _psPerformerRepository.AddPerformerBranchListAsync(performer.Id,
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
                throw new GraException("The requested performer could not be accessed or does not exist.");
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
                    await _psPerformerRepository.AddPerformerBranchListAsync(currentPerformer.Id,
                        branchesToAdd);
                }
                if (branchesToRemove.Count > 0)
                {
                    await _psPerformerRepository.RemovePerformerBranchListAsync(currentPerformer.Id,
                        branchesToRemove);
                }
            }

            currentPerformer.AllBranches = performer.AllBranches;

            currentPerformer = await _psPerformerRepository.UpdateSaveAsync(authId,
                currentPerformer);
            return currentPerformer;
        }

        public async Task RemovePerformerAsync(int performerId)
        {
            var performer = await _psPerformerRepository.GetByIdAsync(performerId);

            if (performer == null)
            {
                throw new GraException("The requested performer could not be accessed or does not exist.");
            }

            var performerImages = await _psPerformerImageRepository.GetByPerformerIdAsync(
                performer.Id);
            foreach (var image in performerImages)
            {
                await RemovePerformerImageAsync(image);
            }

            var performerProgram = await _psProgramRepository.GetByPerformerIdAsync(performer.Id);
            foreach (var program in performerProgram)
            {
                await RemoveProgramAsync(program.Id);
            }

            await _psPerformerScheduleRepository.RemovePerformerScheduleAsync(performer.Id);
            await _psPerformerRepository.RemovePerformerBranchesAsync(performer.Id);

            var referencesFile = _pathResolver.ResolveContentFilePath(performer.ReferencesFilename);
            if (System.IO.File.Exists(referencesFile))
            {
                System.IO.File.Delete(referencesFile);
            }

            await _psPerformerRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), performer.Id);
        }

        public async Task SetPerformerRegistrationCompeltedAsync(int performerId)
        {
            var authId = GetClaimId(ClaimType.UserId);

            var performer = await _psPerformerRepository.GetByIdAsync(performerId);
            performer.RegistrationCompleted = true;
            await _psPerformerRepository.UpdateSaveAsync(authId, performer);
        }

        public async Task SetPerformerApprovedAsync(int performerId, bool isApproved)
        {
            var performer = await _psPerformerRepository.GetByIdAsync(performerId);
            if (performer == null)
            {
                throw new GraException("The requested performer could not be accessed or does not exist.");
            }
            performer.IsApproved = isApproved;

            await _psPerformerRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), performer);
        }

        public async Task SetPerformerReferencesAsync(int performerId, byte[] referencesBytes,
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

        public async Task RemovePerformerImageByIdAsync(int imageId)
        {
            var image = await _psPerformerImageRepository.GetByIdAsync(imageId);
            await RemovePerformerImageAsync(image);
        }

        private async Task RemovePerformerImageAsync(PsPerformerImage image)
        {
            var authId = GetClaimId(ClaimType.UserId);

            await _psPerformerImageRepository.RemoveSaveAsync(authId, image.Id);
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
            var program = await _psProgramRepository.GetByIdAsync(id);
            if (program == null)
            {
                throw new GraException("The requested program could not be accessed or does not exist.");
            }

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

            if (currentProgram == null)
            {
                throw new GraException("The requested program could not be accessed or does not exist.");
            }

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

            var settings = await GetSettingsAsync();
            var schedulingStage = GetSchedulingStage(settings);
            if (schedulingStage > PsSchedulingStage.SchedulingOpen)
            {
                throw new GraException("Cannot remove programs after scheduling has opened.");
            }

            var program = await _psProgramRepository.GetByIdAsync(programId);
            if (program == null)
            {
                throw new GraException("The requested program could not be accessed or does not exist.");
            }

            var ageGroupIds = program.AgeGroups.Select(_ => _.Id).ToList();
            await _psProgramRepository.RemoveProgramAgeGroupsAsync(programId, ageGroupIds);

            var images = program.ProgramImages;
            foreach (var image in images)
            {
                await RemoveProgramImageAsync(image);
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

        public async Task RemoveProgramImageByIdAsync(int imageId)
        {
            var image = await _psProgramImageRepository.GetByIdAsync(imageId);
            await RemoveProgramImageAsync(image);
        }

        private async Task RemoveProgramImageAsync(PsProgramImage image)
        {
            var authId = GetClaimId(ClaimType.UserId);

            await _psPerformerImageRepository.RemoveSaveAsync(authId, image.Id);
            var file = _pathResolver.ResolveContentFilePath(image.Filename);
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
        }

        public async Task<ICollection<PsKit>> GetAllKitsAsync()
        {
            return await _psKitRepository.GetAllAsync();
        }

        public async Task<DataWithCount<ICollection<PsKit>>> GetPaginatedKitListAsync(
            BaseFilter filter)
        {
            return await _psKitRepository.PageAsync(filter);
        }

        public async Task<List<int>> GetKitIndexListAsync()
        {
            return await _psKitRepository.GetIndexListAsync();
        }

        public async Task<int> GetKitSelectionCountAsync(int kitId)
        {
            return await _psBranchSelectionRepository.GetCountByKitIdAsync(kitId);
        }

        public async Task<ICollection<PsBranchSelection>> GetKitBranchSelectionsAsync(int kitId)
        {
            return await _psBranchSelectionRepository.GetByKitIdAsync(kitId);
        }

        public async Task<PsKit> GetKitByIdAsync(int kitId)
        {
            var kit = await _psKitRepository.GetByIdAsync(kitId);
            if (kit == null)
            {
                throw new GraException("The requested kit could not be accessed or does not exist.");
            }
            return kit;
        }

        public async Task<PsKit> AddKitAsync(PsKit kit, List<int> ageSelection)
        {
            var authId = GetClaimId(ClaimType.UserId);

            kit.Description = kit.Description?.Trim();
            kit.Name = kit.Name?.Trim();
            kit.Website = kit.Website?.Trim();

            kit = await _psKitRepository.AddSaveAsync(authId, kit);

            await _psProgramRepository.AddProgramAgeGroupsAsync(kit.Id, ageSelection);

            return kit;
        }

        public async Task<PsKit> UpdateKitAsync(PsKit kit, List<int> ageSelection)
        {
            var authId = GetClaimId(ClaimType.UserId);

            var currentKit = await _psKitRepository.GetByIdAsync(kit.Id);
            if (currentKit == null)
            {
                throw new GraException("The requested kit could not be accessed or does not exist.");
            }

            currentKit.Description = kit.Description?.Trim();
            currentKit.Name = kit.Name?.Trim();
            currentKit.Website = kit.Website?.Trim();

            currentKit = await _psKitRepository.UpdateSaveAsync(authId, currentKit);

            var agesToAdd = ageSelection.Except(currentKit.AgeGroups.Select(_ => _.Id)).ToList();
            var agesToRemove = currentKit.AgeGroups.Select(_ => _.Id).Except(ageSelection).ToList();

            await _psKitRepository.AddKitAgeGroupsAsync(currentKit.Id, agesToAdd);
            await _psKitRepository.RemoveKitAgeGroupsAsync(currentKit.Id, agesToRemove);

            return currentKit;
        }

        public async Task RemoveKitAsync(int kitId)
        {
            var kit = await _psKitRepository.GetByIdAsync(kitId);

            if (kit == null)
            {
                throw new GraException("The requested kit could not be accessed or does not exist.");
            }

            var ageGroupIds = kit.AgeGroups.Select(_ => _.Id).ToList();
            await _psKitRepository.RemoveKitAgeGroupsAsync(kit.Id, ageGroupIds);

            var kitImages = await _psKitImageRepository.GetByKitIdAsync(kit.Id);
            foreach (var image in kitImages)
            {
                await RemoveKitImageAsync(image);
            }

            await _psKitRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), kit.Id);
        }

        public async Task AddKitImageAsync(int kitId, byte[] imageBytes,
            string fileExtension)
        {
            var siteId = GetCurrentSiteId();

            var kitImageDirectory = _pathResolver.ResolveContentFilePath(
                        Path.Combine($"site{siteId}", KitImagePath));
            Directory.CreateDirectory(kitImageDirectory);

            var kit = await _psKitRepository.GetByIdAsync(kitId);

            var kitFilename = AlphanumericRegex.Replace(kit.Name, "");
            var imageFilename = $"{kitFilename}{fileExtension}";

            while (System.IO.File.Exists(Path.Combine(kitImageDirectory, imageFilename)))
            {
                imageFilename = $"{kitFilename}" +
                    $"_{Path.GetRandomFileName().Replace(".", "")}{fileExtension}";
            }

            var imagePath = Path.Combine($"site{siteId}", KitImagePath, imageFilename);
            var filePath = _pathResolver.ResolveContentFilePath(imagePath);
            System.IO.File.WriteAllBytes(filePath, imageBytes);

            var kitImage = new PsKitImage()
            {
                KitId = kit.Id,
                Filename = imagePath
            };

            await _psKitImageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                kitImage);
        }

        public async Task RemoveKitImageByIdAsync(int imageId)
        {
            var image = await _psKitImageRepository.GetByIdAsync(imageId);

            await RemoveKitImageAsync(image);
        }

        private async Task RemoveKitImageAsync(PsKitImage image)
        {
            var authId = GetClaimId(ClaimType.UserId);

            await _psPerformerImageRepository.RemoveSaveAsync(authId, image.Id);
            var file = _pathResolver.ResolveContentFilePath(image.Filename);
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
        }

        public async Task UpdateBranchProgramSelectionAsync(PsBranchSelection branchSelection)
        {
            var currentBranchSelection = await _psBranchSelectionRepository.GetByIdAsync(
                branchSelection.Id);

            if (currentBranchSelection == null)
            {
                throw new GraException("Selection does not exist.");
            }
            else if (currentBranchSelection.ProgramId.HasValue == false)
            {
                throw new GraException("Selection is not a program selection.");
            }

            await ValidateScheduleTimeAsync(currentBranchSelection.ProgramId.Value,
                branchSelection.RequestedStartTime,
                currentBranchSelection.BackToBackProgram);

            var program = await _psProgramRepository.GetByIdAsync(
                currentBranchSelection.ProgramId.Value);

            currentBranchSelection.RequestedStartTime = branchSelection.RequestedStartTime;
            currentBranchSelection.ScheduleStartTime = branchSelection.RequestedStartTime
                .AddMinutes(-program.SetupTimeMinutes);

            await _psBranchSelectionRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentBranchSelection);
        }

        public async Task<string> ValidateScheduleTimeAsync(int programId, DateTime programStart,
            bool backToBack)
        {
            var blackoutDate = await _psBlackoutDateRepository.GetByDateAsync(programStart);
            if (blackoutDate != null)
            {
                throw new GraException($"Programs cannot be booked on {blackoutDate.Reason}.");
            }

            var program = await _psProgramRepository.GetByIdAsync(programId);

            var scheduleDate = await _psPerformerScheduleRepository.GetPerformerDateScheduleAsync(
                program.PerformerId, programStart);

            if (scheduleDate != null && scheduleDate.StartTime.HasValue == false)
            {
                throw new GraException("The peformer is not available on that day.");
            }

            var scheduleStartTime = scheduleDate?.StartTime ?? DateTime.Parse("8:00 AM");
            var scheduleEndTime = scheduleDate?.EndTime ?? DateTime.Parse("8:00 PM");

            var programLength = program.ProgramLengthMinutes;
            if (backToBack)
            {
                programLength += program.ProgramLengthMinutes + program.BackToBackMinutes;
            }

            if (programStart.TimeOfDay < scheduleStartTime.TimeOfDay
                || programStart.TimeOfDay >= scheduleEndTime.TimeOfDay
                || programStart.AddMinutes(programLength).TimeOfDay > scheduleEndTime.TimeOfDay
                || programStart.AddMinutes(programLength).Date != programStart.Date)
            {
                throw new GraException("The peformer is not available at that time.");
            }

            var setupStartTime = programStart.AddMinutes(-program.SetupTimeMinutes).TimeOfDay;
            var breakdownEndTime = programStart.AddMinutes(programLength + program.BreakdownTimeMinutes)
                .TimeOfDay;

            var bookedTimes = await _psBranchSelectionRepository.GetByPerformerIdAsync(
                program.PerformerId, programStart);

            if (bookedTimes.Any(_ => (_.ScheduleStartTime.TimeOfDay <= setupStartTime
                    && _.ScheduleStartTime.AddMinutes(_.ScheduleDuration).TimeOfDay >= setupStartTime)
                || (_.ScheduleStartTime.TimeOfDay <= breakdownEndTime
                        && _.ScheduleStartTime.AddMinutes(_.ScheduleDuration).TimeOfDay >= breakdownEndTime)
                || (_.ScheduleStartTime.TimeOfDay >= setupStartTime
                    && _.ScheduleStartTime.AddMinutes(_.ScheduleDuration).TimeOfDay <= breakdownEndTime)))
            {
                throw new GraException("The peformer is already booked during that time.");
            }

            if (bookedTimes.Any(_ => (_.ScheduleStartTime.AddMinutes(_.ScheduleDuration + 60).TimeOfDay >= setupStartTime
                    && _.ScheduleStartTime.AddMinutes(_.ScheduleDuration).TimeOfDay <= setupStartTime)
                || (_.ScheduleStartTime.AddMinutes(-60).TimeOfDay <= breakdownEndTime
                    && _.ScheduleStartTime.TimeOfDay >= breakdownEndTime)))
            {
                return "The performer has programs within an hour of the selected time slot, please ensure they will have enough time to drive to/from those branches.";
            }

            return null;
        }

        public async Task SetSelectionSecretCodeAsync(int selectionId, string secretCode)
        {
            secretCode = secretCode?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(secretCode))
            {
                throw new ArgumentNullException("Please enter a Secret Code.");
            }
            else if (secretCode.All(char.IsLetterOrDigit) == false)
            {
                throw new ArgumentNullException("Only letters and numbers are allowed.");
            }
            else if (secretCode.Length > 50)
            {
                throw new ArgumentNullException("Please enter less than 50 characters.");
            }

            var existingTrigger = await _triggerRepository.GetByCodeAsync(GetCurrentSiteId(), 
                secretCode, false);
            if (existingTrigger != null)
            {
                throw new ArgumentNullException("Code is already in use by a trigger.");
            }

            var currentSelection = await _psBranchSelectionRepository.GetByIdAsync(selectionId);
            if (currentSelection.SecretCode != secretCode)
            {
                var existingSelection = await _psBranchSelectionRepository
                    .GetByCodeAsync(secretCode);
                if (existingSelection != null)
                {
                    throw new ArgumentNullException("Code is already in use by a performer selection.");
                }

                currentSelection.SecretCode = secretCode;
                await _psBranchSelectionRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), 
                    currentSelection);
            }
        }

        public async Task<bool> BranchAgeGroupAlreadySelectedAsync(int ageGroupId, 
            int branchSelectionId)
        {
            return await _psBranchSelectionRepository.BranchAgeGroupAlreadySelectedAsync(ageGroupId, 
                branchSelectionId);
        }

        public async Task UpdateBranchKitSelectionAsync(PsBranchSelection branchSelection)
        {
            var currentBranchSelection = await _psBranchSelectionRepository.GetByIdAsync(
                branchSelection.Id);

            if (currentBranchSelection == null)
            {
                throw new GraException("Selection does not exist.");
            }
            else if (currentBranchSelection.KitId.HasValue == false)
            {
                throw new GraException("Selection is not a kit selection.");
            }

            var ageGroupExists = await _psBranchSelectionRepository
                .BranchAgeGroupAlreadySelectedAsync(branchSelection.AgeGroupId, branchSelection.Id);
            if (ageGroupExists)
            {
                throw new GraException("Branch already has a selection for that age group.");
            }

            var validAgeGroup = await _psKitRepository.IsValidAgeGroupAsync(
                branchSelection.KitId.Value,
                branchSelection.AgeGroupId);
            if (validAgeGroup == false)
            {
                throw new GraException("Invalid age group for that kit.");
            }

            currentBranchSelection.AgeGroupId = branchSelection.AgeGroupId;
            currentBranchSelection.KitId = branchSelection.KitId;

            await _psBranchSelectionRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentBranchSelection);
        }
    }
}
