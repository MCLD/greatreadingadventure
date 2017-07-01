using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository.Extensions;

namespace GRA.Data.Repository
{
    public class DrawingRepository
        : AuditingRepository<Model.Drawing, Domain.Model.Drawing>, IDrawingRepository
    {
        public DrawingRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DrawingRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<Drawing>> PageAllAsync(DrawingFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderByDescending(_ => _.Id)
                .ApplyPagination(filter)
                .ProjectTo<Drawing>()
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(DrawingFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        private IQueryable<Model.Drawing> ApplyFilters(DrawingFilter filter)
        {
            var drawingList = DbSet
                .AsNoTracking()
                .Where(_ => _.DrawingCriterion.SiteId == filter.SiteId
                    && _.IsArchived == filter.Archived);

            if (filter.SystemIds?.Any() == true)
            {
                drawingList = drawingList
                    .Where(_ => filter.SystemIds.Contains(_.RelatedSystemId));
            }

            if (filter.BranchIds?.Any() == true)
            {
                drawingList = drawingList
                    .Where(_ => filter.BranchIds.Contains(_.RelatedBranchId));
            }

            if (filter.UserIds?.Any() == true)
            {
                drawingList = drawingList.Where(_ => filter.UserIds.Contains(_.CreatedBy));
            }

            if (filter.ProgramIds?.Any() == true)
            {
                IQueryable<Model.Drawing> nullList = null;
                IQueryable<Model.Drawing> valueList = null;
                if (filter.ProgramIds.Any(_ => _.HasValue == false))
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

        public async Task<Drawing> GetByIdAsync(int id, int skip, int take)
        {
            var drawing = await DbSet
                .AsNoTracking()
                .Include(_ => _.DrawingCriterion)
                .Where(_ => _.Id == id)
                .ProjectTo<Drawing>()
                .SingleOrDefaultAsync();

            if (drawing == null)
            {
                throw new Exception($"Drawing id {id} could not be found.");
            }

            drawing.Winners = await _context.PrizeWinners
                .AsNoTracking()
                .Include(_ => _.User)
                .Where(_ => _.DrawingId == id && _.User.IsDeleted == false)
                .OrderBy(_ => _.User.LastName)
                .ThenBy(_ => _.User.FirstName)
                .ThenBy(_ => _.UserId)
                .Skip(skip)
                .Take(take)
                .ProjectTo<PrizeWinner>()
                .ToListAsync();
            return drawing;
        }

        public async Task<int> GetWinnerCountAsync(int id)
        {
            return await _context.PrizeWinners
                .AsNoTracking()
                .Where(_ => _.DrawingId == id)
                .CountAsync();
        }

        public async Task SetArchivedAsync(int userId, int drawingId, bool archive)
        {
            var drawing = _mapper.Map<Model.Drawing, Drawing>(await DbSet.Where(_ => _.Id == drawingId).SingleOrDefaultAsync());
            if (drawing != null)
            {
                drawing.IsArchived = archive;
                await UpdateSaveAsync(userId, drawing);
            }
        }
    }
}
