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
    }
}
