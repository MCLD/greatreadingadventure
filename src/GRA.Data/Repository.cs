using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data
{
    public class Repository : Domain.IRepository
    {
        private Context context;
        public Repository(Context context)
        {
            //context = new Data.Context();
            this.context = context;
        }

        public IEnumerable<Domain.Site> GetSites()
        {
            return context.Sites.Select(s => new Domain.Site
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
