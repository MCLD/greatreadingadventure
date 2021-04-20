using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class SocialHeaderRepository
        : AuditingRepository<Model.SocialHeader, Domain.Model.SocialHeader>,
        ISocialHeaderRepository
    {
        public SocialHeaderRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SocialHeaderRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<Domain.Model.SocialHeader> GetByDateAsync(DateTime asOf)
        {
            var socialHeader = await DbSet
                .AsNoTracking()
                .Where(_ => _.StartDate <= asOf)
                .OrderByDescending(_ => _.StartDate)
                .ProjectTo<Domain.Model.SocialHeader>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (socialHeader != null)
            {
                socialHeader.NextStartDate = await DbSet
                    .AsNoTracking()
                    .Where(_ => _.StartDate > asOf)
                    .OrderBy(_ => _.StartDate)
                    .Select(_ => _.StartDate)
                    .FirstOrDefaultAsync();
            }

            return socialHeader;
        }

        public async Task<ICollection<Domain.Model.SocialHeader>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderByDescending(_ => _.StartDate)
                .ApplyPagination(filter)
                .ProjectTo<Domain.Model.SocialHeader>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        private IQueryable<Model.SocialHeader> ApplyFilters(BaseFilter filter)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);
        }
    }
}