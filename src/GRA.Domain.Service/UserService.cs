using System;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using GRA.Domain.Repository;

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

        public AuthenticationResult AuthenticateUser(string username, string password)
        {
            var authResult = userRepository.AuthenticateUser(username, password);

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

        public User RegisterUser(User user, string password)
        {
            //todo: handle validation (username isn't already in use, etc)
            user.BranchId = 1;
            user.ProgramId = 1;
            user.SiteId = 1;
            var registeredUser = userRepository.AddSave(0, user);
            userRepository.SetUserPassword(registeredUser.Id, password);
            return registeredUser;
        }
    }
}