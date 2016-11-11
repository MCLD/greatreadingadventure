using GRA.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;

namespace GRA.Controllers.ServiceFacade
{
    public class Controller
    {
        public readonly IConfigurationRoot config;
        public readonly Service service;
        public readonly UserManager<Domain.Model.Participant> userManager;
        public Controller(IConfigurationRoot config, 
            Service service,
            UserManager<Domain.Model.Participant> userManager)
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
            if(userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }
            this.userManager = userManager;
        }
    }
}
