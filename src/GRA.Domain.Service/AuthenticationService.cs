using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using GRA.Domain.Service.Abstract;
using GRA.Domain.Repository;
using System.Text;

namespace GRA.Domain.Service
{
    public class AuthenticationService : Abstract.BaseUserService<AuthenticationService>
    {
        private readonly GRA.Abstract.ITokenGenerator _tokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IRecoveryTokenRepository _recoveryTokenRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly EmailService _emailService;

        public AuthenticationService(ILogger<AuthenticationService> logger,
            GRA.Abstract.ITokenGenerator tokenGenerator,
            IUserContextProvider userContextProvider,
            IUserRepository userRepository,
            IRecoveryTokenRepository recoveryTokenRepository,
            IRoleRepository roleRepository,
            EmailService emailService) : base(logger, userContextProvider)
        {
            _tokenGenerator = Require.IsNotNull(tokenGenerator, nameof(tokenGenerator));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _recoveryTokenRepository = Require.IsNotNull(recoveryTokenRepository,
                nameof(recoveryTokenRepository));
            _roleRepository = Require.IsNotNull(roleRepository, nameof(roleRepository));
            _emailService = Require.IsNotNull(emailService, nameof(emailService));
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

        public async Task ResetPassword(int userIdToReset, string password)
        {
            int authUserId = GetClaimId(ClaimType.UserId);
            int activeUserId = GetActiveUserId();
            if (activeUserId == userIdToReset
                || HasPermission(Permission.EditParticipants))
            {
                await _userRepository.SetUserPasswordAsync(authUserId, userIdToReset, password);
            }
            else
            {
                _logger.LogError($"User {authUserId} doesn't have permission to reset password for {userIdToReset}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task ResetPassword(string username, string password, string token)
        {
            var fixedToken = token.Trim().ToLower();

            var user = await _userRepository.GetByUsernameAsync(username);

            var tokens = await _recoveryTokenRepository.GetByUserIdAsync(user.Id);
            var validTokens = tokens
                .Where(_ => _.Token == fixedToken)
                .OrderByDescending(_ => _.CreatedBy);
            if (validTokens.Count() > 0)
            {
                if ((validTokens.First().CreatedAt - DateTime.Now).TotalHours > 24)
                {
                    throw new GraException($"Token {token} has expired.");
                }

                foreach (var request in tokens)
                {
                    await _recoveryTokenRepository.RemoveSaveAsync(-1, request.Id);
                }

                await _userRepository.SetUserPasswordAsync(user.Id, user.Id, password);
            }
            else
            {
                throw new GraException($"Token {token} is not valid.");
            }
        }

        public async Task GenerateTokenAndEmail(string username, string recoveryUrl)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
            {
                _logger.LogError($"Username '{username}' doesn't exist so can't create a recovery token.");
                throw new GraException($"User '{username}' not found.");
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                _logger.LogError($"User {user.Id} doesn't have an email address configured so cannot send a recovery token.");
                throw new GraException($"User '{username}' doesn't have an email address configured.");
            }

            // clear any existing tokens
            var existingRequests = await _recoveryTokenRepository.GetByUserIdAsync(user.Id);
            _logger.LogInformation($"Found {existingRequests.Count()} existing recovery tokens for user {user.Id}.");
            foreach (var request in existingRequests)
            {
                await _recoveryTokenRepository.RemoveSaveAsync(-1, request.Id);
            }

            string tokenString = _tokenGenerator.Generate().ToUpper().Trim();

            // insert new token
            var token = await _recoveryTokenRepository.AddSaveAsync(-1, new RecoveryToken
            {
                Token = tokenString.ToLower(),
                UserId = user.Id
            });
            _logger.LogInformation($"Inserted token for user {user.Id}.");

            string subject = "Password reset";
            string mailBody = $"Your password reset token is: {tokenString}";
            mailBody += $"\n\r{recoveryUrl}?username={username}&token={tokenString}";

            await _emailService.Send(user.Id, subject, mailBody);
        }

        public async Task EmailAllUsernames(string email)
        {
            var lookupEmail = email.Trim();
            var usernames = await _userRepository.GetUserIdAndUsernames(lookupEmail);

            if (usernames == null || usernames.Data.Count() == 0)
            {
                throw new GraException($"There are no usernames associated with email address: '{lookupEmail}'.");
            }

            var sb = new StringBuilder($"The following usernames are associated with '{lookupEmail}':");
            sb.AppendLine();
            foreach(string username in usernames.Data)
            {
                sb.AppendLine($"- {username}");
            }

            string subject = "Usernames associated with your email address";
            await _emailService.Send(usernames.Id, subject, sb.ToString());
        }
    }
}