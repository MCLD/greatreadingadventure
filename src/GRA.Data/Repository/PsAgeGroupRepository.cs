using System.Collections.Generic;
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
    public class PsAgeGroupRepository 
        : AuditingRepository<Model.PsAgeGroup, Domain.Model.PsAgeGroup>, IPsAgeGroupRepository
    {
        public PsAgeGroupRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsAgeGroupRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<PsAgeGroup>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .ProjectTo<PsAgeGroup>()
                .ToListAsync();
        }

        public async Task<DataWithCount<ICollection<PsAgeGroup>>> GetPaginatedListAsync(
            BaseFilter filter)
        {
            var ageGroups = DbSet.AsNoTracking();

            var count = await ageGroups.CountAsync();

            var ageGroupList = await ageGroups
                .ApplyPagination(filter)
                .ProjectTo<PsAgeGroup>()
                .ToListAsync();

            return new DataWithCount<ICollection<PsAgeGroup>>
            {
                Data = ageGroupList,
                Count = count
            };
        }
    }
}
