﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
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
                .Where(_ => _.Branch.Id == branchId)
                .OrderBy(_ => _.SelectedAt)
                .ProjectTo<PsBranchSelection>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<int> GetCountByKitIdAsync(int kitId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.KitId.HasValue && _.KitId == kitId)
                .CountAsync();
        }

        public async Task<ICollection<PsBranchSelection>> GetByKitIdAsync(int kitId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.KitId.HasValue && _.KitId == kitId)
                .ProjectTo<PsBranchSelection>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<int> GetCountByPerformerIdAsync(int performerId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.ProgramId.HasValue && _.Program.PerformerId == performerId)
                .CountAsync();
        }

        public async Task<ICollection<PsBranchSelection>> GetByPerformerIdAsync(
            int performerId, DateTime? date = null)
        {
            var query = DbSet
                .AsNoTracking()
                .Where(_ => _.ProgramId.HasValue && _.Program.PerformerId == performerId);

            if (date.HasValue)
            {
                query = query.Where(_ => _.RequestedStartTime.Date == date.Value.Date);
            }

            return await query
                .OrderBy(_ => _.RequestedStartTime)
                .ProjectTo<PsBranchSelection>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PsBranchSelection> GetByCodeAsync(string secretCode)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SecretCode == secretCode)
                .ProjectTo<PsBranchSelection>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> BranchAgeGroupAlreadySelectedAsync(int ageGroupId, int branchId,
            int? currentSelectionId = null)
        {
            var selection = DbSet
                .AsNoTracking()
                .Where(_ => _.AgeGroupId == ageGroupId && _.BranchId == branchId);

            if (currentSelectionId.HasValue)
            {
                selection = selection.Where(_ => _.Id != currentSelectionId.Value);
            }

            return await selection.AnyAsync();
        }
    }
}
