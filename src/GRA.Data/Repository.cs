using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GRA.Data
{
    public class Repository : Domain.IRepository
    {
        private readonly ILogger<Repository> logger;
        private readonly Context context;
        public Repository(ILogger<Repository> logger, Context context)
        {
            if(logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            this.logger = logger;
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.context = context;
        }

        public IEnumerable<Domain.Model.Site> GetSites()
        {
            return context.Sites.Select(s => new Domain.Model.Site
            {
                Path = s.Path
            });
        }
    }
}
