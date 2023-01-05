using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPrizeWinnerRepository : IRepository<PrizeWinner>
    {
        Task<int> CountByWinningUserId(int siteId, int userId, bool? redeemed = null);

        Task<int> CountByWinningUserId(int siteId, ICollection<int> userIds, bool? redeemed = null);

        Task<int> GetBranchPrizeRedemptionCountAsync(int branchId, IEnumerable<int> triggerIds);

        Task<List<PrizeCount>> GetHouseholdUnredeemedPrizesAsync(int headId);

        Task<PrizeWinner> GetPrizeForVendorCodeAsync(int vendorCodeId);

        Task<ICollection<PrizeWinner>> GetRedemptionsAsync(ReportCriterion criterion);

        Task<int> GetSystemPrizeRedemptionCountAsync(int systemId, IEnumerable<int> triggerIds);

        Task<PrizeWinner> GetUserDrawingPrizeAsync(int userId, int drawingId);

        Task<ICollection<PrizeWinner>> GetUserPrizesAsync(ReportCriterion criterion);

        Task<PrizeWinner> GetUserTriggerPrizeAsync(int userId, int triggerId);

        Task<ICollection<PrizeWinner>> GetVendorCodePrizesAsync(int userId);

        Task<ICollection<PrizeWinner>> PageByWinnerAsync(int siteId, int userId, int skip, int take);

        Task<ICollection<PrizeWinner>> PageByWinnerAsync(int siteId, ICollection<int> userIds, int skip, int take);
    }
}
