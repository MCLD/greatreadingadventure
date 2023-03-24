using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IVendorCodeTypeRepository : IRepository<VendorCodeType>
    {
        Task<int> CountAsync(BaseFilter filter);

        Task<ICollection<VendorCodeType>> GetAllAsync(int siteId);

        Task<ICollection<VendorCodeType>> PageAsync(BaseFilter filter);

        Task<bool> SiteHasCodesAsync(int siteId);

        Task<bool> SiteHasEmailAwards(int siteId);
    }
}
