using System;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using System.Threading.Tasks;
using System.Collections.Generic;
using GRA.Domain.Service.Abstract;

namespace GRA.Domain.Service
{
    public class UserService : Abstract.BaseUserService<UserService>
    {
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IStaticAvatarRepository _staticAvatarRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly SampleDataService _configurationService;
        public UserService(ILogger<UserService> logger,
            IUserContextProvider userContextProvider,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IBadgeRepository badgeRepository,
            IBookRepository bookRepository,
            INotificationRepository notificationRepository,
            IProgramRepository programRepository,
            IStaticAvatarRepository staticAvatarRepository,
            IUserLogRepository userLogRepository,
            IUserRepository userRepository,
            SampleDataService configurationService)
            : base(logger, userContextProvider)
        {
            _authorizationCodeRepository = Require.IsNotNull(authorizationCodeRepository,
                nameof(authorizationCodeRepository));
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _bookRepository = Require.IsNotNull(bookRepository, nameof(bookRepository));
            _notificationRepository = Require.IsNotNull(notificationRepository,
                nameof(notificationRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _staticAvatarRepository = Require.IsNotNull(staticAvatarRepository,
                nameof(staticAvatarRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository, nameof(userLogRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _configurationService = Require.IsNotNull(configurationService,
                nameof(configurationService));
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                throw new GraException("Someone has already chosen that username, please try another.");
            }

            user.CanBeDeleted = true;
            user.IsLockedOut = false;
            var registeredUser = await _userRepository.AddSaveAsync(0, user);
            await _userRepository
                .SetUserPasswordAsync(registeredUser.Id, registeredUser.Id, password);
            return registeredUser;
        }

        public async Task<DataWithCount<IEnumerable<User>>> GetPaginatedUserListAsync(int skip,
            int take,
            string search = null,
            SortUsersBy sortBy = SortUsersBy.LastName)
        {
            int siteId = GetClaimId(ClaimType.SiteId);
            if (HasPermission(Permission.ViewParticipantList))
            {
                return new DataWithCount<IEnumerable<User>>
                {
                    Data = await _userRepository.PageAllAsync(siteId, skip, take, search, sortBy),
                    Count = await _userRepository.GetCountAsync(siteId, search)
                };
            }
            else
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to view all participants.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<User>>>
            GetPaginatedFamilyListAsync(
            int householdHeadUserId,
            int skip,
            int take)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            var authUser = await _userRepository.GetByIdAsync(authUserId);
            if (authUserId == householdHeadUserId
                || authUser.HouseholdHeadUserId == householdHeadUserId
                || HasPermission(Permission.ViewParticipantList))
            {
                return new DataWithCount<IEnumerable<User>>
                {
                    Data = await _userRepository
                        .PageHouseholdAsync(householdHeadUserId, skip, take),
                    Count = await _userRepository.GetHouseholdCountAsync(householdHeadUserId)
                };
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view household participants.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<int>
            FamilyMemberCountAsync(int householdHeadUserId)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            var authUser = await _userRepository.GetByIdAsync(authUserId);
            if (authUserId == householdHeadUserId
                || authUser.HouseholdHeadUserId == householdHeadUserId
                || HasPermission(Permission.ViewParticipantList))
            {
                return await _userRepository.GetHouseholdCountAsync(householdHeadUserId);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to get a count of household participants.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<User> GetDetails(int userId)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            var authUser = await _userRepository.GetByIdAsync(authUserId);
            var requestedUser = await _userRepository.GetByIdAsync(userId);
            if (authUserId == userId
                || requestedUser.HouseholdHeadUserId == authUserId
                || authUser.HouseholdHeadUserId == userId
                || HasPermission(Permission.ViewParticipantDetails))
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user.AvatarId != null)
                {
                    var avatar = await _staticAvatarRepository.GetByIdAsync((int)user.AvatarId);
                    if (avatar != null)
                    {
                        user.StaticAvatarFilename = avatar.Filename;
                    }
                }
                return user;
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to view participant details.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<User> Update(User userToUpdate)
        {
            int requestingUserId = GetActiveUserId();

            if (requestingUserId == userToUpdate.Id)
            {
                // users can only update some of their own fields
                var currentEntity = await _userRepository.GetByIdAsync(userToUpdate.Id);
                currentEntity.AvatarId = userToUpdate.AvatarId;
                currentEntity.BranchId = userToUpdate.BranchId;
                currentEntity.BranchName = null;
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
                //currentEntity.Username = userToUpdate.Username;
                return await _userRepository.UpdateSaveAsync(requestingUserId, currentEntity);
            }
            else
            {
                _logger.LogError($"User {requestingUserId} doesn't have permission to update user {userToUpdate.Id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<User> MCUpdate(User userToUpdate)
        {
            int requestedByUserId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.EditParticipants))
            {
                // admin users can update anything except siteid
                var currentEntity = await _userRepository.GetByIdAsync(userToUpdate.Id);
                userToUpdate.SiteId = currentEntity.SiteId;
                return await _userRepository.UpdateSaveAsync(requestedByUserId, userToUpdate);
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to update user {userToUpdate.Id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task Remove(int userIdToRemove)
        {
            int requestedByUserId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.DeleteParticipants))
            {
                var userLookup = await _userRepository.GetByIdAsync(userIdToRemove);
                if (!userLookup.CanBeDeleted)
                {
                    throw new GraException($"User {userIdToRemove} cannot be deleted.");
                }
                var familyCount = await _userRepository.GetHouseholdCountAsync(userIdToRemove);
                if (familyCount > 0)
                {
                    throw new GraException($"User {userIdToRemove} is the head of a family. Please remove all family members first.");
                }
                await _userRepository.RemoveSaveAsync(requestedByUserId, userIdToRemove);
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to remove user {userIdToRemove}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<UserLog>>>
            GetPaginatedUserHistoryAsync(int userId,
            int skip,
            int take)
        {
            int requestedByUserId = GetActiveUserId();
            if (requestedByUserId == userId
               || HasPermission(Permission.ViewParticipantDetails))
            {
                return new DataWithCount<IEnumerable<UserLog>>
                {
                    Data = await _userLogRepository.PageHistoryAsync(userId, skip, take),
                    Count = await _userLogRepository.GetHistoryItemCountAsync(userId)
                };
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to view details for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<Book>>>
            GetPaginatedUserBookListAsync(int userId, int skip, int take)
        {
            int requestedByUserId = GetActiveUserId();
            if (requestedByUserId == userId
               || HasPermission(Permission.ViewParticipantDetails))
            {
                return new DataWithCount<IEnumerable<Book>>
                {
                    Data = await _bookRepository.GetForUserAsync(userId),
                    Count = await _bookRepository.GetCountForUserAsync(userId)
                };
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to view details for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<string>
            ActivateAuthorizationCode(string authorizationCode)
        {
            string fixedCode = authorizationCode.Trim().ToLower();
            int siteId = GetClaimId(ClaimType.SiteId);
            var authCode
                = await _authorizationCodeRepository.GetByCodeAsync(siteId, fixedCode);

            if (authCode == null)
            {
                return null;
            }
            int userId = GetClaimId(ClaimType.UserId);
            await _userRepository.AddRoleAsync(userId, userId, authCode.RoleId);
            if (authCode.IsSingleUse)
            {
                await _authorizationCodeRepository.RemoveSaveAsync(userId, authCode.Id);
            }
            else
            {
                authCode.Uses++;
                await _authorizationCodeRepository.UpdateSaveAsync(userId, authCode);
            }

            // if the program doesn't have an email address assigned, perform that action here
            // TODO in the future this should be replaced with the initial setup process
            var user = await _userRepository.GetByIdAsync(userId);
            var program = await _programRepository.GetByIdAsync(user.ProgramId);
            if(string.IsNullOrEmpty(program.FromEmailAddress))
            {
                program.FromEmailAddress = user.Email;
                program.FromEmailName = user.FullName;
                await _programRepository.UpdateSaveAsync(userId, program);
            }

            return authCode.RoleName;
        }

        public async Task AddHouseholdMemberAsync(int householdHeadUserId, User memberToAdd)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            var householdHead = await _userRepository.GetByIdAsync(householdHeadUserId);

            if (householdHead.HouseholdHeadUserId != null)
            {
                _logger.LogError($"User {authUserId} cannot add a household member for {householdHeadUserId} who isn't a head of household.");
                throw new GraException("Cannot add a household member to someone who isn't a head of household.");
            }

            if (authUserId == householdHeadUserId
               || HasPermission(Permission.EditParticipants))
            {
                memberToAdd.HouseholdHeadUserId = householdHeadUserId;
                memberToAdd.SiteId = householdHead.SiteId;
                memberToAdd.CanBeDeleted = true;
                memberToAdd.IsLockedOut = false;
                var registeredUser = await _userRepository.AddSaveAsync(authUserId, memberToAdd);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to add a household member to {householdHeadUserId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task RegisterHouseholdMemberAsync(User memberToRegister, string password)
        {
            int authUserId = GetClaimId(ClaimType.UserId);

            if (authUserId == (int)memberToRegister.HouseholdHeadUserId
               || HasPermission(Permission.EditParticipants))
            {
                var user = await GetDetails(memberToRegister.Id);
                if (!string.IsNullOrWhiteSpace(user.Username))
                {
                    _logger.LogError($"User {authUserId} cannot register household member {memberToRegister.Id} who is already registered.");
                    throw new GraException("Household member is already registered");
                }

                var existingUser = await _userRepository.GetByUsernameAsync(memberToRegister.Username);
                if (existingUser != null)
                {
                    throw new GraException("Someone has already chosen that username, please try another.");
                }

                user.Username = memberToRegister.Username;
                await _userRepository.UpdateSaveAsync(authUserId, user);
                await _userRepository
                    .SetUserPasswordAsync(authUserId, user.Id, password);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to register household member {memberToRegister.Id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<Badge>>>
            GetPaginatedBadges(int userId, int skip, int take)
        {
            int activeUserId = GetActiveUserId();

            if (userId == activeUserId
                || HasPermission(Permission.ViewParticipantDetails))
            {
                return new DataWithCount<IEnumerable<Badge>>
                {
                    Data = await _badgeRepository.PageForUserAsync(userId, skip, take),
                    Count = await _badgeRepository.GetCountForUserAsync(userId)
                };
            }
            else
            {
                _logger.LogError($"User {activeUserId} doesn't have permission to view details for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<IEnumerable<Notification>> GetNotificationsForUser()
        {
            return await _notificationRepository.GetByUserIdAsync(GetActiveUserId());
        }

        public async Task ClearNotificationsForUser()
        {
            await _notificationRepository.RemoveByUserId(GetActiveUserId());
        }
    }
}