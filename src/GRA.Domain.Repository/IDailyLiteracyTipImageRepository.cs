using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IDailyLiteracyTipImageRepository : IRepository<DailyLiteracyTipImage>
    {
        Task<int> CountAsync(DailyImageFilter filter);

        Task DecreaseDayAsync(int imageId, int siteId);

        Task<DailyLiteracyTipImage> GetByDay(int dailyLiteracyTipId, int day);

        public Task<Tuple<int, int>> GetFirstLastDayAsync(int dailyLiteracyTipId);

        Task<int> GetLatestDayAsync(int dailyLiteracyTipId);

        Task<bool> ImageNameExistsAsync(int tipId, string name, string extension);

        Task IncreaseDayAsync(int imageId, int siteId);

        Task<ICollection<DailyLiteracyTipImage>> PageAsync(DailyImageFilter filter);

        new Task RemoveSaveAsync(int userId, int imageId);

        Task UpdateDayAndShiftOthersAsync(int imageId, int newDay, int siteId);
    }
}
