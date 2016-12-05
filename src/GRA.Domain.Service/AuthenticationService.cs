using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using GRA.Domain.Service.Abstract;
using GRA.Domain.Repository;

namespace GRA.Domain.Service
{
    public class AuthenticationService : Abstract.BaseUserService<AuthenticationService>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecoveryTokenRepository _recoveryTokenRepository;
        private readonly IRoleRepository _roleRepository;

        public AuthenticationService(ILogger<AuthenticationService> logger,
            IUserContextProvider userContextProvider,
            IUserRepository userRepository,
            IRecoveryTokenRepository recoveryTokenRepository,
            IRoleRepository roleRepository) : base(logger, userContextProvider)
        {
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _recoveryTokenRepository = Require.IsNotNull(recoveryTokenRepository,
                nameof(recoveryTokenRepository));
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
        public async Task ResetPassword(int userIdToReset, string password)
        {
            int userId = await GetClaimId(ClaimType.UserId);
            if (userId == userIdToReset
                || await HasPermission(Permission.EditParticipants))
            {
                await _userRepository.SetUserPasswordAsync(userId, userIdToReset, password);
            }
            else
            {
                _logger.LogError($"User {userId} doesn't have permission to reset password for {userIdToReset}.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task ResetPassword(string username, string password, string token)
        {
            var fixedToken = token.Trim().ToLower();

            var user = await _userRepository.GetByUsernameAsync(username);

            var tokens = await _recoveryTokenRepository.GetByUserIdAsync(user.Id);
            var validTokens = tokens
                .Where(_ => _.Token.Contains(token))
                .OrderByDescending(_ => _.CreatedBy);
            if (validTokens.Count() > 0)
            {
                if ((validTokens.First().CreatedAt - DateTime.Now).TotalHours > 24)
                {
                    throw new Exception($"Token {token} has expired.");
                }

                foreach (var request in tokens)
                {
                    await _recoveryTokenRepository.RemoveSaveAsync(-1, request.Id);
                }

                await _userRepository.SetUserPasswordAsync(user.Id, user.Id, password);
            }
            else
            {
                throw new Exception($"Token {token} is not valid.");
            }
        }

        public async Task GenerateRecoveryToken(string username)
        {
            throw new NotImplementedException();
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
            {
                _logger.LogError($"Username '{username}' doesn't exist so can't create a recovery token.");
                throw new Exception($"User '{username}' not found.");
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                _logger.LogError($"User {user.Id} doesn't have an email address configured so cannot send a recovery token.");
                throw new Exception($"User '{username}' doesn't have an email address configured.");
            }

            // clear any existing tokens
            var existingRequests = await _recoveryTokenRepository.GetByUserIdAsync(user.Id);
            _logger.LogInformation($"Found {existingRequests.Count()} existing recovery tokens for user {user.Id}.");
            foreach (var request in existingRequests)
            {
                await _recoveryTokenRepository.RemoveSaveAsync(-1, request.Id);
            }

            // insert new token
            var token = await _recoveryTokenRepository.AddSaveAsync(-1, new RecoveryToken
            {
                //TODO generate token - lowercase and trimmed
                Token = "generate_token",
                UserId = user.Id
            });
            _logger.LogInformation($"Inserted token for user {user.Id}.");

            //TODO send email with token
        }
    }
}