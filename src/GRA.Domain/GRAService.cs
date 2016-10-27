using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain
{
    public class GRAService
    {
        private readonly GRARepository repo;
        public GRAService(GRARepository repository)
        {
            if(repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            repo = repository;
        }
    }
}
