using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class SiteService : BaseUserService<SiteService>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IProgramRepository _programRepository;
        private readonly SegmentService _segmentService;
        private readonly SiteLookupService _siteLookupService;
        private readonly ISiteRepository _siteRepository;
        private readonly ISiteSettingRepository _siteSettingRepository;
        private readonly ISpatialDistanceRepository _spatialDistanceRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IUserRepository _userRepository;

        public SiteService(ILogger<SiteService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            SegmentService segmentService,
            ISiteRepository siteRepository,
            ISiteSettingRepository siteSettingRepository,
            ISpatialDistanceRepository spatialDistanceRepository,
            ISystemRepository systemRepository,
            SiteLookupService siteLookupService,
            IUserRepository userRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageSites);
            ArgumentNullException.ThrowIfNull(branchRepository);
            ArgumentNullException.ThrowIfNull(programRepository);
            ArgumentNullException.ThrowIfNull(segmentService);
            ArgumentNullException.ThrowIfNull(siteLookupService);
            ArgumentNullException.ThrowIfNull(siteRepository);
            ArgumentNullException.ThrowIfNull(siteSettingRepository);
            ArgumentNullException.ThrowIfNull(spatialDistanceRepository);
            ArgumentNullException.ThrowIfNull(systemRepository);
            ArgumentNullException.ThrowIfNull(userRepository);

            _branchRepository = branchRepository;
            _programRepository = programRepository;
            _segmentService = segmentService;
            _siteLookupService = siteLookupService;
            _siteRepository = siteRepository;
            _siteSettingRepository = siteSettingRepository;
            _spatialDistanceRepository = spatialDistanceRepository;
            _systemRepository = systemRepository;
            _userRepository = userRepository;
        }

        public async Task<Branch> AddBranchAsync(Branch branch)
        {
            ArgumentNullException.ThrowIfNull(branch);
            VerifyPermission(Permission.ManageSystems);

            branch.Address = branch.Address.Trim();
            branch.Name = branch.Name.Trim();
            branch.Telephone = branch.Telephone?.Trim();
            branch.Url = branch.Url?.Trim();

            return await _branchRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), branch);
        }

        public async Task<Program> AddProgramAsync(Program program)
        {
            ArgumentNullException.ThrowIfNull(program);
            VerifyPermission(Permission.ManagePrograms);
            var siteId = GetCurrentSiteId();
            var filter = new BaseFilter()
            {
                SiteId = siteId
            };
            program.Position = await _programRepository.CountAsync(filter);
            program.SiteId = siteId;

            return await _programRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), program);
        }

        public async Task<Model.System> AddSystemAsync(Model.System system)
        {
            ArgumentNullException.ThrowIfNull(system);
            VerifyPermission(Permission.ManageSystems);
            system.SiteId = GetCurrentSiteId();
            return await _systemRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), system);
        }

        public async Task DecreaseProgramPositionAsync(int programId)
        {
            VerifyPermission(Permission.ManagePrograms);
            await _programRepository.DecreasePositionAsync(programId, GetCurrentSiteId());
        }

        public async Task<IEnumerable<Branch>> GetAllBranches(bool includeSystemName = false)
        {
            var branchList = await _branchRepository.GetAllAsync(GetCurrentSiteId());
            if (includeSystemName)
            {
                foreach (var branch in branchList)
                {
                    branch.Name = $"{branch.Name} ({branch.SystemName})";
                }
            }
            return branchList;
        }

        public async Task<Branch> GetBranchByIdAsync(int branchId)
        {
            return await _branchRepository.GetByIdAsync(branchId);
        }

        public async Task<IEnumerable<Branch>> GetBranches(int systemId,
            bool prioritizeUserBranch = false)
        {
            var branchList = await _branchRepository.GetBySystemAsync(systemId);
            if (prioritizeUserBranch)
            {
                branchList = branchList
                    .OrderByDescending(_ => _.Id == GetClaimId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
            }
            return branchList;
        }

        public async Task<int> GetBranchInUseAsync(int branchId)
        {
            return await _branchRepository.IsInUseAsync(branchId);
        }

        public async Task<string> GetBranchName(int branchId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new GraException("Could not find branch.");
            }
            else
            {
                return branch.Name;
            }
        }

        public async Task<byte[]> GetIcsFile(string siteUrl)
        {
            var site = await _siteRepository.GetByIdAsync(GetCurrentSiteId());
            if (site.RegistrationOpens == null)
            {
                _logger.LogError("Can't generate calendar file because RegistrationOpens date is not set for site {siteId}",
                    site.Id);
                throw new GraException("Unable to generate calendar file.");
            }
            string project = new(site.Name.Where(_ => char.IsLetterOrDigit(_)).ToArray());

            // if the start time is midnight then let's bump the calendar appointment to 0700
            var localStartTime = (DateTime)site.RegistrationOpens;
            if (localStartTime.Hour + localStartTime.Minute + localStartTime.Second == 0)
            {
                localStartTime = localStartTime.AddHours(7);
            }

            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.Append("PRODID:-//").Append(project).AppendLine("//EN");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("BEGIN:VEVENT");
            sb.Append("SUMMARY; LANGUAGE = en - us:").AppendLine(site.Name);
            sb.AppendLine("CLASS:PUBLIC");
            sb.Append("CREATED:").AppendFormat("{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow).AppendLine();
            sb.Append("DESCRIPTION:Remember to join ").Append(site.Name).Append(": ").AppendLine(siteUrl);
            sb.Append("DTSTART:").AppendFormat("{0:yyyyMMddTHHmmss}", localStartTime).AppendLine();
            sb.Append("DTEND:").AppendFormat("{0:yyyyMMddTHHmmss}", localStartTime).AppendLine();
            sb.AppendLine("SEQUENCE:0");
            sb.Append("UID:").Append(Guid.NewGuid()).AppendLine();
            sb.Append("LOCATION:").AppendLine(siteUrl);
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<DataWithCount<ICollection<Branch>>> GetPaginatedBranchListAsync(
            BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            VerifyPermission(Permission.ManageSystems);
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<Branch>>
            {
                Data = await _branchRepository.PageAsync(filter),
                Count = await _branchRepository.CountAsync(filter)
            };
        }

        public async Task<DataWithCount<IEnumerable<Site>>> GetPaginatedListAsync(BaseFilter filter)
        {
            VerifyManagementPermission();
            return await _siteRepository.PageAsync(filter);
        }

        public async Task<DataWithCount<ICollection<Program>>> GetPaginatedProgramListAsync(
        BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            VerifyPermission(Permission.ManagePrograms);
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<Program>>
            {
                Data = await _programRepository.PageAsync(filter),
                Count = await _programRepository.CountAsync(filter)
            };
        }

        public async Task<DataWithCount<ICollection<Model.System>>> GetPaginatedSystemListAsync(
            BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            VerifyPermission(Permission.ManageSystems);
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<Model.System>>
            {
                Data = await _systemRepository.PageAsync(filter),
                Count = await _systemRepository.CountAsync(filter)
            };
        }

        public async Task<Program> GetProgramByIdAsync(int programId)
        {
            return await _programRepository.GetByIdAsync(programId);
        }

        public async Task<IEnumerable<Program>> GetProgramList()
        {
            return await _programRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<Model.System> GetSystemByIdAsync(int id)
        {
            return await _systemRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Model.System>> GetSystemList(bool prioritizeUserSystem = false)
        {
            var systemList = await _systemRepository.GetAllAsync(GetCurrentSiteId());
            if (prioritizeUserSystem)
            {
                systemList = systemList
                    .OrderByDescending(_ => _.Id == GetClaimId(ClaimType.SystemId))
                    .ThenBy(_ => _.Name);
            }
            return systemList;
        }

        public async Task IncreaseProgramPositionAsync(int programId)
        {
            VerifyPermission(Permission.ManagePrograms);
            await _programRepository.IncreasePositionAsync(programId, GetCurrentSiteId());
        }

        public async Task RemoveBranchAsync(int branchId)
        {
            var branchRemoveTimer = Stopwatch.StartNew();
            try
            {
                VerifyPermission(Permission.ManageSystems);
                if (!await _branchRepository.ValidateBySiteAsync(branchId, GetCurrentSiteId()))
                {
                    throw new GraException("Permission denied - branch belongs to a different site.");
                }
                var branch = await _branchRepository.GetByIdAsync(branchId);
                var inUseFor = await _branchRepository.IsInUseAsync(branchId);
                if (inUseFor > 0)
                {
                    throw new GraException($"Users currently have branch {branch.Name} selected.");
                }
                if (!string.IsNullOrWhiteSpace(branch.Geolocation))
                {
                    await _spatialDistanceRepository.RemoveBranchReferencesAsync(branchId);
                    await _spatialDistanceRepository.InvalidateHeadersAsync(GetCurrentSiteId());
                }
                await _branchRepository.RemoveSaveAsync(GetActiveUserId(), branchId);
                _logger.LogInformation("Removed branch id {BranchId} after {ElapsedMs} ms",
                    branchId,
                    branchRemoveTimer.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error removing branch id {BranchId} after {ElapsedMs} ms {ErrorMessage}",
                    branchId,
                    branchRemoveTimer.ElapsedMilliseconds,
                    ex.Message);

                throw;
            }
            finally
            {
                branchRemoveTimer.Stop();
            }
        }

        public async Task RemoveProgramAsync(int programId)
        {
            VerifyPermission(Permission.ManagePrograms);
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var program = await _programRepository.GetByIdAsync(programId);
            if (program.SiteId != siteId)
            {
                _logger.LogError("User {UserId} cannot remove program {ProgramId} for site {SiteId}.",
                    authId,
                    programId,
                    program.SiteId);
                throw new GraException($"Permission denied - program belongs to site id {program.SiteId}.");
            }
            if (await _programRepository.IsInUseAsync(programId, siteId))
            {
                throw new GraException($"Users currently belong to the program \"{program.Name}\".");
            }
            var allPrograms = await _programRepository.GetAllAsync(siteId);
            var programsEx = allPrograms.Where(_ => _.Id != programId);
            if (programsEx.Any())
            {
                await _userRepository
                    .ChangeDeletedUsersProgramAsync(programId, programsEx.First().Id);
            }

            if (program.ButtonSegmentId.HasValue)
            {
                await _segmentService.RemoveSegmentAsync(program.ButtonSegmentId.Value);
            }

            await _programRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), program);
        }

        public async Task RemoveSystemAsync(int systemId)
        {
            VerifyPermission(Permission.ManageSystems);
            var system = await _systemRepository.GetByIdAsync(systemId);
            if (system.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - system belongs to site id {system.SiteId}.");
            }
            if (await _systemRepository.IsInUseAsync(systemId))
            {
                throw new GraException($"Branches currently belong to system {system.Name}.");
            }
            await _systemRepository.RemoveSaveAsync(GetActiveUserId(), systemId);
        }

        public async Task UpdateAsync(Site site)
        {
            VerifyManagementPermission();
            await _siteRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), site);
            await _siteLookupService.ReloadSiteCacheAsync();
        }

        public async Task UpdateBranchAsync(Branch branch)
        {
            ArgumentNullException.ThrowIfNull(branch);
            VerifyPermission(Permission.ManageSystems);
            var currentBranch = await _branchRepository.GetByIdAsync(branch.Id);
            if (!await _branchRepository.ValidateBySiteAsync(currentBranch.Id, GetCurrentSiteId()))
            {
                throw new GraException("Permission denied - branch belongs to a different site.");
            }

            var invalidateSpatialHeaders = false;
            if (currentBranch.Geolocation != branch.Geolocation)
            {
                invalidateSpatialHeaders = true;
            }

            currentBranch.Address = branch.Address.Trim();
            currentBranch.Geolocation = branch.Geolocation;
            currentBranch.Name = branch.Name.Trim();
            currentBranch.Telephone = branch.Telephone?.Trim();
            currentBranch.Url = branch.Url?.Trim();

            await _branchRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentBranch);

            if (invalidateSpatialHeaders)
            {
                await _spatialDistanceRepository.InvalidateHeadersAsync(GetCurrentSiteId());
            }
        }

        public async Task UpdateProgramAsync(Program program)
        {
            ArgumentNullException.ThrowIfNull(program);
            VerifyPermission(Permission.ManagePrograms);
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var currentProgram = await _programRepository.GetByIdAsync(program.Id);
            if (currentProgram.SiteId != siteId)
            {
                _logger.LogError("User {AuthId} cannot remove program {CurrentProgramId} for site {SiteId}.",
                    authId,
                    currentProgram.Id,
                    currentProgram.SiteId);
                throw new GraException($"Permission denied - program belongs to site id {currentProgram.SiteId}.");
            }

            currentProgram.AchieverPointAmount = program.AchieverPointAmount;
            currentProgram.AgeMaximum = program.AgeMaximum;
            currentProgram.AgeMinimum = program.AgeMinimum;
            currentProgram.AgeRequired = program.AgeRequired;
            currentProgram.AskAge = program.AskAge;
            currentProgram.AskSchool = program.AskSchool;
            currentProgram.ButtonSegmentId = program.ButtonSegmentId;
            currentProgram.DailyLiteracyTipId = program.DailyLiteracyTipId;
            currentProgram.DashboardAlert = program.DashboardAlert?.Trim();
            currentProgram.DashboardAlertType = program.DashboardAlertType;
            currentProgram.JoinBadgeAltText = program.JoinBadgeAltText?.Trim();
            currentProgram.JoinBadgeId ??= program.JoinBadgeId;
            currentProgram.JoinBadgeName = program.JoinBadgeName;
            currentProgram.Name = program.Name;
            currentProgram.PointTranslationId = program.PointTranslationId;
            currentProgram.SchoolRequired = program.SchoolRequired;

            await _programRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentProgram);
        }

        public async Task UpdateProgramListOrderAsync(Collection<int> programOrderList)
        {
            ArgumentNullException.ThrowIfNull(programOrderList);
            VerifyPermission(Permission.ManagePrograms);
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();

            var programs = await _programRepository.GetAllAsync(GetCurrentSiteId());
            var programIdList = programs.Select(_ => _.Id);
            if (programOrderList.All(programIdList.Contains) && programOrderList.Count == programIdList.Count())
            {
                _logger.LogError("User {UserId} cannot update programs {Programs} belonging to another site.",
                    authId,
                    string.Join(", ", programOrderList));
                throw new GraException("Invalid program selection.");
            }
            foreach (var program in programs)
            {
                program.Position = programOrderList.IndexOf(program.Id);
                await _programRepository.UpdateSaveAsync(authId, program);
            }
        }

        public async Task UpdateSiteSettingAsync(int siteId, string key, string value)
        {
            ArgumentNullException.ThrowIfNull(key?.Trim());
            ArgumentNullException.ThrowIfNull(value?.Trim());
            VerifyManagementPermission();

            var currentSiteSettings = await _siteSettingRepository.GetBySiteIdAsync(siteId);
            var currentSetting = currentSiteSettings.FirstOrDefault(_ => _.Key == key);
            if (currentSetting != null)
            {
                currentSetting.Value = value;
                await _siteSettingRepository.UpdateAsync(GetActiveUserId(), currentSetting);
            }
            else
            {
                await _siteSettingRepository.AddAsync(GetActiveUserId(), new SiteSetting
                {
                    CreatedAt = _dateTimeProvider.Now,
                    CreatedBy = GetActiveUserId(),
                    Key = key,
                    Value = value,
                    SiteId = siteId
                });
            }
            await _siteSettingRepository.SaveAsync();
            await _siteLookupService.ReloadSiteCacheAsync();
        }

        public async Task UpdateSiteSettingsAsync(int siteId, IEnumerable<SiteSetting> siteSettings)
        {
            VerifyManagementPermission();
            var currentSiteSettings = await _siteSettingRepository.GetBySiteIdAsync(siteId);
            var siteSettingsToAdd = siteSettings
                .Where(_ => !currentSiteSettings.Select(s => s.Key).Contains(_.Key));
            var siteSettingsToRemove = currentSiteSettings
                .Where(_ => !siteSettings.Select(s => s.Key).Contains(_.Key))
                .Select(_ => _.Id);
            var siteSettingsToUpdate = siteSettings
                .Where(_ => currentSiteSettings.Any(s => s.Key == _.Key && s.Value != _.Value));

            var userId = GetClaimId(ClaimType.UserId);
            await _siteSettingRepository.AddListAsync(userId, siteSettingsToAdd);
            await _siteSettingRepository.RemoveListAsync(userId, siteSettingsToRemove);
            await _siteSettingRepository.UpdateListAsync(userId, siteSettingsToUpdate);
            await _siteSettingRepository.SaveAsync();

            await _siteLookupService.ReloadSiteCacheAsync();
        }

        public async Task UpdateSystemAsync(Model.System system)
        {
            ArgumentNullException.ThrowIfNull(system);
            VerifyPermission(Permission.ManageSystems);
            var currentSystem = await _systemRepository.GetByIdAsync(system.Id);
            if (currentSystem.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - system belongs to site id {currentSystem.SiteId}.");
            }
            currentSystem.Name = system.Name;
            await _systemRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentSystem);
        }

        public async Task<bool> ValidateBranch(int branchId, int systemId)
        {
            return await _branchRepository.ValidateAsync(branchId, systemId);
        }
    }
}
