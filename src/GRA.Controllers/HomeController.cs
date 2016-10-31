using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(Domain.Service service) : base(service) { }

        public IActionResult Index(string site = null)
        {
            var siteList = service.GetSitePaths();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
