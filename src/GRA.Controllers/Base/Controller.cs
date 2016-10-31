using GRA.Domain;
using Microsoft.Extensions.Configuration;
using System;

namespace GRA.Controllers
{
    public class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        protected readonly IConfigurationRoot config;
        protected readonly Service service;
        public Controller(ServiceFacade.Controller context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.config = context.config;
            this.service = context.service;
        }
    }
}
