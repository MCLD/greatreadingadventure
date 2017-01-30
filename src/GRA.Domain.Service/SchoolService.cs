using GRA.Domain.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;

namespace GRA.Domain.Service
{
    public class SchoolService : BaseUserService<SchoolService>
    {
        private readonly IEnteredSchoolRepository _enteredSchoolRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISchoolTypeRepository _schoolTypeRepository;
        private readonly ISchoolDistrictRepository _schoolDistrictRepository;
        private readonly IUserRepository _userRepository;
        public SchoolService(ILogger<SchoolService> logger,
            IUserContextProvider userContextProvider,
            IEnteredSchoolRepository enteredSchoolRepository,
            ISchoolDistrictRepository schoolDistrictRepository,
            ISchoolRepository schoolRepository,
            ISchoolTypeRepository schoolTypeRepository,
            IUserRepository userRepository) : base(logger, userContextProvider)
        {
            _enteredSchoolRepository = Require.IsNotNull(enteredSchoolRepository,
                nameof(enteredSchoolRepository));
            _schoolDistrictRepository = Require.IsNotNull(schoolDistrictRepository,
                nameof(schoolDistrictRepository));
            _schoolRepository = Require.IsNotNull(schoolRepository, nameof(schoolRepository));
            _schoolTypeRepository = Require.IsNotNull(schoolTypeRepository,
                nameof(schoolTypeRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
        }

        public async Task<ICollection<SchoolDistrict>> GetDistrictsAsync()
        {
            return await _schoolDistrictRepository.GetAllAsync(GetCurrentSiteId());
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

        public async Task<EnteredSchool> AddEnteredSchool(string schoolName, int districtId)
        {
            var district = await _schoolDistrictRepository.GetByIdAsync(districtId);
            if (district == null)
            {
                throw new GraException("Please select a school district.");
            }
            var enteredSchool = new EnteredSchool
            {
                SiteId = district.SiteId,
                Name = schoolName,
                SchoolDistrictId = districtId
            };
            return await _enteredSchoolRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                enteredSchool);
        }

        public async Task<School> AddEnteredSchoolToList(int enteredSchoolId,
            string schoolName,
            int schoolDistrictId,
            int schoolTypeId)
        {
            VerifyPermission(Permission.ManageSchools);
            var enteredSchool = await _enteredSchoolRepository.GetByIdAsync(enteredSchoolId);
            var newSchool = await _schoolRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                new School
                {
                    Name = schoolName,
                    SchoolDistrictId = schoolDistrictId,
                    SiteId = enteredSchool.SiteId,
                    SchoolTypeId = schoolTypeId
                });
            await _enteredSchoolRepository.ConvertSchoolAsync(GetClaimId(ClaimType.UserId),
                enteredSchoolId,
                newSchool.Id);
            return newSchool;
        }

        public async Task<SchoolDistrict> AddDistrict(string districtName)
        {
            VerifyPermission(Permission.ManageSchools);
            return await _schoolDistrictRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                new SchoolDistrict
                {
                    SiteId = GetCurrentSiteId(),
                    Name = districtName
                });
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

        public async Task<School> AddSchool(string schoolName, int districtId, int typeId)
        {
            VerifyPermission(Permission.ManageSchools);
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
                throw new GraException($"Permission denied - school belongs site id {school.SiteId}.");
            }
            if (await _schoolRepository.IsInUseAsync(GetCurrentSiteId(), schoolId))
            {
                throw new GraException($"Users currently have school id {schoolId} selected.");
            }
            await _schoolRepository.RemoveSaveAsync(GetActiveUserId(), schoolId);
        }

        public async Task<DataWithCount<ICollection<School>>> GetPaginatedListAsync(int skip,
            int take,
            int? districtId = default(int?),
            int? typeId = default(int?))
        {
            VerifyPermission(Permission.ManageSchools);
            return await _schoolRepository
                .GetPaginatedListAsync(GetCurrentSiteId(), skip, take, districtId, typeId);
        }

        public async Task UpdateSchoolAsync(School school)
        {
            VerifyPermission(Permission.ManageSchools);
            var currentSchool = await _schoolRepository.GetByIdAsync(school.Id);
            if (currentSchool.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - school belongs site id {currentSchool.SiteId}.");
            }
            currentSchool.Name = school.Name;
            currentSchool.SchoolDistrictId = school.SchoolDistrictId;
            currentSchool.SchoolTypeId = school.SchoolTypeId;
            await _schoolRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentSchool);
        }

        public async Task<DataWithCount<ICollection<SchoolDistrict>>> GetPaginatedDistrictListAsync(int skip,
            int take)
        {
            VerifyPermission(Permission.ManageSchools);
            return await _schoolDistrictRepository
                .GetPaginatedListAsync(GetCurrentSiteId(), skip, take);
        }

        public async Task UpdateDistrictAsync(SchoolDistrict district)
        {
            VerifyPermission(Permission.ManageSchools);
            var currentDistrict = await _schoolDistrictRepository.GetByIdAsync(district.Id);
            if (currentDistrict.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - district belongs site id {currentDistrict.SiteId}.");
            }
            currentDistrict.Name = district.Name;
            await _schoolDistrictRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), currentDistrict);
        }

        public async Task<DataWithCount<ICollection<SchoolType>>> GetPaginatedTypeListAsync(int skip,
            int take)
        {
            VerifyPermission(Permission.ManageSchools);
            return await _schoolTypeRepository
                .GetPaginatedListAsync(GetCurrentSiteId(), skip, take);
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

        public async Task<DataWithCount<ICollection<EnteredSchool>>> GetPaginatedEnteredListAsync(int skip,
            int take)
        {
            VerifyPermission(Permission.ManageSchools);
            return await _enteredSchoolRepository
                .GetPaginatedListAsync(GetCurrentSiteId(), skip, take);
        }
    }
}
