using GRA.Domain.Repository;
using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using GRA.Domain.Service.Abstract;
using System.Text;
using System.Linq;
using System;
using System.Collections;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Service
{
    public class SiteService : BaseUserService<SiteService>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISystemRepository _systemRepository;

        public SiteService(ILogger<SiteService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            ISiteRepository siteRepository,
            ISystemRepository systemRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _systemRepository = Require.IsNotNull(systemRepository, nameof(systemRepository));
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