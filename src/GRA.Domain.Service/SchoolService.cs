using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class SchoolService : BaseUserService<SchoolService>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISchoolDistrictRepository _schoolDistrictRepository;

        public SchoolService(ILogger<SchoolService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            ISchoolDistrictRepository schoolDistrictRepository,
            ISchoolRepository schoolRepository) : base(logger, dateTimeProvider, userContextProvider)
        {
            _schoolDistrictRepository = schoolDistrictRepository
                ?? throw new ArgumentNullException(nameof(schoolDistrictRepository));
            _schoolRepository = schoolRepository
                ?? throw new ArgumentNullException(nameof(schoolRepository));
        }

        public async Task<ICollection<SchoolDistrict>> GetDistrictsAsync()
        {
            return await _schoolDistrictRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<ICollection<School>> GetSchoolsAsync(int? districtId = default)
        {
            return await _schoolRepository.GetAllAsync(GetCurrentSiteId(), districtId);
        }

        public async Task<SchoolDetails> GetSchoolDetailsAsync(int schoolId)
        {
            var school = await _schoolRepository.GetByIdAsync(schoolId);
            return new SchoolDetails()
            {
                School = school,
                Schools = await _schoolRepository.GetAllAsync(GetCurrentSiteId(),
                    school.SchoolDistrictId),
                SchoolDistrictId = school.SchoolDistrictId,
            };
        }

        public async Task<SchoolDistrict> AddDistrict(SchoolDistrict district)
        {
            VerifyPermission(Permission.ManageSchools);
            if (district == null)
            {
                throw new GraException("Cannot add empty district.");
            }
            district.Name = district.Name.Trim();
            district.SiteId = GetCurrentSiteId();
            return await _schoolDistrictRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                district);
        }

        public async Task<School> AddSchool(string schoolName, int districtId)
        {
            VerifyPermission(Permission.ManageSchools);

            return await _schoolRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                new School
                {
                    SiteId = GetCurrentSiteId(),
                    Name = schoolName,
                    SchoolDistrictId = districtId
                });
        }

        public async Task RemoveDistrict(int districtId)
        {
            VerifyPermission(Permission.ManageSchools);
            var district = await _schoolDistrictRepository.GetByIdAsync(districtId);
            if (district.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - district belongs to site id {district.SiteId}");
            }
            var schools = await _schoolRepository.GetAllAsync(GetCurrentSiteId(), districtId);
            if (schools.Count > 0)
            {
                throw new GraException($"Could not delete district - there are {schools.Count} associated schools");
            }
            await _schoolDistrictRepository.RemoveSaveAsync(GetActiveUserId(), districtId);
        }

        public async Task RemoveSchool(int schoolId)
        {
            VerifyPermission(Permission.ManageSchools);
            var school = await _schoolRepository.GetByIdAsync(schoolId);
            if (school.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - school belongs to site id {school.SiteId}.");
            }
            if (await _schoolRepository.IsInUseAsync(GetCurrentSiteId(), schoolId))
            {
                throw new GraException($"Users currently have school id {schoolId} selected.");
            }
            await _schoolRepository.RemoveSaveAsync(GetActiveUserId(), schoolId);
        }

        public async Task<School> GetByIdAsync(int id)
        {
            return await _schoolRepository.GetByIdAsync(id);
        }

        public async Task<DataWithCount<ICollection<School>>> GetPaginatedListAsync(
            BaseFilter filter)
        {
            VerifyPermission(Permission.ManageSchools);
            if (filter == null)
            {
                filter = new BaseFilter();
            }
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<School>>
            {
                Data = await _schoolRepository.PageAsync(filter),
                Count = await _schoolRepository.CountAsync(filter)
            };
        }

        public async Task UpdateSchoolAsync(School school)
        {
            VerifyPermission(Permission.ManageSchools);
            if (school == null)
            {
                throw new GraException("Cannot update empty school.");
            }
            var currentSchool = await _schoolRepository.GetByIdAsync(school.Id);
            if (currentSchool.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - school belongs to site id {currentSchool.SiteId}.");
            }
            currentSchool.Name = school.Name;
            currentSchool.SchoolDistrictId = school.SchoolDistrictId;

            await _schoolRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentSchool);
        }

        public async Task<DataWithCount<ICollection<SchoolDistrict>>> GetPaginatedDistrictListAsync(
            BaseFilter filter)
        {
            VerifyPermission(Permission.ManageSchools);
            if (filter == null)
            {
                filter = new BaseFilter();
            }
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<SchoolDistrict>>
            {
                Data = await _schoolDistrictRepository.PageAsync(filter),
                Count = await _schoolDistrictRepository.CountAsync(filter)
            };
        }

        public async Task UpdateDistrictAsync(SchoolDistrict district)
        {
            if (district == null)
            {
                throw new GraException("Cannot update empty district.");
            }
            VerifyPermission(Permission.ManageSchools);
            var currentDistrict = await _schoolDistrictRepository.GetByIdAsync(district.Id);
            if (currentDistrict.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - district belongs site id {currentDistrict.SiteId}.");
            }

            currentDistrict.Name = district.Name.Trim();
            await _schoolDistrictRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentDistrict);
        }

        public async Task<SchoolDistrict> GetDistrictByIdAsync(int schoolDistrictId)
        {
            return await _schoolDistrictRepository.GetByIdAsync(schoolDistrictId);
        }

        public async Task<IList<SchoolImportExport>> GetForExportAsync()
        {
            return await _schoolRepository.GetForExportAsync();
        }
    }
}
