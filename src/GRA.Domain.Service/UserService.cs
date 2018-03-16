using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class UserService : Abstract.BaseUserService<UserService>
    {
        private readonly GRA.Abstract.IPasswordValidator _passwordValidator;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IGroupInfoRepository _groupInfoRepository;
        private readonly IGroupTypeRepository _groupTypeRepository;
        private readonly IMailRepository _mailRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPrizeWinnerRepository _prizeWinnerRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IRequiredQuestionnaireRepository _requireQuestionnaireRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVendorCodeRepository _vendorCodeRepository;
        private readonly ActivityService _activityService;
        private readonly SampleDataService _configurationService;
        private readonly SchoolService _schoolService;
        private readonly SiteLookupService _siteLookupService;
        private readonly VendorCodeService _vendorCodeService;
        public UserService(ILogger<UserService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            GRA.Abstract.IPasswordValidator passwordValidator,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IBadgeRepository badgeRepository,
            IBookRepository bookRepository,
            IBranchRepository branchRepository,
            IGroupInfoRepository groupInfoRepository,
            IGroupTypeRepository groupTypeRepository,
            IMailRepository mailRepository,
            INotificationRepository notificationRepository,
            IPrizeWinnerRepository prizeWinnerRepository,
            IProgramRepository programRepository,
            IRequiredQuestionnaireRepository requireQuestionnaireRepository,
            IRoleRepository roleRepository,
            ISchoolRepository schoolRepository,
            ISiteRepository siteRepository,
            ISystemRepository systemRepository,
            IUserLogRepository userLogRepository,
            IUserRepository userRepository,
            IVendorCodeRepository vendorCodeRepository,
            ActivityService activityService,
            SampleDataService configurationService,
            SchoolService schoolService,
            SiteLookupService siteLookupService,
            VendorCodeService vendorCodeService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _passwordValidator = Require.IsNotNull(passwordValidator, nameof(passwordValidator));
            _authorizationCodeRepository = Require.IsNotNull(authorizationCodeRepository,
                nameof(authorizationCodeRepository));
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _bookRepository = Require.IsNotNull(bookRepository, nameof(bookRepository));
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _groupInfoRepository = groupInfoRepository
                ?? throw new ArgumentNullException(nameof(groupInfoRepository));
            _groupTypeRepository = groupTypeRepository
                ?? throw new ArgumentNullException(nameof(groupTypeRepository));
            _mailRepository = Require.IsNotNull(mailRepository, nameof(mailRepository));
            _notificationRepository = Require.IsNotNull(notificationRepository,
                nameof(notificationRepository));
            _prizeWinnerRepository = Require.IsNotNull(prizeWinnerRepository,
                nameof(prizeWinnerRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _requireQuestionnaireRepository = Require.IsNotNull(requireQuestionnaireRepository,
                nameof(requireQuestionnaireRepository));
            _roleRepository = Require.IsNotNull(roleRepository, nameof(roleRepository));
            _schoolRepository = Require.IsNotNull(schoolRepository, nameof(schoolRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _systemRepository = Require.IsNotNull(systemRepository, nameof(systemRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository, nameof(userLogRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _vendorCodeRepository = Require.IsNotNull(vendorCodeRepository, nameof(vendorCodeRepository));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _configurationService = Require.IsNotNull(configurationService,
                nameof(configurationService));
            _schoolService = Require.IsNotNull(schoolService, nameof(schoolService));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(VendorCodeService));
        }

        public async Task<User> RegisterUserAsync(User user, string password,
            bool MCRegistration = false, bool allowDuringCloseProgram = false)
        {
            if (allowDuringCloseProgram == false)
            {
                VerifyCanRegister();
            }

            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                throw new GraException("Someone has already chosen that username, please try another.");
            }

            await ValidateUserFields(user);

            _passwordValidator.Validate(password);

            user.CanBeDeleted = true;
            user.IsLockedOut = false;

            user.CardNumber = user.CardNumber?.Trim();
            user.Email = user.Email?.Trim();
            user.FirstName = user.FirstName?.Trim();
            user.LastName = user.LastName?.Trim();
            user.PhoneNumber = user.PhoneNumber?.Trim();
            user.PostalCode = user.PostalCode?.Trim();
            user.Username = user.Username?.Trim();

            var registeredUser = new User();
            if (MCRegistration)
            {
                registeredUser = await _userRepository.AddSaveAsync(
                    GetClaimId(ClaimType.UserId), user);
            }
            else
            {
                registeredUser = await _userRepository.AddSaveAsync(0, user);
            }

            await _userRepository
                .SetUserPasswordAsync(registeredUser.Id, registeredUser.Id, password);

            await JoinedProgramNotificationBadge(registeredUser);
            await _activityService.AwardUserTriggersAsync(registeredUser.Id, false);

            return registeredUser;
        }

        public async Task<DataWithCount<IEnumerable<User>>> GetPaginatedUserListAsync(
            UserFilter filter)
        {
            if (HasPermission(Permission.ViewParticipantList))
            {
                filter.SiteId = GetClaimId(ClaimType.SiteId);
                return new DataWithCount<IEnumerable<User>>
                {
                    Data = await _userRepository.PageAllAsync(filter),
                    Count = await _userRepository.GetCountAsync(filter)
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
                _logger.LogError($"User {authUserId} doesn't have permission to view family/group participants.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<int>
            FamilyMemberCountAsync(int householdHeadUserId, bool skipPermissions = false)
        {
            if (skipPermissions)
            {
                return await _userRepository.GetHouseholdCountAsync(householdHeadUserId);
            }
            else
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
                    _logger.LogError($"User {authUserId} doesn't have permission to get a count of family/group participants.");
                    throw new GraException("Permission denied.");
                }
            }
        }

        public async Task<User> GetDetails(int userId)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            var authUser = await _userRepository.GetByIdAsync(authUserId);
            var requestedUser = await _userRepository.GetByIdAsync(userId);
            if (requestedUser == null)
            {
                throw new GraException("The requested participant could not be accessed or does not exist.");
            }
            if (authUserId == userId
                || requestedUser.HouseholdHeadUserId == authUserId
                || authUser.HouseholdHeadUserId == userId
                || HasPermission(Permission.ViewParticipantDetails))
            {
                return requestedUser;
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
                currentEntity.IsAdmin = await UserHasRoles(userToUpdate.Id);
                currentEntity.Age = userToUpdate.Age;
                currentEntity.AvatarId = userToUpdate.AvatarId;
                currentEntity.BranchName = null;
                currentEntity.CardNumber = userToUpdate.CardNumber?.Trim();
                currentEntity.DailyPersonalGoal = userToUpdate.DailyPersonalGoal;
                currentEntity.Email = userToUpdate.Email?.Trim();
                currentEntity.FirstName = userToUpdate.FirstName?.Trim();
                currentEntity.IsHomeschooled = userToUpdate.IsHomeschooled;
                currentEntity.LastName = userToUpdate.LastName?.Trim();
                currentEntity.PhoneNumber = userToUpdate.PhoneNumber?.Trim();
                currentEntity.PostalCode = userToUpdate.PostalCode?.Trim();
                currentEntity.ProgramId = userToUpdate.ProgramId;
                currentEntity.ProgramName = null;
                currentEntity.SchoolId = userToUpdate.SchoolId;
                currentEntity.SchoolNotListed = userToUpdate.SchoolNotListed;
                currentEntity.SystemName = null;
                //currentEntity.Username = userToUpdate.Username;

                bool restrictChangingSystemBranch = await _siteLookupService
                    .GetSiteSettingBoolAsync(currentEntity.SiteId,
                    SiteSettingKey.Users.RestrictChangingSystemBranch);

                if (!restrictChangingSystemBranch)
                {
                    currentEntity.SystemId = userToUpdate.SystemId;
                    currentEntity.BranchId = userToUpdate.BranchId;
                }

                await ValidateUserFields(currentEntity);

                var updatedUser = await _userRepository
                    .UpdateSaveAsync(requestingUserId, currentEntity);

                return updatedUser;
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
                var currentEntity = await _userRepository.GetByIdAsync(userToUpdate.Id);
                currentEntity.IsAdmin = await UserHasRoles(userToUpdate.Id);
                currentEntity.Age = userToUpdate.Age;
                currentEntity.AvatarId = userToUpdate.AvatarId;
                currentEntity.BranchId = userToUpdate.BranchId;
                currentEntity.BranchName = null;
                currentEntity.CardNumber = userToUpdate.CardNumber?.Trim();
                currentEntity.DailyPersonalGoal = userToUpdate.DailyPersonalGoal;
                currentEntity.Email = userToUpdate.Email?.Trim();
                currentEntity.FirstName = userToUpdate.FirstName?.Trim();
                currentEntity.IsHomeschooled = userToUpdate.IsHomeschooled;
                currentEntity.LastName = userToUpdate.LastName?.Trim();
                currentEntity.PhoneNumber = userToUpdate.PhoneNumber?.Trim();
                currentEntity.PostalCode = userToUpdate.PostalCode?.Trim();
                currentEntity.ProgramId = userToUpdate.ProgramId;
                currentEntity.ProgramName = null;
                currentEntity.SchoolId = userToUpdate.SchoolId;
                currentEntity.SchoolNotListed = userToUpdate.SchoolNotListed;
                currentEntity.SystemId = userToUpdate.SystemId;
                currentEntity.SystemName = null;

                if (HasPermission(Permission.EditParticipantUsernames)
                    && !string.IsNullOrWhiteSpace(currentEntity.Username)
                    && !string.IsNullOrWhiteSpace(userToUpdate.Username))
                {
                    if (!string.Equals(userToUpdate.Username, currentEntity.Username,
                    System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (await UsernameInUseAsync(userToUpdate.Username))
                        {
                            throw new GraException("Username is in use by another user.");
                        }
                        else
                        {
                            currentEntity.Username = userToUpdate.Username?.Trim();
                        }
                    }
                }

                await ValidateUserFields(currentEntity);

                var updatedUser = await _userRepository
                    .UpdateSaveAsync(requestedByUserId, currentEntity);

                return updatedUser;
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to update user {userToUpdate.Id}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task RemoveAsync(int userIdToRemove)
        {
            int requestedByUserId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.DeleteParticipants))
            {
                var user = await _userRepository.GetByIdAsync(userIdToRemove);
                if (!user.CanBeDeleted)
                {
                    throw new GraException($"{user.FullName} cannot be deleted.");
                }
                var familyCount = await _userRepository.GetHouseholdCountAsync(userIdToRemove);
                if (familyCount > 0)
                {
                    var group = await _groupInfoRepository.GetByUserIdAsync(user.Id);
                    string callIt = "family";
                    if (group != null)
                    {
                        callIt = "group";
                    }
                    throw new GraException($"{user.FullName} is the head of a {callIt}. Please remove all {callIt} members first.");
                }
                user.IsDeleted = true;
                user.Username = null;
                await _userRepository.UpdateSaveAsync(requestedByUserId, user);
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

        public async Task<DataWithCount<ICollection<Book>>>
            GetPaginatedUserBookListAsync(int userId, BookFilter filter)
        {
            int requestedByUserId = GetActiveUserId();
            if (requestedByUserId == userId
               || HasPermission(Permission.ViewParticipantDetails))
            {
                filter.UserIds = new List<int>() { userId };
                return await _bookRepository.GetPaginatedListForUserAsync(filter);
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to view details for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<string>
            ActivateAuthorizationCode(string authorizationCode, int? joiningUserId = null)
        {
            string fixedCode = authorizationCode.Trim().ToLower();
            int siteId = GetCurrentSiteId();
            var authCode
                = await _authorizationCodeRepository.GetByCodeAsync(siteId, fixedCode);

            if (authCode == null)
            {
                return null;
            }
            int userId = joiningUserId ?? GetClaimId(ClaimType.UserId);

            var userRoles = await _userRepository.GetUserRolesAsync(userId);
            if (userRoles.Contains(authCode.RoleId))
            {
                throw new GraException($"You already belong to the role '{authCode.RoleName}'");
            }

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
            var site = await _siteRepository.GetByIdAsync(user.SiteId);
            if (string.IsNullOrEmpty(site.FromEmailAddress))
            {
                site.FromEmailAddress = user.Email;
                site.FromEmailName = user.FullName;
                await _siteRepository.UpdateSaveAsync(userId, site);
            }

            user.IsAdmin = true;
            await _userRepository.UpdateSaveAsync(userId, user);

            return authCode.RoleName;
        }

        public async Task<bool> ValidateAuthorizationCode(string authorizationCode)
        {
            string fixedCode = authorizationCode.Trim().ToLower();
            int siteId = GetCurrentSiteId();
            var authCode = await _authorizationCodeRepository.GetByCodeAsync(siteId, fixedCode);
            if (authCode == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<User> AddHouseholdMemberAsync(int householdHeadUserId, User memberToAdd)
        {
            VerifyCanHouseholdAction();

            int authUserId = GetClaimId(ClaimType.UserId);
            var householdHead = await _userRepository.GetByIdAsync(householdHeadUserId);

            if (householdHead.HouseholdHeadUserId != null)
            {
                _logger.LogError($"User {authUserId} cannot add a household member for {householdHeadUserId} who isn't a head of family/group.");
                throw new GraException("Cannot add a household member to someone who isn't a head of family/group.");
            }

            if (authUserId == householdHeadUserId
               || HasPermission(Permission.EditParticipants))
            {
                memberToAdd.HouseholdHeadUserId = householdHeadUserId;
                memberToAdd.SiteId = householdHead.SiteId;
                memberToAdd.CanBeDeleted = true;
                memberToAdd.IsLockedOut = false;
                memberToAdd.IsAdmin = false;

                memberToAdd.CardNumber = memberToAdd.CardNumber?.Trim();
                memberToAdd.Email = memberToAdd.Email?.Trim();
                memberToAdd.FirstName = memberToAdd.FirstName?.Trim();
                memberToAdd.LastName = memberToAdd.LastName?.Trim();
                memberToAdd.PhoneNumber = memberToAdd.PhoneNumber?.Trim();
                memberToAdd.PostalCode = memberToAdd.PostalCode?.Trim();
                memberToAdd.Username = memberToAdd.Username?.Trim();

                await ValidateUserFields(memberToAdd);

                var registeredUser = await _userRepository.AddSaveAsync(authUserId, memberToAdd);
                await JoinedProgramNotificationBadge(registeredUser);
                await _activityService.AwardUserTriggersAsync(registeredUser.Id, false);
                return registeredUser;
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to add a family/group member to {householdHeadUserId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task RegisterHouseholdMemberAsync(User memberToRegister, string password)
        {
            VerifyCanRegister();
            int authUserId = GetClaimId(ClaimType.UserId);

            if (authUserId == (int)memberToRegister.HouseholdHeadUserId
               || HasPermission(Permission.EditParticipants))
            {
                var user = await _userRepository.GetByIdAsync(memberToRegister.Id);
                if (!string.IsNullOrWhiteSpace(user.Username))
                {
                    _logger.LogError($"User {authUserId} cannot register family/group member {memberToRegister.Id} who is already registered.");
                    throw new GraException("Household member is already registered");
                }

                var existingUser = await _userRepository.GetByUsernameAsync(memberToRegister.Username);
                if (existingUser != null)
                {
                    throw new GraException("Someone has already chosen that username, please try another.");
                }

                _passwordValidator.Validate(password);

                user.Username = memberToRegister.Username?.Trim();
                var registeredUser = await _userRepository.UpdateSaveAsync(authUserId, user);
                await _userRepository
                    .SetUserPasswordAsync(authUserId, user.Id, password);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to register family/group member {memberToRegister.Id}.");
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

        public async Task<(int totalAddCount, int addUserId)>
            CountParticipantsToAdd(string username, string password)
        {
            string trimmedUsername = username.Trim();
            VerifyCanHouseholdAction();

            var authUser = await _userRepository.GetByIdAsync(GetClaimId(ClaimType.UserId));

            if (authUser.HouseholdHeadUserId != null)
            {
                throw new GraException("Only a family or group manager can add members");
            }

            var authenticationResult = await _userRepository.AuthenticateUserAsync(trimmedUsername, password);
            if (!authenticationResult.PasswordIsValid)
            {
                throw new GraException("The username and password entered do not match");
            }
            if (authenticationResult.User.Id == authUser.Id)
            {
                throw new GraException("You cannot add yourself");
            }

            // one for the person we are adding
            int totalAddCount = 1;
            if (authenticationResult.User.HouseholdHeadUserId == null)
            {
                var household = await _userRepository
                    .GetHouseholdAsync(authenticationResult.User.Id);
                // add on the total household count
                totalAddCount += household.Count();
            }

            return (totalAddCount: totalAddCount, addUserId: authenticationResult.User.Id);
        }

        public async Task<string> AddParticipantToHouseholdAlreadyAuthorizedAsync(int userId)
        {
            VerifyCanHouseholdAction();

            var authUser = await _userRepository.GetByIdAsync(GetClaimId(ClaimType.UserId));

            if (authUser.HouseholdHeadUserId != null)
            {
                throw new GraException("Only a family or group manager can add members");
            }

            var user = await _userRepository.GetByIdAsync(userId);

            bool hasFamily = false;
            if (user.HouseholdHeadUserId == null)
            {
                var household = await _userRepository
                    .GetHouseholdAsync(user.Id);
                foreach (var member in household)
                {
                    hasFamily = true;
                    member.HouseholdHeadUserId = authUser.Id;
                    await _userRepository.UpdateSaveAsync(authUser.Id, member);
                }
            }

            var isGroup = await _groupInfoRepository.GetByUserIdAsync(authUser.Id);
            var callIt = isGroup != null ? "group" : "family";

            if (hasFamily == true)
            {
                var infoGroup
                    = await _groupInfoRepository.GetByUserIdAsync(user.Id);
                if (infoGroup != null)
                {
                    _logger.LogInformation($"Existing {callIt} manager {user.Id} is is added to new head {authUser.Id}, group {infoGroup.Id}/{infoGroup.Name} is being removed.");
                    await _groupInfoRepository.RemoveSaveAsync(authUser.Id, infoGroup.Id);
                }
            }

            user.HouseholdHeadUserId = authUser.Id;
            await _userRepository.UpdateSaveAsync(authUser.Id, user);
            string addedMembers = user.FullName;
            if (hasFamily)
            {
                addedMembers += $" and their {callIt}";
            }
            return addedMembers;
        }


        public async Task<string> AddParticipantToHouseholdAsync(string username, string password)
        {
            VerifyCanHouseholdAction();

            var authUser = await _userRepository.GetByIdAsync(GetClaimId(ClaimType.UserId));

            if (authUser.HouseholdHeadUserId != null)
            {
                throw new GraException("Only a family or group manager can add members");
            }

            string trimmedUsername = username.Trim();
            var authenticationResult = await _userRepository.AuthenticateUserAsync(trimmedUsername, password);
            if (!authenticationResult.PasswordIsValid)
            {
                throw new GraException("The username and password entered do not match");
            }
            if (authenticationResult.User.Id == authUser.Id)
            {
                throw new GraException("You cannot add yourself");
            }

            return await AddParticipantToHouseholdAlreadyAuthorizedAsync(authenticationResult.User.Id);
        }

        public async Task<IEnumerable<User>> GetHouseholdAsync(int householdHeadUserId,
            bool includePendingQuestionnaire, bool includeVendorCode, bool includeMail,
            bool includePrize = false)
        {
            var authId = GetClaimId(ClaimType.UserId);
            if (!HasPermission(Permission.ViewParticipantDetails)
                && householdHeadUserId != authId)
            {
                var authUser = await _userRepository.GetByIdAsync(authId);
                if (authUser.HouseholdHeadUserId != householdHeadUserId)
                {
                    _logger.LogError($"User {authId} doesn't have permission to view details for {householdHeadUserId}.");
                    throw new GraException("Permission denied.");
                }
            }

            var household = await _userRepository.GetHouseholdAsync(householdHeadUserId);

            if (includeVendorCode || includeMail || includePrize || includePendingQuestionnaire)
            {
                if (includeMail && householdHeadUserId != authId
                    && !HasPermission(Permission.ReadAllMail))
                {
                    _logger.LogError($"User {authId} doesn't have permission to view mail for {householdHeadUserId}.");
                    throw new GraException("Permission denied.");
                }

                if (includePrize && !HasPermission(Permission.ViewUserPrizes))
                {
                    _logger.LogError($"User {authId} doesn't have permission to view prizes for {householdHeadUserId}.");
                    throw new GraException("Permission denied.");
                }
                var siteId = GetCurrentSiteId();
                foreach (var member in household)
                {
                    if (includeMail)
                    {
                        member.HasNewMail = await _mailRepository.UserHasUnreadAsync(member.Id);
                    }
                    if (includePrize)
                    {
                        member.HasUnclaimedPrize = (await _prizeWinnerRepository
                            .CountByWinningUserId(GetCurrentSiteId(), member.Id, false)) > 0;
                    }
                    if (includeVendorCode)
                    {
                        await _vendorCodeService.PopulateVendorCodeStatusAsync(member);
                    }
                    if (includePendingQuestionnaire)
                    {
                        member.HasPendingQuestionnaire = (await _requireQuestionnaireRepository
                            .GetForUser(siteId, member.Id, member.Age)).Any();
                    }
                }
            }
            return household;
        }

        public async Task PromoteToHeadOfHouseholdAsync(int userId)
        {
            VerifyCanHouseholdAction();

            var authId = GetClaimId(ClaimType.UserId);
            if (!HasPermission(Permission.EditParticipants))
            {
                _logger.LogError($"User {authId} doesn't have permission to promote family/group members to manager.");
                throw new GraException("Permission denied.");
            }

            var user = await _userRepository.GetByIdAsync(userId); ;
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                _logger.LogError($"User {userId} cannot be promoted to family/group manager without a username.");
                throw new GraException("User does not have a username.");
            }
            if (!user.HouseholdHeadUserId.HasValue)
            {
                _logger.LogError($"User {userId} cannot be promoted to family/group manager.");
                throw new GraException("User does not have a family/group or is already the manager.");
            }
            var headUser = await _userRepository.GetByIdAsync(user.HouseholdHeadUserId.Value);
            var household = await _userRepository.GetHouseholdAsync(user.HouseholdHeadUserId.Value);

            int oldHeadUserId = headUser.Id;

            user.HouseholdHeadUserId = null;
            await _userRepository.UpdateSaveAsync(authId, user);
            headUser.HouseholdHeadUserId = user.Id;
            await _userRepository.UpdateSaveAsync(authId, headUser);
            foreach (var member in household)
            {
                if (member.Id != user.Id)
                {
                    member.HouseholdHeadUserId = user.Id;
                    await _userRepository.UpdateAsync(authId, member);
                }
            }

            var groupInfo = await _groupInfoRepository.GetByUserIdAsync(oldHeadUserId);
            if (groupInfo != null)
            {
                groupInfo.UserId = user.Id;
            }

            await _userRepository.SaveAsync();
        }

        public async Task RemoveFromHouseholdAsync(int userId)
        {
            VerifyCanHouseholdAction();

            var authId = GetClaimId(ClaimType.UserId);
            if (!HasPermission(Permission.EditParticipants))
            {
                _logger.LogError($"User {authId} doesn't have permission to remove family/group members.");
                throw new GraException("Permission denied.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                _logger.LogError($"User {userId} cannot be removed from a family/group without a username.");
                throw new GraException("Participant does not have a username.");
            }
            if (!user.HouseholdHeadUserId.HasValue)
            {
                _logger.LogError($"User {userId} cannot be removed from a family/group.");
                throw new GraException("Participant does not have a family/group or is the manager.");
            }
            user.HouseholdHeadUserId = null;
            await _userRepository.UpdateSaveAsync(authId, user);
        }

        public async Task MCAddParticipantToHouseholdAsync(int householdId, int userToAddId)
        {
            VerifyCanHouseholdAction();

            var authId = GetClaimId(ClaimType.UserId);
            if (!HasPermission(Permission.EditParticipants))
            {
                _logger.LogError($"User {authId} doesn't add existing participants to family/group.");
                throw new GraException("Permission denied.");
            }
            var userToAdd = await _userRepository.GetByIdAsync(userToAddId);
            if (userToAdd.HouseholdHeadUserId.HasValue
                || (await _userRepository.GetHouseholdCountAsync(userToAddId)) > 0)
            {
                _logger.LogError($"User {authId} cannot add {userToAddId} to a different family/group.");
                throw new GraException("Participant already belongs to a family/group.");
            }
            var user = await _userRepository.GetByIdAsync(householdId);
            userToAdd.HouseholdHeadUserId = user.HouseholdHeadUserId ?? user.Id;

            await _userRepository.UpdateSaveAsync(authId, userToAdd);
        }

        public async Task<bool> UsernameInUseAsync(string username)
        {
            string trimmedUsername = username.Trim();
            return await _userRepository.UsernameInUseAsync(GetCurrentSiteId(), trimmedUsername);
        }

        private async Task<bool> UserHasRoles(int userId)
        {
            var roles = await _roleRepository.GetPermisisonNamesForUserAsync(userId);
            return roles != null && roles.Count() > 0;
        }

        private async Task JoinedProgramNotificationBadge(User registeredUser)
        {
            var program = await _programRepository.GetByIdAsync(registeredUser.ProgramId);
            var site = await _siteRepository.GetByIdAsync(registeredUser.SiteId);

            var notification = new Notification
            {
                PointsEarned = 0,
                Text = $"<span class=\"fa fa-thumbs-o-up\"></span> You've successfully joined <strong>{site.Name}</strong>!",
                UserId = registeredUser.Id,
                IsJoining = true
            };

            if (program.JoinBadgeId != null)
            {
                var badge = await _badgeRepository.GetByIdAsync((int)program.JoinBadgeId);
                await _badgeRepository.AddUserBadge(registeredUser.Id, badge.Id);
                await _userLogRepository.AddAsync(registeredUser.Id, new UserLog
                {
                    UserId = registeredUser.Id,
                    PointsEarned = 0,
                    IsDeleted = false,
                    BadgeId = badge.Id,
                    Description = $"Joined {site.Name}!"
                });
                notification.BadgeId = badge.Id;
                notification.BadgeFilename = badge.Filename;
            }
            await _notificationRepository.AddSaveAsync(registeredUser.Id, notification);
        }

        private async Task ValidateUserFields(User user)
        {
            if (!(await _systemRepository.ValidateAsync(user.SystemId, user.SiteId)))
            {
                throw new GraException("Invalid System selection.");
            }
            if (!(await _branchRepository.ValidateAsync(user.BranchId, user.SystemId)))
            {
                throw new GraException("Invalid Branch selection.");
            }
            if (!(await _programRepository.ValidateAsync(user.ProgramId, user.SiteId)))
            {
                throw new GraException("Invalid Program selection.");
            }
            if (user.SchoolId.HasValue)
            {
                if (!(await _schoolRepository.ValidateAsync(user.SchoolId.Value, user.SiteId)))
                {
                    throw new GraException("Invalid School selection.");
                }
            }
        }

        public async Task<GroupInfo> GetGroupFromHouseholdHeadAsync(int householdHeadUserId)
        {
            return await _groupInfoRepository.GetByUserIdAsync(householdHeadUserId);
        }

        public async Task<IEnumerable<GroupType>> GetGroupTypeListAsync()
        {
            return await _groupTypeRepository.GetAllForListAsync(GetCurrentSiteId());
        }

        public async Task<GroupInfo> CreateGroup(int currentUserId,
            GroupInfo groupInfo)
        {
            if (currentUserId != groupInfo.UserId)
            {
                // verify user has modify users permission
                if (!HasPermission(Permission.EditParticipants))
                {
                    int userId = GetClaimId(ClaimType.UserId);
                    _logger.LogError($"User {userId} doesn't have permission to create a group.");
                    throw new GraException("Permission denied.");
                }
            }

            var sanitizedGroupInfo = new GroupInfo
            {
                Name = groupInfo.Name,
                GroupTypeId = groupInfo.GroupTypeId,
                UserId = groupInfo.UserId
            };

            return await _groupInfoRepository.AddSaveAsync(currentUserId, groupInfo);
        }

        public async Task<GroupInfo> UpdateGroupName(int currentUserId, GroupInfo groupInfo)
        {
            if (currentUserId != groupInfo.UserId)
            {
                // verify user has modify users permission
                if (!HasPermission(Permission.EditParticipants))
                {
                    int userId = GetClaimId(ClaimType.UserId);
                    _logger.LogError($"User {userId} doesn't have permission to update a group name.");
                    throw new GraException("Permission denied.");
                }
            }

            var currentGroup = await _groupInfoRepository.GetByUserIdAsync(groupInfo.UserId);
            currentGroup.Name = groupInfo.Name;
            currentGroup.GroupType = null;
            currentGroup.User = null;
            return await _groupInfoRepository.UpdateSaveAsync(currentUserId, currentGroup);
        }

        public async Task<GroupInfo> UpdateGroup(int currentUserId, GroupInfo groupInfo)
        {
            if (!HasPermission(Permission.EditParticipants))
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to update a group.");
                throw new GraException("Permission denied.");
            }

            var currentGroup = await _groupInfoRepository.GetByUserIdAsync(groupInfo.UserId);
            currentGroup.Name = groupInfo.Name;
            currentGroup.GroupTypeId = groupInfo.GroupTypeId;
            currentGroup.GroupType = null;
            currentGroup.User = null;
            return await _groupInfoRepository.UpdateSaveAsync(currentUserId, currentGroup);
        }
    }
}
