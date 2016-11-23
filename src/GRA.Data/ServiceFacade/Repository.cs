using Microsoft.Extensions.Configuration;
using System;

namespace GRA.Data.ServiceFacade
{
    public class Repository
    {
        public readonly Context context;
        public readonly AutoMapper.IMapper mapper;
        public readonly IConfigurationRoot config;
        public Repository(Context context,
            AutoMapper.IMapper mapper,
            IConfigurationRoot config)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            this.context = context;
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            this.mapper = mapper;
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            this.config = config;
        }
    }
}
