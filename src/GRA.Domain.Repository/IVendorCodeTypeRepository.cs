using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IVendorCodeTypeRepository : IRepository<VendorCodeType>
    {
        Task<ICollection<VendorCodeType>> GetAllAsync(int siteId);
        Task<ICollection<VendorCodeType>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);
        Task<bool> SiteHasCodesAsync(int siteId);
    }
}
