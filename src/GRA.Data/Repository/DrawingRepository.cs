using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DrawingRepository
        : AuditingRepository<Model.Drawing, Drawing>, IDrawingRepository
    {
        public DrawingRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DrawingRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int> GetCountAsync(DrawingFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return await ApplyFilters(filter).CountAsync();
        }

        public async Task<Drawing> GetDetailsWinners(int id)
        {
            return await GetDetailsWinnersInternal(id, null, null);
        }

        public async Task<Drawing> GetDetailsWinners(int id, int skip, int take)
        {
            return await GetDetailsWinnersInternal(id, skip, take);
        }

        public async Task<int> GetWinnerCountAsync(int id)
        {
            return await _context.PrizeWinners
                .AsNoTracking()
                .Where(_ => _.DrawingId == id)
                .CountAsync();
        }

        public async Task<IEnumerable<Drawing>> PageAllAsync(DrawingFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return await ApplyFilters(filter)
                .OrderByDescending(_ => _.Id)
                .ApplyPagination(filter)
                .ProjectToType<Drawing>()
                .ToListAsync();
        }

        public async Task SetArchivedAsync(int userId, int drawingId, bool archive)
        {
            var modelDrawing = await DbSet.Where(_ => _.Id == drawingId).SingleOrDefaultAsync();
            var dataDrawing = _mapper.Map<Model.Drawing, Drawing>(modelDrawing);
            if (dataDrawing != null)
            {
                dataDrawing.IsArchived = archive;
                await UpdateSaveAsync(userId, dataDrawing);
            }
        }

        private IQueryable<Model.Drawing> ApplyFilters(DrawingFilter filter)
        {
            var drawingList = DbSet
                .AsNoTracking()
                .Where(_ => _.DrawingCriterion.SiteId == filter.SiteId
                    && _.IsArchived == filter.Archived);

            if (filter.SystemIds?.Count > 0)
            {
                drawingList = drawingList
                    .Where(_ => filter.SystemIds.Contains(_.RelatedSystemId));
            }

            if (filter.BranchIds?.Count > 0)
            {
                drawingList = drawingList
                    .Where(_ => filter.BranchIds.Contains(_.RelatedBranchId));
            }

            if (filter.UserIds?.Count > 0)
            {
                drawingList = drawingList.Where(_ => filter.UserIds.Contains(_.CreatedBy));
            }

            if (filter.ProgramIds?.Count > 0)
            {
                IQueryable<Model.Drawing> nullList = null;
                IQueryable<Model.Drawing> valueList = null;
                if (filter.ProgramIds.Any(_ => !_.HasValue))
                {
                    nullList = drawingList.Where(_ => _.DrawingCriterion.ProgramId == null);
                }
                if (filter.ProgramIds.Any(_ => _.HasValue))
                {
                    var programValues = filter.ProgramIds.Where(_ => _.HasValue);
                    valueList = drawingList
                        .Where(_ => programValues.Contains(_.DrawingCriterion.ProgramId));
                }
                if (nullList != null && valueList != null)
                {
                    drawingList = nullList.Union(valueList);
                }
                else if (nullList != null)
                {
                    drawingList = nullList;
                }
                else if (valueList != null)
                {
                    drawingList = valueList;
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                drawingList = drawingList.Where(_ => _.Name.Contains(filter.Search)
                    || _.DrawingCriterion.Name.Contains(filter.Search));
            }

            return drawingList;
        }

        private async Task<Drawing> GetDetailsWinnersInternal(int id, int? skip, int? take)
        {
            var drawing = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id)
                .ProjectToType<Drawing>()
                .SingleOrDefaultAsync()
                ?? throw new GraException($"Drawing id {id} could not be found.");

            var winners = _context.PrizeWinners
                .AsNoTracking()
                .Where(_ => _.DrawingId == id && !_.User.IsDeleted)
                .OrderBy(_ => _.User.LastName)
                .ThenBy(_ => _.User.FirstName)
                .ThenBy(_ => _.UserId);

            if (skip.HasValue && take.HasValue)
            {
                drawing.Winners = await winners
                    .Skip(skip.Value)
                    .Take(take.Value)
                    .ProjectToType<PrizeWinner>()
                    .ToListAsync();
            }
            else
            {
                drawing.Winners = await winners
                    .ProjectToType<PrizeWinner>()
                    .ToListAsync();
            }

            return drawing;
        }
    }
}
