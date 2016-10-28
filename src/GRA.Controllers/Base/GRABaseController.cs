using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class GRABaseController : Controller
    {
        protected readonly Domain.GRAService service;

        public GRABaseController(Domain.GRAService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            this.service = service;
        }
    }
}
