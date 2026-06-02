using GRA.Controllers.Filter;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.Base
{
    [ServiceFilter(typeof(UserFilterAttribute), Order = 2)]
    [ServiceFilter(typeof(NotificationFilter))]
    public abstract class UserController(ServiceFacade.Controller context) : Controller(context)
    {
    }
}
