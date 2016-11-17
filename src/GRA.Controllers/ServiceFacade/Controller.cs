using GRA.Domain.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;

namespace GRA.Controllers.ServiceFacade
{
    public class Controller
    {
        public readonly IConfigurationRoot config;
        public readonly UserManager<Domain.Model.User> userManager;

        public Controller(
            IConfigurationRoot config, 
            UserManager<Domain.Model.User> userManager)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            this.config = config;
            if(userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }
            this.userManager = userManager;
        }
    }
}
