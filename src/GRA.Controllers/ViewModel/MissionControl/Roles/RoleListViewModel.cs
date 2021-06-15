using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Roles
{
    public class RoleListViewModel
    {
        public PaginateViewModel PaginateModel { get; set; }
        public Role Role { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public IDictionary<int, int> UsersInRoles { get; }

        public RoleListViewModel()
        {
            UsersInRoles = new Dictionary<int, int>();
        }

        public void SetUsersInRoles(IDictionary<int, int> usersInRoles)
        {
            if (usersInRoles?.Count > 0)
            {
                UsersInRoles.Clear();
                foreach (var roleId in usersInRoles.Keys)
                {
                    UsersInRoles.Add(roleId, usersInRoles[roleId]);
                }
            }
        }
    }
}