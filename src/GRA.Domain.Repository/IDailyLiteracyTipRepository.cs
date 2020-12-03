using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IDailyLiteracyTipRepository : IRepository<DailyLiteracyTip>
    {
        Task<IEnumerable<DailyLiteracyTip>> GetAllAsync(int siteId);
        Task<int> CountAsync(BaseFilter filter);
        Task<ICollection<DailyLiteracyTip>> PageAsync(BaseFilter filter);
        Task<bool> IsInUseAsync(int dailyLiteracyTipId);
    }
}
