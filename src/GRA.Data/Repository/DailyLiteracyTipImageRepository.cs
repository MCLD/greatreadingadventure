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
    public class DailyLiteracyTipImageRepository
        : AuditingRepository<Model.DailyLiteracyTipImage, DailyLiteracyTipImage>,
        IDailyLiteracyTipImageRepository
    {
        public DailyLiteracyTipImageRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DailyLiteracyTipImageRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int> CountAsync(DailyImageFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<DailyLiteracyTipImage> GetByDay(int dailyLiteracyTipId, int day)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.DailyLiteracyTipId == dailyLiteracyTipId && _.Day == day)
                .ProjectTo<DailyLiteracyTipImage>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetLatestDayAsync(int dailyLiteracyTipId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.DailyLiteracyTipId == dailyLiteracyTipId)
                .DefaultIfEmpty()
                .MaxAsync(_ => (int?)_.Day) ?? 0;
        }

        public async Task<ICollection<DailyLiteracyTipImage>> PageAsync(DailyImageFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Day)
                .ApplyPagination(filter)
                .ProjectTo<DailyLiteracyTipImage>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var image = await DbSet.AsNoTracking()
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();

            await DbSet
                .Where(_ => _.DailyLiteracyTipId == image.DailyLiteracyTipId && _.Day > image.Day)
                .ForEachAsync(_ => _.Day--);

            await base.RemoveSaveAsync(userId, id);
        }

        public async Task UpdateSaveAsync(int userId, DailyLiteracyTipImage image, int newDay)
        {
            if (image != null)
            {
                if (newDay > image.Day)
                {
                    await DbSet.Where(_ => _.DailyLiteracyTipId == image.DailyLiteracyTipId
                        && _.Day > image.Day && _.Day <= newDay)
                        .ForEachAsync(_ => _.Day--);
                }
                else
                {
                    await DbSet.Where(_ => _.DailyLiteracyTipId == image.DailyLiteracyTipId
                        && _.Day < image.Day && _.Day >= newDay)
                        .ForEachAsync(_ => _.Day++);
                }
                image.Day = newDay;
                await base.UpdateSaveAsync(userId, image);
            }
        }

        private IQueryable<Model.DailyLiteracyTipImage> ApplyFilters(DailyImageFilter filter)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.DailyLiteracyTipId == filter.DailyLiteracyTipId);
        }

        public async Task IncreaseDayAsync(int userId, int imageId, int siteId)
        {
            var image = await DbSet.Include(_ => _.DailyLiteracyTip)
              .FirstOrDefaultAsync(_ => _.Id == imageId);

            if (image == null || image.DailyLiteracyTip.SiteId != siteId)
            {
                return;
            }

            var next = await DbSet.FirstOrDefaultAsync(_ =>
              _.DailyLiteracyTipId == image.DailyLiteracyTipId && _.Day == image.Day + 1);

            if (next == null)
            {
                return;
            }

            await SwapDaysAsync(userId, image, next);
        }

        public async Task DecreaseDayAsync(int userId, int imageId, int siteId)
        {
            var image = await DbSet.Include(_ => _.DailyLiteracyTip)
              .FirstOrDefaultAsync(_ => _.Id == imageId);

            if (image == null || image.DailyLiteracyTip.SiteId != siteId) return;

            var prev = await DbSet.FirstOrDefaultAsync(_ =>
              _.DailyLiteracyTipId == image.DailyLiteracyTipId && _.Day == image.Day - 1);

            if (prev == null) return;

            await SwapDaysAsync(userId, image, prev);
        }

        private async Task SwapDaysAsync(int userId,
            Model.DailyLiteracyTipImage a,
            Model.DailyLiteracyTipImage b)
        {
            var temp = a.Day;
            a.Day = b.Day;
            b.Day = temp;

            _context.Update(a);
            _context.Update(b);
            await _context.SaveChangesAsync();
        }
    }
}
