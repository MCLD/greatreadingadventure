﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class UserService : Abstract.BaseUserService<UserService>
    {
        private const int UnsubscribeTokenLength = 16;

        private readonly IPathResolver _pathResolver;
        private readonly ICodeGenerator _codeGenerator;
        private readonly IPasswordValidator _passwordValidator;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IGroupInfoRepository _groupInfoRepository;
        private readonly IGroupTypeRepository _groupTypeRepository;
        private readonly IJobRepository _jobRepository;
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
        private readonly ActivityService _activityService;
        private readonly EmailManagementService _emailManagementService;
        private readonly SiteLookupService _siteLookupService;
        private readonly UserImportService _userImportService;
        private readonly VendorCodeService _vendorCodeService;
        private readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;

        public UserService(ILogger<UserService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IPathResolver pathResolver,
            ICodeGenerator codeGenerator,
            IPasswordValidator passwordValidator,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IBadgeRepository badgeRepository,
            IBookRepository bookRepository,
            IBranchRepository branchRepository,
            IGroupInfoRepository groupInfoRepository,
            IGroupTypeRepository groupTypeRepository,
            IJobRepository jobRepository,
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
            ActivityService activityService,
            EmailManagementService emailManagementService,
            SiteLookupService siteLookupService,
            UserImportService userImportService,
            VendorCodeService vendorCodeService,
            IStringLocalizer<Resources.Shared> sharedLocalizer)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _pathResolver = pathResolver
                ?? throw new ArgumentNullException(nameof(pathResolver));
            _codeGenerator = codeGenerator
                ?? throw new ArgumentNullException(nameof(codeGenerator));
            _passwordValidator = passwordValidator
                ?? throw new ArgumentNullException(nameof(passwordValidator));
            _authorizationCodeRepository = authorizationCodeRepository
                ?? throw new ArgumentNullException(nameof(authorizationCodeRepository));
            _badgeRepository = badgeRepository
                ?? throw new ArgumentNullException(nameof(badgeRepository));
            _bookRepository = bookRepository
                ?? throw new ArgumentNullException(nameof(bookRepository));
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _groupInfoRepository = groupInfoRepository
                ?? throw new ArgumentNullException(nameof(groupInfoRepository));
            _groupTypeRepository = groupTypeRepository
                ?? throw new ArgumentNullException(nameof(groupTypeRepository));
            _jobRepository = jobRepository
                ?? throw new ArgumentNullException(nameof(jobRepository));
            _mailRepository = mailRepository
                ?? throw new ArgumentNullException(nameof(mailRepository));
            _notificationRepository = notificationRepository
                ?? throw new ArgumentNullException(nameof(notificationRepository));
            _prizeWinnerRepository = prizeWinnerRepository
                ?? throw new ArgumentNullException(nameof(prizeWinnerRepository));
            _programRepository = programRepository
                ?? throw new ArgumentNullException(nameof(programRepository));
            _requireQuestionnaireRepository = requireQuestionnaireRepository
                ?? throw new ArgumentNullException(nameof(requireQuestionnaireRepository));
            _roleRepository = roleRepository
                ?? throw new ArgumentNullException(nameof(roleRepository));
            _schoolRepository = schoolRepository
                ?? throw new ArgumentNullException(nameof(schoolRepository));
            _siteRepository = siteRepository
                ?? throw new ArgumentNullException(nameof(siteRepository));
            _systemRepository = systemRepository
                ?? throw new ArgumentNullException(nameof(systemRepository));
            _userLogRepository = userLogRepository
                ?? throw new ArgumentNullException(nameof(userLogRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _activityService = activityService
                ?? throw new ArgumentNullException(nameof(activityService));
            _emailManagementService = emailManagementService
                ?? throw new ArgumentNullException(nameof(emailManagementService));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _userImportService = userImportService
                ?? throw new ArgumentNullException(nameof(userImportService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
            _sharedLocalizer = sharedLocalizer
                ?? throw new ArgumentNullException(nameof(sharedLocalizer));
        }

        public async Task<User> RegisterUserAsync(User user, string password,
            bool MCRegistration = false, bool allowDuringCloseProgram = false)
        {
            var siteId = GetCurrentSiteId();

            if (!allowDuringCloseProgram)
            {
                VerifyCanRegister();
            }

            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                throw new GraException(Annotations.Validate.UsernameTaken);
            }

            await ValidateUserFields(user);

            _passwordValidator.Validate(password);

            user.CanBeDeleted = true;
            user.IsLockedOut = false;
            user.IsNewsSubscribed = false;

            user.CardNumber = user.CardNumber?.Trim();
            user.Email = user.Email?.Trim();
            user.FirstName = user.FirstName?.Trim();
            user.LastName = user.LastName?.Trim();
            user.PhoneNumber = user.PhoneNumber?.Trim();
            user.PostalCode = user.PostalCode?.Trim();
            user.Username = user.Username?.Trim();

            var unsubscribeToken = _codeGenerator.Generate(UnsubscribeTokenLength, false);
            while (await _userRepository.UnsubscribeTokenExists(siteId, unsubscribeToken))
            {
                unsubscribeToken = _codeGenerator.Generate(UnsubscribeTokenLength, false);
            }
            user.UnsubscribeToken = unsubscribeToken;

            var emailSubscribe = false;
            if (user.IsEmailSubscribed)
            {
                user.IsEmailSubscribed = false;
                var (askEmailSubscription, _) = await _siteLookupService
                    .GetSiteSettingStringAsync(siteId, SiteSettingKey.Users.AskEmailSubPermission);
                if (askEmailSubscription)
                {
                    emailSubscribe = true;
                }
            }

            User registeredUser;
            if (MCRegistration)
            {
                registeredUser = await _userRepository.AddSaveAsync(
                    GetClaimId(ClaimType.UserId), user);
                if (emailSubscribe)
                {
                    registeredUser.IsEmailSubscribed = await _emailManagementService
                        .SetUserEmailSubscriptionStatusAsync(registeredUser.Id, true);
                }
            }
            else
            {
                string cultureName = _userContextProvider.GetCurrentCulture().Name;
                if (cultureName != Culture.DefaultName)
                {
                    user.Culture = cultureName;
                }

                registeredUser = await _userRepository.AddSaveAsync(0, user);
                if (emailSubscribe)
                {
                    registeredUser.IsEmailSubscribed = await _emailManagementService
                        .SetUserEmailSubscriptionStatusAsync(registeredUser.Id, true, newUser: true);
                }
            }

            await _userRepository
                .SetUserPasswordAsync(registeredUser.Id, registeredUser.Id, password);

            await JoinedProgramNotificationBadge(registeredUser);

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            await AwardUserBadgesAsync(registeredUser.Id, false, false);
            sw.Stop();
            if (sw.Elapsed.TotalSeconds > 5)
            {
                _logger.LogInformation("Registration for user id {UserId} took {Elapsed} ms to award triggers",
                    registeredUser.Id,
                    sw?.Elapsed.TotalMilliseconds);
            }

            return registeredUser;
        }

        public async Task<DataWithCount<IEnumerable<User>>> GetPaginatedUserListAsync(
            UserFilter filter)
        {
            if (HasPermission(Permission.ViewParticipantList))
            {
                if (!string.IsNullOrEmpty(filter.Search))
                {
                    var vendorCode = await _vendorCodeService
                        .GetVendorCodeByCode(filter.Search.ToUpper(CultureInfo.InvariantCulture));
                    if (vendorCode?.UserId.HasValue == true)
                    {
                        filter.UserIds = new List<int>
                        {
                            vendorCode.UserId.Value
                        };
                        filter.Search = null;
                    }
                }
                filter.SiteId = GetClaimId(ClaimType.SiteId);
                return new DataWithCount<IEnumerable<User>>
                {
                    Data = await _userRepository.PageAllAsync(filter),
                    Count = await _userRepository.GetCountAsync(filter)
                };
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to view all participants.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
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
                _logger.LogError("User {UserId} doesn't have permission to view family/group participants",
                    authUserId);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
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
                    _logger.LogError("User {UserId} doesn't have permission to get a count of family/group participants",
                        authUserId);
                    throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
                }
            }
        }

        public async Task<User> GetDetails(int userId)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            var authUser = await _userRepository.GetByIdAsync(authUserId);
            var requestedUser = await _userRepository.GetByIdAsync(userId);

            if (requestedUser != null
                && (authUserId == userId
                    || requestedUser.HouseholdHeadUserId == authUserId
                    || authUser.HouseholdHeadUserId == userId))
            {
                return requestedUser;
            }
            else
            {
                _logger.LogError("User {AuthUserId} is not allowed to view participant details of {UserId}",
                    authUserId,
                    userId);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
            }
        }

        public async Task<User> GetDetailsByPermission(int userId)
        {
            VerifyPermission(Permission.ViewParticipantDetails);

            var requestedUser = await _userRepository.GetByIdAsync(userId);
            if (requestedUser == null)
            {
                throw new GraException("The requested participant could not be accessed or does not exist.");
            }

            return requestedUser;
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

                if (userToUpdate.IsEmailSubscribed != currentEntity.IsEmailSubscribed)
                {
                    var (askEmailSubscription, _) = await _siteLookupService
                        .GetSiteSettingStringAsync(GetCurrentSiteId(),
                            SiteSettingKey.Users.AskEmailSubPermission);
                    if (askEmailSubscription)
                    {
                        updatedUser.IsEmailSubscribed = await _emailManagementService
                            .SetUserEmailSubscriptionStatusAsync(updatedUser.Id,
                                userToUpdate.IsEmailSubscribed);
                    }
                    else if (userToUpdate.IsEmailSubscribed)
                    {
                        updatedUser.IsEmailSubscribed = await _emailManagementService
                            .SetUserEmailSubscriptionStatusAsync(updatedUser.Id, false);
                    }
                }

                return updatedUser;
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to update user {EditingUserId}",
                    requestingUserId,
                    userToUpdate.Id);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
            }
        }

        public async Task<User> MCUpdate(User userToUpdate)
        {
            int requestedByUserId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.EditParticipants))
            {
                var currentEntity = await _userRepository.GetByIdAsync(userToUpdate.Id);

                currentEntity.Age = userToUpdate.Age;
                currentEntity.BranchId = userToUpdate.BranchId;
                currentEntity.BranchName = null;
                currentEntity.CardNumber = userToUpdate.CardNumber?.Trim();
                currentEntity.DailyPersonalGoal = userToUpdate.DailyPersonalGoal;
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
                currentEntity.Email = null;

                if (currentEntity.IsSystemUser)
                {
                    if (!string.IsNullOrEmpty(userToUpdate.Username))
                    {
                        throw new GraException("The System Account cannot have a username configured.");
                    }
                    if (!string.IsNullOrEmpty(userToUpdate.Email))
                    {
                        throw new GraException("The System Account cannot have an email address configured.");
                    }
                    currentEntity.FirstName = "System Account";
                    currentEntity.IsAdmin = false;
                    currentEntity.Username = null;
                }
                else
                {
                    currentEntity.FirstName = userToUpdate.FirstName?.Trim();
                    currentEntity.Email = userToUpdate.Email?.Trim();
                    currentEntity.IsAdmin = await UserHasRoles(userToUpdate.Id);

                    if (HasPermission(Permission.EditParticipantUsernames)
                        && !string.IsNullOrWhiteSpace(currentEntity.Username)
                        && !string.IsNullOrWhiteSpace(userToUpdate.Username)
                        && !string.Equals(userToUpdate.Username, currentEntity.Username,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        if (await UsernameInUseAsync(userToUpdate.Username))
                        {
                            throw new GraException(_sharedLocalizer[GRA
                                .Annotations
                                .Validate
                                .UsernameTaken]);
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

                if (userToUpdate.IsEmailSubscribed != currentEntity.IsEmailSubscribed)
                {
                    var (askEmailSubscription, _) = await _siteLookupService
                        .GetSiteSettingStringAsync(GetCurrentSiteId(),
                            SiteSettingKey.Users.AskEmailSubPermission);
                    if (askEmailSubscription)
                    {
                        updatedUser.IsEmailSubscribed = await _emailManagementService
                            .SetUserEmailSubscriptionStatusAsync(updatedUser.Id,
                                userToUpdate.IsEmailSubscribed);
                    }
                    else if (userToUpdate.IsEmailSubscribed)
                    {
                        updatedUser.IsEmailSubscribed = await _emailManagementService
                            .SetUserEmailSubscriptionStatusAsync(updatedUser.Id, false);
                    }
                }

                return updatedUser;
            }
            else
            {
                _logger.LogError($"User {requestedByUserId} doesn't have permission to update user {userToUpdate.Id}.");
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
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
                _logger.LogError("User {UserId} doesn't have permission to remove user {EditingUserId}",
                    requestedByUserId,
                    userIdToRemove);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
            }
        }

        public async Task<UserLog> GetUserLogByIdAsync(int id)
        {
            int requestedByUserId = GetActiveUserId();

            var userLog = await _userLogRepository.GetByIdAsync(id);

            if (requestedByUserId == userLog.UserId)
            {
                return userLog;
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to view user log {UserLogId}",
                   requestedByUserId,
                   id);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
            }
        }

        public async Task<DataWithCount<ICollection<UserLog>>>
            GetPaginatedUserHistoryAsync(int userId, UserLogFilter filter)
        {
            int requestedByUserId = GetActiveUserId();
            if (requestedByUserId == userId
               || HasPermission(Permission.ViewParticipantDetails))
            {
                filter.UserIds = new List<int> { userId };
                return await _userLogRepository.GetPaginatedHistoryAsync(filter);
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to view details for {EditingUserId}",
                    requestedByUserId,
                    userId);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
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
                _logger.LogError("User {UserId} doesn't have permission to view details for {EditingUserId}",
                    requestedByUserId,
                    userId);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
            }
        }

        public async Task<Branch> GetUsersBranch(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return await _branchRepository.GetByIdAsync(user.BranchId);
        }

        public async Task<string>
            ActivateAuthorizationCode(string authorizationCode, int? joiningUserId = null)
        {
            string fixedCode = authorizationCode.Trim().ToLower(CultureInfo.InvariantCulture);
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

            if (!HasPermission(Permission.NewsAutoSubscribe))
            {
                var rolePermissions = (await _roleRepository
                    .GetPermissionNamesForRoleAsync(authCode.RoleId)).ToList();

                if (rolePermissions.Contains(nameof(Permission.NewsAutoSubscribe))
                    && (rolePermissions.Contains(nameof(Permission.AccessMissionControl))
                        || HasPermission(Permission.AccessMissionControl)))
                {
                    user.IsNewsSubscribed = true;
                }
            }

            await _userRepository.UpdateSaveAsync(userId, user);

            return authCode.RoleName;
        }

        public async Task<User> AddHouseholdMemberAsync(int householdHeadUserId, User memberToAdd,
            bool skipHouseholdActionVerification = false)
        {
            if (!skipHouseholdActionVerification)
            {
                VerifyCanHouseholdAction();
            }

            var siteId = GetCurrentSiteId();
            int authUserId = GetClaimId(ClaimType.UserId);
            var householdHead = await _userRepository.GetByIdAsync(householdHeadUserId);

            if (householdHead.HouseholdHeadUserId != null)
            {
                _logger.LogError("User {UserId} cannot add a household member for {EditingUserId} who isn't a head of family/group",
                    authUserId,
                    householdHeadUserId);
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
                memberToAdd.IsNewsSubscribed = false;

                memberToAdd.CardNumber = memberToAdd.CardNumber?.Trim();
                memberToAdd.Email = memberToAdd.Email?.Trim();
                memberToAdd.FirstName = memberToAdd.FirstName?.Trim();
                memberToAdd.LastName = memberToAdd.LastName?.Trim();
                memberToAdd.PhoneNumber = memberToAdd.PhoneNumber?.Trim();
                memberToAdd.PostalCode = memberToAdd.PostalCode?.Trim();
                memberToAdd.Username = memberToAdd.Username?.Trim();

                var unsubscribeToken = _codeGenerator.Generate(UnsubscribeTokenLength, false);
                while (await _userRepository.UnsubscribeTokenExists(siteId, unsubscribeToken))
                {
                    unsubscribeToken = _codeGenerator.Generate(UnsubscribeTokenLength, false);
                }
                memberToAdd.UnsubscribeToken = unsubscribeToken;

                var emailSubscribe = false;
                if (memberToAdd.IsEmailSubscribed)
                {
                    memberToAdd.IsEmailSubscribed = false;
                    var (askEmailSubscription, _) = await _siteLookupService
                        .GetSiteSettingStringAsync(siteId,
                            SiteSettingKey.Users.AskEmailSubPermission);
                    if (askEmailSubscription)
                    {
                        emailSubscribe = true;
                    }
                }

                await ValidateUserFields(memberToAdd);

                var registeredUser = await _userRepository.AddSaveAsync(authUserId, memberToAdd);

                if (emailSubscribe)
                {
                    registeredUser.IsEmailSubscribed = await _emailManagementService
                        .SetUserEmailSubscriptionStatusAsync(registeredUser.Id, true,
                        headOfHouseholdId: householdHeadUserId);
                }

                await JoinedProgramNotificationBadge(registeredUser);

                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                await AwardUserBadgesAsync(registeredUser.Id, false, false);
                sw.Stop();
                if (sw.Elapsed.TotalSeconds > 5)
                {
                    _logger.LogInformation("Registration for household member {UserId} took {Elapsed} ms to award triggers",
                        registeredUser.Id,
                        sw?.Elapsed.TotalMilliseconds);
                }

                return registeredUser;
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to add a family/group member to {EditingUserId}",
                    authUserId,
                    householdHeadUserId);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
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
                    _logger.LogError("User {UserId} cannot register family/group member {EditingUserId} who is already registered.",
                        authUserId,
                        memberToRegister.Id);
                    throw new GraException("Household member is already registered");
                }

                var existingUser = await _userRepository.GetByUsernameAsync(memberToRegister.Username);
                if (existingUser != null)
                {
                    throw new GraException(_sharedLocalizer[Annotations.Validate.UsernameTaken]);
                }

                _passwordValidator.Validate(password);

                user.Username = memberToRegister.Username?.Trim();
                await _userRepository.UpdateSaveAsync(authUserId, user);
                await _userRepository
                    .SetUserPasswordAsync(authUserId, user.Id, password);
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to register family/group member {EditingUserId}",
                    authUserId,
                    memberToRegister.Id);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
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

            var authenticationResult = await _userRepository.AuthenticateUserAsync(
                trimmedUsername,
                password,
                _userContextProvider.GetCurrentCulture().Name);

            if (!authenticationResult.PasswordIsValid)
            {
                throw new GraException(_sharedLocalizer[Annotations
                    .Validate
                    .UsernamePasswordMismatch]);
            }
            if (authenticationResult.User.Id == authUser.Id)
            {
                throw new GraException("You cannot add yourself");
            }

            // one for the person we are adding
            int totalAdded = 1;
            if (authenticationResult.User.HouseholdHeadUserId == null)
            {
                var household = await _userRepository
                    .GetHouseholdAsync(authenticationResult.User.Id);
                // add on the total household count
                totalAdded += household.Count();
            }

            return (totalAddCount: totalAdded, addUserId: authenticationResult.User.Id);
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
            var callIt = isGroup != null
                ? _sharedLocalizer[GRA.Annotations.Interface.Group]
                : _sharedLocalizer[GRA.Annotations.Interface.Family];

            if (hasFamily)
            {
                var infoGroup
                    = await _groupInfoRepository.GetByUserIdAsync(user.Id);
                if (infoGroup != null)
                {
                    _logger.LogInformation("Existing {GroupType} manager {UserId} is is added to new head {HeadUserId}, group {GroupId}/{GroupName} is being removed",
                        callIt,
                        user.Id,
                        authUser.Id,
                        infoGroup.Id,
                        infoGroup.Name);
                    await _groupInfoRepository.RemoveSaveAsync(authUser.Id, infoGroup.Id);
                }
            }

            user.HouseholdHeadUserId = authUser.Id;
            await _userRepository.UpdateSaveAsync(authUser.Id, user);
            string addedMembers = user.FullName;
            if (hasFamily)
            {
                addedMembers += " " + _sharedLocalizer[Annotations.Interface.AndTheir, callIt];
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
            var authenticationResult = await _userRepository.AuthenticateUserAsync(
                trimmedUsername,
                password,
                _userContextProvider.GetCurrentCulture().Name);

            if (!authenticationResult.PasswordIsValid)
            {
                throw new GraException(_sharedLocalizer[Annotations
                    .Validate
                    .UsernamePasswordMismatch]);
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
                    _logger.LogError("User {UserId} doesn't have permission to view details for {HouseholdHeadUserId}.",
                        authId,
                        householdHeadUserId);
                    throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
                }
            }

            var household = await _userRepository.GetHouseholdAsync(householdHeadUserId);

            if (includeVendorCode || includeMail || includePrize || includePendingQuestionnaire)
            {
                if (includeMail && householdHeadUserId != authId
                    && !HasPermission(Permission.ReadAllMail))
                {
                    _logger.LogError("User {UserId} doesn't have permission to view mail for {HouseholdHeadUserId}.",
                        authId,
                        householdHeadUserId);
                    throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
                }

                if (includePrize && !HasPermission(Permission.ViewUserPrizes))
                {
                    _logger.LogError("User {UserId} doesn't have permission to view prizes for {HouseholdHeadUserId}.",
                        authId,
                        householdHeadUserId);
                    throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
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
                            .GetForUser(siteId, member.Id, member.Age)).Count > 0;
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
                _logger.LogError("User {UserId} doesn't have permission to promote family/group members to manager.",
                    authId);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                _logger.LogError("User {UserId} cannot be promoted to family/group manager without a username.",
                    userId);
                throw new GraException("User does not have a username.");
            }
            if (!user.HouseholdHeadUserId.HasValue)
            {
                _logger.LogError("User {UserId} cannot be promoted to family/group manager.",
                    userId);
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
                await _groupInfoRepository.RemoveAsync(authId, groupInfo.Id);

                var newGroup = new GroupInfo
                {
                    GroupTypeId = groupInfo.GroupTypeId,
                    Name = groupInfo.Name,
                    UserId = userId
                };
                await _groupInfoRepository.AddAsync(authId, newGroup);
            }

            await _userRepository.SaveAsync();
        }

        public async Task RemoveFromHouseholdAsync(int userId)
        {
            VerifyCanHouseholdAction();

            var authId = GetClaimId(ClaimType.UserId);
            var user = await _userRepository.GetByIdAsync(userId);
            if (!HasPermission(Permission.EditParticipants)
                && user.HouseholdHeadUserId != authId)
            {
                _logger.LogError("User {UserId} doesn't have permission to remove family/group members.",
                    authId);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
            }

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
                _logger.LogError("User {UserId} doesn't add existing participants to family/group.",
                    authId);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
            }
            var userToAdd = await _userRepository.GetByIdAsync(userToAddId);
            if (userToAdd.HouseholdHeadUserId.HasValue
                || (await _userRepository.GetHouseholdCountAsync(userToAddId)) > 0)
            {
                _logger.LogError("User {UserId} cannot add {AddUserId} to a different family/group.",
                    authId,
                    userToAddId);
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
            return roles?.Count() > 0;
        }

        private async Task JoinedProgramNotificationBadge(User registeredUser)
        {
            var program = await _programRepository.GetByIdAsync(registeredUser.ProgramId);
            var site = await _siteRepository.GetByIdAsync(registeredUser.SiteId);

            // note this text is localized and displayed properly in SessionTimeoutFilterAttribute
            var notification = new Notification
            {
                PointsEarned = 0,
                Text = $"<span class=\"fa fa-thumbs-o-up\"></span> You've successfully joined <strong>{site.Name}</strong>!",
                UserId = registeredUser.Id,
                IsJoiner = true
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
                    Description = _sharedLocalizer[Annotations.Interface.Joined, site.Name]
                });
                notification.BadgeId = badge.Id;
                notification.BadgeFilename = badge.Filename;
            }
            await _notificationRepository.AddSaveAsync(registeredUser.Id, notification);
        }

        public async Task AwardUserBadgesAsync(int userId, bool awardJoinBadge,
            bool awardHousehold)
        {
            if (awardJoinBadge)
            {
                await AwardMissingJoinBadgeAsync(userId, awardHousehold);
            }

            await _activityService.AwardUserTriggersAsync(userId, awardHousehold);
        }

        private async Task AwardMissingJoinBadgeAsync(int userId, bool awardHousehold)
        {
            var site = await _siteLookupService.GetByIdAsync(GetCurrentSiteId());

            var user = await _userRepository.GetByIdAsync(userId);

            var program = await _programRepository.GetByIdAsync(user.ProgramId);

            var badgeList = new List<Badge>();
            if (program.JoinBadgeId.HasValue
                && !await _badgeRepository.UserHasJoinBadgeAsync(user.Id))
            {
                var badge = await _badgeRepository.GetByIdAsync(program.JoinBadgeId.Value);
                await _badgeRepository.AddUserBadge(user.Id, badge.Id);

                badgeList.Add(badge);

                await _userLogRepository.AddAsync(user.Id, new UserLog
                {
                    UserId = user.Id,
                    PointsEarned = 0,
                    IsDeleted = false,
                    BadgeId = badge.Id,
                    Description = _sharedLocalizer[Annotations.Interface.Joined, site.Name]
                });

                // note this text is localized and displayed properly in SessionTimeoutFilterAttribute
                var notification = new Notification
                {
                    BadgeFilename = badge.Filename,
                    BadgeId = badge.Id,
                    PointsEarned = 0,
                    Text = $"<span class=\"fa fa-thumbs-o-up\"></span> You've successfully joined <strong>{site.Name}</strong>!",
                    UserId = user.Id,
                    IsJoiner = true
                };

                await _notificationRepository.AddSaveAsync(user.Id, notification);
            }

            if (awardHousehold)
            {
                var householdMemebers = await _userRepository.GetHouseholdAsync(userId);
                if (householdMemebers.Any())
                {
                    var programList = new List<Program> { program };
                    foreach (var member in householdMemebers)
                    {
                        var memberProgram = programList
                            .SingleOrDefault(_ => _.Id == member.ProgramId);
                        if (memberProgram == null)
                        {
                            memberProgram = await _programRepository.GetByIdAsync(member.ProgramId);
                            programList.Add(memberProgram);
                        }

                        if (memberProgram.JoinBadgeId.HasValue
                             && !await _badgeRepository.UserHasJoinBadgeAsync(member.Id))
                        {
                            var badge = badgeList
                                .SingleOrDefault(_ => _.Id == memberProgram.JoinBadgeId.Value);
                            if (badge == null)
                            {
                                badge = await _badgeRepository.GetByIdAsync(
                                    memberProgram.JoinBadgeId.Value);
                                badgeList.Add(badge);
                            }

                            await _badgeRepository.AddUserBadge(member.Id,
                                memberProgram.JoinBadgeId.Value);

                            await _userLogRepository.AddAsync(user.Id, new UserLog
                            {
                                UserId = member.Id,
                                PointsEarned = 0,
                                IsDeleted = false,
                                BadgeId = badge.Id,
                                Description = _sharedLocalizer[Annotations.Interface.Joined, site.Name]
                            });

                            // note this text is localized and displayed properly in SessionTimeoutFilterAttribute
                            var notification = new Notification
                            {
                                BadgeFilename = badge.Filename,
                                BadgeId = badge.Id,
                                PointsEarned = 0,
                                Text = $"<span class=\"fa fa-thumbs-o-up\"></span> You've successfully joined <strong>{site.Name}</strong>!",
                                UserId = member.Id,
                                IsJoiner = true
                            };

                            await _notificationRepository.AddSaveAsync(member.Id, notification);
                        }
                    }
                }
            }
        }

        private async Task ValidateUserFields(User user)
        {
            if (!(await _systemRepository.ValidateAsync(user.SystemId, user.SiteId)))
            {
                throw new GraException(Annotations.Validate.System);
            }
            if (!(await _branchRepository.ValidateAsync(user.BranchId, user.SystemId)))
            {
                throw new GraException(Annotations.Validate.Branch);
            }
            if (!(await _programRepository.ValidateAsync(user.ProgramId, user.SiteId)))
            {
                throw new GraException(Annotations.Validate.Program);
            }
            if (user.SchoolId.HasValue
                && !(await _schoolRepository.ValidateAsync(user.SchoolId.Value, user.SiteId)))
            {
                throw new GraException(Annotations.Validate.School);
            }
        }

        public async Task<GroupInfo> GetGroupInfoByIdAsync(int id)
        {
            if (!HasPermission(Permission.ViewParticipantDetails)
                && !HasPermission(Permission.ViewAllReporting))
            {
                _logger.LogError("User {UserId} doesn't have permission to view group info.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
            }
            return await _groupInfoRepository.GetByIdAsync(id);
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
            if (currentUserId != groupInfo.UserId
                && !HasPermission(Permission.EditParticipants))
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError("User {UserId} doesn't have permission to create a group.",
                    userId);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
            }

            var existingGroup = await _groupInfoRepository.GetByUserIdAsync(groupInfo.UserId);
            if (existingGroup != null)
            {
                throw new GraException("Participant is already a member of a group");
            }

            var sanitizedGroupInfo = new GroupInfo
            {
                Name = groupInfo.Name,
                GroupTypeId = groupInfo.GroupTypeId,
                UserId = groupInfo.UserId
            };

            return await _groupInfoRepository.AddSaveAsync(currentUserId, sanitizedGroupInfo);
        }

        public async Task<GroupInfo> UpdateGroupName(int currentUserId, GroupInfo groupInfo)
        {
            if (currentUserId != groupInfo.UserId && !HasPermission(Permission.EditParticipants))
            {
                int userId = GetClaimId(ClaimType.UserId);
                _logger.LogError("User {UserId} doesn't have permission to update a group name.",
                    userId);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
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
                _logger.LogError("User {UserId} doesn't have permission to update a group.",
                    userId);
                throw new GraException(_sharedLocalizer[Annotations.Validate.Permission]);
            }

            var currentGroup = await _groupInfoRepository.GetByUserIdAsync(groupInfo.UserId);
            currentGroup.Name = groupInfo.Name;
            currentGroup.GroupTypeId = groupInfo.GroupTypeId;
            currentGroup.GroupType = null;
            currentGroup.User = null;
            return await _groupInfoRepository.UpdateSaveAsync(currentUserId, currentGroup);
        }

        public async Task<IEnumerable<GroupInfo>> GetGroupInfosAsync()
        {
            return await _groupInfoRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<ICollection<int>> GetUserRolesAsync(int userId)
        {
            VerifyPermission(Permission.ManageRoles);
            return await _userRepository.GetUserRolesAsync(userId);
        }

        public async Task UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds)
        {
            VerifyPermission(Permission.ManageRoles);
            var authId = GetClaimId(ClaimType.UserId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                throw new GraException("User doesn't have a username and can't be assigned roles");
            }

            if (await _roleRepository.HasInvalidRolesAsync(roleIds))
            {
                throw new GraException("Role list contains invalid roles.");
            }

            var userRoles = await _userRepository.GetUserRolesAsync(userId);

            if (await _roleRepository.ListContainsAdminRoleAsync(userRoles)
                && !await _roleRepository.ListContainsAdminRoleAsync(roleIds))
            {
                var adminCount = await _roleRepository.GetUsersWithAdminRoleCountAsync();
                if (adminCount <= 1)
                {
                    throw new GraException("Cannot remove the last participant in the System Administrator role.");
                }
            }

            var rolesToAdd = roleIds.Except(userRoles);
            var rolesToRemove = userRoles.Except(roleIds);

            await _userRepository.UpdateUserRolesAsync(authId, userId, rolesToAdd, rolesToRemove);

            if (user.IsAdmin && !roleIds.Any())
            {
                user.IsAdmin = false;
                await _userRepository.UpdateAsync(authId, user);
            }
            else if (!user.IsAdmin && roleIds.Any())
            {
                user.IsAdmin = true;
                await _userRepository.UpdateAsync(authId, user);
            }

            await _userRepository.SaveAsync();
        }

        public async Task UserNewsSubscribe(bool subscribe)
        {
            VerifyPermission(Permission.AccessMissionControl);

            var userId = GetClaimId(ClaimType.UserId);
            var user = await _userRepository.GetByIdAsync(userId);

            user.IsNewsSubscribed = subscribe;

            await _userRepository.UpdateSaveAsync(userId, user);
        }

        public async Task<string> GetUsersNameByIdAsync(int userId)
        {
            VerifyPermission(Permission.AccessMissionControl);

            var user = await _userRepository.GetByIdAsync(userId);

            return user.FullName;
        }

        public async Task UpdateCulture(string cultureName)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            var authUser = await _userRepository.GetByIdAsync(authUserId);
            if (authUser != null)
            {
                authUser.Culture = cultureName;
                await _userRepository.UpdateSaveNoAuditAsync(authUser);
            }
        }

        public async Task<DataWithCount<ICollection<GroupInfo>>> GetPaginatedGroupListAsync(
            GroupFilter filter)
        {
            VerifyPermission(Permission.ViewParticipantDetails);

            filter.SiteId = GetCurrentSiteId();

            return await _groupInfoRepository.PageAsync(filter);
        }

        public async Task<bool> UsersToAddExceedsHouseholdLimitAsync(int householdHeadId,
            int usersToAdd)
        {
            (bool IsSet, int maximumHouseholdSize) = await _siteLookupService
                .GetSiteSettingIntAsync(GetCurrentSiteId(),
                    SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup);

            if (IsSet)
            {
                var householdCount = await _userRepository.GetHouseholdCountAsync(householdHeadId);
                householdCount++;

                if (householdCount + usersToAdd > maximumHouseholdSize)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<GroupType> GetDefaultGroupTypeAsync()
        {
            return await _groupTypeRepository.GetDefaultAsync(GetCurrentSiteId());
        }

        public async Task<ICollection<User>> GetHouseholdUsersWithAvailablePrizeAsync(int headId,
            int? drawingId, int? triggerId)
        {
            VerifyPermission(Permission.ViewUserPrizes);

            if (!drawingId.HasValue && !triggerId.HasValue)
            {
                throw new GraException("No prize specified.");
            }

            return await _userRepository.GetHouseholdUsersWithAvailablePrizeAsync(headId,
                drawingId, triggerId);
        }

        public async Task EnsureUserUnsubscribeTokensAsync()
        {
            var users = await _userRepository.GetAllUsersWithoutUnsubscribeToken();
            if (users.Count > 0)
            {
                _logger.LogInformation("Users without an unsubscribe token: {UserCount}",
                    users.Count);

                var systemUserId = await _userRepository.GetSystemUserId();
                foreach (var user in users)
                {
                    var unsubscribeToken = _codeGenerator.Generate(UnsubscribeTokenLength, false);
                    while (await _userRepository.UnsubscribeTokenExists(user.SiteId,
                        unsubscribeToken))
                    {
                        unsubscribeToken = _codeGenerator.Generate(UnsubscribeTokenLength, false);
                    }
                    user.UnsubscribeToken = unsubscribeToken;

                    await _userRepository.UpdateAsync(systemUserId, user);
                }
                await _userRepository.SaveAsync();

                _logger.LogInformation("Unsubsribe token added to {UserCount} users", users.Count);
            }
        }

        public async Task<bool> HasReceivedBulkEmailAsync(int emailTemplateId, string emailAddress)
        {
            return await _userRepository.HasReceivedBulkEmailAsync(emailTemplateId, emailAddress);
        }

        public async Task SentBulkEmailAsync(int userId, int emailTemplateId, string emailAddress)
        {
            await _userRepository.AddBulkEmailLogAsync(userId, emailTemplateId, emailAddress);
        }

        public async Task<JobStatus> ImportHouseholdMembersAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var requestingUser = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.ImportHouseholdMembers))
            {
                var sw = new Stopwatch();
                sw.Start();

                var job = await _jobRepository.GetByIdAsync(jobId);
                var jobDetails
                    = JsonConvert
                        .DeserializeObject<JobDetailsHouseholdImport>(job.SerializedParameters);

                string filename = jobDetails.Filename;

                token.Register(() =>
                {
                    _logger.LogWarning("Import of {FilePath} for user {UserId} was cancelled after {Elapsed} ms.",
                        filename,
                        requestingUser,
                        sw?.Elapsed.TotalMilliseconds);
                });

                string fullPath = _pathResolver.ResolvePrivateTempFilePath(filename);

                if (!File.Exists(fullPath))
                {
                    _logger.LogError("Could not find {FilePath}", fullPath);
                    return new JobStatus
                    {
                        PercentComplete = 0,
                        Status = "Could not find the import file.",
                        Error = true,
                        Complete = true
                    };
                }

                UserImportResult userImportResult = null;
                try
                {
                    userImportResult = await _userImportService.GetFromExcelAsync(fullPath,
                            jobDetails.ProgmamId);
                }
                finally
                {
                    File.Delete(fullPath);
                }

                if (token.IsCancellationRequested)
                {
                    return new JobStatus
                    {
                        PercentComplete = 100,
                        Status = $"Operation cancelled."
                    };
                }

                if (userImportResult.Errors?.Count > 0)
                {
                    var sb = new StringBuilder("<strong>Import failed</strong>");
                    sb.Append(" Issues detected:<ul>");
                    foreach (string error in userImportResult.Errors)
                    {
                        sb.Append("<li>").Append(error).Append("</li>");
                    }
                    sb.Append("</ul>");

                    return new JobStatus
                    {
                        PercentComplete = 100,
                        Complete = true,
                        Status = sb.ToString(),
                        Error = true
                    };
                }
                else if (userImportResult == null || userImportResult.Users.Count == 0)
                {
                    var sb = new StringBuilder("<strong>Import failed</strong>");
                    sb.Append(" No users to add.");

                    return new JobStatus
                    {
                        PercentComplete = 100,
                        Complete = true,
                        Status = sb.ToString(),
                        Error = true
                    };
                }

                var householdHead = await _userRepository
                    .GetByIdAsync(jobDetails.HeadOfHouseholdId);

                var groupInfo = await GetGroupFromHouseholdHeadAsync(householdHead.Id);
                if (groupInfo == null)
                {
                    var householdLimitExceeded = await UsersToAddExceedsHouseholdLimitAsync(
                        householdHead.Id, userImportResult.Users.Count);

                    if (householdLimitExceeded)
                    {
                        var defaultGroupType = await GetDefaultGroupTypeAsync();
                        if (defaultGroupType == null)
                        {
                            _logger.LogError("Household import for {UserId} should be forced to make a group but no group types are configured",
                                householdHead.Id);
                        }
                        else
                        {
                            var group = new GroupInfo
                            {
                                GroupTypeId = defaultGroupType.Id,
                                Name = $"{householdHead.FullName}'s Group",
                                UserId = householdHead.Id
                            };

                            await CreateGroup(requestingUser, group);
                        }
                    }
                }

                var currentUser = 1;
                var userCount = userImportResult.Users.Count;

                string callIt = groupInfo == null ? "Household" : "Group";

                progress.Report(new JobStatus
                {
                    PercentComplete = currentUser * 100 / userCount,
                    Status = $"Adding {callIt} members ({currentUser}/{userCount})...",
                    Error = false
                });
                var lastUpdateSent = (int)sw.Elapsed.TotalSeconds;

                foreach (var importUser in userImportResult.Users)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    var secondsFromLastUpdate = (int)sw.Elapsed.TotalSeconds - lastUpdateSent;
                    if (secondsFromLastUpdate >= 5)
                    {
                        progress.Report(new JobStatus
                        {
                            PercentComplete = currentUser * 100 / userCount,
                            Status = $"Adding {callIt} members ({currentUser}/{userCount})...",
                            Error = false
                        });
                        lastUpdateSent = (int)sw.Elapsed.TotalSeconds;
                    }
                    var user = new User
                    {
                        Age = importUser.Age,
                        BranchId = householdHead.BranchId,
                        FirstName = importUser.FirstName?.Trim(),
                        IsFirstTime = jobDetails.FirstTimeParticipating,
                        IsHomeschooled = jobDetails.IsHomeSchooled,
                        LastName = importUser.LastName?.Trim(),
                        PostalCode = householdHead.PostalCode,
                        ProgramId = jobDetails.ProgmamId,
                        SchoolId = jobDetails.SchoolId,
                        SchoolNotListed = jobDetails.SchoolNotListed,
                        SystemId = householdHead.SystemId
                    };

                    await AddHouseholdMemberAsync(householdHead.Id, user, true);

                    currentUser++;
                }

                if (token.IsCancellationRequested && userCount >= currentUser)
                {
                    return new JobStatus
                    {
                        PercentComplete = 100,
                        Status = $"Operation cancelled at user {currentUser}."
                    };
                }

                return new JobStatus
                {
                    PercentComplete = 100,
                    Complete = true,
                    Status = "<strong>Import Complete</strong>"
                };
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to import household members.",
                    requestingUser);
                return new JobStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true,
                    Complete = true
                };
            }
        }

        public async Task<bool> IsEmailSubscribedAsync(string email)
        {
            return await _userRepository.IsEmailSubscribedAsync(email);
        }
    }
}
