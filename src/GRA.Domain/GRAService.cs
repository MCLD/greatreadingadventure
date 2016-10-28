using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain
{
    public class GRAService
    {
        private readonly IGRARepository repo;
        public GRAService(IGRARepository repository)
        {
            if(repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            repo = repository;
        }

        public IEnumerable<Domain.Site> GetSitePaths()
        {
            return repo.GetSites();
        }
    }
}
