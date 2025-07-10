using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IDailyLiteracyTipImageRepository : IRepository<DailyLiteracyTipImage>
    {
        Task<int> CountAsync(DailyImageFilter filter);

        Task<DailyLiteracyTipImage> GetByDay(int dailyLiteracyTipId, int day);

        Task<int> GetLatestDayAsync(int dailyLiteracyTipId);

        Task<ICollection<DailyLiteracyTipImage>> PageAsync(DailyImageFilter filter);

        new Task RemoveSaveAsync(int userId, int imageId);

        Task UpdateSaveAsync(int userId, DailyLiteracyTipImage image, int newDay);

        Task IncreaseDayAsync(int userId, int imageId, int siteId);

        Task DecreaseDayAsync(int userId, int imageId, int siteId);
    }
}