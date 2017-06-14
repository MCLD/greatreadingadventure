using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task AddRoleAsync(int currentUserId, int userId, int roleId);
        Task<ICollection<int>> GetUserRolesAsync(int userId);
        Task<AuthenticationResult> AuthenticateUserAsync(string username, string password);
        Task<IEnumerable<int>> GetAllUserIds(int siteId);
        Task<User> GetByUsernameAsync(string username);
        Task<int> GetCountAsync(UserFilter filter);
        Task<int> GetCountAsync(ReportCriterion request);
        Task<int> GetHouseholdCountAsync(int householdHeadUserId);
        Task<DataWithId<IEnumerable<string>>> GetUserIdAndUsernames(string email);
        Task<IEnumerable<Model.User>> PageAllAsync(UserFilter filter);
        Task<IEnumerable<Model.User>>
            PageHouseholdAsync(int householdHeadUserId, int skip, int take);
        Task SetUserPasswordAsync(int currentUserId, int userId, string password);
        Task<IEnumerable<User>> GetHouseholdAsync(int householdHeadUserId);
        Task<bool> UsernameInUseAsync(int siteId, string username);
        Task<List<int>> GetUserIdsByBranchProgram(ReportCriterion criterion);
        Task<int> GetAchieverCountAsync(ReportCriterion request);
    }
}
