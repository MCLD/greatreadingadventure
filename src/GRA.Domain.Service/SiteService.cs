using System;
using System.Collections.Generic;
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
        private readonly ISiteRepository _siteRepository;
        private readonly ISiteSettingRepository _siteSettingRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly SiteLookupService _siteLookupService;

        public SiteService(ILogger<SiteService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            ISiteRepository siteRepository,
            ISiteSettingRepository siteSettingRepository,
            ISystemRepository systemRepository,
            SiteLookupService siteLookupService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageSites);
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _siteSettingRepository = siteSettingRepository
                ?? throw new ArgumentNullException(nameof(siteSettingRepository));
            _systemRepository = Require.IsNotNull(systemRepository, nameof(systemRepository));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentException(nameof(siteLookupService));
        }

        public async Task<DataWithCount<IEnumerable<Site>>> GetPaginatedListAsync(BaseFilter filter)
        {
            VerifyManagementPermission();
            return await _siteRepository.PageAsync(filter);
        }

        public async Task UpdateAsync(Site site)
        {
            VerifyManagementPermission();
            await _siteRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), site);
            await _siteLookupService.ReloadSiteCacheAsync();
        }

        public async Task UpdateSiteSettingsAsync(int siteId, IEnumerable<SiteSetting> siteSettings)
        {
            VerifyManagementPermission();
            var currentSiteSettings = await _siteSettingRepository.GetBySiteIdAsync(siteId);
            var siteSettingsToAdd = siteSettings
                .Where(_ => currentSiteSettings.Select(s => s.Key).Contains(_.Key) == false);
            var siteSettingsToRemove = currentSiteSettings
                .Where(_ => siteSettings.Select(s => s.Key).Contains(_.Key) == false)
                .Select(_ => _.Id);
            var siteSettingsToUpdate = siteSettings
                .Where(_ => currentSiteSettings
                    .Where(s => s.Key == _.Key)
                    .Any(s => s.Value != _.Value));

            var userId = GetClaimId(ClaimType.UserId);
            await _siteSettingRepository.AddListAsync(userId, siteSettingsToAdd);
            await _siteSettingRepository.RemoveListAsync(userId, siteSettingsToRemove);
            await _siteSettingRepository.UpdateListAsync(userId, siteSettingsToUpdate);
            await _siteSettingRepository.SaveAsync();

            await _siteLookupService.ReloadSiteCacheAsync();
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

        public async Task<DataWithCount<ICollection<Model.System>>> GetPaginatedSystemListAsync(
            BaseFilter filter)
        {
            VerifyPermission(Permission.ManageSystems);
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<Model.System>>
            {
                Data = await _systemRepository.PageAsync(filter),
                Count = await _systemRepository.CountAsync(filter)
            };
        }

        public async Task<Model.System> AddSystemAsync(Model.System system)
        {
            VerifyPermission(Permission.ManageSystems);
            system.SiteId = GetCurrentSiteId();
            return await _systemRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), system);
        }

        public async Task UpdateSystemAsync(Model.System system)
        {
            VerifyPermission(Permission.ManageSystems);
            var currentSystem = await _systemRepository.GetByIdAsync(system.Id);
            if (currentSystem.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - system belongs to site id {currentSystem.SiteId}.");
            }
            currentSystem.Name = system.Name;
            await _systemRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentSystem);
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

        public async Task<DataWithCount<ICollection<Branch>>> GetPaginatedBranchListAsync(
            BaseFilter filter)
        {
            VerifyPermission(Permission.ManageSystems);
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<Branch>>
            {
                Data = await _branchRepository.PageAsync(filter),
                Count = await _branchRepository.CountAsync(filter)
            };
        }

        public async Task<Branch> AddBranchAsync(Branch branch)
        {
            VerifyPermission(Permission.ManageSystems);
            return await _branchRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), branch);
        }

        public async Task UpdateBranchAsync(Branch branch)
        {
            VerifyPermission(Permission.ManageSystems);
            var currentBranch = await _branchRepository.GetByIdAsync(branch.Id);
            if (await _branchRepository.ValidateBySiteAsync(currentBranch.Id, GetCurrentSiteId()) == false)
            {
                throw new GraException($"Permission denied - branch belongs to a different site.");
            }

            currentBranch.Address = branch.Address;
            currentBranch.Name = branch.Name;
            currentBranch.Telephone = branch.Telephone;
            currentBranch.Url = branch.Url;
            await _branchRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentBranch);
        }

        public async Task RemoveBranchAsync(int branchId)
        {
            VerifyPermission(Permission.ManageSystems);
            if (await _branchRepository.ValidateBySiteAsync(branchId, GetCurrentSiteId()) == false)
            {
                throw new GraException($"Permission denied - branch belongs to a different site.");
            }
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (await _branchRepository.IsInUseAsync(branchId))
            {
                throw new GraException($"Users currently have branch {branch.Name} selected.");
            }
            await _branchRepository.RemoveSaveAsync(GetActiveUserId(), branchId);
        }

        public async Task<Branch> GetBranchByIdAsync(int branchId)
        {
            return await _branchRepository.GetByIdAsync(branchId);
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

        public async Task<bool> ValidateBranch(int branchId, int systemId)
        {
            return await _branchRepository.ValidateAsync(branchId, systemId);
        }

        public async Task<IEnumerable<Program>> GetProgramList()
        {
            return await _programRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<DataWithCount<ICollection<Program>>> GetPaginatedProgramListAsync(
        BaseFilter filter)
        {
            VerifyPermission(Permission.ManagePrograms);
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<Program>>
            {
                Data = await _programRepository.PageAsync(filter),
                Count = await _programRepository.CountAsync(filter)
            };
        }

        public async Task<Program> AddProgramAsync(Program program)
        {
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

        public async Task UpdateProgramAsync(Program program)
        {
            VerifyPermission(Permission.ManagePrograms);
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var currentProgram = await _programRepository.GetByIdAsync(program.Id);
            if (currentProgram.SiteId != siteId)
            {
                _logger.LogError($"User {authId} cannot remove program {currentProgram.Id} for site {currentProgram.SiteId}.");
                throw new GraException($"Permission denied - program belongs to site id {currentProgram.SiteId}.");
            }

            currentProgram.AchieverPointAmount = program.AchieverPointAmount;
            currentProgram.AgeMaximum = program.AgeMaximum;
            currentProgram.AgeMinimum = program.AgeMinimum;
            currentProgram.AgeRequired = program.AgeRequired;
            currentProgram.AskAge = program.AskAge;
            currentProgram.AskSchool = program.AskSchool;
            currentProgram.DailyLiteracyTipId = program.DailyLiteracyTipId;
            currentProgram.JoinBadgeId = currentProgram.JoinBadgeId ?? program.JoinBadgeId;
            currentProgram.JoinBadgeName = program.JoinBadgeName;
            currentProgram.Name = program.Name;
            currentProgram.PointTranslationId = program.PointTranslationId;
            currentProgram.SchoolRequired = program.SchoolRequired;

            await _programRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentProgram);
        }

        public async Task RemoveProgramAsync(int programId)
        {
            VerifyPermission(Permission.ManagePrograms);
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            var program = await _programRepository.GetByIdAsync(programId);
            if (program.SiteId != siteId)
            {
                _logger.LogError($"User {authId} cannot remove program {programId} for site {program.SiteId}.");
                throw new GraException($"Permission denied - program belongs to site id {program.SiteId}.");
            }
            if (await _programRepository.IsInUseAsync(programId, siteId))
            {
                throw new GraException($"Users currently belong to the program \"{program.Name}\".");
            }
            await _programRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), program);
        }

        public async Task UpdateProgramListOrderAsync(List<int> programOrderList)
        {
            VerifyPermission(Permission.ManagePrograms);
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();

            var programs = await _programRepository.GetAllAsync(GetCurrentSiteId());
            var programIdList = programs.Select(_ => _.Id);
            if (programOrderList.All(programIdList.Contains) && programOrderList.Count == programIdList.Count())
            {
                _logger.LogError($"User {authId} cannot update programs {string.Join(", ", programOrderList)} belonging to another site.");
                throw new GraException("Invalid program selection.");
            }
            foreach (var program in programs)
            {
                program.Position = programOrderList.IndexOf(program.Id);
                await _programRepository.UpdateSaveAsync(authId, program);
            }
        }

        public async Task DecreaseProgramPositionAsync(int programId)
        {
            VerifyPermission(Permission.ManagePrograms);
            await _programRepository.DecreasePositionAsync(programId, GetCurrentSiteId());
        }

        public async Task IncreaseProgramPositionAsync(int programId)
        {
            VerifyPermission(Permission.ManagePrograms);
            await _programRepository.IncreasePositionAsync(programId, GetCurrentSiteId());
        }

        public async Task<Program> GetProgramByIdAsync(int programId)
        {
            return await _programRepository.GetByIdAsync(programId);
        }

        public async Task<string> GetBaseUrl(string scheme, string host)
        {
            var site = await _siteRepository.GetByIdAsync(GetCurrentSiteId());
            if (site.IsHttpsForced)
            {
                scheme = "https";
            }
            if (site.IsDefault)
            {
                return $"{scheme}://{host}";
            }
            else
            {
                return $"{scheme}://{host}/{site.Path}";
            }
        }

        public async Task<string> GetWsUrl(string httpScheme, string host)
        {
            var site = await _siteRepository.GetByIdAsync(GetCurrentSiteId());
            if (site.IsHttpsForced)
            {
                httpScheme = "https";
            }
            string wsScheme = httpScheme == "https" ? "wss" : "ws";
            if (site.IsDefault)
            {
                return $"{wsScheme}://{host}";
            }
            else
            {
                return $"{wsScheme}://{host}/{site.Path}";
            }
        }

        public async Task<byte[]> GetIcsFile(string siteUrl)
        {
            var site = await _siteRepository.GetByIdAsync(GetCurrentSiteId());
            if (site.RegistrationOpens == null)
            {
                _logger.LogError($"Can't generate calendar file becuase RegistrationOpens date is not set for site {site.Id}");
                throw new GraException("Unable to generate calendar file.");
            }
            string project = new string(site.Name.Where(_ => char.IsLetterOrDigit(_)).ToArray());

            // if the start time is midnight then let's bump the calendar appointment to 0700
            var localStartTime = (DateTime)site.RegistrationOpens;
            if (localStartTime.Hour + localStartTime.Minute + localStartTime.Second == 0)
            {
                localStartTime = localStartTime.AddHours(7);
            }

            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine($"PRODID:-//{project}//EN");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"SUMMARY; LANGUAGE = en - us:{site.Name}");
            sb.AppendLine("CLASS:PUBLIC");
            sb.AppendLine($"CREATED:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
            sb.AppendLine($"DESCRIPTION:Remember to join {site.Name}: {siteUrl}");
            sb.AppendLine($"DTSTART:{localStartTime:yyyyMMddTHHmmss}");
            sb.AppendLine($"DTEND:{localStartTime:yyyyMMddTHHmmss}");
            sb.AppendLine("SEQUENCE:0");
            sb.AppendLine($"UID:{Guid.NewGuid()}");
            sb.AppendLine($"LOCATION:{siteUrl}");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}