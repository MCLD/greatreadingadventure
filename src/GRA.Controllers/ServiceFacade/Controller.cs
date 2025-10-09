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
        public readonly IConfiguration Config;
        public readonly IDateTimeProvider DateTimeProvider;
        public readonly MapsterMapper.IMapper Mapper;
        public readonly IPathResolver PathResolver;
        public readonly IStringLocalizer<Resources.Shared> SharedLocalizer;
        public readonly SiteLookupService SiteLookupService;
        public readonly IUserContextProvider UserContextProvider;

        public Controller(
            MapsterMapper.IMapper mapper,
            IConfiguration config,
            IDateTimeProvider dateTimeProvider,
            IPathResolver pathResolver,
            IUserContextProvider userContextProvider,
            IStringLocalizer<Resources.Shared> sharedLocalizer,
            SiteLookupService siteLookupService)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(dateTimeProvider);
            ArgumentNullException.ThrowIfNull(mapper);
            ArgumentNullException.ThrowIfNull(pathResolver);
            ArgumentNullException.ThrowIfNull(sharedLocalizer);
            ArgumentNullException.ThrowIfNull(siteLookupService);
            ArgumentNullException.ThrowIfNull(userContextProvider);

            Config = config;
            DateTimeProvider = dateTimeProvider;
            Mapper = mapper;
            PathResolver = pathResolver;
            SharedLocalizer = sharedLocalizer;
            SiteLookupService = siteLookupService;
            UserContextProvider = userContextProvider;
        }
    }
}
