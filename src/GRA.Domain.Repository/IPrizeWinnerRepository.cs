using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPrizeWinnerRepository : IRepository<PrizeWinner>
    {
        Task<ICollection<PrizeWinner>> PageByWinnerAsync(int siteId, int userId, int skip, int take);
        Task<int> CountByWinningUserId(int siteId, int userId, bool? redeemed = null);
        Task<PrizeWinner> GetUserDrawingPrizeAsync(int userId, int drawingId);
        Task<PrizeWinner> GetUserTriggerPrizeAsync(int userId, int triggerId);
        Task<ICollection<PrizeWinner>> GetRedemptionsAsync(ReportCriterion criterion);
        Task<ICollection<PrizeWinner>> GetUserPrizesAsync(ReportCriterion criterion);
        Task<int> GetSystemPrizeRedemptionCountAsync(int systemId, IEnumerable<int> triggerIds);
        Task<int> GetBranchPrizeRedemptionCountAsync(int branchId, IEnumerable<int> triggerIds);
        Task<List<PrizeCount>> GetHouseholdUnredeemedPrizesAsync(int headId);
        Task<ICollection<PrizeWinner>> GetVendorCodePrizesAsync(int userId);
        Task<PrizeWinner> GetPrizeForVendorCodeAsync(int vendorCodeId);
    }
}
