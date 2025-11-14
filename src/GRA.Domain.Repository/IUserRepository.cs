using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task AddRoleAsync(int currentUserId, int userId, int roleId);

        Task<AuthenticationResult> AuthenticateUserAsync(
            string username,
            string password,
            string culture);

        Task ChangeDeletedUsersProgramAsync(int oldProgram, int newProgram);

        Task<int> GetAchieverCountAsync(ReportCriterion request);

        Task<IEnumerable<int>> GetAllUserIds(int siteId);

        Task<ICollection<User>> GetAllUsersWithoutUnsubscribeToken();

        Task<User> GetByUnsubscribeToken(int siteId, string token);

        Task<User> GetByUsernameAsync(string username);

        Task<User> GetContactDetailsAsync(int siteId, int userId);

        Task<int> GetCountAsync(UserFilter filter);

        Task<int> GetCountAsync(ReportCriterion request);

        Task<string> GetCultureAsync(int userId);

        Task<int> GetFirstTimeCountAsync(ReportCriterion request);

        Task<string> GetFullNameByIdAsync(int userId);

        Task<IEnumerable<User>> GetHouseholdAsync(int householdHeadUserId);

        Task<int> GetHouseholdCountAsync(int householdHeadUserId);

        Task<IEnumerable<int>> GetHouseHoldUserIdsAsync(int householdHeadUserId);

        Task<ICollection<User>> GetHouseholdUsersWithAvailablePrizeAsync(
            int headId, int? drawingId, int? triggerId);

        Task<IEnumerable<int>> GetNewsSubscribedUserIdsAsync(int siteId, bool excludeCannotBeEmailed);

        Task<IDictionary<User, int>> GetStaffRegisteredParticipantsAsync(ReportCriterion criterion);

        Task<IDictionary<string, int>> GetSubscribedLanguageCountAsync(string unspecifiedString);

        Task<int> GetSystemUserId();

        Task<IEnumerable<User>> GetTopScoresAsync(ReportCriterion criterion, int scoresToReturn);

        Task<DataWithId<IEnumerable<string>>> GetUserIdAndUsernames(string email);

        Task<List<int>> GetUserIdsByBranchProgram(ReportCriterion criterion);

        Task<ICollection<int>> GetUserRolesAsync(int userId);

        Task<IEnumerable<User>> GetUsersByCriterionAsync(ReportCriterion criterion);

        Task<ICollection<User>> GetUsersByEmailAddressAsync(string email);

        Task<DataWithCount<ICollection<User>>> GetUsersInRoleAsync(int roleId, BaseFilter filter);

        Task<IDictionary<string, DateTime>> GetViewedPackingSlipsAsync(int userId);

        Task<int> GetWelcomePendingCountAsync(int welcomeEmailId, int memberLongerThanHours);

        Task<IEnumerable<User>> GetWelcomeRecipientsAsync(int skip,
            int take,
            int memberLongerThanHours);

        Task<bool> IsAnyoneSubscribedAsync();

        Task<bool> IsEmailSubscribedAsync(string email);

        Task<IEnumerable<User>> PageAllAsync(UserFilter filter);

        Task<IEnumerable<User>> PageHouseholdAsync(
            int householdHeadUserId,
            int skip,
            int take);

        Task<int> ReassignBranchAsync(int oldBranchId, int newBranchId);

        Task SetCannotBeEmailedAsync(int currentUserId, int userId, bool cannotBeEmailed);

        Task SetUserPasswordAsync(int currentUserId, int userId, string password);

        Task<bool> UnsubscribeTokenExists(int siteId, string token);

        Task UpdateUserRolesAsync(int currentUserId,
            int userId,
            IEnumerable<int> rolesToAdd,
            IEnumerable<int> rolesToRemove);

        Task<bool> UsernameInUseAsync(int siteId, string username);

        Task ViewPackingSlipAsync(int userId, string packingSlip);
    }
}
