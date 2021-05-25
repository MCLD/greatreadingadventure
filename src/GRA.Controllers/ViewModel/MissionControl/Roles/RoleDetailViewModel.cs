using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Roles
{
    public class RoleDetailViewModel
    {
        public string Action { get; set; }
        public string Permissions { get; set; }
        public Role Role { get; set; }
        public IEnumerable<string> SelectedPermissions { get; set; }
        public IEnumerable<string> UnselectedPermissions { get; set; }
        public int UsersInRole { get; set; }
    }
}