using System;
using System.Collections.Generic;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class SiteService : Abstract.BaseService<SiteService>
    {
        private readonly ISiteRepository siteRepository;

        public SiteService(ILogger<SiteService> logger,
            ISiteRepository siteRepository)
            : base(logger)
        {
            if (siteRepository == null)
            {
                throw new ArgumentNullException(nameof(siteRepository));
            }
            this.siteRepository = siteRepository;
        }

        public IEnumerable<Site> GetAllSites()
        {
            return siteRepository.GetAll();
        }
    }
}