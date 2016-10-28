using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.SqlServer
{
    public class GRARepositorySqlServer : Domain.IGRARepository
    {
        GRARepositorySqlContext context = null;
        public GRARepositorySqlServer()
        {
            context = new GRARepositorySqlContext();
        }

        public void Dispose()
        {
            if(context != null)
            {
                context.Dispose();
            }
        }

        public IEnumerable<Domain.Site> GetSites()
        {
            return context.Sites.Select(s => new Domain.Site
            {
                Path = s.Path
            });
        }
    }
}
