using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using GRA.Domain.Model;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class UserRepository
        : AuditingRepository<Model.User, Domain.Model.User>, IUserRepository
    {
        private readonly Security.Abstract.IPasswordHasher passwordHasher;
        public UserRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<UserRepository> logger,
            Security.Abstract.IPasswordHasher passwordHasher) : base(repositoryFacade, logger)
        {
            if (passwordHasher == null)
            {
                throw new ArgumentNullException(nameof(passwordHasher));
            }
            this.passwordHasher = passwordHasher;
        }

        public async Task AddRoleAsync(int currentUserId, int userId, int roleId)
        {
            var userRoleAssignment = new Model.UserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.Now
            };
            await context.UserRoles.AddAsync(userRoleAssignment);
        }

        public async Task SetUserPasswordAsync(int userId, string password)
        {
            var user = DbSet.Find(userId);
            user.PasswordHash = passwordHasher.HashPassword(password);
            await SaveAsync();
        }
        public async Task<User> GetByUsernameAsync(string username)
        {
            var dbUser = await DbSet
                .AsNoTracking()
                .Where(_ => _.Username == username)
                .SingleOrDefaultAsync();
            if (dbUser != null)
            {
                return mapper.Map<Model.User, User>(dbUser);
            }
            else
            {
                return null;
            }
        }

        public async Task<AuthenticationResult> AuthenticateUserAsync(string username, string password)
        {
            var result = new AuthenticationResult
            {
                FoundUser = false,
                PasswordIsValid = false
            };

            var dbUser = await DbSet
                .Where(_ => _.Username == username)
                .SingleOrDefaultAsync();
            if (dbUser != null)
            {
                result.FoundUser = true;
                result.PasswordIsValid = 
                    passwordHasher.VerifyHashedPassword(dbUser.PasswordHash, password);
                if (result.PasswordIsValid)
                {
                    result.User = mapper.Map<Model.User, User>(dbUser);
                    dbUser.LastAccess = DateTime.Now;
                    await SaveAsync();
                }
            }
            return result;
        }
    }
}
