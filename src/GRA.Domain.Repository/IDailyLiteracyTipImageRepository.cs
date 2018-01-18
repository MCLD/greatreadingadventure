using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IDailyLiteracyTipImageRepository : IRepository<DailyLiteracyTipImage>
    {
        Task<int> CountAsync(DailyImageFilter filter);
        Task<ICollection<DailyLiteracyTipImage>> PageAsync(DailyImageFilter filter);
        Task UpdateSaveAsync(int userId, DailyLiteracyTipImage image, int newDay);
        new Task RemoveSaveAsync(int userId, int imageId);
        Task<DailyLiteracyTipImage> GetByDay(int dailyLiteracyTipId, int day);
    }
}
