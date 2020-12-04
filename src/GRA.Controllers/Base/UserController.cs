using GRA.Controllers.Filter;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.Base
{
    [ServiceFilter(typeof(UserFilter), Order = 2)]
    [ServiceFilter(typeof(NotificationFilter))]
    public abstract class UserController : Controller
    {
        protected UserController(ServiceFacade.Controller context) : base(context)
        {
        }
    }
}
