using GRA.Domain;
using Microsoft.Extensions.Configuration;
using System;

namespace GRA.Controllers.ServiceFacade
{
    public class Controller
    {
        public readonly IConfigurationRoot config;
        public readonly Service service;
        public Controller(IConfigurationRoot config, Service service)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            this.config = config;
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            this.service = service;
        }
    }
}
