using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class GRABaseController : Controller
    {
        private readonly Domain.GRAService srv;

        public GRABaseController(Domain.GRAService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            this.srv = service;
        }
    }
}
