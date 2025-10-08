using System;
using GRA.Abstract;
using Microsoft.Extensions.Configuration;

namespace GRA.Data.ServiceFacade
{
    public class Repository
    {
        public readonly Context context;
        public readonly MapsterMapper.IMapper mapper;
        public readonly IConfiguration config;
        public readonly IDateTimeProvider dateTimeProvider;
        public readonly IEntitySerializer entitySerializer;
        public Repository(Context context,
            MapsterMapper.IMapper mapper,
            IConfiguration config,
            IDateTimeProvider dateTimeProvider,
            IEntitySerializer entitySerializer)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.dateTimeProvider = dateTimeProvider 
                ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            this.entitySerializer = entitySerializer 
                ?? throw new ArgumentNullException(nameof(entitySerializer));
        }
    }
}
