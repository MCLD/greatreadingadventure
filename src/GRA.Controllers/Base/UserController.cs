using GRA.Controllers.Filter;
using GRA.Domain.Repository.Extensions;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
