using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IPrizeWinnerRepository : IRepository<PrizeWinner>
    {
        Task<ICollection<PrizeWinner>> PageByWinnerAsync(int siteId, int userId, int skip, int take);
        Task<int> CountByWinningUserId(int siteId, int userId, bool? redeemed = null);
        Task<PrizeWinner> GetUserTriggerPrizeAsync(int userId, int triggerId);
        Task<ICollection<PrizeWinner>> GetRedemptionsAsync(ReportCriterion criterion);
        Task<ICollection<PrizeWinner>> GetUserPrizesAsync(ReportCriterion criterion);
    }
}
