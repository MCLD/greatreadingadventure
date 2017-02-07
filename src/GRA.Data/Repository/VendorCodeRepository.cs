using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class VendorCodeRepository
        : AuditingRepository<Model.VendorCode, VendorCode>, IVendorCodeRepository
    {
        public VendorCodeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<VendorCodeRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
