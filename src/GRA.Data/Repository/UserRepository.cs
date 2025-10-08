using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
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
            ArgumentNullException.ThrowIfNull(passwordHasher);

            _passwordHasher = passwordHasher;
        }

        public async Task AddRoleAsync(int currentUserId, int userId, int roleId)
        {
            var userLookup = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == userId && !_.IsDeleted)
                .SingleOrDefaultAsync()
                ?? throw new GraException($"Unable to add roles to user {userId}.");

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
            ArgumentNullException.ThrowIfNull(request);
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
                .ProjectToType<User>()
                .ToListAsync();
        }

        public override async Task<User> GetByIdAsync(int id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id && !_.IsDeleted)
                .ProjectToType<User>()
                .SingleOrDefaultAsync();
        }

        public async Task<User> GetByUnsubscribeToken(int siteId, string token)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.UnsubscribeToken == token)
                .ProjectToType<User>()
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

        public async Task<User> GetContactDetailsAsync(int siteId, int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && !_.IsDeleted && _.Id == userId)
                .Select(_ => new User
                {
                    Email = _.Email,
                    FirstName = _.FirstName,
                    Id = _.Id,
                    LastName = _.LastName,
                    PhoneNumber = _.PhoneNumber,
                    Username = _.Username
                })
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetCountAsync(ReportCriterion request)
        {
            ArgumentNullException.ThrowIfNull(request);
            return await ApplyUserFilter(request).CountAsync();
        }

        public async Task<int> GetCountAsync(UserFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return await ApplyUserFilter(filter)
                .CountAsync();
        }

        public async Task<string> GetCultureAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && _.Id == userId)
                .Select(_ => _.Culture)
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetFirstTimeCountAsync(ReportCriterion request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var users = ApplyUserFilter(request);
            return await users.Where(_ => _.IsFirstTime).CountAsync();
        }

        public async Task<string> GetFullNameByIdAsync(int userId)
        {
            var user = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == userId)
                .ProjectToType<User>()
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
                .ProjectToType<User>()
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

        public async Task<IEnumerable<int>> GetHouseHoldUserIdsAsync(int householdHeadUserId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted
                    && (_.HouseholdHeadUserId == householdHeadUserId || _.Id == householdHeadUserId))
                .OrderBy(_ => _.CreatedAt)
                .ProjectToType<User>()
                .Select(_ => _.Id)
                .ToListAsync();
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
                .ProjectToType<User>()
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

        public async Task<IDictionary<User, int>>
            GetStaffRegisteredParticipantsAsync(ReportCriterion criterion)
        {
            ArgumentNullException.ThrowIfNull(criterion);

            var systemUserId = await GetSystemUserId();

            // this cannot use the ApplyUserFilter() method as dates are handled differently
            var userList = DbSet.AsNoTracking().Where(_ => !_.IsDeleted
                    && _.CreatedBy != systemUserId
                    && _.Id != systemUserId
                    && _.IsMcRegistered
                    && _.SiteId == criterion.SiteId);

            if (criterion.SystemId != null)
            {
                userList = userList.Where(_ => criterion.SystemId == _.SystemId);
            }

            if (criterion.BranchId != null)
            {
                userList = userList.Where(_ => criterion.BranchId == _.BranchId);
            }

            if (criterion.StartDate != null)
            {
                userList = userList.Where(_ => _.CreatedAt >= criterion.StartDate);
            }

            if (criterion.EndDate != null)
            {
                userList = userList.Where(_ => _.CreatedAt <= criterion.EndDate);
            }

            var registeredUserCount = userList.GroupBy(_ => _.CreatedBy)
                .Select(_ => new
                {
                    _.Key,
                    Value = _.Count()
                });

            return await registeredUserCount
                .Join(DbSet.AsNoTracking(),
                    ruc => ruc.Key,
                    all => all.Id,
                    (ruc, all) => new
                    {
                        Key = new User
                        {
                            FirstName = all.FirstName,
                            Id = all.Id,
                            LastName = all.LastName,
                            Username = all.Username
                        },
                        ruc.Value
                    })
                .ToDictionaryAsync(k => k.Key, v => v.Value);
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

                _logger.LogInformation("Inserted System Account, id is: {UserId}", systemUser.Id);

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
            ArgumentNullException.ThrowIfNull(criterion);
            return await ApplyUserFilter(criterion)
                .OrderByDescending(_ => _.PointsEarned)
                .Take(scoresToReturn)
                .ProjectToType<User>()
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
            ArgumentNullException.ThrowIfNull(criterion);
            return await ApplyUserFilter(criterion)
                .ProjectToType<User>()
                .ToListAsync();
        }

        public async Task<ICollection<User>> GetUsersByEmailAddressAsync(string email)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.Email == email)
                .ProjectToType<User>()
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

        public async Task<IDictionary<string, DateTime>> GetViewedPackingSlipsAsync(int userId)
        {
            // UserPackingSlipViews for the current user
            var slips = await _context.UserPackingSlipViews
                .Where(_ => _.UserId == userId)
                .ToListAsync();

            // Any which have been received but not removed from UserPackingSlipViews
            var receivedSlips = await _context.VendorCodePackingSlips
                .AsNoTracking()
                .Where(_ => slips.Select(_ => _.PackingSlip).Contains(_.PackingSlip)
                    && _.IsReceived)
                .Select(_ => _.PackingSlip)
                .ToListAsync();

            // Sync up UserPackingSlipViews to remove slips which have been viewed
            if (receivedSlips?.Count > 0)
            {
                foreach (var receivedSlip in receivedSlips)
                {
                    var matchingSlips = slips.Where(_ => _.PackingSlip == receivedSlip).ToList();
                    _context.UserPackingSlipViews.RemoveRange(matchingSlips);
                    foreach (var matchingSlip in matchingSlips)
                    {
                        slips.Remove(matchingSlip);
                    }
                }
                await _context.SaveChangesAsync();
            }

            // return the list of viewed and not received
            return slips.ToDictionary(k => k.PackingSlip, v => v.ViewedAt);
        }

        public async Task<int> GetWelcomePendingCountAsync(int welcomeEmailId,
            int memberLongerThanHours)
        {
            return await DbSet
                .AsNoTracking()
                .Join(_context.DirectEmailHistories,
                    u => u.Email,
                    deh => deh.ToEmailAddress,
                    (u, deh) => new
                    {
                        User = u,
                        DirectEmailAddress = deh.ToEmailAddress
                    })
                .Where(_ => !_.User.IsDeleted
                    && !string.IsNullOrEmpty(_.User.Email)
                    && _.User.IsEmailSubscribed
                    && string.IsNullOrEmpty(_.DirectEmailAddress)
                    && _.User.CreatedAt.AddHours(memberLongerThanHours) <= _dateTimeProvider.Now)
                .CountAsync();
        }

        public async Task<IEnumerable<User>> GetWelcomeRecipientsAsync(int skip,
                    int take,
            int memberLongerThanHours)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted
                    && _.IsEmailSubscribed
                    && !string.IsNullOrEmpty(_.Email)
                    && _.CreatedAt.AddHours(memberLongerThanHours) <= _dateTimeProvider.Now)
                .OrderBy(_ => _.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(_ => new User
                {
                    Email = _.Email,
                    FirstName = _.FirstName,
                    Id = _.Id,
                    LastName = _.LastName,
                    UnsubscribeToken = _.UnsubscribeToken
                })
                .ToListAsync();
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
            ArgumentNullException.ThrowIfNull(filter);
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
                .ProjectToType<User>()
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
                .ProjectToType<User>()
                .ToListAsync();
        }

        public async Task<int> ReassignBranchAsync(int oldBranchId, int newBranchId)
        {
            int reassignedCount = 0;
            var reassignTimer = Stopwatch.StartNew();
            var userList = DbSet.Where(_ => _.BranchId == oldBranchId);
            foreach (var user in userList)
            {
                user.BranchId = newBranchId;
                reassignedCount++;
            }
            DbSet.UpdateRange(userList);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Reassigned {UserCount} users from branch id {OldBranchId} to branch id {NewBranchId} in {ElapsedMs} ms",
                reassignedCount,
                oldBranchId,
                newBranchId,
                reassignTimer.ElapsedMilliseconds);
            reassignTimer.Stop();
            return reassignedCount;
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

        public async Task ViewPackingSlipAsync(int userId, string packingSlip)
        {
            var slipReceived = await _context
                .VendorCodePackingSlips
                .AsNoTracking()
                .SingleOrDefaultAsync(_ => _.PackingSlip == packingSlip && _.IsReceived);

            // if not received, log the view
            if (slipReceived == null)
            {
                var packingSlipViewed = await _context
                    .UserPackingSlipViews
                    .Where(_ => _.UserId == userId && _.PackingSlip == packingSlip)
                    .SingleOrDefaultAsync();

                if (packingSlipViewed == null)
                {
                    await _context.UserPackingSlipViews.AddAsync(new Model.UserPackingSlipView
                    {
                        PackingSlip = packingSlip,
                        UserId = userId,
                        ViewedAt = _dateTimeProvider.Now
                    });
                }
                else
                {
                    packingSlipViewed.ViewedAt = _dateTimeProvider.Now;
                    _context.UserPackingSlipViews.Update(packingSlipViewed);
                }
                await _context.SaveChangesAsync();
            }
        }

        private IQueryable<Model.User> ApplyUserFilter(UserFilter filter)
        {
            var userList = DbSet.AsNoTracking()
                .Where(_ => !_.IsDeleted && _.SiteId == filter.SiteId);

            if (filter.SystemIds?.Count > 0)
            {
                userList = userList.Where(_ => filter.SystemIds.Contains(_.SystemId));
            }

            if (filter.BranchIds?.Count > 0)
            {
                userList = userList.Where(_ => filter.BranchIds.Contains(_.BranchId));
            }

            if (filter.ProgramIds?.Count > 0)
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
                    .Where(_ => !_.IsDeleted && _.HouseholdHeadUserId.HasValue)
                    .Select(u => u.HouseholdHeadUserId)
                    .Distinct();

                userList = userList
                    .Where(_ => !filter.UserIds.Contains(_.Id)
                        && !householdHeadList.Contains(_.Id)
                        && !_.HouseholdHeadUserId.HasValue);
            }
            else if (filter.UserIds?.Count > 0)
            {
                userList = userList.Where(_ => filter.UserIds.Contains(_.Id));
            }

            if (filter.IsSubscribed != null)
            {
                userList = userList.Where(_ => _.IsEmailSubscribed == filter.IsSubscribed);
            }

            if (filter.HasMultiplePrimaryVendorCodes == true)
            {
                var userIdsMultiplePrimaryVendorCodes = _context.VendorCodes
                    .AsNoTracking()
                    .Where(_ => _.UserId.HasValue)
                    .GroupBy(_ => _.UserId)
                    .Where(_ => _.Count() > 1)
                    .Select(_ => _.Key);

                userList = userList.Where(_ => userIdsMultiplePrimaryVendorCodes.Contains(_.Id));
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
                        && _.v.IsDonated == true
                        && !_.v.ReassignedByUserId.HasValue)
                    .Select(_ => _.u);
            }

            return userList;
        }
    }
}
