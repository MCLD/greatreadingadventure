using System;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;

namespace GRA.Domain.Service
{
    public class UserService : Abstract.BaseService<UserService>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        public UserService(ILogger<UserService> logger,
            IBookRepository bookRepository,
            IUserLogRepository userLogRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
            : base(logger)
        {
            _bookRepository = Require.IsNotNull(bookRepository, nameof(bookRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository, nameof(userLogRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _roleRepository = Require.IsNotNull(roleRepository, nameof(roleRepository));
        }

        public async Task<AuthenticationResult> AuthenticateUserAsync(string username,
            string password)
        {
            var authResult = await _userRepository.AuthenticateUserAsync(username, password);

            if (!authResult.FoundUser)
            {
                authResult.AuthenticationMessage = $"Could not find username '{username}'";
            }
            else if (!authResult.PasswordIsValid)
            {
                authResult.AuthenticationMessage = "The provided password is incorrect.";
            }
            else
            {
                authResult.PermissionNames
                    = await _roleRepository.GetPermisisonNamesForUserAsync(authResult.User.Id);
            }
            return authResult;
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                throw new Exception("Someone has already chosen that username, please try another.");
            }

            user.CanBeDeleted = true;
            user.IsLockedOut = false;
            var registeredUser = await _userRepository.AddSaveAsync(0, user);
            await _userRepository.SetUserPasswordAsync(registeredUser.Id, registeredUser.Id, password);
            return registeredUser;
        }

        public async Task ResetPassword(ClaimsPrincipal user, int userIdToReset, string password)
        {
            int userId = GetId(user, ClaimType.UserId);
            if (userId == userIdToReset
                || UserHasPermission(user, Permission.EditParticipants))
            {
                await _userRepository.SetUserPasswordAsync(userId, userIdToReset, password);
            }
            else
            {
                logger.LogError($"User {userId} doesn't have permission to reset password for {userIdToReset}.");
                throw new Exception("Permission denied.");
            }

        }

        public async Task<DataWithCount<IEnumerable<User>>>
           GetPaginatedUserListAsync(ClaimsPrincipal user,
           int skip,
           int take)
        {
            int siteId = GetId(user, ClaimType.SiteId);
            if (UserHasPermission(user, Permission.ViewParticipantList))
            {
                var dataTask = _userRepository.PageAllAsync(siteId, skip, take);
                var countTask = _userRepository.GetCountAsync();
                await Task.WhenAll(dataTask, countTask);
                return new DataWithCount<IEnumerable<User>>
                {
                    Data = dataTask.Result,
                    Count = countTask.Result
                };
            }
            else
            {
                int userId = GetId(user, ClaimType.UserId);
                logger.LogError($"User {userId} doesn't have permission to view all participants.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<User>>>
            GetPaginatedFamilyListAsync(ClaimsPrincipal user,
            int householdHeadUserId,
            int skip,
            int take)
        {
            int requestingUserId = GetId(user, ClaimType.UserId);
            if (requestingUserId == householdHeadUserId
                || UserHasPermission(user, Permission.ViewParticipantList))
            {
                var dataTask = _userRepository.PageFamilyAsync(householdHeadUserId, skip, take);
                var countTask = _userRepository.GetFamilyCountAsync(householdHeadUserId);
                await Task.WhenAll(dataTask, countTask);
                return new DataWithCount<IEnumerable<User>>
                {
                    Data = dataTask.Result,
                    Count = countTask.Result
                };
            }
            else
            {
                logger.LogError($"User {requestingUserId} doesn't have permission to view household participants.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<int>
            FamilyMemberCountAsync(ClaimsPrincipal user, int householdHeadUserId)
        {
            int requestingUserId = GetId(user, ClaimType.UserId);
            if (requestingUserId == householdHeadUserId
                || UserHasPermission(user, Permission.ViewParticipantList))
            {
                return await _userRepository.GetFamilyCountAsync(householdHeadUserId);
            }
            else
            {
                logger.LogError($"User {requestingUserId} doesn't have permission to get a count of household participants.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<User> GetDetails(ClaimsPrincipal user, int userId)
        {
            if (UserHasPermission(user, Permission.ViewParticipantDetails))
            {
                return await _userRepository.GetByIdAsync(userId);
            }
            else
            {
                int requestedByUserId = GetId(user, ClaimType.UserId);
                logger.LogError($"User {requestedByUserId} doesn't have permission to view participant details.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<User> Update(ClaimsPrincipal user, User userToUpdate)
        {
            int requestedByUserId = GetId(user, ClaimType.UserId);

            if (UserHasPermission(user, Permission.EditParticipants))
            {
                // admin users can update anything except siteid
                var currentEntity = await _userRepository.GetByIdAsync(userToUpdate.Id);
                userToUpdate.SiteId = currentEntity.SiteId;
                return await _userRepository.UpdateSaveAsync(requestedByUserId, userToUpdate);
            }
            else if (requestedByUserId == userToUpdate.Id)
            {
                // users can only update some of their own fields
                var currentEntity = await _userRepository.GetByIdAsync(userToUpdate.Id);
                currentEntity.AvatarId = userToUpdate.AvatarId;
                currentEntity.BranchId = userToUpdate.BranchId;
                currentEntity.BranchName = null;
                currentEntity.CanBeDeleted = userToUpdate.CanBeDeleted;
                currentEntity.CardNumber = userToUpdate.CardNumber;
                currentEntity.Email = userToUpdate.Email;
                currentEntity.FirstName = userToUpdate.FirstName;
                currentEntity.LastName = userToUpdate.LastName;
                currentEntity.PhoneNumber = userToUpdate.PhoneNumber;
                currentEntity.PostalCode = userToUpdate.PostalCode;
                currentEntity.ProgramId = userToUpdate.ProgramId;
                currentEntity.ProgramName = null;
                currentEntity.SystemId = userToUpdate.SystemId;
                currentEntity.SystemName = null;
                currentEntity.Username = userToUpdate.Username;
                return await _userRepository.UpdateSaveAsync(requestedByUserId, currentEntity);
            }
            else
            {
                logger.LogError($"User {requestedByUserId} doesn't have permission to update user {userToUpdate.Id}.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task Remove(ClaimsPrincipal user, int userIdToRemove)
        {
            int requestedByUserId = GetId(user, ClaimType.UserId);

            if (UserHasPermission(user, Permission.DeleteParticipants))
            {
                var userLookup = await _userRepository.GetByIdAsync(userIdToRemove);
                if (!userLookup.CanBeDeleted)
                {
                    throw new Exception($"User {userIdToRemove} cannot be deleted.");
                }
                var familyCount = await _userRepository.GetFamilyCountAsync(userIdToRemove);
                if (familyCount > 0)
                {
                    throw new Exception($"User {userIdToRemove} is the head of a family. Please remove all family members first.");
                }
                await _userRepository.RemoveSaveAsync(requestedByUserId, userIdToRemove);
            }
            else
            {
                logger.LogError($"User {requestedByUserId} doesn't have permission to remove user {userIdToRemove}.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<UserLog>>>
            GetPaginatedUserHistoryAsync(ClaimsPrincipal user,
            int userId,
            int skip,
            int take)
        {
            int requestedByUserId = GetId(user, ClaimType.UserId);
            if (requestedByUserId == userId
               || UserHasPermission(user, Permission.ViewParticipantDetails))
            {
                var dataTask = _userLogRepository.PageHistoryAsync(userId, skip, take);
                var countTask = _userLogRepository.GetHistoryItemCountAsync(userId);
                await Task.WhenAll(dataTask, countTask);
                return new DataWithCount<IEnumerable<UserLog>>
                {
                    Data = dataTask.Result,
                    Count = countTask.Result
                };
            }
            else
            {
                logger.LogError($"User {requestedByUserId} doesn't have permission to view details for {userId}.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<Book>>>
            GetPaginatedUserBookListAsync(ClaimsPrincipal user, int userId, int skip, int take)
        {
            int requestedByUserId = GetId(user, ClaimType.UserId);
            if (requestedByUserId == userId
               || UserHasPermission(user, Permission.ViewParticipantDetails))
            {
                var dataTask = _bookRepository.GetForUserAsync(userId);
                var countTask = _bookRepository.GetCountForUserAsync(userId);
                await Task.WhenAll(dataTask, countTask);
                return new DataWithCount<IEnumerable<Book>>
                {
                    Data = dataTask.Result,
                    Count = countTask.Result
                };
            }
            else
            {
                logger.LogError($"User {requestedByUserId} doesn't have permission to view details for {userId}.");
                throw new Exception("Permission denied.");
            }
        }
    }
}