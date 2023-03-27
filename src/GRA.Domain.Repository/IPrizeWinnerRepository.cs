using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IPrizeWinnerRepository : IRepository<PrizeWinner>
    {
        Task<int> CountByWinnerIdAsync(PrizeFilter filter);

        Task<int> GetBranchPrizeRedemptionCountAsync(int branchId, IEnumerable<int> triggerIds);

        Task<List<PrizeCount>> GetHouseholdUnredeemedPrizesAsync(int headId);

        Task<PrizeWinner> GetPrizeForVendorCodeAsync(int vendorCodeId);

        Task<ICollection<PrizeWinner>> GetRedemptionsAsync(ReportCriterion criterion);

        Task<int> GetSystemPrizeRedemptionCountAsync(int systemId, IEnumerable<int> triggerIds);

        Task<PrizeWinner> GetUserDrawingPrizeAsync(int userId, int drawingId);

        Task<ICollection<PrizeWinner>> GetUserPrizesAsync(ReportCriterion criterion);

        Task<PrizeWinner> GetUserTriggerPrizeAsync(int userId, int triggerId);

        Task<ICollection<PrizeWinner>> GetVendorCodePrizesAsync(int userId);

        Task<ICollection<PrizeWinner>> PageByWinnerAsync(PrizeFilter filter);
    }
}
