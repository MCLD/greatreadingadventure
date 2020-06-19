﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
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

        public async Task<ICollection<VendorCodeType>> GetAllAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .ProjectTo<VendorCodeType>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ICollection<VendorCodeType>> GetEmailAwardTypesAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && !string.IsNullOrWhiteSpace(_.EmailAwardSubject))
                .ProjectTo<VendorCodeType>(_mapper.ConfigurationProvider)
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
                .OrderBy(_ => _.Description)
                .ApplyPagination(filter)
                .ProjectTo<VendorCodeType>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        private IQueryable<Model.VendorCodeType> ApplyFilters(BaseFilter filter)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);
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
                    && !string.IsNullOrWhiteSpace(_.EmailAwardSubject));
        }

        public async Task<string> GetEmailAwardInstructionText(int typeId, int languageId)
        {
            return await _context.VendorCodeTypeTexts
                .AsNoTracking()
                .Where(_ => _.VendorCodeTypeId == typeId && _.LanguageId == languageId)
                .Select(_ => _.EmailAwardInstructions)
                .SingleOrDefaultAsync();
        }
    }
}
