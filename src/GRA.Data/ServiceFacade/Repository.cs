using GRA.Abstract;
using Microsoft.Extensions.Configuration;
using System;

namespace GRA.Data.ServiceFacade
{
    public class Repository
    {
        public readonly Context context;
        public readonly AutoMapper.IMapper mapper;
        public readonly IConfigurationRoot config;
        public readonly IEntitySerializer entitySerializer;
        public Repository(Context context,
            AutoMapper.IMapper mapper,
            IConfigurationRoot config,
            IEntitySerializer entitySerializer)
        {
            this.context = Require.IsNotNull(context, nameof(context));
            this.mapper = Require.IsNotNull(mapper, nameof(mapper));
            this.config = Require.IsNotNull(config, nameof(config));
            this.entitySerializer = Require.IsNotNull(entitySerializer, nameof(entitySerializer));
        }
    }
}
