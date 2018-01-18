using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<ICollection<DailyLiteracyTipImage>> PageAsync(DailyImageFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Day)
                .ApplyPagination(filter)
                .ProjectTo<DailyLiteracyTipImage>()
                .ToListAsync();
        }

        private IQueryable<Model.DailyLiteracyTipImage> ApplyFilters(DailyImageFilter filter)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.DailyLiteracyTipId == filter.DailyLiteracyTipId);
        }

        public async Task UpdateSaveAsync(int userId, DailyLiteracyTipImage image, int newDay)
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

        public override async Task RemoveSaveAsync(int userId, int imageId)
        {
            var image = await DbSet.AsNoTracking()
                .Where(_ => _.Id == imageId)
                .SingleOrDefaultAsync();

            await DbSet
                .Where(_ => _.DailyLiteracyTipId == image.DailyLiteracyTipId && _.Day > image.Day)
                .ForEachAsync(_ => _.Day--);

            await base.RemoveSaveAsync(userId, imageId);
        }

        public async Task<DailyLiteracyTipImage> GetByDay(int dailyLiteracyTipId, int day)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.DailyLiteracyTipId == dailyLiteracyTipId && _.Day == day)
                .ProjectTo<DailyLiteracyTipImage>()
                .SingleOrDefaultAsync();
        }
    }
}
