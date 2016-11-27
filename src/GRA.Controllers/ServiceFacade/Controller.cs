using Microsoft.Extensions.Configuration;

namespace GRA.Controllers.ServiceFacade
{
    public class Controller
    {
        public readonly IConfigurationRoot config;

        public Controller(
            IConfigurationRoot config)
        {
            this.config = Require.IsNotNull(config, nameof(config));
        }
    }
}
