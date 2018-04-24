using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Roles
{
    public class AuthorizationCodeListViewModel
    {
        public IEnumerable<AuthorizationCode> AuthorizationCodes { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public AuthorizationCode AuthorizationCode { get; set; }
        public SelectList RoleList { get; set; }
    }
}
