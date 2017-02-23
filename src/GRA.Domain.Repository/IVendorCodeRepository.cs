using GRA.Domain.Model;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IVendorCodeRepository : IRepository<VendorCode>
    {
        Task<VendorCode> AssignCodeAsync(int vendorCodeTypeId, int userId);
    }
}
