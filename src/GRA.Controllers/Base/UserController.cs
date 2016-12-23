using Microsoft.AspNetCore.Mvc;
using GRA.Controllers.Filter;

namespace GRA.Controllers.Base
{
    [ServiceFilter(typeof(UserFilter), Order = 2)]
    [ServiceFilter(typeof(NotificationFilter))]
    public abstract class UserController : Controller
    {
        public UserController(ServiceFacade.Controller context) : base(context)
        {
        }
    }
}
