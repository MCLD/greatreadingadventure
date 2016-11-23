using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using GRA.Domain.Model;
using System.Collections.Generic;

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

        public void AddRole(int currentUserId, int userId, int roleId)
        {
            var userRoleAssignment = new Model.UserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.Now
            };
            context.UserRoles.Add(userRoleAssignment);
        }

        public void SetUserPassword(int userId, string password)
        {
            var user = DbSet.Find(userId);
            user.PasswordHash = passwordHasher.HashPassword(password);
            Save();
        }
        public User GetByUsername(string username)
        {
            var dbUser = DbSet
                .AsNoTracking()
                .Where(_ => _.Username == username)
                .SingleOrDefault();
            if (dbUser != null)
            {
                return mapper.Map<Model.User, User>(dbUser);
            }
            else
            {
                return null;
            }
        }

        public AuthenticatedUser GetByUsernamePassword(string username, string password)
        {
            AuthenticatedUser authUser = new AuthenticatedUser
            {
                Authenticated = false
            };
            var dbUser = DbSet
                .Where(_ => _.Username == username)
                .SingleOrDefault();
            if (dbUser != null)
            {
                authUser.User = mapper.Map<Model.User, User>(dbUser);
                var passwordMatch
                    = passwordHasher.VerifyHashedPassword(dbUser.PasswordHash, password);
                if (passwordMatch)
                {
                    authUser.Authenticated = true;
                    authUser.Permissions = GetPermissions(dbUser.Id);
                    dbUser.LastAccess = DateTime.Now;
                    Save();
                }
            }
            return authUser;
        }

        public ICollection<Permission> GetPermissions(int userId)
        {
            ICollection<Permission> permission = new HashSet<Permission>();

            var roleIds = context
                 .UserRoles
                 .AsNoTracking()
                 .Where(_ => _.UserId == userId)
                 .Select(_ => _.RoleId);

            var permissionNames = context
                .RolePermissions
                .AsNoTracking()
                .Where(_ => roleIds.Contains(_.RoleId))
                .Select(_ => _.Permission.Name)
                .Distinct()
                .OrderBy(_ => _);

            if (permissionNames.Count() == 0)
            {
                return permission;
            }

            foreach (var permissionName in permissionNames)
            {
                permission.Add((Permission)Enum.Parse(typeof(Permission), permissionName));
            }

            return permission;
        }
    }
}
