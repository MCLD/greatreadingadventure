using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class UserRepository
        : AuditingRepository<Model.User, User>, IUserRepository
    {
        private readonly Security.Abstract.IPasswordHasher _passwordHasher;
        public UserRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<UserRepository> logger,
            Security.Abstract.IPasswordHasher passwordHasher) : base(repositoryFacade, logger)
        {
            _passwordHasher = Require.IsNotNull(passwordHasher, nameof(passwordHasher));
        }

        public async Task AddRoleAsync(int currentUserId, int userId, int roleId)
        {
            var userLookup = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == userId && _.IsDeleted == false)
                .SingleOrDefaultAsync();

            if (userLookup == null)
            {
                throw new GraException($"Unable to add roles to user {userId}.");
            }

            var userRoleAssignment = new Model.UserRole
            {
                UserId = userLookup.Id,
                RoleId = roleId,
                CreatedBy = currentUserId,
                CreatedAt = _dateTimeProvider.Now
            };
            await _context.UserRoles.AddAsync(userRoleAssignment);
        }

        public async Task<ICollection<int>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.RoleId)
                .ToListAsync();
        }

        public async Task SetUserPasswordAsync(int currentUserId, int userId, string password)
        {
            var user = DbSet.Find(userId);
            string original = _entitySerializer.Serialize(user);
            user.PasswordHash = _passwordHasher.HashPassword(password);
            await UpdateSaveAsync(currentUserId, user, original);
        }
        public async Task<User> GetByUsernameAsync(string username)
        {
            var lookupUser = await DbSet
                .AsNoTracking()
                .Where(_ => _.Username == username && _.IsDeleted == false)
                .SingleOrDefaultAsync();
            if (lookupUser != null)
            {
                return _mapper.Map<Model.User, User>(lookupUser);
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

            var lookupUser = await DbSet
                .Where(_ => _.Username == username && _.IsDeleted == false)
                .SingleOrDefaultAsync();
            if (lookupUser != null)
            {
                result.FoundUser = true;
                result.PasswordIsValid =
                    _passwordHasher.VerifyHashedPassword(lookupUser.PasswordHash, password);
                if (result.PasswordIsValid)
                {
                    result.User = _mapper.Map<Model.User, User>(lookupUser);
                    lookupUser.LastAccess = _dateTimeProvider.Now;
                    await SaveAsync();
                }
            }
            return result;
        }

        public async Task<IEnumerable<User>> PageAllAsync(UserFilter filter)
        {
            var userList = ApplyUserFilter(filter);

            switch (filter.SortBy)
            {
                case SortUsersBy.FirstName:
                    if (filter.OrderDescending)
                    {
                        userList = userList
                            .OrderByDescending(_ => _.FirstName)
                            .ThenByDescending(_ => _.LastName)
                            .ThenByDescending(_ => _.Username);
                    }
                    else
                    {
                        userList = userList
                            .OrderBy(_ => _.FirstName)
                            .ThenBy(_ => _.LastName)
                            .ThenBy(_ => _.Username);
                    }
                    break;
                case SortUsersBy.LastName:
                    if (filter.OrderDescending)
                    {
                        userList = userList
                            .OrderByDescending(_ => _.LastName)
                            .ThenByDescending(_ => _.FirstName)
                            .ThenByDescending(_ => _.Username);
                    }
                    else
                    {
                        userList = userList
                            .OrderBy(_ => _.LastName)
                            .ThenBy(_ => _.FirstName)
                            .ThenBy(_ => _.Username);
                    }
                    break;
                case SortUsersBy.RegistrationDate:
                    if (filter.OrderDescending)
                    {
                        userList = userList
                            .OrderByDescending(_ => _.CreatedAt)
                            .ThenByDescending(_ => _.LastName)
                            .ThenByDescending(_ => _.FirstName)
                            .ThenByDescending(_ => _.Username);
                    }
                    else
                    {
                        userList = userList
                            .OrderBy(_ => _.CreatedAt)
                            .ThenBy(_ => _.LastName)
                            .ThenBy(_ => _.FirstName)
                            .ThenBy(_ => _.Username);
                    }
                    break;
                case SortUsersBy.Username:
                    if (filter.OrderDescending)
                    {
                        userList = userList
                            .OrderBy(_ => string.IsNullOrWhiteSpace(_.Username))
                            .ThenByDescending(_ => _.Username)
                            .ThenByDescending(_ => _.LastName)
                            .ThenByDescending(_ => _.FirstName);
                    }
                    else
                    {
                        userList = userList
                            .OrderBy(_ => string.IsNullOrWhiteSpace(_.Username))
                            .ThenBy(_ => _.Username)
                            .ThenBy(_ => _.LastName)
                            .ThenBy(_ => _.FirstName);
                    }
                    break;
            }

            return await userList
                .ApplyPagination(filter)
                .Include(_ => _.Branch)
                .Include(_ => _.Program)
                .Include(_ => _.System)
                .ProjectTo<User>()
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(UserFilter filter)
        {
            return await ApplyUserFilter(filter)
                .CountAsync();
        }

        private IQueryable<Model.User> ApplyUserFilter(UserFilter filter)
        {
            var userList = DbSet.AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.SiteId == filter.SiteId);

            if (filter.SystemIds?.Any() == true)
            {
                userList = userList.Where(_ => filter.SystemIds.Contains(_.SystemId));
            }

            if (filter.BranchIds?.Any() == true)
            {
                userList = userList.Where(_ => filter.BranchIds.Contains(_.BranchId));
            }

            if (filter.ProgramIds?.Any() == true)
            {
                userList = userList.Where(_ => filter.ProgramIds.Cast<int>().Contains(_.ProgramId));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                userList = userList.Where(_ => _.Username.Contains(filter.Search)
                        || (_.FirstName + " " + _.LastName).Contains(filter.Search)
                        || _.Email.Contains(filter.Search));
            }

            if (filter.CanAddToHousehold)
            {
                var householdHeadList = DbSet.AsNoTracking()
                    .Where(_ => _.HouseholdHeadUserId.HasValue)
                    .Select(u => u.HouseholdHeadUserId)
                    .Distinct();

                userList = userList
                    .Where(_ => !filter.UserIds.Contains(_.Id)
                        && !householdHeadList.Contains(_.Id)
                        && !_.HouseholdHeadUserId.HasValue);
            }

            return userList;
        }

        private IQueryable<Model.User> ApplyUserFilter(ReportCriterion criterion)
        {
            var userList = DbSet.AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.SiteId == criterion.SiteId);

            if (criterion.SystemId != null)
            {
                userList = userList.Where(_ => criterion.SystemId == _.SystemId);
            }

            if (criterion.BranchId != null)
            {
                userList = userList.Where(_ => criterion.BranchId == _.BranchId);
            }

            if (criterion.ProgramId != null)
            {
                userList = userList.Where(_ => criterion.ProgramId == _.ProgramId);
            }

            if (criterion.StartDate != null)
            {
                userList = userList.Where(_ => _.CreatedAt >= criterion.StartDate);
            }

            if (criterion.EndDate != null)
            {
                userList = userList.Where(_ => _.CreatedAt <= criterion.EndDate);
            }

            return userList;
        }


        public async Task<int> GetCountAsync(ReportCriterion request)
        {
            return await ApplyUserFilter(request).CountAsync();
        }

        public async Task<int> GetAchieverCountAsync(ReportCriterion request)
        {
            return await ApplyUserFilter(request)
                .Where(_ => _.IsAchiever == true)
                .CountAsync();
        }

        public async Task<IEnumerable<User>>
            PageHouseholdAsync(int householdHeadUserId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Include(_ => _.Branch)
                .Include(_ => _.Program)
                .Include(_ => _.System)
                .Where(_ => _.IsDeleted == false
                       && _.HouseholdHeadUserId == householdHeadUserId)
                .Skip(skip)
                .Take(take)
                .ProjectTo<User>()
                .ToListAsync();
        }

        public async Task<int> GetHouseholdCountAsync(int householdHeadUserId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                       && _.HouseholdHeadUserId == householdHeadUserId)
                       .CountAsync();
        }

        public override async Task<User> GetByIdAsync(int id)
        {
            return await DbSet
                .AsNoTracking()
                .Include(_ => _.Branch)
                .Include(_ => _.Program)
                .Include(_ => _.System)
                .Include(_ => _.EnteredSchool)
                .Where(_ => _.Id == id && _.IsDeleted == false)
                .ProjectTo<User>(_ => _.EnteredSchoolName)
                .SingleOrDefaultAsync();
        }

        public async Task<DataWithId<IEnumerable<string>>> GetUserIdAndUsernames(string email)
        {
            var userIdLookup = await DbSet
                .AsNoTracking()
                .Where(_ => _.Email == email && _.IsDeleted == false)
                .FirstOrDefaultAsync();

            if (userIdLookup == null)
            {
                return null;
            }

            return new DataWithId<IEnumerable<string>>
            {
                Id = userIdLookup.Id,
                Data = await DbSet
                    .AsNoTracking()
                    .Where(_ => _.Email == email
                        && !string.IsNullOrEmpty(_.Username)
                        && _.IsDeleted == false)
                    .Select(_ => _.Username)
                    .ToListAsync()
            };
        }

        public async Task<IEnumerable<int>> GetAllUserIds(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.IsDeleted == false)
                .Select(_ => _.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetHouseholdAsync(int householdHeadUserId)
        {
            var household = await DbSet
                .AsNoTracking()
                .Include(_ => _.Branch)
                .Include(_ => _.Program)
                .Include(_ => _.System)
                .Where(_ => _.IsDeleted == false
                       && _.HouseholdHeadUserId == householdHeadUserId)
                .ProjectTo<User>()
                .ToListAsync();

            return household;
        }

        public async Task<bool> UsernameInUseAsync(int siteId, string username)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.Username == username && _.IsDeleted == false)
                .AnyAsync();
        }

        public async Task<List<int>> GetUserIdsByBranchProgram(ReportCriterion criterion)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                    && _.BranchId == criterion.BranchId
                    && _.ProgramId == criterion.ProgramId)
                .Select(_ => _.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetTopScoresAsync(ReportCriterion criterion, int scoresToReturn)
        {
            return await ApplyUserFilter(criterion)
                .OrderByDescending(_ => _.PointsEarned)
                .Take(scoresToReturn)
                .ProjectTo<User>()
                .ToListAsync();
        }
    }
}
