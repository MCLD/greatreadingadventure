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
    public class PsKitRepository
        : AuditingRepository<Model.PsKit, Domain.Model.PsKit>, IPsKitRepository
    {
        public PsKitRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsKitRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<PsKit>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .ProjectTo<PsKit>()
                .ToListAsync();
        }

        public async Task<DataWithCount<ICollection<PsKit>>> PageAsync(BaseFilter filter)
        {
            var kits = DbSet.AsNoTracking();

            var count = await kits.CountAsync();

            var kitList = await kits
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<PsKit>()
                .ToListAsync();

            return new DataWithCount<ICollection<PsKit>>
            {
                Data = kitList,
                Count = count
            };
        }

        public async Task<List<int>> GetIndexListAsync()
        {
            return await DbSet
                .AsNoTracking()
                .OrderBy(_ => _.Name)
                .Select(_ => _.Id)
                .ToListAsync();
        }

        public async Task AddKitAgeGroupsAsync(int kitIdId, List<int> ageGroupIds)
        {
            var kitAgeGroups = ageGroupIds
                .Select(_ => new Model.PsKitAgeGroup
                {
                    AgeGroupId = _,
                    KitId = kitIdId
                });

            await _context.PsKitAgeGroups.AddRangeAsync(kitAgeGroups);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveKitAgeGroupsAsync(int kitId, List<int> ageGroupIds)
        {
            var kitAgeGroups = _context.PsKitAgeGroups
                .AsNoTracking()
                .Where(_ => _.KitId == kitId && ageGroupIds.Contains(_.AgeGroupId));

            _context.PsKitAgeGroups.RemoveRange(kitAgeGroups);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsValidAgeGroupAsync(int kitId, int ageGroupId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == kitId 
                    && _.AgeGroups.Select(a => a.AgeGroupId).Contains(ageGroupId))
                .AnyAsync();
        }
    }
}
