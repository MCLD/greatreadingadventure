using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class SchoolRepository
        : AuditingRepository<Model.School, School>, ISchoolRepository
    {
        public SchoolRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SchoolRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<School>> GetAllAsync(int siteId,
            int? districtId = default(int?),
            int? typeId = default(int?))
        {
            var schoolList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId);

            if (districtId != null)
            {
                schoolList = schoolList.Where(_ => _.SchoolDistrictId == (int)districtId);
            }

            if (typeId != null)
            {
                schoolList = schoolList.Where(_ => _.SchoolTypeId == (int)typeId);
            }

            return await schoolList
                .OrderBy(_ => _.Name)
                .ProjectTo<School>()
                .ToListAsync();
        }

        public async Task<bool> IsInUseAsync(int siteId, int schoolId)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(_ => _.SiteId == siteId && _.SchoolId == schoolId && _.IsDeleted == false);
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<School>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<School>()
                .ToListAsync();
        }

        private IQueryable<Model.School> ApplyFilters(BaseFilter filter)
        {
            var schoolList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                schoolList = schoolList.Where(_ => _.Name.Contains(filter.Search));
            }

            return schoolList;
        }

        public async Task<bool> ValidateAsync(int schoolId, int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == schoolId && _.SiteId == siteId)
                .AnyAsync();
        }

        public async Task<bool> AnyPrivateSchoolsAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.SchoolDistrict.IsPrivate)
                .AnyAsync();
        }
        
        public async Task<List<School>> GetPrivateSchoolListAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Join(_context.SchoolDistricts,
                    s => s.SchoolDistrictId,
                    sd => sd.Id,
                    (s, sd) => new { s, sd})
                .Where(_ => _.s.SiteId == siteId && _.sd.IsPrivate == true)
                .Select(_ => _.s)
                .OrderBy(_ => _.Name)
                .ProjectTo<School>()
                .ToListAsync();
        }

        public async Task<bool> AnyCharterSchoolsAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.SchoolDistrict.IsCharter)
                .AnyAsync();
        }

        public async Task<List<School>> GetCharterSchoolListAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Join(_context.SchoolDistricts,
                    s => s.SchoolDistrictId,
                    sd => sd.Id,
                    (s, sd) => new { s, sd })
                .Where(_ => _.s.SiteId == siteId && _.sd.IsCharter == true)
                .Select(_ => _.s)
                .OrderBy(_ => _.Name)
                .ProjectTo<School>()
                .ToListAsync();
        }
    }
}