using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter).CountAsync();
        }

        public async Task<ICollection<VendorCodeType>> GetAllAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .ProjectToType<VendorCodeType>()
                .ToListAsync();
        }

        // honors site id, skip, and take
        public async Task<ICollection<VendorCodeType>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Description)
                .ApplyPagination(filter)
                .ProjectToType<VendorCodeType>()
                .ToListAsync();
        }

        public async Task<bool> SiteHasCodesAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .AnyAsync();
        }

        public async Task<bool> SiteHasEmailAwards(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .AnyAsync(_ => _.SiteId == siteId
                    && _.EmailAwardMessageTemplateId.HasValue);
        }

        private IQueryable<Model.VendorCodeType> ApplyFilters(BaseFilter filter)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);
        }
    }
}
