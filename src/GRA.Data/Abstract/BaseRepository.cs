//using System;

//namespace GRA.Data.Abstract
//{
//    using Microsoft.Extensions.Logging;

//    public class BaseRepository<T>
//    {
//        protected readonly Context context;

//        protected readonly AutoMapper.IMapper mapper;

//        protected readonly ILogger<T> logger;

//        public BaseRepository(
//            Context context, 
//            ILogger<T> logger, 
//            AutoMapper.IMapper mapper)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }
//            this.context = context;
//            if (mapper == null)
//            {
//                throw new ArgumentNullException(nameof(mapper));
//            }
//            this.mapper = mapper;
//            if (logger == null)
//            {
//                throw new ArgumentNullException(nameof(logger));
//            }
//            this.logger = logger;
//        }
//    }
//}
