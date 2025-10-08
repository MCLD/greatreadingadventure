using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsBranchSelectionRepository
        : AuditingRepository<Model.PsBranchSelection, Domain.Model.PsBranchSelection>, IPsBranchSelectionRepository
    {
        public PsBranchSelectionRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsBranchSelectionRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<PsBranchSelection>> GetByBranchIdAsync(int branchId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Branch.Id == branchId && !_.IsDeleted)
                .OrderBy(_ => _.SelectedAt)
                .ProjectToType<PsBranchSelection>()
                .ToListAsync();
        }

        public async Task<int> GetCountByKitIdAsync(int kitId)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(_ => _.KitId.HasValue && _.KitId == kitId && !_.IsDeleted);
        }

        public async Task<ICollection<PsBranchSelection>> GetByKitIdAsync(int kitId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.KitId.HasValue && _.KitId == kitId && !_.IsDeleted)
                .ProjectToType<PsBranchSelection>()
                .ToListAsync();
        }

        public async Task<int> GetCountByPerformerIdAsync(int performerId)
        {
            return await DbSet
                .AsNoTracking()
                .CountAsync(_ => _.ProgramId.HasValue
                    && _.Program.PerformerId == performerId
                    && !_.IsDeleted);
        }

        public async Task<ICollection<PsBranchSelection>> GetByPerformerIdAsync(
            int performerId, DateTime? date = null)
        {
            var query = DbSet
                .AsNoTracking()
                .Where(_ => _.ProgramId.HasValue
                    && _.Program.PerformerId == performerId
                    && !_.IsDeleted);

            if (date.HasValue)
            {
                query = query.Where(_ => _.RequestedStartTime.Date == date.Value.Date);
            }

            return await query
                .OrderBy(_ => _.RequestedStartTime)
                .ProjectToType<PsBranchSelection>()
                .ToListAsync();
        }

        public async Task<PsBranchSelection> GetByCodeAsync(string secretCode)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SecretCode == secretCode && !_.IsDeleted)
                .ProjectToType<PsBranchSelection>()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> BranchAgeGroupAlreadySelectedAsync(int ageGroupId, int branchId,
            int? currentSelectionId = null)
        {
            var selection = DbSet
                .AsNoTracking()
                .Where(_ => _.AgeGroupId == ageGroupId
                    && _.BranchId == branchId
                    && !_.IsDeleted);

            if (currentSelectionId.HasValue)
            {
                selection = selection.Where(_ => _.Id != currentSelectionId.Value);
            }

            return await selection.AnyAsync();
        }
    }
}
