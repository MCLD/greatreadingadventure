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
    public class SchoolTypeRepository
        : AuditingRepository<Model.SchoolType, Domain.Model.SchoolType>, ISchoolTypeRepository
    {
        public SchoolTypeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SchoolTypeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<SchoolType>> GetAllAsync(int siteId,
             int? districtId = default(int?))
        {
            if (districtId == null)
            {
                return await DbSet
                    .AsNoTracking()
                    .Where(_ => _.SiteId == siteId)
                    .OrderBy(_ => _.Name)
                    .ProjectTo<SchoolType>()
                    .ToListAsync();
            }
            else
            {
                return await _context.Schools
                    .AsNoTracking()
                    .Where(_ => _.SiteId == siteId && _.SchoolDistrictId == (int)districtId)
                    .Select(_ => _.SchoolType)
                    .Distinct()
                    .OrderBy(_ => _.Name)
                    .ProjectTo<SchoolType>()
                    .ToListAsync();
            }
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<SchoolType>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<SchoolType>()
                .ToListAsync();
        }

        private IQueryable<Model.SchoolType> ApplyFilters(BaseFilter filter)
        {
            var schoolTypeList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                schoolTypeList = schoolTypeList.Where(_ => _.Name.Contains(filter.Search));
            }

            return schoolTypeList;
        }
    }
}
