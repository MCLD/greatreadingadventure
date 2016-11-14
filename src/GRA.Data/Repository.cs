using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data
{
    public class Repository : Domain.IRepository
    {
        private readonly ILogger<Repository> logger;
        private readonly Context context;
        private readonly AutoMapper.IMapper mapper;
        public Repository(ILogger<Repository> logger, Context context, AutoMapper.IMapper mapper)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            this.logger = logger;
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.context = context;
            if (mapper == null)
            {
                throw new ArgumentNullException("mapper");
            }
            this.mapper = mapper;
        }

        public IEnumerable<Domain.Model.Site> GetSites()
        {
            return context.Sites.ProjectTo<Domain.Model.Site>();
        }

        public bool AddSite(Domain.Model.Site site)
        {
            var addSite = mapper.Map<Domain.Model.Site, Data.Model.Site>(site);
            context.Sites.Add(addSite);
            return context.SaveChanges() == 1;
        }
    }
}
