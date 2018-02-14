using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class SchoolService : BaseUserService<SchoolService>
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISchoolTypeRepository _schoolTypeRepository;
        private readonly ISchoolDistrictRepository _schoolDistrictRepository;
        private readonly IUserRepository _userRepository;
        public SchoolService(ILogger<SchoolService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            ISchoolDistrictRepository schoolDistrictRepository,
            ISchoolRepository schoolRepository,
            ISchoolTypeRepository schoolTypeRepository,
            IUserRepository userRepository) : base(logger, dateTimeProvider, userContextProvider)
        {
            _schoolDistrictRepository = Require.IsNotNull(schoolDistrictRepository,
                nameof(schoolDistrictRepository));
            _schoolRepository = Require.IsNotNull(schoolRepository, nameof(schoolRepository));
            _schoolTypeRepository = Require.IsNotNull(schoolTypeRepository,
                nameof(schoolTypeRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
        }

        public async Task<ICollection<SchoolDistrict>> GetDistrictsAsync(
            bool excludeUserUnselectable = false)
        {
            return await _schoolDistrictRepository.GetAllAsync(GetCurrentSiteId(),
                excludeUserUnselectable);
        }

        public async Task<ICollection<SchoolType>> GetTypesAsync(int? districtId = default(int?))
        {
            return await _schoolTypeRepository.GetAllAsync(GetCurrentSiteId(), districtId);
        }

        public async Task<ICollection<School>> GetSchoolsAsync(int? districtId = default(int?),
            int? typeId = default(int?))
        {
            return await _schoolRepository.GetAllAsync(GetCurrentSiteId(), districtId, typeId);
        }

        public async Task<SchoolDetails> GetSchoolDetailsAsync(int schoolId)
        {
            var school = await _schoolRepository.GetByIdAsync(schoolId);
            return new SchoolDetails()
            {
                School = school,
                Schools = await _schoolRepository.GetAllAsync(GetCurrentSiteId(),
                    school.SchoolDistrictId,
                    school.SchoolTypeId),
                SchoolDistrictId = school.SchoolDistrictId,
                SchoolTypeId = school.SchoolTypeId
            };
        }

        public async Task<SchoolDistrict> AddDistrict(SchoolDistrict district)
        {
            VerifyPermission(Permission.ManageSchools);
            district.Name = district.Name.Trim();
            district.SiteId = GetCurrentSiteId();
            return await _schoolDistrictRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                district);
        }

        public async Task<SchoolType> AddSchoolType(string typeName)
        {
            VerifyPermission(Permission.ManageSchools);
            return await _schoolTypeRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                new SchoolType
                {
                    SiteId = GetCurrentSiteId(),
                    Name = typeName
                });
        }

        public async Task<School> AddSchool(string schoolName, int districtId, int? typeId)
        {
            VerifyPermission(Permission.ManageSchools);
            var district = await _schoolDistrictRepository.GetByIdAsync(districtId);
            if (district.IsCharter || district.IsPrivate)
            {
                typeId = null;
            }
            else if (typeId.HasValue == false)
            {
                throw new GraException("No school type selected.");
            }
            return await _schoolRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                new School
                {
                    SiteId = GetCurrentSiteId(),
                    Name = schoolName,
                    SchoolDistrictId = districtId,
                    SchoolTypeId = typeId
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
            var schools = await _schoolRepository.GetAllAsync(GetCurrentSiteId(), districtId: districtId);
            if (schools.Count > 0)
            {
                throw new GraException($"Could not delete district - there are {schools.Count} associated schools");
            }
            await _schoolDistrictRepository.RemoveSaveAsync(GetActiveUserId(), districtId);
        }

        public async Task RemoveType(int typeId)
        {
            VerifyPermission(Permission.ManageSchools);
            var type = await _schoolTypeRepository.GetByIdAsync(typeId);
            if (type.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - district belongs to site id {type.SiteId}.");
            }
            var schools = await _schoolRepository.GetAllAsync(GetCurrentSiteId(), typeId: typeId);
            if (schools.Count > 0)
            {
                throw new GraException($"Could not delete school type - there are {schools.Count} associated schools");
            }
            await _schoolTypeRepository.RemoveSaveAsync(GetActiveUserId(), typeId);
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

        public async Task<DataWithCount<ICollection<School>>> GetPaginatedListAsync(
            BaseFilter filter)
        {
            VerifyPermission(Permission.ManageSchools);
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
            var currentSchool = await _schoolRepository.GetByIdAsync(school.Id);
            if (currentSchool.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - school belongs to site id {currentSchool.SiteId}.");
            }
            currentSchool.Name = school.Name;
            currentSchool.SchoolDistrictId = school.SchoolDistrictId;

            var district = await _schoolDistrictRepository.GetByIdAsync(school.SchoolDistrictId);
            if (district.IsCharter || district.IsPrivate)
            {
                currentSchool.SchoolTypeId = null;
            }
            else if (school.SchoolTypeId.HasValue == false)
            {
                throw new GraException("No school type selected.");
            }
            else
            {
                currentSchool.SchoolTypeId = school.SchoolTypeId;
            }

            await _schoolRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentSchool);
        }

        public async Task<DataWithCount<ICollection<SchoolDistrict>>> GetPaginatedDistrictListAsync(
            BaseFilter filter)
        {
            VerifyPermission(Permission.ManageSchools);
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<SchoolDistrict>>
            {
                Data = await _schoolDistrictRepository.PageAsync(filter),
                Count = await _schoolDistrictRepository.CountAsync(filter)
            };
        }

        public async Task UpdateDistrictAsync(SchoolDistrict district)
        {
            VerifyPermission(Permission.ManageSchools);
            var currentDistrict = await _schoolDistrictRepository.GetByIdAsync(district.Id);
            if (currentDistrict.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - district belongs site id {currentDistrict.SiteId}.");
            }
            currentDistrict.IsCharter = district.IsCharter;
            currentDistrict.IsPrivate = district.IsPrivate;
            currentDistrict.Name = district.Name.Trim();
            await _schoolDistrictRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentDistrict);
        }

        public async Task<DataWithCount<ICollection<SchoolType>>> GetPaginatedTypeListAsync(
            BaseFilter filter)
        {
            VerifyPermission(Permission.ManageSchools);
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<SchoolType>>
            {
                Data = await _schoolTypeRepository.PageAsync(filter),
                Count = await _schoolTypeRepository.CountAsync(filter)
            };
        }

        public async Task UpdateTypeAsync(SchoolType type)
        {
            VerifyPermission(Permission.ManageSchools);
            var currentType = await _schoolTypeRepository.GetByIdAsync(type.Id);
            if (currentType.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - type belongs site id {currentType.SiteId}.");
            }
            currentType.Name = type.Name;
            await _schoolTypeRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentType);
        }

        public async Task<bool> AnyPrivateSchoolsAsync()
        {
            return await _schoolRepository.AnyPrivateSchoolsAsync(GetCurrentSiteId());
        }

        public async Task<List<School>> GetPrivateSchoolListAsync()
        {
            return await _schoolRepository.GetPrivateSchoolListAsync(GetCurrentSiteId());
        }

        public async Task<bool> AnyCharterSchoolsAsync()
        {
            return await _schoolRepository.AnyCharterSchoolsAsync(GetCurrentSiteId());
        }

        public async Task<List<School>> GetCharterSchoolListAsync()
        {
            return await _schoolRepository.GetCharterSchoolListAsync(GetCurrentSiteId());
        }

        public async Task<SchoolDistrict> GetDistrictByIdAsync(int schoolDistrictId)
        {
            return await _schoolDistrictRepository.GetByIdAsync(schoolDistrictId);
        }
    }
}
