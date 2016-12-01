using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class SiteRepository
        : AuditingRepository<Model.Site, Site>, ISiteRepository
    {
        public SiteRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SiteRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<Site>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .ProjectTo<Site>()
                .ToListAsync();
        }
    }
}
