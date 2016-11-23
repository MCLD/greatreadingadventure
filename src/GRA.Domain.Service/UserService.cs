using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using GRA.Domain.Repository;

namespace GRA.Domain.Service
{
    public class UserService : Abstract.BaseService<UserService>
    {
        private readonly IUserRepository userRepository;
        public UserService(ILogger<UserService> logger,
            IUserRepository userRepository) 
            : base(logger) {
            if(userRepository == null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }
            this.userRepository = userRepository;
        }
        public AuthenticatedUser AuthenticateUser(string username, string password)
        {
            var authUser = userRepository.GetByUsernamePassword(username, password);
            if (authUser.User == null)
            {
                authUser.AuthenticationMessage = $"Could not find username '{username}'";
            } else if(!authUser.Authenticated)
            {
                authUser.AuthenticationMessage = "The provided password is incorrect.";
            }
            return authUser;
        }
    }
}