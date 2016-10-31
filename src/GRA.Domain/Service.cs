using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GRA.Domain
{
    public class Service
    {
        private readonly ILogger<Service> logger;
        private readonly IRepository repo;
        public Service(ILogger<Service> logger, IRepository repository)
        {
            if(logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            this.logger = logger;
            if(repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            repo = repository;
        }

        public IEnumerable<Model.Site> GetSitePaths()
        {
            return repo.GetSites();
        }
    }
}
