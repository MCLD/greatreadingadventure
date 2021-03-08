using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IVendorCodePackingSlipRepository : IRepository<Model.VendorCodePackingSlip>
    {
        public Task<VendorCodePackingSlip> GetByPackingSlipNumberAsync(long packingSlipNumber);
    }
}
