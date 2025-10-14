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
            return await ApplyFilters(filter).CountAsync();
        }

        public async Task DecreaseDayAsync(int imageId, int siteId)
        {
            var images = await DbSet
                .Include(_ => _.DailyLiteracyTip)
                .Where(_ => _.DailyLiteracyTip.DailyLiteracyTipImages.Any(i => i.Id == imageId))
                .OrderBy(_ => _.Day)
                .ToListAsync();

            var image = images.FirstOrDefault(_ => _.Id == imageId);
            if (image == null || image.DailyLiteracyTip.SiteId != siteId)
            {
                return;
            }

            var prev = images.FirstOrDefault(_ => _.Day == image.Day - 1);
            if (prev == null)
            {
                return;
            }

            await SwapDaysAsync(image, prev);
        }

        public async Task<DailyLiteracyTipImage> GetByDay(int dailyLiteracyTipId, int day)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.DailyLiteracyTipId == dailyLiteracyTipId && _.Day == day)
                .ProjectToType<DailyLiteracyTipImage>()
                .SingleOrDefaultAsync();
        }

        public async Task<Tuple<int, int>> GetFirstLastDayAsync(int dailyLiteracyTipId)
        {
            var minDay = await DbSet
                .AsNoTracking()
                .Where(_ => _.DailyLiteracyTipId == dailyLiteracyTipId)
                .MinAsync(_ => (int?)_.Day);

            if (minDay.HasValue)
            {
                var maxDay = await DbSet
                    .AsNoTracking()
                    .Where(_ => _.DailyLiteracyTipId == dailyLiteracyTipId)
                    .MaxAsync(_ => (int?)_.Day);

                if (maxDay.HasValue)
                {
                    return Tuple.Create(minDay.Value, maxDay.Value);
                }
            }
            return null;
        }

        public async Task<int> GetLatestDayAsync(int dailyLiteracyTipId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.DailyLiteracyTipId == dailyLiteracyTipId)
                .DefaultIfEmpty()
                .MaxAsync(_ => (int?)_.Day) ?? 0;
        }

        public async Task<bool> ImageNameExistsAsync(int tipId, string name, string extension)
        {
            return await DbSet.AnyAsync(_ =>
                _.DailyLiteracyTipId == tipId &&
                _.Name == name &&
                _.Extension == extension);
        }

        public async Task IncreaseDayAsync(int imageId, int siteId)
        {
            var images = await DbSet
                .Include(_ => _.DailyLiteracyTip)
                .Where(_ => _.DailyLiteracyTip.DailyLiteracyTipImages.Any(i => i.Id == imageId))
                .OrderBy(_ => _.Day)
                .ToListAsync();

            var image = images.FirstOrDefault(_ => _.Id == imageId);
            if (image == null || image.DailyLiteracyTip.SiteId != siteId)
            {
                return;
            }

            var next = images.FirstOrDefault(_ => _.Day == image.Day + 1);
            if (next == null)
            {
                return;
            }

            await SwapDaysAsync(image, next);
        }

        public async Task<ICollection<DailyLiteracyTipImage>> PageAsync(DailyImageFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Day)
                .ApplyPagination(filter)
                .ProjectToType<DailyLiteracyTipImage>()
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

        public async Task UpdateDayAndShiftOthersAsync(int imageId, int newDay, int siteId)
        {
            var image = await DbSet.Include(i => i.DailyLiteracyTip)
                .Where(i => i.Id == imageId)
                .FirstOrDefaultAsync();

            var tipId = image.DailyLiteracyTipId;
            var currentDay = image.Day;

            if (newDay < currentDay)
            {
                var shift = DbSet.Where(i => i.DailyLiteracyTipId == tipId
                    && i.Day >= newDay && i.Day < currentDay);
                await shift.ForEachAsync(i => i.Day++);
            }
            else
            {
                var shift = DbSet.Where(i => i.DailyLiteracyTipId == tipId
                    && i.Day <= newDay && i.Day > currentDay);
                await shift.ForEachAsync(i => i.Day--);
            }

            image.Day = newDay;

            await _context.SaveChangesAsync();
        }

        private IQueryable<Model.DailyLiteracyTipImage> ApplyFilters(DailyImageFilter filter)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.DailyLiteracyTipId == filter.DailyLiteracyTipId);
        }

        private async Task SwapDaysAsync(
            Model.DailyLiteracyTipImage a,
            Model.DailyLiteracyTipImage b)
        {
            (b.Day, a.Day) = (a.Day, b.Day);

            _context.Update(a);
            _context.Update(b);
            await _context.SaveChangesAsync();
        }
    }
}
