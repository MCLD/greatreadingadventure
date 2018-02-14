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
    public class SchoolDistrictRepository
        : AuditingRepository<Model.SchoolDistrict, SchoolDistrict>, ISchoolDistrictRepository
    {
        public SchoolDistrictRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SchoolDistrictRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<SchoolDistrict>> GetAllAsync(int siteId,
            bool excludeUserUnselectable)
        {
            var districtList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId);

            if (excludeUserUnselectable)
            {
                districtList = districtList
                    .Where(_ => _.IsPrivate == false && _.IsCharter == false);
            }

            return await districtList
                .OrderByDescending(_ => _.IsPrivate)
                    .ThenByDescending(_ => _.IsCharter)
                    .ThenBy(_ => _.Name)
                .ProjectTo<SchoolDistrict>()
                .ToListAsync();
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<SchoolDistrict>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderByDescending(_ => _.IsPrivate)
                    .ThenByDescending(_ => _.IsCharter)
                    .ThenBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<SchoolDistrict>()
                .ToListAsync();
        }

        private IQueryable<Model.SchoolDistrict> ApplyFilters(BaseFilter filter)
        {
            var schoolDistrictList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                schoolDistrictList = schoolDistrictList.Where(_ => _.Name.Contains(filter.Search));
            }

            return schoolDistrictList;
        }
    }
}