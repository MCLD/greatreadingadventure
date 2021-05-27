using System.Collections.Generic;
using System.Linq;

namespace GRA.Controllers.ViewModel.MissionControl.Roles
{
    public class UsersInRoleViewModel : ViewModel.Shared.PaginateViewModel
    {
        public UsersInRoleViewModel()
        {
            Users = new List<Domain.Model.User>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<Domain.Model.User> Users { get; }

        public void AddUsers(IEnumerable<Domain.Model.User> users)
        {
            if (users?.Any() == true)
            {
                (Users as List<Domain.Model.User>)?.AddRange(users);
            }
        }
    }
}