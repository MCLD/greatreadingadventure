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

        public async Task<IEnumerable<Drawing>> PageAllAsync(int siteId, int skip, int take)
        {
            return await DbSet
                    .AsNoTracking()
                    .Include(_ => _.DrawingCriterion)
                    .Where(_ => _.DrawingCriterion.SiteId == siteId)
                    .OrderByDescending(_ => _.Id)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<Drawing>()
                    .ToListAsync();
        }

        public async Task<int> GetCountAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Include(_ => _.DrawingCriterion)
                .Where(_ => _.DrawingCriterion.SiteId == siteId)
                .CountAsync();
        }

        public async Task<Drawing> GetByIdAsync(int id, int skip, int take)
        {
            var drawing = await DbSet
                .AsNoTracking()
                .Include(_ => _.DrawingCriterion)
                .Where(_ => _.Id == id)
                .ProjectTo<Drawing>()
                .SingleAsync();

            if (drawing != null)
            {
                drawing.Winners = await _context.DrawingWinners
                    .AsNoTracking()
                    .Include(_ => _.User)
                    .Where(_ => _.DrawingId == id && _.User.IsDeleted == false)
                    .OrderBy(_ => _.User.LastName)
                    .ThenBy(_ => _.User.FirstName)
                    .ThenBy(_ => _.UserId)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<DrawingWinner>()
                    .ToListAsync();
            }
            return drawing;
        }

        public async Task<int> GetWinnerCountAsync(int id)
        {
            return await _context.DrawingWinners
                .AsNoTracking()
                .Where(_ => _.DrawingId == id)
                .CountAsync();
        }

        public async Task<IEnumerable<DrawingWinner>> PageUserAsync(int userId, int skip, int take)
        {
            return await _context.DrawingWinners
                .AsNoTracking()
                .Include(_ => _.Drawing)
                .Where(_ => _.UserId == userId)
                .OrderBy(_ => _.RedeemedAt.HasValue)
                .ThenByDescending(_ => _.RedeemedAt.Value)
                .Skip(skip)
                .Take(take)
                .ProjectTo<DrawingWinner>()
                .ToListAsync();
        }

        public async Task<int> GetUserWinCountAsync(int userId)
        {
            return await _context.DrawingWinners
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .ProjectTo<DrawingWinner>()
                .CountAsync();
        }

        public async Task RedeemWinnerAsync(int drawingId, int userId)
        {
            var drawingWinner = await _context.DrawingWinners
                .AsNoTracking()
                .Where(_ => _.DrawingId == drawingId && _.UserId == userId)
                .SingleOrDefaultAsync();
            
            if (drawingWinner != null)
            {
                if (!drawingWinner.RedeemedAt.HasValue)
                {
                    drawingWinner.RedeemedAt = DateTime.Now;
                    _context.DrawingWinners.Update(drawingWinner);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                throw new Exception($"DrawingWinner DrawingId {drawingId} UserId {userId} could not be found.");
            }
        }

        public async Task RemoveWinnerAsync(int drawingId, int userId)
        {
            var drawingWinner = await _context.DrawingWinners
                .AsNoTracking()
                .Where(_ => _.DrawingId == drawingId && _.UserId == userId)
                .SingleOrDefaultAsync();

            if (drawingWinner != null)
            {
                _context.DrawingWinners.Remove(drawingWinner);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"DrawingWinner DrawingId {drawingId} UserId {userId} could not be found.");
            }
        }

        public async Task AddWinnerAsync(DrawingWinner winner)
        {
            await _context.DrawingWinners
                .AddAsync(_mapper.Map<Model.DrawingWinner>(winner));
        }
    }
}
