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
    public class SchoolDistrictRepository
        : AuditingRepository<Model.SchoolDistrict, SchoolDistrict>, ISchoolDistrictRepository
    {
        public SchoolDistrictRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SchoolDistrictRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<SchoolDistrict>> GetAllAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .OrderBy(_ => _.Name)
                .ProjectTo<SchoolDistrict>()
                .ToListAsync();
        }
    }
}