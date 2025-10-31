using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class AuthenticationService : Abstract.BaseUserService<AuthenticationService>
    {
        private readonly EmailService _emailService;
        private readonly LanguageService _languageService;
        private readonly GRA.Abstract.IPasswordValidator _passwordValidator;
        private readonly IRecoveryTokenRepository _recoveryTokenRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly SiteLookupService _siteLookupService;
        private readonly GRA.Abstract.ITokenGenerator _tokenGenerator;

        private readonly IUserRepository _userRepository;

        public AuthenticationService(ILogger<AuthenticationService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            GRA.Abstract.ITokenGenerator tokenGenerator,
            GRA.Abstract.IPasswordValidator passwordValidator,
            EmailService emailService,
            IRecoveryTokenRepository recoveryTokenRepository,
            IRoleRepository roleRepository,
            IUserContextProvider userContextProvider,
            IUserRepository userRepository,
            LanguageService languageService,
            SiteLookupService siteLookupService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _tokenGenerator = tokenGenerator
                ?? throw new ArgumentNullException(nameof(tokenGenerator));
            _passwordValidator = passwordValidator
                ?? throw new ArgumentNullException(nameof(passwordValidator));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _recoveryTokenRepository = recoveryTokenRepository
                ?? throw new ArgumentNullException(nameof(recoveryTokenRepository));
            _roleRepository = roleRepository
                ?? throw new ArgumentNullException(nameof(roleRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
        }

        public async Task<AuthenticationResult> AuthenticateUserAsync(string username,
            string password, bool allowDuringCloseProgram = false)
        {
            string trimmedUsername = username.Trim();
            var authResult = await _userRepository.AuthenticateUserAsync(
                trimmedUsername,
                password,
                _userContextProvider.GetCurrentCulture().Name);

            if (!authResult.FoundUser)
            {
                authResult.Message = Annotations.Validate.Username;
                authResult.Arguments = new[] { trimmedUsername };
            }
            else if (!authResult.PasswordIsValid)
            {
                authResult.Message = Annotations.Validate.Password;
            }
            else
            {
                authResult.PermissionNames
                    = await _roleRepository.GetPermisisonNamesForUserAsync(authResult.User.Id);

                if (!authResult.PermissionNames.Contains(nameof(Permission.AccessMissionControl))
                    && !authResult.PermissionNames.Contains(nameof(Permission.AccessPerformerRegistration))
                    && !allowDuringCloseProgram)
                {
                    var userContext = GetUserContext();
                    if (userContext.SiteStage == SiteStage.BeforeRegistration
                        || userContext.SiteStage == SiteStage.AccessClosed)
                    {
                        throw new GraException(Annotations.Validate.NotOpenSignins);
                    }
                }
            }
            return authResult;
        }

        public async Task<Models.ServiceResult> EmailAllUsernames(string email)
        {
            var site = await _siteLookupService.GetByIdAsync(GetCurrentSiteId());

            var lookupEmail = email.Trim();
            var usernames = await _userRepository.GetUserIdAndUsernames(lookupEmail);

            if (usernames?.Data.Any() != true)
            {
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = "There are no usernames associated with the email address: {0}.",
                    Arguments = new[] { lookupEmail }
                };
            }

            var user = await _userRepository.GetByIdAsync(usernames.Id);
            if (user.CannotBeEmailed)
            {
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = Annotations.Validate.EmailAddressInvalid,
                    Arguments = new[] { user.Email }
                };
            }

            var sb = new StringBuilder();
            foreach (string username in usernames.Data)
            {
                sb.Append("- ").AppendLine(username);
            }

            var directEmailDetails = new DirectEmailDetails(site.Name)
            {
                DirectEmailSystemId = "UsernameRecovery",
                LanguageId = await _languageService
                    .GetLanguageIdAsync(CultureInfo.CurrentUICulture.Name),
                SendingUserId = await _userRepository.GetSystemUserId(),
                ToUserId = usernames.Id
            };
            directEmailDetails.Tags.Add("Email", lookupEmail);
            directEmailDetails.Tags.Add("Content", sb.ToString());

            var siteLink = await _siteLookupService.GetSiteLinkAsync(site.Id);

            directEmailDetails.Tags.Add("Sitelink", siteLink.AbsoluteUri);

            var result = new Models.ServiceResult();

            try
            {
                var history = await _emailService.SendDirectAsync(directEmailDetails);
                result.Status = history?.Successful == true
                    ? Models.ServiceResultStatus.Success
                    : Models.ServiceResultStatus.Error;
            }
            catch (GraException ex)
            {
                if (ex?.InnerException is MimeKit.ParseException)
                {
                    result.Status = Models.ServiceResultStatus.Error;
                    result.Message = Annotations.Validate.EmailAddressInvalid;
                    result.Arguments = new[] { email };
                }
            }

            return result;
        }

        public async Task<Models.ServiceResult> GenerateTokenAndEmail(string username,
            string recoveryUrl)
        {
            string trimmedUsername = username.Trim();
            var user = await _userRepository.GetByUsernameAsync(trimmedUsername);

            if (user == null)
            {
                _logger.LogInformation("Username {Username} doesn't exist so can't create a recovery token.",
                    trimmedUsername);
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = Annotations.Validate.Username,
                    Arguments = new[] { trimmedUsername }
                };
            }
            else if (string.IsNullOrEmpty(user.Email))
            {
                _logger.LogInformation("User {Username} ({UserId}) doesn't have an email address configured so cannot send a recovery token.",
                    user?.Username,
                    user?.Id);
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = Annotations.Validate.EmailConfigured,
                    Arguments = new[] { trimmedUsername }
                };
            }
            else if (user.CannotBeEmailed)
            {
                _logger.LogInformation("Username {Username} has an invalid email address {Email} and cannot be emailed.",
                    trimmedUsername,
                    user.Email.Trim());
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = Annotations.Validate.AssociatedEmailAddressInvalid,
                    Arguments = new[] { trimmedUsername }
                };
            }

            // clear any existing tokens
            var existingRequests = await _recoveryTokenRepository.GetByUserIdAsync(user.Id);
            foreach (var request in existingRequests)
            {
                await _recoveryTokenRepository.RemoveSaveAsync(-1, request.Id);
            }

            string tokenString = _tokenGenerator.Generate().ToUpperInvariant().Trim();

            // insert new token
            await _recoveryTokenRepository.AddSaveAsync(-1, new RecoveryToken
            {
                Token = tokenString.ToLowerInvariant(),
                UserId = user.Id
            });

            _logger.LogInformation("Cleared {Existing} existing recovery tokens and inserted a new one for {Username} ({UserId})",
                existingRequests.Count(),
                user?.Username,
                user.Id);

            var site = await _siteLookupService.GetByIdAsync(GetCurrentSiteId());

            var directEmailDetails = new DirectEmailDetails(site.Name)
            {
                DirectEmailSystemId = "PasswordRecovery",
                LanguageId = await _languageService
                    .GetLanguageIdAsync(CultureInfo.CurrentUICulture.Name),
                SendingUserId = await _userRepository.GetSystemUserId(),
                ToUserId = user.Id
            };
            directEmailDetails.Tags.Add("RecoveryLink",
                $"{recoveryUrl}?username={trimmedUsername}&token={tokenString}");
            directEmailDetails.Tags.Add("RecoveryBaseLink", recoveryUrl);
            directEmailDetails.Tags.Add("Username", trimmedUsername);
            directEmailDetails.Tags.Add("Token", tokenString);

            var siteLink = await _siteLookupService.GetSiteLinkAsync(site.Id);

            directEmailDetails.Tags.Add("Sitelink", siteLink.AbsoluteUri);

            var result = new Models.ServiceResult();

            try
            {
                var history = await _emailService.SendDirectAsync(directEmailDetails);
                result.Status = history?.Successful == true
                    ? Models.ServiceResultStatus.Success
                    : Models.ServiceResultStatus.Error;
            }
            catch (GraException ex)
            {
                if (ex?.InnerException is MimeKit.ParseException)
                {
                    result.Status = Models.ServiceResultStatus.Error;
                    result.Message = Annotations.Validate.AssociatedEmailAddressInvalid;
                    result.Arguments = new[] { username };
                }
            }

            return result;
        }

        public async Task ResetPassword(int userIdToReset, string password)
        {
            _passwordValidator.Validate(password);
            int authUserId = GetClaimId(ClaimType.UserId);
            int activeUserId = GetActiveUserId();
            if (activeUserId == userIdToReset
                || HasPermission(Permission.EditParticipants))
            {
                await _userRepository.SetUserPasswordAsync(authUserId, userIdToReset, password);
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to reset password for {ResetPasswordUserId}.",
                    authUserId,
                    userIdToReset);
                throw new GraException(Annotations.Validate.Permission);
            }
        }

        public async Task<Models.ServiceResult> ResetPassword(string username, string password, string token)
        {
            string trimmedUsername = username.Trim();
            _passwordValidator.Validate(password);
            var fixedToken = token.Trim().ToLowerInvariant();

            var user = await _userRepository.GetByUsernameAsync(trimmedUsername);
            if (user == null)
            {
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = Annotations.Validate.Username,
                    Arguments = new[] { trimmedUsername }
                };
            }

            var tokens = await _recoveryTokenRepository.GetByUserIdAsync(user.Id);
            var validTokens = tokens
                .Where(_ => _.Token == fixedToken)
                .OrderByDescending(_ => _.CreatedBy);
            if (validTokens.Any())
            {
                if ((validTokens.First().CreatedAt - _dateTimeProvider.Now).TotalHours > 24)
                {
                    return new Models.ServiceResult
                    {
                        Status = Models.ServiceResultStatus.Error,
                        Message = Annotations.Validate.TokenExpired,
                        Arguments = new[] { token }
                    };
                }

                foreach (var request in tokens)
                {
                    await _recoveryTokenRepository.RemoveSaveAsync(-1, request.Id);
                }

                await _userRepository.SetUserPasswordAsync(user.Id, user.Id, password);
            }
            else
            {
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = Annotations.Validate.TokenExpired,
                    Arguments = new[] { token }
                };
            }

            return new Models.ServiceResult(Models.ServiceResultStatus.Success);
        }

        public async Task<AuthenticationResult> RevalidateUserAsync(int userId)
        {
            return new AuthenticationResult
            {
                FoundUser = true,
                PasswordIsValid = true,
                PermissionNames = await _roleRepository.GetPermisisonNamesForUserAsync(userId),
                User = await _userRepository.GetByIdAsync(userId)
            };
        }
    }
}
