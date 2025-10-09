using System;
using GRA.Abstract;
using Microsoft.Extensions.Configuration;

namespace GRA.Data.ServiceFacade
{
    public class Repository
    {
        public readonly IConfiguration config;
        public readonly Context context;
        public readonly IDateTimeProvider dateTimeProvider;
        public readonly IEntitySerializer entitySerializer;
        public readonly MapsterMapper.IMapper mapper;

        public Repository(Context context,
            MapsterMapper.IMapper mapper,
            IConfiguration config,
            IDateTimeProvider dateTimeProvider,
            IEntitySerializer entitySerializer)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(dateTimeProvider);
            ArgumentNullException.ThrowIfNull(entitySerializer);
            ArgumentNullException.ThrowIfNull(mapper);

            this.config = config;
            this.context = context;
            this.dateTimeProvider = dateTimeProvider;
            this.entitySerializer = entitySerializer;
            this.mapper = mapper;
        }
    }
}
