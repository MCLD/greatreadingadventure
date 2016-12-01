using GRA.Domain.Service;
using Microsoft.Extensions.Configuration;

namespace GRA.Controllers.ServiceFacade
{
    public class Controller
    {
        public readonly AutoMapper.IMapper Mapper;
        public readonly IConfigurationRoot Config;
        public readonly SiteService SiteService;

        public Controller(
            AutoMapper.IMapper mapper,
            IConfigurationRoot config,
            SiteService siteService)
        {
            Mapper = Require.IsNotNull(mapper, nameof(mapper));
            Config = Require.IsNotNull(config, nameof(config));
            SiteService = Require.IsNotNull(siteService, nameof(siteService));
        }
    }
}
