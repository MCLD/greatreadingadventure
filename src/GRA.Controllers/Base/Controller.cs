using GRA.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;

namespace GRA.Controllers.Base
{
    public class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        protected readonly IConfigurationRoot config;
        protected readonly Service service;
        protected readonly UserManager<Domain.Model.Participant> userManager;
        public Controller(ServiceFacade.Controller context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.config = context.config;
            this.service = context.service;
            this.userManager = context.userManager;

            // sensible default
            ViewData["Title"] = "Great Reading Adventure";
        }
    }
}
