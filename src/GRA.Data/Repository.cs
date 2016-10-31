using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data
{
    public class Repository : Domain.IRepository
    {
        private readonly Context context;
        public Repository(Context context)
        {
            this.context = context;
        }
        
        public IEnumerable<Domain.Model.Site> GetSites()
        {
            return context.Sites.Select(s => new Domain.Model.Site
            {
                Path = s.Path
            });
        }

        public void Dispose()
        {
            if(context != null)
            {
                context.Dispose();
            }
        }
    }
}
