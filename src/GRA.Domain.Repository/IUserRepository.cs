using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IUserRepository : IRepository<Model.User>
    {
        void SetUserPassword(int currentUserId, string password);
        void AddRole(int currentUserId, int userId, int roleId);
        Model.User GetByUsername(string username);
        Model.AuthenticatedUser GetByUsernamePassword(string username, string password);
    }
}
