using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            _passwordHasher = passwordHasher
                ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task AddRoleAsync(int currentUserId, int userId, int roleId)
        {
            var userLookup = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == userId && !_.IsDeleted)
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

        public async Task<AuthenticationResult> AuthenticateUserAsync(string username,
            string password,
            string culture)
        {
            var result = new AuthenticationResult
            {
                FoundUser = false,
                PasswordIsValid = false
            };

            var lookupUser = await DbSet
                .Where(_ => _.Username == username && !_.IsDeleted)
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

                    if (culture != Culture.DefaultName)
                    {
                        // if the user is using a non-default culture, update their record
                        lookupUser.Culture = culture;
                    }

                    await SaveAsync();
                }
            }
            return result;
        }

        public async Task ChangeDeletedUsersProgramAsync(int oldProgram, int newProgram)
        {
            var usersToMove = DbSet.Where(_ => _.ProgramId == oldProgram && _.IsDeleted);
            foreach (var user in usersToMove)
            {
                user.ProgramId = newProgram;
            }
            DbSet.UpdateRange(usersToMove);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetAchieverCountAsync(ReportCriterion request)
        {
            return await ApplyUserFilter(request)
                .Where(_ => _.AchievedAt.HasValue)
                .CountAsync();
        }

        public async Task<IEnumerable<int>> GetAllUserIds(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && !_.IsDeleted)
                .Select(_ => _.Id)
                .ToListAsync();
        }

        public async Task<ICollection<User>> GetAllUsersWithoutUnsubscribeToken()
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsSystemUser && string.IsNullOrWhiteSpace(_.UnsubscribeToken))
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public override async Task<User> GetByIdAsync(int id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id && !_.IsDeleted)
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<User> GetByUnsubscribeToken(int siteId, string token)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.UnsubscribeToken == token)
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            var lookupUser = await DbSet
                .AsNoTracking()
                .Where(_ => _.Username == username && !_.IsDeleted)
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

        public async Task<int> GetCountAsync(ReportCriterion request)
        {
            return await ApplyUserFilter(request).CountAsync();
        }

        public async Task<int> GetCountAsync(UserFilter filter)
        {
            return await ApplyUserFilter(filter)
                .CountAsync();
        }

        public async Task<int> GetFirstTimeCountAsync(ReportCriterion request)
        {
            var users = ApplyUserFilter(request);
            return await users.Where(_ => _.IsFirstTime).CountAsync();
        }

        public async Task<string> GetFullNameByIdAsync(int userId)
        {
            var user = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == userId)
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
            return user?.FullName;
        }

        public async Task<IEnumerable<User>> GetHouseholdAsync(int householdHeadUserId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && _.HouseholdHeadUserId == householdHeadUserId)
                .OrderBy(_ => _.FirstName)
                .ThenBy(_ => _.LastName)
                .ThenBy(_ => _.Username)
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<int> GetHouseholdCountAsync(int householdHeadUserId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted
                       && _.HouseholdHeadUserId == householdHeadUserId)
                       .CountAsync();
        }

        public async Task<ICollection<User>> GetHouseholdUsersWithAvailablePrizeAsync(
            int headId, int? drawingId, int? triggerId)
        {
            var householdMemberIds = DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && (_.Id == headId || _.HouseholdHeadUserId == headId))
                .Select(_ => _.Id);

            return await _context.PrizeWinners
                .Where(_ => !_.RedeemedAt.HasValue
                    && _.DrawingId == drawingId
                    && _.TriggerId == triggerId
                    && householdMemberIds.Contains(_.UserId))
                .Select(_ => _.User)
                .OrderBy(_ => _.HouseholdHeadUserId.HasValue)
                .ThenBy(_ => _.FirstName)
                .ThenBy(_ => _.LastName)
                .ThenBy(_ => _.Username)
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<int>> GetNewsSubscribedUserIdsAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && !_.IsDeleted && _.IsNewsSubscribed)
                .Select(_ => _.Id)
                .ToListAsync();
        }

        public async Task<IDictionary<string, int>>
            GetSubscribedLanguageCountAsync(string unspecifiedString)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && _.IsEmailSubscribed)
                .GroupBy(_ => _.Culture)
                .Select(_ => new { Culture = _.Key, Count = _.Count() })
                .ToDictionaryAsync(k => k.Culture ?? unspecifiedString, v => v.Count);
        }

        public async Task<int> GetSystemUserId()
        {
            var systemUser = await DbSet
                .AsNoTracking()
                .SingleOrDefaultAsync(_ => _.IsSystemUser);

            if (systemUser == null)
            {
                await DbSet.AddAsync(new Model.User
                {
                    BranchId = (await _context.Branches.OrderBy(_ => _.Id).FirstAsync()).Id,
                    CanBeDeleted = false,
                    CreatedAt = _dateTimeProvider.Now,
                    CreatedBy = -1,
                    FirstName = "System Account",
                    IsActive = false,
                    IsAdmin = true,
                    IsDeleted = true,
                    IsLockedOut = true,
                    IsSystemUser = true,
                    LockedOutAt = _dateTimeProvider.Now,
                    LockedOutFor = "System Account",
                    SiteId = (await _context.Sites.SingleAsync(_ => _.IsDefault)).Id,
                    SystemId = (await _context.Systems.OrderBy(_ => _.Id).FirstAsync()).Id,
                    ProgramId = (await _context.Programs.OrderBy(_ => _.Id).FirstAsync()).Id
                });
                await _context.SaveChangesAsync();

                systemUser = await DbSet
                    .SingleOrDefaultAsync(_ => _.IsSystemUser);

                _logger.LogInformation("Inserted System Account, id is: {0}", systemUser.Id);

                var site = await _context.Sites.SingleOrDefaultAsync(_ => _.IsDefault);
                if (site != null)
                {
                    site.CreatedBy = systemUser.Id;
                    _context.Sites.Update(site);
                }

                systemUser.CreatedBy = systemUser.Id;
                DbSet.Update(systemUser);

                await _context.SaveChangesAsync();
            }
            return systemUser.Id;
        }

        public async Task<IEnumerable<User>> GetTopScoresAsync(ReportCriterion criterion,
            int scoresToReturn)
        {
            return await ApplyUserFilter(criterion)
                .OrderByDescending(_ => _.PointsEarned)
                .Take(scoresToReturn)
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<DataWithId<IEnumerable<string>>> GetUserIdAndUsernames(string email)
        {
            var userIdLookup = await DbSet
                .AsNoTracking()
                .Where(_ => _.Email == email && !_.IsDeleted)
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
                        && !_.IsDeleted)
                    .Select(_ => _.Username)
                    .OrderBy(_ => _)
                    .ToListAsync()
            };
        }

        public async Task<List<int>> GetUserIdsByBranchProgram(ReportCriterion criterion)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted
                    && _.BranchId == criterion.BranchId
                    && _.ProgramId == criterion.ProgramId)
                .Select(_ => _.Id)
                .ToListAsync();
        }

        public async Task<ICollection<int>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.RoleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByCriterionAsync(ReportCriterion criterion)
        {
            return await ApplyUserFilter(criterion)
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ICollection<User>> GetUsersByEmailAddressAsync(string email)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.Email == email)
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<DataWithCount<ICollection<User>>> GetUsersInRoleAsync(int roleId,
            BaseFilter filter)
        {
            var users = _context.UserRoles
                .AsNoTracking()
                .Where(_ => _.RoleId == roleId
                    && !_.User.IsDeleted
                    && _.User.SiteId == filter.SiteId)
                .Select(_ => new User
                {
                    BranchName = _.User.Branch.Name,
                    CreatedAt = _.User.CreatedAt,
                    FirstName = _.User.FirstName,
                    Id = _.UserId,
                    LastAccess = _.User.LastAccess,
                    LastName = _.User.LastName,
                    SystemName = _.User.System.Name,
                    Username = _.User.Username
                });

            return new DataWithCount<ICollection<User>>
            {
                Count = await users.CountAsync(),
                Data = await users
                    .OrderBy(_ => _.Username)
                    .ApplyPagination(filter)
                    .ToListAsync()
            };
        }

        public async Task<bool> IsAnyoneSubscribedAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsEmailSubscribed && !_.IsDeleted)
                .AnyAsync();
        }

        public async Task<bool> IsEmailSubscribedAsync(string email)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Email == email && _.IsEmailSubscribed && !_.IsDeleted)
                .AnyAsync();
        }

        public async Task<IEnumerable<User>> PageAllAsync(UserFilter filter)
        {
            var userList = ApplyUserFilter(filter);

            switch (filter.SortBy)
            {
                // default is by last name
                default:
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
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>>
            PageHouseholdAsync(int householdHeadUserId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted
                       && _.HouseholdHeadUserId == householdHeadUserId)
                .OrderBy(_ => _.LastName)
                .ThenBy(_ => _.FirstName)
                .ThenBy(_ => _.Username)
                .Skip(skip)
                .Take(take)
                .ProjectTo<User>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task SetUserPasswordAsync(int currentUserId, int userId, string password)
        {
            var user = DbSet.Find(userId);
            if (user.IsSystemUser)
            {
                throw new GraException("Cannot set a password for the System User.");
            }
            string original = _entitySerializer.Serialize(user);
            user.PasswordHash = _passwordHasher.HashPassword(password);
            await UpdateSaveAsync(currentUserId, user, original);
        }

        public async Task<bool> UnsubscribeTokenExists(int siteId, string token)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.UnsubscribeToken == token)
                .AnyAsync();
        }

        public async Task UpdateUserRolesAsync(int currentUserId,
                                                                                    int userId,
            IEnumerable<int> rolesToAdd,
            IEnumerable<int> rolesToRemove)
        {
            var now = _dateTimeProvider.Now;

            var addRoles = rolesToAdd.Select(_ => new Model.UserRole
            {
                RoleId = _,
                UserId = userId,
                CreatedAt = now,
                CreatedBy = currentUserId
            });
            var removeRoles = _context.UserRoles
                .Where(_ => _.UserId == userId && rolesToRemove.Contains(_.RoleId));

            await _context.UserRoles.AddRangeAsync(addRoles);
            _context.UserRoles.RemoveRange(removeRoles);
        }

        public async Task<bool> UsernameInUseAsync(int siteId, string username)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.Username == username && !_.IsDeleted)
                .AnyAsync();
        }

        private IQueryable<Model.User> ApplyUserFilter(UserFilter filter)
        {
            var userList = DbSet.AsNoTracking()
                .Where(_ => !_.IsDeleted && _.SiteId == filter.SiteId);

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
                userList = userList
                    .Where(_ => filter.ProgramIds.Cast<int>().Contains(_.ProgramId));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                userList = userList.Where(
                    _ => _.Username.Contains(filter.Search)
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
            else if (filter.UserIds?.Any() == true)
            {
                userList = userList.Where(_ => filter.UserIds.Contains(_.Id));
            }

            if (filter.IsSubscribed != null)
            {
                userList = userList.Where(_ => _.IsEmailSubscribed == filter.IsSubscribed);
            }

            return userList;
        }

        private IQueryable<Model.User> ApplyUserFilter(ReportCriterion criterion)
        {
            var userList = DbSet.AsNoTracking()
                .Where(_ => !_.IsDeleted && _.SiteId == criterion.SiteId);

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

            if (criterion.SchoolId != null)
            {
                userList = userList.Where(_ => _.SchoolId == criterion.SchoolId);
            }

            if (criterion.VendorCodeTypeId != null)
            {
                userList = userList.Join(_context.VendorCodes,
                    u => u.Id,
                    v => v.UserId,
                    (u, v) => new { u, v })
                    .Where(_ => _.v.VendorCodeTypeId == criterion.VendorCodeTypeId.Value
                        && _.v.IsDonated == true)
                    .Select(_ => _.u);
            }

            return userList;
        }
    }
}
