using GRA.Domain.Service;
using Microsoft.Extensions.Configuration;

namespace GRA.Controllers.ServiceFacade
{
    public class Controller
    {
        public readonly IConfigurationRoot Config;
        public readonly SiteService SiteService;

        public Controller(
            IConfigurationRoot config,
            SiteService siteService)
        {
            Config = Require.IsNotNull(config, nameof(config));
            SiteService = Require.IsNotNull(siteService, nameof(siteService));
        }
    }
}
