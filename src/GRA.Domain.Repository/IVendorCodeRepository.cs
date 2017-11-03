using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IVendorCodeRepository : IRepository<VendorCode>
    {
        Task<VendorCode> AssignCodeAsync(int vendorCodeTypeId, int userId);
        Task<VendorCode> GetUserVendorCode(int userId);
        Task<VendorCode> GetByCode(string code);
        Task<ICollection<VendorCode>> GetEarnedCodesAsync(ReportCriterion criterion);
    }
}
