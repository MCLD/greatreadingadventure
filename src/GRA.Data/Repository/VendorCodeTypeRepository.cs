using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Repository.Extensions;
using GRA.Domain.Model.Filters;

namespace GRA.Data.Repository
{
    public class VendorCodeTypeRepository
        : AuditingRepository<Model.VendorCodeType, VendorCodeType>, IVendorCodeTypeRepository
    {
        public VendorCodeTypeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<VendorCodeTypeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<VendorCodeType>> GetAllAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .ProjectTo<VendorCodeType>()
                .ToListAsync();
        }

        // honors site id, skip, and take
        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter).CountAsync();
        }

        // honors site id, skip, and take
        public async Task<ICollection<VendorCodeType>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .ApplyPagination(filter)
                .ProjectTo<VendorCodeType>()
                .ToListAsync();
        }

        private IQueryable<Model.VendorCodeType> ApplyFilters(BaseFilter filter)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);
        }
    }
}
