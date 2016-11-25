using System;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class UserService : Abstract.BaseService<UserService>
    {
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        public UserService(ILogger<UserService> logger,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
            : base(logger)
        {
            if (userRepository == null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }
            this.userRepository = userRepository;
            if (roleRepository == null)
            {
                throw new ArgumentNullException(nameof(roleRepository));
            }
            this.roleRepository = roleRepository;
        }

        public async Task<AuthenticationResult> AuthenticateUserAsync(string username, string password)
        {
            var authResult = await userRepository.AuthenticateUserAsync(username, password);

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
                authResult.PermissionNames = roleRepository.GetPermisisonNamesForUser(authResult.User.Id);
            }
            return authResult;
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            //todo: handle validation (username isn't already in use, etc)
            user.BranchId = 1;
            user.ProgramId = 1;
            user.SiteId = 1;
            var registeredUser = await userRepository.AddSaveAsync(0, user);
            await userRepository.SetUserPasswordAsync(registeredUser.Id, password);
            return registeredUser;
        }
    }
}