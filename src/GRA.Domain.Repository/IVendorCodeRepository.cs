using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IVendorCodeRepository : IRepository<VendorCode>
    {
        Task<VendorCode> AssignCodeAsync(int vendorCodeTypeId, int userId);
        Task<VendorCode> GetUserVendorCode(int userId);
        Task<VendorCode> GetByCode(string code);
        Task<ICollection<VendorCode>> GetEarnedCodesAsync(ReportCriterion criterion);
        Task<ICollection<VendorCode>> GetPendingHouseholdCodes(int headOfHouseholdId);

        Task<ICollection<VendorCodeEmailAward>> GetUnreportedEmailAwardCodes(int siteId,
            int vendorCodeTypeId);
        Task<VendorCodeStatus> GetStatusAsync();
    }
}
