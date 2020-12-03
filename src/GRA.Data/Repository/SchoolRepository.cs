using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            int? districtId = default(int?))
        {
            var schoolList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId);

            if (districtId != null)
            {
                return await schoolList
                    .Where(_ => _.SchoolDistrictId == (int)districtId)
                    .OrderBy(_ => _.Name)
                    .ProjectTo<School>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
            else
            {
                return await schoolList.Select(_ => new School
                {
                    Id = _.Id,
                    Name = _.Name + " (" + _.SchoolDistrict.Name + ")",
                    SchoolDistrictId = _.SchoolDistrictId
                })
                .OrderBy(_ => _.Name)
                .ToListAsync();
            }
        }

        public async Task<bool> IsInUseAsync(int siteId, int schoolId)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(_ => _.SiteId == siteId && _.SchoolId == schoolId && !_.IsDeleted);
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
                .ProjectTo<School>(_mapper.ConfigurationProvider)
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
    }
}