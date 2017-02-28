using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class DrawingRepository
        : AuditingRepository<Model.Drawing, Domain.Model.Drawing>, IDrawingRepository
    {
        public DrawingRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DrawingRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<Drawing>> PageAllAsync(int siteId, int skip, int take, bool archived)
        {
            return await DbSet
                    .AsNoTracking()
                    .Include(_ => _.DrawingCriterion)
                    .Where(_ => _.DrawingCriterion.SiteId == siteId && _.IsArchived == archived)
                    .OrderByDescending(_ => _.Id)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<Drawing>()
                    .ToListAsync();
        }

        public async Task<int> GetCountAsync(int siteId, bool archived)
        {
            return await DbSet
                .AsNoTracking()
                .Include(_ => _.DrawingCriterion)
                .Where(_ => _.DrawingCriterion.SiteId == siteId && _.IsArchived == archived)
                .CountAsync();
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
