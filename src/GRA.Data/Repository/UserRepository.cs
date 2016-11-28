using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using GRA.Domain.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class UserRepository
        : AuditingRepository<Model.User, User>, IUserRepository
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

        public async Task<AuthenticationResult> AuthenticateUserAsync(string username,
            string password)
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

        public async Task<User> AddPointsSaveAsync(int currentUserId,
            int whoEarnedUserId,
            int pointsEarned,
            bool loggingAsAdminUser)
        {
            if (pointsEarned < 0)
            {
                throw new Exception($"Cannot log negative points");
            }

            User returnUser = null;

            var dbUser = await DbSet
                .Where(_ => _.Id == whoEarnedUserId)
                .SingleOrDefaultAsync();

            if (dbUser == null)
            {
                throw new Exception($"Could not find single user with id {whoEarnedUserId}");
            }

            string original = null;

            if (loggingAsAdminUser)
            {
                original = SerializeEntity(dbUser);
            }

            dbUser.PointsEarned += pointsEarned;

            // update the user's achiever status if they've crossed the threshhold
            var program = await context
                .Programs
                .AsNoTracking()
                .Where(_ => _.Id == dbUser.ProgramId)
                .SingleOrDefaultAsync();

            if (dbUser.PointsEarned >= program.AchieverPointAmount)
            {
                dbUser.IsAchiever = true;
            }

            // save user's changes
            if (loggingAsAdminUser)
            {
                returnUser = await UpdateSaveAsync(currentUserId, dbUser, original);
            }
            else
            {
                await SaveAsync();
            }

            // achiever

            return returnUser ?? await GetByIdAsync(whoEarnedUserId);

        }

        public async override Task<ICollection<User>> PageAllAsync(int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false)
                .Skip(skip)
                .Take(take)
                .ProjectTo<User>()
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false)
                .CountAsync();
        }

        public async override Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await context.Users
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .SingleAsync();
            entity.IsDeleted = true;
            await base.UpdateAsync(userId, entity, null);
            await base.SaveAsync();
        }
    }
}
