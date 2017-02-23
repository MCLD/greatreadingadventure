using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IVendorCodeTypeRepository : IRepository<VendorCodeType>
    {
        Task<ICollection<VendorCodeType>> GetAllAsync(int siteId);
        Task<ICollection<VendorCodeType>> PageAsync(Filter filter);
        Task<int> CountAsync(Filter filter);
    }
}
