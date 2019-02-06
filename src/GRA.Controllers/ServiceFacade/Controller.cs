using System;
using GRA.Abstract;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace GRA.Controllers.ServiceFacade
{
    public class Controller
    {
        public readonly AutoMapper.IMapper Mapper;
        public readonly IConfiguration Config;
        public readonly IDateTimeProvider DateTimeProvider;
        public readonly IPathResolver PathResolver;
        public readonly IUserContextProvider UserContextProvider;
        public readonly IStringLocalizer<Resources.Shared> SharedLocalizer;
        public readonly SiteLookupService SiteLookupService;

        public Controller(
            AutoMapper.IMapper mapper,
            IConfiguration config,
            IDateTimeProvider dateTimeProvider,
            IPathResolver pathResolver,
            IUserContextProvider userContextProvider,
            IStringLocalizer<Resources.Shared> sharedLocalizer,
            SiteLookupService siteLookupService)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            DateTimeProvider = dateTimeProvider
                ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            PathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
            UserContextProvider = userContextProvider
                ?? throw new ArgumentNullException(nameof(userContextProvider));
            SharedLocalizer = sharedLocalizer
                ?? throw new ArgumentNullException(nameof(sharedLocalizer));
            SiteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
        }
    }
}
