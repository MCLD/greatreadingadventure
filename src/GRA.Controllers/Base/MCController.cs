using Microsoft.AspNetCore.Mvc;
using GRA.Controllers.Filter;

namespace GRA.Controllers.Base
{
    [ServiceFilter(typeof(MissionControlFilter), Order = 2)]
    public abstract class MCController : Controller
    {
        public MCController(ServiceFacade.Controller context) : base(context)
        {
        }

        protected string GetBadgeMakerUrl(string origin, string email)
        {
            string URL = "https://www.openbadges.me/designer.html";
            URL = URL + $"?origin={origin}";
            URL = URL + $"&email={email}";
            return URL;
        }
    }
}
