using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Configuration;

namespace GRA.Controllers.ServiceFacade
{
    public class Controller
    {
        public readonly AutoMapper.IMapper Mapper;
        public readonly IConfigurationRoot Config;
        public readonly IUserContextProvider UserContextProvider;
        public readonly SiteLookupService SiteLookupService;

        public Controller(
            AutoMapper.IMapper mapper,
            IConfigurationRoot config,
            IUserContextProvider userContextProvider,
            SiteLookupService siteLookupService)
        {
            Mapper = Require.IsNotNull(mapper, nameof(mapper));
            Config = Require.IsNotNull(config, nameof(config));
            UserContextProvider = Require.IsNotNull(userContextProvider, 
                nameof(userContextProvider));
            SiteLookupService = Require.IsNotNull(siteLookupService, nameof(siteLookupService));
        }
    }
}
