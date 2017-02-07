using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

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
                .AsNoTracking();

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
                .AnyAsync(_ => _.SiteId == siteId && _.SchoolId == schoolId);
        }

        public async Task<DataWithCount<ICollection<School>>> GetPaginatedListAsync(int siteId,
            int skip,
            int take,
            int? districtId = default(int?),
            int? typeId = default(int?))
        {
            var schoolList = DbSet
                .AsNoTracking();

            if (districtId != null)
            {
                schoolList = schoolList.Where(_ => _.SchoolDistrictId == (int)districtId);
            }

            if (typeId != null)
            {
                schoolList = schoolList.Where(_ => _.SchoolTypeId == (int)typeId);
            }

            return new DataWithCount<ICollection<School>>()
            {
                Data = await schoolList
                    .OrderBy(_ => _.Name)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<School>()
                    .ToListAsync(),
                Count = await schoolList.CountAsync()
            };
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