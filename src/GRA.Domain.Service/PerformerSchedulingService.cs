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
        public static readonly string KitImagePath = "kitimages";
        public static readonly string PerformerImagePath = "performerimages";
        public static readonly string ProgramImagePath = "programimages";
        private static readonly Regex AlphanumericRegex = new("[^a-zA-Z0-9 -]");
        private readonly IPathResolver _pathResolver;
        private readonly IPsAgeGroupRepository _psAgeGroupRepository;
        private readonly IPsBlackoutDateRepository _psBlackoutDateRepository;
        private readonly IPsBranchSelectionRepository _psBranchSelectionRepository;
        private readonly IPsKitImageRepository _psKitImageRepository;
        private readonly IPsKitRepository _psKitRepository;
        private readonly IPsPerformerImageRepository _psPerformerImageRepository;
        private readonly IPsPerformerRepository _psPerformerRepository;
        private readonly IPsPerformerScheduleRepository _psPerformerScheduleRepository;
        private readonly IPsProgramImageRepository _psProgramImageRepository;
        private readonly IPsProgramRepository _psProgramRepository;
        private readonly IPsSettingsRepository _psSettingsRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly ITriggerRepository _triggerRepository;

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
            ISystemRepository systemRepository,
            ITriggerRepository triggerRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManagePerformers);
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
            _systemRepository = systemRepository
                ?? throw new ArgumentNullException(nameof(systemRepository));
            _triggerRepository = triggerRepository
                ?? throw new ArgumentNullException(nameof(triggerRepository));
        }

        public async Task<PsAgeGroup> AddAgeGroupAsync(PsAgeGroup ageGroup)
        {
            ArgumentNullException.ThrowIfNull(ageGroup);
            VerifyManagementPermission();
            ageGroup.Name = ageGroup.Name?.Trim();

            return await _psAgeGroupRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                ageGroup);
        }

        public async Task<PsBlackoutDate> AddBlackoutDateAsync(PsBlackoutDate blackoutDate)
        {
            VerifyManagementPermission();
            ArgumentNullException.ThrowIfNull(blackoutDate);
            blackoutDate.Reason = blackoutDate.Reason?.Trim();

            return await _psBlackoutDateRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                blackoutDate);
        }

        public async Task AddBranchExclusionAsync(int branchId)
        {
            VerifyManagementPermission();
            await _psSettingsRepository.AddBranchExclusionAsync(branchId);
        }

        public async Task<PsBranchSelection> AddBranchKitSelectionAsync(
            PsBranchSelection branchSelection)
        {
            var authId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.SchedulePerformers))
            {
                _logger.LogError("User {AuthId} doesn't have permission to select kit.", authId);
                throw new GraException("Permission denied.");
            }
            ArgumentNullException.ThrowIfNull(branchSelection);
            if (!branchSelection.KitId.HasValue)
            {
                throw new GraException("No kit selected.");
            }

            var kit = await GetKitByIdAsync(branchSelection.KitId.Value);

            var validAgeGroup = await _psKitRepository.IsValidAgeGroupAsync(kit.Id,
                branchSelection.AgeGroupId);
            if (!validAgeGroup)
            {
                throw new GraException("Invalid age group for kit.");
            }

            var settings = await _psSettingsRepository.GetBySiteIdAsync(GetCurrentSiteId());
            var branchSelections = await _psBranchSelectionRepository
                .GetByBranchIdAsync(branchSelection.BranchId);

            if (branchSelections.Count >= settings.SelectionsPerBranch)
            {
                throw new GraException($"The branch has already made its {settings.SelectionsPerBranch} selections.");
            }

            var selectedAgeGroupIds = branchSelections.Select(_ => _.AgeGroupId);
            if (selectedAgeGroupIds.Any(_ => _ == branchSelection.AgeGroupId))
            {
                throw new GraException("The branch already has a selection for that age group.");
            }

            branchSelection.BackToBackProgram = false;
            branchSelection.ProgramId = null;
            branchSelection.RequestedStartTime = default;
            branchSelection.SelectedAt = _dateTimeProvider.Now;
            branchSelection.ScheduleStartTime = default;
            branchSelection.ScheduleDuration = 0;
            branchSelection.CreatedBy = authId;

            return await _psBranchSelectionRepository.AddSaveAsync(authId,
                branchSelection);
        }

        public async Task<PsBranchSelection> AddBranchProgramSelectionAsync(
            PsBranchSelection branchSelection)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.SchedulePerformers))
            {
                _logger.LogError("User {UserId} doesn't have permission to schedule program.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            var authId = GetClaimId(ClaimType.UserId);
            ArgumentNullException.ThrowIfNull(branchSelection);
            if (!branchSelection.ProgramId.HasValue)
            {
                throw new GraException("No program selected.");
            }

            var program = await GetProgramByIdAsync(branchSelection.ProgramId.Value,
                onlyApproved: true);

            var validAgeGroup = await _psProgramRepository.IsValidAgeGroupAsync(program.Id,
                branchSelection.AgeGroupId);
            if (!validAgeGroup)
            {
                throw new GraException("Invalid age group for program.");
            }

            var programAvailableAtBranch = await _psProgramRepository.AvailableAtBranchAsync(
                program.Id, branchSelection.BranchId);
            if (!programAvailableAtBranch)
            {
                throw new GraException("The performer does not performer at that branch.");
            }

            var settings = await _psSettingsRepository.GetBySiteIdAsync(GetCurrentSiteId());
            var branchSelections = await _psBranchSelectionRepository
                .GetByBranchIdAsync(branchSelection.BranchId);

            if (branchSelections.Count >= settings.SelectionsPerBranch)
            {
                throw new GraException($"The branch has already made its {settings.SelectionsPerBranch} selections.");
            }

            var selectedAgeGroupIds = branchSelections.Select(_ => _.AgeGroupId);
            if (selectedAgeGroupIds.Any(_ => _ == branchSelection.AgeGroupId))
            {
                throw new GraException("The branch already has a selection for that age group.");
            }

            branchSelection.BackToBackProgram = await _psAgeGroupRepository
                .BranchHasBackToBackAsync(branchSelection.AgeGroupId, branchSelection.BranchId);

            await ValidateScheduleTimeAsync(program.Id, branchSelection.RequestedStartTime,
                branchSelection.BackToBackProgram);

            branchSelection.KitId = null;
            branchSelection.SelectedAt = _dateTimeProvider.Now;
            branchSelection.CreatedBy = authId;
            branchSelection.ScheduleStartTime = branchSelection.RequestedStartTime
                .AddMinutes(-program.SetupTimeMinutes);
            branchSelection.ScheduleDuration = program.SetupTimeMinutes
                + program.ProgramLengthMinutes + program.BreakdownTimeMinutes;
            if (branchSelection.BackToBackProgram)
            {
                branchSelection.ScheduleDuration += program.ProgramLengthMinutes
                    + program.BackToBackMinutes;
            }

            branchSelection.OnSiteContactName = branchSelection.OnSiteContactName.Trim();
            branchSelection.OnSiteContactEmail = branchSelection.OnSiteContactEmail.Trim();
            branchSelection.OnSiteContactPhone = branchSelection.OnSiteContactPhone.Trim();

            return await _psBranchSelectionRepository.AddSaveAsync(authId,
                branchSelection);
        }

        public async Task<PsKit> AddKitAsync(PsKit kit, List<int> ageSelection)
        {
            VerifyManagementPermission();
            ArgumentNullException.ThrowIfNull(kit);
            var authId = GetClaimId(ClaimType.UserId);

            kit.Description = kit.Description?.Trim();
            kit.Name = kit.Name?.Trim();
            kit.Website = kit.Website?.Trim();

            var addedKit = await _psKitRepository.AddSaveAsync(authId, kit);

            await _psKitRepository.AddKitAgeGroupsAsync(addedKit.Id, ageSelection);

            return addedKit;
        }

        public async Task AddKitImageAsync(int kitId, byte[] imageBytes,
            string fileExtension)
        {
            VerifyManagementPermission();
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
            File.WriteAllBytes(filePath, imageBytes);

            var kitImage = new PsKitImage
            {
                KitId = kit.Id,
                Filename = imagePath
            };

            await _psKitImageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                kitImage);
        }

        public async Task<PsPerformer> AddPerformerAsync(PsPerformer performer,
            List<int> branchAvailability)
        {
            var authId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.AccessPerformerRegistration))
            {
                _logger.LogError("User {UserId} doesn't have permission to add a performer.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            ArgumentNullException.ThrowIfNull(performer);
            var existingPerformer = await _psPerformerRepository.GetByUserIdAsync(authId);
            if (existingPerformer != null)
            {
                throw new GraException("Performer already created.");
            }

            performer.BillingAddress = performer.BillingAddress.Trim();
            performer.ContactName = performer.ContactName.Trim();
            performer.Name = performer.Name.Trim();
            performer.Phone = performer.Phone.Trim();
            performer.References = performer.References.Trim();
            performer.UserId = authId;
            performer.VendorId = performer.VendorId.Trim();
            performer.Website = performer.Website?.Trim();

            var addedPerformer = await _psPerformerRepository.AddSaveAsync(authId, performer);

            if (!addedPerformer.AllBranches)
            {
                await _psPerformerRepository.AddPerformerBranchListAsync(addedPerformer.Id,
                    branchAvailability);
            }

            return addedPerformer;
        }

        public async Task AddPerformerImageAsync(int performerId, byte[] imageBytes,
            string fileExtension)
        {
            var authId = GetClaimId(ClaimType.UserId);

            var performer = await _psPerformerRepository.GetByIdAsync(performerId);

            if (!HasPermission(Permission.ManagePerformers)
                && (performer.UserId != authId
                    || !HasPermission(Permission.AccessPerformerRegistration)))
            {
                _logger.LogError("User {AuthId} doesn't have permission to set performer {PerformerId} images.",
                    authId,
                    performer.Id);
                throw new GraException("Permission denied.");
            }

            var siteId = GetCurrentSiteId();

            var performerImageDirectory = _pathResolver.ResolveContentFilePath(
                        Path.Combine($"site{siteId}", PerformerImagePath));
            Directory.CreateDirectory(performerImageDirectory);

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

            var performerImage = new PsPerformerImage
            {
                PerformerId = performer.Id,
                Filename = imagePath
            };

            await _psPerformerImageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                performerImage);
        }

        public async Task<PsProgram> AddProgramAsync(PsProgram program, List<int> ageSelection)
        {
            var authId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.AccessPerformerRegistration))
            {
                _logger.LogError("User id {AuthId} does not have permission to add program.",
                    authId);
                throw new GraException("Permission denied.");
            }
            ArgumentNullException.ThrowIfNull(program);
            program.Description = program.Description?.Trim();
            program.IsApproved = true;
            program.Setup = program.Setup?.Trim();
            program.Title = program.Title?.Trim();

            var addedProgram = await _psProgramRepository.AddSaveAsync(authId, program);

            await _psProgramRepository.AddProgramAgeGroupsAsync(addedProgram.Id, ageSelection);

            return addedProgram;
        }

        public async Task AddProgramImageAsync(int programId, byte[] imageBytes,
            string fileExtension)
        {
            var authId = GetClaimId(ClaimType.UserId);

            var program = await _psProgramRepository.GetByIdAsync(programId);

            if (!HasPermission(Permission.ManagePerformers))
            {
                var performer = await _psPerformerRepository
                    .GetByIdAsync(program.PerformerId);
                if (performer.UserId != authId
                    || !HasPermission(Permission.AccessPerformerRegistration))
                {
                    _logger.LogError("User id {AuthId} does not have permission to add program {ProgramId} image.",
                        authId,
                        programId);
                    throw new GraException("Permission denied.");
                }
            }

            var siteId = GetCurrentSiteId();

            var programImageDirectory = _pathResolver.ResolveContentFilePath(
                        Path.Combine($"site{siteId}", ProgramImagePath));
            Directory.CreateDirectory(programImageDirectory);

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

            var programImage = new PsProgramImage
            {
                ProgramId = program.Id,
                Filename = imagePath
            };

            await _psProgramImageRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                programImage);
        }

        public async Task<bool> BranchAgeGroupAlreadySelectedAsync(int ageGroupId,
            int branchId, int? currentSelectionId = null)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.SchedulePerformers))
            {
                _logger.LogError("User {UserId} doesn't have permission to verify branch age group selection.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            return await _psBranchSelectionRepository.BranchAgeGroupAlreadySelectedAsync(ageGroupId,
                branchId, currentSelectionId);
        }

        public async Task<bool> BranchAgeGroupHasBackToBackAsync(int ageGroupId,
            int branchId)
        {
            if (!HasPermission(Permission.ManagePerformers)
                   && !HasPermission(Permission.SchedulePerformers))
            {
                _logger.LogError("User {UserId} doesn't have permission to verify branch back to back status",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            return await _psAgeGroupRepository.BranchHasBackToBackAsync(ageGroupId, branchId);
        }

        public async Task DeleteBranchSelectionAsync(PsBranchSelection branchSelection)
        {
            ArgumentNullException.ThrowIfNull(branchSelection);
            var currentBranchSelection = await _psBranchSelectionRepository.GetByIdAsync(
                branchSelection.Id);
            currentBranchSelection.IsDeleted = true;
            await _psBranchSelectionRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentBranchSelection);
        }

        public async Task<PsPerformer> EditPerformerAsync(PsPerformer performer,
            List<int> branchAvailability)
        {
            ArgumentNullException.ThrowIfNull(performer);
            var authId = GetClaimId(ClaimType.UserId);

            var currentPerformer = await _psPerformerRepository.GetByIdAsync(performer.Id)
                ?? throw new GraException("The requested performer could not be accessed or does not exist.");

            if (!HasPermission(Permission.ManagePerformers)
                && (currentPerformer.UserId != authId
                    || !HasPermission(Permission.AccessPerformerRegistration)))
            {
                _logger.LogError("User {AuthId} doesn't have permission to edit performer {PerformerId}.",
                    authId,
                    currentPerformer.Id);
                throw new GraException("Permission denied.");
            }

            currentPerformer.BillingAddress = performer.BillingAddress.Trim();
            currentPerformer.ContactName = performer.ContactName.Trim();
            currentPerformer.HasFingerprintCard = performer.HasFingerprintCard;
            currentPerformer.HasInsurance = performer.HasInsurance;
            currentPerformer.Name = performer.Name.Trim();
            currentPerformer.Email = performer.Email.Trim();
            currentPerformer.Phone = performer.Phone.Trim();
            currentPerformer.PhonePreferred = performer.PhonePreferred;
            currentPerformer.References = performer.References.Trim();
            currentPerformer.VendorId = performer.VendorId.Trim();
            currentPerformer.Website = performer.Website?.Trim();

            if (!currentPerformer.AllBranches || !performer.AllBranches)
            {
                var branchesToAdd = new List<int>();
                var branchesToRemove = new List<int>();

                if (currentPerformer.AllBranches)
                {
                    branchesToAdd = branchAvailability;
                }
                else
                {
                    var currentBranches = (await _psPerformerRepository
                    .GetPerformerBranchesAsync(currentPerformer.Id))
                    .Select(_ => _.Id)
                    .ToList();

                    if (performer.AllBranches)
                    {
                        branchesToRemove = currentBranches;
                    }
                    else
                    {
                        branchesToAdd = branchAvailability.Except(currentBranches).ToList();
                        branchesToRemove = currentBranches.Except(branchAvailability).ToList();
                    }
                }

                if (branchesToAdd?.Count > 0)
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

            return await _psPerformerRepository.UpdateSaveAsync(authId, currentPerformer);
        }

        public async Task EditPerformerScheduleAsync(int performerId, List<PsScheduleDate> schedule)
        {
            var authId = GetClaimId(ClaimType.UserId);

            var performer = await _psPerformerRepository.GetByIdAsync(performerId);

            if (!HasPermission(Permission.ManagePerformers)
                && (performer.UserId != authId
                    || !HasPermission(Permission.AccessPerformerRegistration)))
            {
                _logger.LogError("User {AuthId} doesn't have permission to set performer {PerformerId} schedule.",
                    authId,
                    performer.Id);
                throw new GraException("Permission denied.");
            }
            ArgumentNullException.ThrowIfNull(schedule);
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
                _logger.LogError("Invalid schedule date count for user {AuthId}: {Schedule} instead of {TotalDates}",
                    authId,
                    schedule.Count,
                    totalDates);
                throw new GraException("Invalid date count.");
            }
            var invalidDates = schedule
                .Where(_ => _.ParsedDate < settings.ScheduleStartDate.Value.Date
                    || _.ParsedDate > settings.ScheduleEndDate.Value.Date
                    || blackoutDates.Any(b => b.Date.Date == _.ParsedDate))
                .ToList();

            if (invalidDates.Count > 0)
            {
                _logger.LogError("Invalid schedule dates for user {AuthId}: {InvalidDates}",
                    authId,
                    string.Join(", ", invalidDates.Select(_ => _.ParsedDate)));
                throw new GraException("Invalid dates selected.");
            }

            var unavailableDates = schedule
                         .Where(_ => _.Availability == nameof(PsScheduleDateStatus.Unavailable))
                         .Select(_ => new PsPerformerSchedule
                         {
                             PerformerId = performerId,
                             Date = _.ParsedDate
                         });
            var restrictedDates = schedule
                        .Where(_ => _.Availability == nameof(PsScheduleDateStatus.Time))
                        .Select(_ => new PsPerformerSchedule
                        {
                            PerformerId = performerId,
                            Date = _.ParsedDate,
                            StartTime = DateTime.Parse(_.Time[0]),
                            EndTime = DateTime.Parse(_.Time[1])
                        });

            var performerSchedule = unavailableDates.Concat(restrictedDates).ToList();

            await _psPerformerScheduleRepository.SetPerformerScheduleAsync(performerId,
                performerSchedule);

            if (!performer.SetSchedule)
            {
                performer.SetSchedule = true;
                await _psPerformerRepository.UpdateSaveAsync(authId, performer);
            }
        }

        public async Task<ICollection<int>> GetAgeGroupBacktoBackBranchIdsAsync(int ageGroupId)
        {
            VerifyManagementPermission();
            return await _psAgeGroupRepository.GetAgeGroupBackToBackBranchIdsAsync(ageGroupId);
        }

        public async Task<PsAgeGroup> GetAgeGroupByIdAsync(int id)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view age group.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            try
            {
                return await _psAgeGroupRepository.GetByIdAsync(id);
            }
            catch (Exception)
            {
                throw new GraException("The requested age group could not be accessed or does not exist.");
            }
        }

        public async Task<ICollection<PsAgeGroup>> GetAgeGroupsAsync()
        {
            if (!HasPermission(Permission.AccessPerformerRegistration)
                && !HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer scheduling blackout dates.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psAgeGroupRepository.GetAllAsync();
        }

        public async Task<ICollection<PsKit>> GetAllKitsAsync()
        {
            VerifyManagementPermission();
            return await _psKitRepository.GetAllAsync();
        }

        public async Task<ICollection<PsPerformer>> GetAllPerformersAsync()
        {
            VerifyManagementPermission();
            return await _psPerformerRepository.GetAllAsync();
        }

        public async Task<ICollection<PsBlackoutDate>> GetBlackoutDatesAsync()
        {
            if (!HasPermission(Permission.AccessPerformerRegistration)
                && !HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer scheduling blackout dates.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psBlackoutDateRepository.GetAllAsync();
        }

        public async Task<PsBranchSelection> GetBranchProgramSelectionByIdAsync(int id)
        {
            return await _psBranchSelectionRepository.GetByIdAsync(id);
        }

        public async Task<ICollection<PsBranchSelection>>
            GetBranchProgramSelectionsByPerformerAsync(int performerId)
        {
            return await _psBranchSelectionRepository.GetByPerformerIdAsync(performerId);
        }

        public async Task<ICollection<int>> GetExcludedBranchIdsAsync()
        {
            if (!HasPermission(Permission.AccessPerformerRegistration)
                && !HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer scheduling branch ids.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            return await _psSettingsRepository.GetExcludedBranchIdsAsync();
        }

        public async Task<ICollection<PsBranchSelection>> GetKitBranchSelectionsAsync(int kitId)
        {
            VerifyManagementPermission();
            return await _psBranchSelectionRepository.GetByKitIdAsync(kitId);
        }

        public async Task<PsKit> GetKitByIdAsync(int kitId, bool includeAgeGroups = false,
            bool includeImages = false)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view kit.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            var kit = await _psKitRepository.GetByIdAsync(kitId)
                ?? throw new GraException("The requested kit could not be accessed or does not exist.");

            if (includeAgeGroups)
            {
                kit.AgeGroups = await _psKitRepository.GetKitAgeGroupsAsync(kit.Id);
            }
            if (includeImages)
            {
                kit.Images = await _psKitImageRepository.GetByKitIdAsync(kit.Id);
            }

            return kit;
        }

        public async Task<int> GetKitCountAsync()
        {
            return await _psKitRepository.GetKitCountAsync();
        }

        public async Task<List<int>> GetKitIndexListAsync()
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view kit index list.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psKitRepository.GetIndexListAsync();
        }

        public async Task<int> GetKitSelectionCountAsync(int kitId)
        {
            VerifyManagementPermission();
            return await _psBranchSelectionRepository.GetCountByKitIdAsync(kitId);
        }

        public async Task<Branch> GetNonExcludedBranch(int branchId)
        {
            if (!HasPermission(Permission.AccessPerformerRegistration)
                && !HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer scheduling branch.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            return await _psSettingsRepository.GetNonExcludedBranchAsync(branchId);
        }

        public async Task<ICollection<Branch>> GetNonExcludedSystemBranchesAsync(int systemId,
            bool prioritizeUserBranch = false)
        {
            if (!HasPermission(Permission.AccessPerformerRegistration)
                && !HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer scheduling system branches.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            var prioritizedBranch = prioritizeUserBranch
                ? GetClaimId(ClaimType.BranchId) : default(int?);

            return await _psSettingsRepository.GetNonExcludedSystemBranchesAsync(systemId,
                prioritizedBranch);
        }

        public async Task<DataWithCount<ICollection<PsAgeGroup>>> GetPaginatedAgeGroupsAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();
            return await _psAgeGroupRepository.PageAsync(filter);
        }

        public async Task<DataWithCount<ICollection<PsBlackoutDate>>> GetPaginatedBlackoutDatesAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();
            return await _psBlackoutDateRepository.PageAsync(filter);
        }

        public async Task<DataWithCount<ICollection<Branch>>> GetPaginatedExcludedBranchListAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();
            return await _psSettingsRepository.PageExcludedBranchesAsync(filter);
        }

        public async Task<DataWithCount<ICollection<PsKit>>> GetPaginatedKitListAsync(
            BaseFilter filter)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view kit list.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            return await _psKitRepository.PageAsync(filter);
        }

        public async Task<DataWithCount<ICollection<PsPerformer>>> GetPaginatedPerformerListAsync(
            PerformerSchedulingFilter filter)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer list.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psPerformerRepository.PageAsync(filter);
        }

        public async Task<DataWithCount<ICollection<PsProgram>>> GetPaginatedProgramListAsync(
            PerformerSchedulingFilter filter)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view program list.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psProgramRepository.PageAsync(filter);
        }

        public async Task<ICollection<PsAgeGroup>> GetPerformerAgeGroupsAsync(int performerId)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer age groups.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psPerformerRepository.GetPerformerAgeGroupsAsync(performerId);
        }

        public async Task<ICollection<int>> GetPerformerBranchIdsAsync(int performerId,
            int? systemId = null)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer branches.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psPerformerRepository.GetPerformerBranchIdsAsync(performerId, systemId);
        }

        public async Task<ICollection<PsBranchSelection>> GetPerformerBranchSelectionsAsync(
            int performerId, DateTime? date = null)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer branch selections.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psBranchSelectionRepository.GetByPerformerIdAsync(performerId, date);
        }

        public async Task<PsPerformer> GetPerformerByIdAsync(int id,
            bool includeBranches = false,
            bool includeImages = false,
            bool includePrograms = false,
            bool includeSchedule = false,
            bool onlyApproved = false)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            var performer = await _psPerformerRepository.GetByIdAsync(id, onlyApproved)
                ?? throw new GraException("The requested performer could not be accessed or does not exist.");
            if (includeBranches)
            {
                performer.Branches = await _psPerformerRepository.GetPerformerBranchesAsync(
                    performer.Id);
            }
            if (includeImages)
            {
                performer.Images = await _psPerformerImageRepository.GetByPerformerIdAsync(
                    performer.Id);
            }
            if (includePrograms)
            {
                performer.Programs = await _psProgramRepository.GetByPerformerIdAsync(
                    performer.Id, onlyApproved);
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
            bool includeImages = false,
            bool includePrograms = false,
            bool includeSchedule = false)
        {
            var authId = GetClaimId(ClaimType.UserId);
            if (authId != userId)
            {
                _logger.LogError("User {AuthId} doesn't have permission to view performer information for user {UserId}.",
                    authId,
                    userId);
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
                if (includeImages)
                {
                    performer.Images = await _psPerformerImageRepository.GetByPerformerIdAsync(
                        performer.Id);
                }
                if (includePrograms)
                {
                    performer.Programs = await _psProgramRepository.GetByPerformerIdAsync(
                        performer.Id, false);
                }
                if (includeSchedule)
                {
                    performer.Schedule = await _psPerformerScheduleRepository
                        .GetByPerformerIdAsync(performer.Id);
                }
            }

            return performer;
        }

        public async Task<int> GetPerformerCountAsync()
        {
            return await _psPerformerRepository.GetPerformerCountAsync();
        }

        public async Task<PsPerformerSchedule> GetPerformerDateScheduleAsync(int performerId,
            DateTime date)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer date schedule.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psPerformerScheduleRepository.GetPerformerDateScheduleAsync(performerId,
                date);
        }

        public async Task<List<int>> GetPerformerIndexListAsync(bool onlyApproved = false)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer index list.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psPerformerRepository.GetIndexListAsync(onlyApproved);
        }

        public async Task<int> GetPerformerProgramCountAsync(int performerId, 
            bool onlyApproved = false)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer program count.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psProgramRepository.GetCountByPerformerAsync(performerId, onlyApproved);
        }

        public async Task<ICollection<PsPerformerSchedule>> GetPerformerScheduleAsync(
            int performerId)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer schedule.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psPerformerScheduleRepository.GetByPerformerIdAsync(performerId);
        }

        public async Task<int> GetPerformerSelectionCountAsync(int performerId)
        {
            VerifyManagementPermission();
            return await _psBranchSelectionRepository.GetCountByPerformerIdAsync(performerId);
        }

        public async Task<bool> GetPerformerSystemAvailabilityAsync(int performerId, int systemId)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId)} doesn't have permission to view performer system availability.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psPerformerRepository.GetPerformerSystemAvailability(performerId,
                systemId);
        }

        public async Task<PsProgram> GetProgramByIdAsync(int id,
            bool includeAgeGroups = false,
            bool includeImages = false,
            bool onlyApproved = false)
        {
            var authId = GetClaimId(ClaimType.UserId);
            var program = await _psProgramRepository.GetByIdAsync(id, onlyApproved)
                ?? throw new GraException("The requested program could not be accessed or does not exist.");
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                var performer = await _psPerformerRepository.GetByIdAsync(program.PerformerId);
                if (performer.UserId != authId
                    || !HasPermission(Permission.AccessPerformerRegistration))
                {
                    _logger.LogError("User id {AuthId} does not have permission to view program {Id}.",
                        authId,
                        id);
                    throw new GraException("Permission denied.");
                }
            }

            if (includeAgeGroups)
            {
                program.AgeGroups = await _psProgramRepository.GetProgramAgeGroupsAsync(program.Id);
            }
            if (includeImages)
            {
                program.Images = await _psProgramImageRepository.GetByProgramIdAsync(program.Id);
            }

            return program;
        }

        public async Task<int> GetProgramCountAsync()
        {
            return await _psProgramRepository.GetProgramCountAsync();
        }

        public async Task<List<int>> GetProgramIndexListAsync(int? ageGroupId = null,
            bool onlyApproved = false)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view program index list.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
            return await _psProgramRepository.GetIndexListAsync(ageGroupId, onlyApproved);
        }

        public PsSchedulingStage GetSchedulingStage(PsSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings?.ContactEmail)
                || settings?.SelectionsPerBranch == null
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

        public async Task<ICollection<PsBranchSelection>> GetSelectionsByBranchIdAsync(int branchId)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view branch selections.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            return await _psBranchSelectionRepository.GetByBranchIdAsync(branchId);
        }

        public async Task<PsSettings> GetSettingsAsync()
        {
            if (!HasPermission(Permission.AccessPerformerRegistration)
                && !HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer scheduling settings.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            var siteId = GetCurrentSiteId();
            return await _psSettingsRepository.GetBySiteIdAsync(siteId);
        }

        public async Task<List<Model.System>> GetSystemListWithoutExcludedBranchesAsync()
        {
            if (!HasPermission(Permission.AccessPerformerRegistration)
                && !HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer scheduling system list.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            var systems = (await _systemRepository.GetAllAsync(GetCurrentSiteId()))
                .ToList();

            var excludedBranchIds = await _psSettingsRepository
                .GetExcludedBranchIdsAsync();

            foreach (var system in systems)
            {
                system.Branches = system.Branches
                    .Where(_ => !excludedBranchIds.Contains(_.Id)).ToList();
            }

            return systems;
        }

        public async Task<Model.System> GetSystemWithoutExcludedBranchesAsync(int systemId)
        {
            if (!HasPermission(Permission.AccessPerformerRegistration)
                && !HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.ViewPerformerDetails))
            {
                _logger.LogError("User {UserId} doesn't have permission to view performer scheduling system.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            var system = await _systemRepository.GetByIdAsync(systemId);

            system.Branches = await _psSettingsRepository
                .GetNonExcludedSystemBranchesAsync(systemId);

            return system;
        }

        public async Task<bool> ProgramAvailableAtBranchAsync(int programId, int branchId)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.SchedulePerformers))
            {
                _logger.LogError("User {UserId} doesn't have permission to verify program availability at branch.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            return await _psProgramRepository.AvailableAtBranchAsync(programId, branchId);
        }

        public async Task RemoveAgeGroupAsync(int ageGroupId)
        {
            VerifyManagementPermission();
            await _psAgeGroupRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId),
                ageGroupId);
        }

        public async Task RemoveBlackoutDateAsync(int blackoutDateId)
        {
            VerifyManagementPermission();
            await _psBlackoutDateRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId),
                blackoutDateId);
        }

        public async Task RemoveBranchExclusionAsync(int branchId)
        {
            VerifyManagementPermission();
            await _psSettingsRepository.RemoveBranchExclusionAsync(branchId);
        }

        public async Task RemoveKitAsync(int kitId)
        {
            VerifyManagementPermission();
            var kit = await _psKitRepository.GetByIdAsync(kitId)
                ?? throw new GraException("The requested kit could not be accessed or does not exist.");
            var ageGroupIds = (await _psKitRepository.GetKitAgeGroupsAsync(kit.Id))
                .Select(_ => _.Id).ToList();
            await _psKitRepository.RemoveKitAgeGroupsAsync(kit.Id, ageGroupIds);

            var kitImages = await _psKitImageRepository.GetByKitIdAsync(kit.Id);
            foreach (var image in kitImages)
            {
                await RemoveKitImageAsync(image);
            }

            await _psKitRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), kit.Id);
        }

        public async Task RemoveKitImageByIdAsync(int imageId)
        {
            VerifyManagementPermission();
            var image = await _psKitImageRepository.GetByIdAsync(imageId);

            await RemoveKitImageAsync(image);
        }

        public async Task RemovePerformerAsync(int performerId)
        {
            VerifyManagementPermission();
            var performer = await _psPerformerRepository.GetByIdAsync(performerId)
                ?? throw new GraException("The requested performer could not be accessed or does not exist.");
            var performerImages = await _psPerformerImageRepository.GetByPerformerIdAsync(
                performer.Id);
            foreach (var image in performerImages)
            {
                await RemovePerformerImageAsync(image);
            }

            var performerPrograms = await _psProgramRepository
                .GetByPerformerIdAsync(performer.Id, false);
            foreach (var program in performerPrograms)
            {
                await RemoveProgramAsync(program.Id);
            }

            await _psPerformerScheduleRepository.RemovePerformerScheduleAsync(performer.Id);
            await _psPerformerRepository.RemovePerformerBranchesAsync(performer.Id);

            await _psPerformerRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), performer.Id);
        }

        public async Task RemovePerformerImageByIdAsync(int imageId)
        {
            var image = await _psPerformerImageRepository.GetByIdAsync(imageId);

            var authId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.ManagePerformers))
            {
                var performer = await _psPerformerRepository.GetByIdAsync(image.PerformerId);
                if (performer.UserId != authId
                    || !HasPermission(Permission.AccessPerformerRegistration))
                {
                    _logger.LogError("User {AuthId} doesn't have permission to remove performer image {ImageId}.",
                        authId,
                        imageId);
                    throw new GraException("Permission denied.");
                }
            }

            await RemovePerformerImageAsync(image);
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

            var program = await _psProgramRepository.GetByIdAsync(programId)
                ?? throw new GraException("The requested program could not be accessed or does not exist.");
            if (!HasPermission(Permission.ManagePerformers))
            {
                var performer = await _psPerformerRepository
                    .GetByIdAsync(program.PerformerId);
                if (performer.UserId != authId
                    || !HasPermission(Permission.AccessPerformerRegistration))
                {
                    _logger.LogError("User id {AuthId} does not have permission to remove program {ProgramId}.",
                        authId,
                        programId);
                    throw new GraException("Permission denied.");
                }
            }

            var ageGroups = await _psProgramRepository.GetProgramAgeGroupsAsync(program.Id);
            var ageGroupIds = ageGroups.Select(_ => _.Id).ToList();
            await _psProgramRepository.RemoveProgramAgeGroupsAsync(programId, ageGroupIds);

            var images = await _psProgramImageRepository.GetByProgramIdAsync(programId);
            foreach (var image in images)
            {
                await RemoveProgramImageAsync(image);
            }

            await _psProgramRepository.RemoveSaveAsync(authId, programId);
        }

        public async Task RemoveProgramImageByIdAsync(int imageId)
        {
            var image = await _psProgramImageRepository.GetByIdAsync(imageId);

            var authId = GetClaimId(ClaimType.UserId);

            if (!HasPermission(Permission.ManagePerformers))
            {
                var program = await _psProgramRepository.GetByIdAsync(image.ProgramId);
                var performer = await _psPerformerRepository.GetByIdAsync(program.PerformerId);
                if (performer.UserId != authId
                    || !HasPermission(Permission.AccessPerformerRegistration))
                {
                    _logger.LogError("User {AuthId} doesn't have permission to remove program image {ImageId}.",
                        authId,
                        imageId);
                    throw new GraException("Permission denied.");
                }
            }

            await RemoveProgramImageAsync(image);
        }

        public async Task SetPerformerApprovedAsync(int performerId, bool isApproved)
        {
            VerifyManagementPermission();

            var performer = await _psPerformerRepository.GetByIdAsync(performerId)
                ?? throw new GraException("The requested performer could not be accessed or does not exist.");
            performer.IsApproved = isApproved;

            await _psPerformerRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), performer);
        }

        public async Task SetPerformerRegistrationCompeltedAsync(int performerId)
        {
            var authId = GetClaimId(ClaimType.UserId);

            var performer = await _psPerformerRepository.GetByIdAsync(performerId);

            if (!HasPermission(Permission.ManagePerformers)
                && (performer.UserId != authId
                    || !HasPermission(Permission.AccessPerformerRegistration)))
            {
                _logger.LogError("User {AuthId} doesn't have permission to set performer {PerformerId} registration completed.",
                    authId,
                    performer.Id);
                throw new GraException("Permission denied.");
            }

            performer.RegistrationCompleted = true;
            await _psPerformerRepository.UpdateSaveAsync(authId, performer);
        }

        public async Task SetProgramApprovedAsync (int programId, bool isApproved)
        {
            VerifyManagementPermission();

            var program = await _psProgramRepository.GetByIdAsync(programId)
                ?? throw new GraException("The requested program could not be accessed or does not exist.");
            program.IsApproved = isApproved;

            await _psProgramRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), program);
        }

        public async Task SetSelectionSecretCodeAsync(int selectionId, string secretCode)
        {
            VerifyManagementPermission();

            var sanitizedSecretCode = secretCode?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(sanitizedSecretCode))
            {
                throw new GraException("Please enter a Secret Code.");
            }
            else if (!sanitizedSecretCode.All(char.IsLetterOrDigit))
            {
                throw new GraException("Only letters and numbers are allowed.");
            }
            else if (sanitizedSecretCode.Length > 50)
            {
                throw new GraException("Please enter less than 50 characters.");
            }

            var existingTrigger = await _triggerRepository.GetByCodeAsync(GetCurrentSiteId(),
                sanitizedSecretCode, false);
            if (existingTrigger != null)
            {
                throw new GraException("Code is already in use by a trigger.");
            }

            var currentSelection = await _psBranchSelectionRepository.GetByIdAsync(selectionId);
            if (currentSelection.SecretCode != sanitizedSecretCode)
            {
                var existingSelection = await _psBranchSelectionRepository
                    .GetByCodeAsync(sanitizedSecretCode);
                if (existingSelection != null)
                {
                    throw new GraException("Code is already in use by a performer selection.");
                }

                currentSelection.SecretCode = sanitizedSecretCode;
                await _psBranchSelectionRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                    currentSelection);
            }
        }

        public async Task UpdateAgeGroupBackToBackBranchesAsync(int ageGroupId,
            List<int> branchIds)
        {
            VerifyManagementPermission();
            var currentBackToBackBranches = await _psAgeGroupRepository
                .GetAgeGroupBackToBackBranchIdsAsync(ageGroupId);

            var branchesToAdd = branchIds.Except(currentBackToBackBranches).ToList();
            var branchesToRemove = currentBackToBackBranches.Except(branchIds).ToList();

            await _psAgeGroupRepository.AddAgeGroupBackToBackBranchesAsync(ageGroupId,
                branchesToAdd);
            await _psAgeGroupRepository.RemoveAgeGroupBackToBackBranchesAsync(ageGroupId,
                branchesToRemove);
        }

        public async Task UpdateBranchKitSelectionAsync(PsBranchSelection branchSelection)
        {
            VerifyManagementPermission();
            ArgumentNullException.ThrowIfNull(branchSelection);
            var currentBranchSelection = await _psBranchSelectionRepository.GetByIdAsync(
                branchSelection.Id);

            if (currentBranchSelection == null)
            {
                throw new GraException("Selection does not exist.");
            }
            else if (!currentBranchSelection.KitId.HasValue)
            {
                throw new GraException("Selection is not a kit selection.");
            }

            var ageGroupExists = await _psBranchSelectionRepository
                .BranchAgeGroupAlreadySelectedAsync(branchSelection.AgeGroupId,
                    currentBranchSelection.BranchId,
                    currentBranchSelection.Id);
            if (ageGroupExists)
            {
                throw new GraException("Branch already has a selection for that age group.");
            }

            var validAgeGroup = await _psKitRepository.IsValidAgeGroupAsync(
                branchSelection.KitId.Value,
                branchSelection.AgeGroupId);
            if (!validAgeGroup)
            {
                throw new GraException("Invalid age group for that kit.");
            }

            currentBranchSelection.AgeGroupId = branchSelection.AgeGroupId;
            currentBranchSelection.KitId = branchSelection.KitId;
            currentBranchSelection.UpdatedByUserId = GetClaimId(ClaimType.UserId);

            await _psBranchSelectionRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentBranchSelection);
        }

        public async Task UpdateBranchProgramContactAsync(PsBranchSelection branchSelection)
        {
            ArgumentNullException.ThrowIfNull(branchSelection);
            var currentBranchSelection = await _psBranchSelectionRepository.GetByIdAsync(
                branchSelection.Id);

            if (currentBranchSelection == null)
            {
                throw new GraException("Selection does not exist.");
            }
            else if (!currentBranchSelection.ProgramId.HasValue)
            {
                throw new GraException("Selection is not a program selection.");
            }

            if (GetActiveUserId() != currentBranchSelection.CreatedBy)
            {
                VerifyManagementPermission();
            }

            currentBranchSelection.OnSiteContactName = branchSelection.OnSiteContactName.Trim();
            currentBranchSelection.OnSiteContactEmail = branchSelection.OnSiteContactEmail.Trim();
            currentBranchSelection.OnSiteContactPhone = branchSelection.OnSiteContactPhone.Trim();

            await _psBranchSelectionRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentBranchSelection);
        }

        public async Task UpdateBranchProgramSelectionAsync(int branchSelectionId,
            DateTime requestedStartTime)
        {
            VerifyManagementPermission();
            var currentBranchSelection = await _psBranchSelectionRepository.GetByIdAsync(
                branchSelectionId);

            if (currentBranchSelection == null)
            {
                throw new GraException("Selection does not exist.");
            }
            else if (!currentBranchSelection.ProgramId.HasValue)
            {
                throw new GraException("Selection is not a program selection.");
            }

            await ValidateScheduleTimeAsync(currentBranchSelection.ProgramId.Value,
                requestedStartTime,
                currentBranchSelection.BackToBackProgram,
                currentBranchSelection.Id);

            var program = await _psProgramRepository.GetByIdAsync(
                currentBranchSelection.ProgramId.Value);

            currentBranchSelection.RequestedStartTime = requestedStartTime;
            currentBranchSelection.ScheduleStartTime = requestedStartTime
                .AddMinutes(-program.SetupTimeMinutes);

            currentBranchSelection.SelectedAt = _dateTimeProvider.Now;
            currentBranchSelection.UpdatedByUserId = GetClaimId(ClaimType.UserId);

            await _psBranchSelectionRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentBranchSelection);
        }

        public async Task<PsKit> UpdateKitAsync(PsKit kit, List<int> ageSelection)
        {
            VerifyManagementPermission();
            ArgumentNullException.ThrowIfNull(kit);
            var authId = GetClaimId(ClaimType.UserId);

            var currentKit = await _psKitRepository.GetByIdAsync(kit.Id)
                ?? throw new GraException("The requested kit could not be accessed or does not exist.");
            currentKit.Description = kit.Description?.Trim();
            currentKit.Name = kit.Name?.Trim();
            currentKit.Website = kit.Website?.Trim();

            currentKit = await _psKitRepository.UpdateSaveAsync(authId, currentKit);

            var currentAgeGroups = await _psKitRepository.GetKitAgeGroupsAsync(currentKit.Id);
            var agesToAdd = ageSelection.Except(currentAgeGroups.Select(_ => _.Id)).ToList();
            var agesToRemove = currentAgeGroups.Select(_ => _.Id).Except(ageSelection).ToList();

            await _psKitRepository.AddKitAgeGroupsAsync(currentKit.Id, agesToAdd);
            await _psKitRepository.RemoveKitAgeGroupsAsync(currentKit.Id, agesToRemove);

            return currentKit;
        }

        public async Task<PsProgram> UpdateProgramAsync(PsProgram program, List<int> ageSelection)
        {
            ArgumentNullException.ThrowIfNull(program);
            var authId = GetClaimId(ClaimType.UserId);

            var currentProgram = await _psProgramRepository.GetByIdAsync(program.Id)
                ?? throw new GraException("The requested program could not be accessed or does not exist.");
            if (!HasPermission(Permission.ManagePerformers))
            {
                var performer = await _psPerformerRepository
                    .GetByIdAsync(currentProgram.PerformerId);
                if (performer.UserId != authId
                    || !HasPermission(Permission.AccessPerformerRegistration))
                {
                    _logger.LogError("User id {AuthId} does not have permission to edit program {ProgramId}.",
                        authId,
                        currentProgram.Id);
                    throw new GraException("Permission denied.");
                }
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

            var currentAgeGroups = await _psProgramRepository.GetProgramAgeGroupsAsync(
                currentProgram.Id);

            var agesToAdd = ageSelection.Except(currentAgeGroups.Select(_ => _.Id)).ToList();
            var agesToRemove = currentAgeGroups.Select(_ => _.Id).Except(ageSelection).ToList();

            await _psProgramRepository.AddProgramAgeGroupsAsync(currentProgram.Id, agesToAdd);
            await _psProgramRepository.RemoveProgramAgeGroupsAsync(currentProgram.Id, agesToRemove);

            return currentProgram;
        }

        public async Task UpdateSettingsAsync(PsSettings settings)
        {
            VerifyManagementPermission();
            ArgumentNullException.ThrowIfNull(settings);
            var authId = GetClaimId(ClaimType.UserId);

            settings.BranchAvailabilitySupplementalText = settings
                .BranchAvailabilitySupplementalText?.Trim();
            settings.ContactEmail = settings.ContactEmail?.Trim();
            settings.SetupSupplementalText = settings.SetupSupplementalText?.Trim();
            settings.VendorIdPrompt = settings.VendorIdPrompt?.Trim();
            settings.VendorCodeFormat = settings.VendorCodeFormat?.Trim();

            var currentSettings = await GetSettingsAsync();
            if (currentSettings == null)
            {
                settings.SiteId = GetCurrentSiteId();
                await _psSettingsRepository.AddSaveAsync(authId, settings);
            }
            else
            {
                currentSettings.BranchAvailabilitySupplementalText = settings
                    .BranchAvailabilitySupplementalText;
                currentSettings.ContactEmail = settings.ContactEmail?.Trim();
                currentSettings.CoverSheetBranch = settings.CoverSheetBranch?.Trim();
                currentSettings.CoverSheetContact = settings.CoverSheetContact?.Trim();
                currentSettings.SelectionsPerBranch = settings.SelectionsPerBranch;
                currentSettings.RegistrationOpen = settings.RegistrationOpen;
                currentSettings.RegistrationClosed = settings.RegistrationClosed;
                currentSettings.SchedulingPreview = settings.SchedulingPreview;
                currentSettings.SchedulingOpen = settings.SchedulingOpen;
                currentSettings.SchedulingClosed = settings.SchedulingClosed;
                currentSettings.SchedulePosted = settings.SchedulePosted;
                currentSettings.ScheduleStartDate = settings.ScheduleStartDate;
                currentSettings.ScheduleEndDate = settings.ScheduleEndDate;
                currentSettings.SetupSupplementalText = settings.SetupSupplementalText?.Trim();
                currentSettings.VendorIdPrompt = settings.VendorIdPrompt?.Trim();
                currentSettings.VendorCodeFormat = settings.VendorCodeFormat?.Trim();

                await _psSettingsRepository.UpdateSaveAsync(authId, currentSettings);
            }
        }

        public async Task<string> ValidateScheduleTimeAsync(int programId, DateTime programStart,
            bool backToBack, int? currentSelectionId = null)
        {
            if (!HasPermission(Permission.ManagePerformers)
                && !HasPermission(Permission.SchedulePerformers))
            {
                _logger.LogError("User {UserId} doesn't have permission to validate shceudle time.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }

            var blackoutDate = await _psBlackoutDateRepository.GetByDateAsync(programStart);
            if (blackoutDate != null)
            {
                throw new GraException($"Programs cannot be booked on {blackoutDate.Reason}.");
            }

            var program = await _psProgramRepository.GetByIdAsync(programId);

            var scheduleDate = await _psPerformerScheduleRepository.GetPerformerDateScheduleAsync(
                program.PerformerId, programStart);

            if (scheduleDate?.StartTime.HasValue == false)
            {
                throw new GraException("The performer is not available on that day.");
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
                throw new GraException("The performer is not available at that time.");
            }

            var setupStartTime = programStart.AddMinutes(-program.SetupTimeMinutes).TimeOfDay;
            var breakdownEndTime = programStart
                .AddMinutes(programLength + program.BreakdownTimeMinutes)
                .TimeOfDay;

            var bookedTimes = await _psBranchSelectionRepository.GetByPerformerIdAsync(
                program.PerformerId, programStart);

            if (currentSelectionId.HasValue)
            {
                bookedTimes = bookedTimes.Where(_ => _.Id != currentSelectionId.Value).ToList();
            }

            if (bookedTimes.Any(_ => (_.ScheduleStartTime.TimeOfDay <= setupStartTime
                    && _.ScheduleStartTime.AddMinutes(_.ScheduleDuration).TimeOfDay >= setupStartTime)
                || (_.ScheduleStartTime.TimeOfDay <= breakdownEndTime
                        && _.ScheduleStartTime.AddMinutes(_.ScheduleDuration).TimeOfDay >= breakdownEndTime)
                || (_.ScheduleStartTime.TimeOfDay >= setupStartTime
                    && _.ScheduleStartTime.AddMinutes(_.ScheduleDuration).TimeOfDay <= breakdownEndTime)))
            {
                throw new GraException("The performer is already booked during that time.");
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

        private async Task RemoveKitImageAsync(PsKitImage image)
        {
            var authId = GetClaimId(ClaimType.UserId);

            await _psKitImageRepository.RemoveSaveAsync(authId, image.Id);
            var file = _pathResolver.ResolveContentFilePath(image.Filename);
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
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

        private async Task RemoveProgramImageAsync(PsProgramImage image)
        {
            var authId = GetClaimId(ClaimType.UserId);

            await _psProgramImageRepository.RemoveSaveAsync(authId, image.Id);
            var file = _pathResolver.ResolveContentFilePath(image.Filename);
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
        }
    }
}
