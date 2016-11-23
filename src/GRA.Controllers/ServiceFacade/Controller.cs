using GRA.Domain.Service;
using Microsoft.Extensions.Configuration;
using System;

namespace GRA.Controllers.ServiceFacade
{
    public class Controller
    {
        public readonly IConfigurationRoot config;

        public Controller(
            IConfigurationRoot config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            this.config = config;
        }
    }
}
