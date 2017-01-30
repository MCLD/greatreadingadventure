using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

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
                    .OrderBy(_ => _.Name)
                    .ProjectTo<SchoolType>()
                    .ToListAsync();
            } else
            {
                return await _context.Schools
                    .AsNoTracking()
                    .Where(_ => _.SchoolDistrictId == (int)districtId)
                    .Select(_ => _.SchoolType)
                    .OrderBy(_=> _.Name)
                    .ProjectTo<SchoolType>()
                    .ToListAsync();
            }
        }

        public async Task<DataWithCount<ICollection<SchoolType>>> GetPaginatedListAsync(int siteId,
            int skip,
            int take)
        {
            var typeList = DbSet
                .AsNoTracking();

            return new DataWithCount<ICollection<SchoolType>>()
            {
                Data = await typeList
                    .OrderBy(_ => _.Name)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<SchoolType>()
                    .ToListAsync(),
                Count = await typeList.CountAsync()
            };
        }
    }
}
