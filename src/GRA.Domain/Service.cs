using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain
{
    public class Service
    {
        private readonly IRepository repo;
        public Service(IRepository repository)
        {
            if(repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            repo = repository;
        }

        public IEnumerable<Site> GetSitePaths()
        {
            return repo.GetSites();
        }
    }
}
