using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Roles
{
    public class RoleListViewModel
    {
        public IEnumerable<Role> Roles { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public Role Role { get; set; }
    }
}
