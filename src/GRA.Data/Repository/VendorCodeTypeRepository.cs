using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class VendorCodeTypeRepository
        : AuditingRepository<Model.VendorCodeType, VendorCodeType>, IVendorCodeTypeRepository
    {
        public VendorCodeTypeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<VendorCodeTypeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        // honors site id, skip, and take
        public async Task<int> CountAsync(Filter filter)
        {
            return await ApplySiteIdPagination(filter).CountAsync();
        }

        // honors site id, skip, and take
        public async Task<ICollection<VendorCodeType>> PageAsync(Filter filter)
        {
            return await ApplySiteIdPagination(filter)
                .ProjectTo<VendorCodeType>()
                .ToListAsync();
        }

        private IQueryable<Model.VendorCodeType> ApplySiteIdPagination(Filter filter)
        {
            var filteredData = DbSet.AsNoTracking().Where(_ => _.SiteId == filter.SiteId);
            return ApplyPagination(filteredData, filter);
        }
    }
}
