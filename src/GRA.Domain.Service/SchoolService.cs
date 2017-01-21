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
            var schools = await _schoolRepository.GetAllAsync(GetCurrentSiteId(), districtId);
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
            var schools = await _schoolRepository.GetAllAsync(GetCurrentSiteId(), typeId);
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
    }
}
