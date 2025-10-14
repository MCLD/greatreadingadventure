using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Report;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class VendorCodeRepository
        : AuditingRepository<Model.VendorCode, VendorCode>, IVendorCodeRepository
    {
        public VendorCodeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<VendorCodeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Catch any exception related to locking/contention here")]
        public async Task<VendorCode> AssignCodeAsync(int vendorCodeTypeId, int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(_ => _.Id == userId)
                .SingleOrDefaultAsync();

            var success = false;
            int tries = 0;

            Model.VendorCode unusedCode = null;

            while (!success && tries < 10)
            {
                unusedCode = await DbSet
                    .Where(_ => _.SiteId == user.SiteId
                        && _.UserId == null
                        && !_.IsAssigned
                        && _.VendorCodeTypeId == vendorCodeTypeId)
                    .FirstOrDefaultAsync();

                if (unusedCode == null)
                {
                    _logger.LogCritical("No available vendor codes of type {VendorCodeTypeId} to assign to {UserId}.",
                        vendorCodeTypeId,
                        userId);
                    throw new GraException("No available vendor code to assign.");
                }

                unusedCode.UserId = userId;
                unusedCode.IsAssigned = true;
                unusedCode.DateUsed = _dateTimeProvider.Now;

                tries++;
                try
                {
                    await _context.SaveChangesAsync();
                    success = true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Exception trying to update vendor code id {UnusedCodeId}, trying for user {UserId} again ({Tries} tries): {ErrorMessage}",
                        unusedCode.Id,
                        user.Id,
                        tries,
                        ex.Message);
                    await Task.Delay(100);
                    await _context.Entry(unusedCode).ReloadAsync();
                }
            }

            if (!success)
            {
                _logger.LogCritical("Ultimately unsuccessful assigning vendor code type {VendorCodeTypeId} to {UserId}",
                    vendorCodeTypeId,
                    user.Id);
                throw new GraException($"Unable to assign vendor code type {vendorCodeTypeId} to user {user.Id}");
            }

            return await GetByIdAsync(unusedCode.Id);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Catch any exception related to locking/contention here")]
        public async Task<VendorCode> AssociateCodeAsync(int vendorCodeTypeId,
            int userId,
            string reason,
            int activeUserId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(_ => _.Id == userId)
                .SingleOrDefaultAsync();

            var success = false;
            int tries = 0;

            Model.VendorCode unusedCode = null;

            while (!success && tries < 10)
            {
                unusedCode = await DbSet
                    .Where(_ => _.SiteId == user.SiteId
                        && _.UserId == null
                        && !_.IsAssigned
                        && _.VendorCodeTypeId == vendorCodeTypeId)
                    .FirstOrDefaultAsync();

                if (unusedCode == null)
                {
                    _logger.LogCritical("No available vendor codes of type {VendorCodeTypeId} to assign to {UserId}.",
                        vendorCodeTypeId,
                        userId);
                    throw new GraException("No available vendor code to assign.");
                }

                unusedCode.AssociatedUserId = userId;
                unusedCode.IsAssigned = true;
                unusedCode.ReasonForReassignment = reason;
                unusedCode.ReassignedAt = _dateTimeProvider.Now;
                unusedCode.ReassignedByUserId = activeUserId;

                tries++;
                try
                {
                    await _context.SaveChangesAsync();
                    success = true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Exception trying to update vendor code id {UnusedCodeId}, trying for user {UserId} again ({Tries} tries): {ErrorMessage}",
                        unusedCode.Id,
                        user.Id,
                        tries,
                        ex.Message);
                    await Task.Delay(100);
                    await _context.Entry(unusedCode).ReloadAsync();
                }
            }

            if (!success)
            {
                _logger.LogCritical("Ultimately unsuccessful assigning vendor code type {VendorCodeTypeId} to {UserId}",
                    vendorCodeTypeId,
                    user.Id);
                throw new GraException($"Unable to assign vendor code type {vendorCodeTypeId} to user {user.Id}");
            }

            return await GetByIdAsync(unusedCode.Id);
        }

        public async Task<ICollection<string>> GetAllCodesAsync(int vendorCodeTypeId)
        {
            return await DbSet
                .Where(_ => _.VendorCodeTypeId == vendorCodeTypeId)
                .Select(_ => _.Code)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAssociatedVendorCodes(int userId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.AssociatedUserId == userId)
                .OrderBy(_ => _.CreatedAt)
                .Select(_ => _.Code)
                .ToListAsync();
        }

        public async Task<VendorCode> GetByCode(string code)
        {
            var vendorCode = await DbSet.AsNoTracking()
                .Where(_ => _.Code == code)
                .OrderByDescending(_ => _.CreatedAt)
                .FirstOrDefaultAsync();
            return _mapper.Map<VendorCode>(vendorCode);
        }

        public async Task<ICollection<VendorCode>> GetByPackingSlipAsync(string packingSlipNumber)
        {
            var codes = await DbSet
                .Where(_ => _.PackingSlip == packingSlipNumber)
                .AsNoTracking()
                .ProjectToType<VendorCode>()
                .ToListAsync();

            var users = _context
                .Users
                .AsNoTracking()
                .Where(_ => codes.Select(_ => _.UserId).Contains(_.Id))
                .Select(_ => new
                {
                    _.Id,
                    _.IsDeleted,
                    _.FirstName,
                    _.LastName
                });

            var associated = _context
                .Users
                .AsNoTracking()
                .Where(_ => codes.Select(_ => _.AssociatedUserId).Contains(_.Id))
                .Select(_ => new
                {
                    _.Id,
                    _.IsDeleted,
                    _.FirstName,
                    _.LastName
                });

            foreach (var code in codes.Where(_ => _.UserId.HasValue || _.AssociatedUserId.HasValue))
            {
                var user = users.SingleOrDefault(_ => _.Id == code.UserId);
                if (user == null)
                {
                    code.IsUserValid = false;
                    var associatedUser = associated.Where(_ => _.Id == code.AssociatedUserId).ToList();
                    if (associatedUser.Count > 0)
                    {
                        var firstUser = associatedUser[0];
                        code.FirstName = firstUser.FirstName;
                        code.LastName = firstUser.LastName;
                        code.ParticipantName = string.IsNullOrEmpty(firstUser.LastName)
                            ? firstUser.FirstName
                            : $"{firstUser.FirstName} {firstUser.LastName}";
                        code.UserId = code.AssociatedUserId;
                        if (associatedUser.Count == 1)
                        {
                            code.ValidityReason = "Deactivated code, this participant has a different active code.";
                            if (!string.IsNullOrEmpty(code.ReasonForReassignment))
                            {
                                code.ValidityReason
                                    += $" Deactivation reason: {code.ReasonForReassignment}";
                            }
                        }
                    }
                    else
                    {
                        code.ValidityReason = "Unable to map this code to a participant, participant is possibly deleted.";
                    }
                }
                else
                {
                    code.IsUserValid = !user.IsDeleted;
                    code.FirstName = user.FirstName;
                    code.LastName = user.LastName;
                    code.ParticipantName = string.IsNullOrEmpty(user.LastName)
                        ? user.FirstName
                        : $"{user.FirstName} {user.LastName}";
                    if (user.IsDeleted)
                    {
                        code.ValidityReason = "This participant has been deleted.";
                    }
                }
            }

            return codes;
        }

        public async Task<ICollection<VendorCode>> GetEarnedCodesAsync(ReportCriterion criterion)
        {
            ArgumentNullException.ThrowIfNull(criterion);

            // Includes deleted users
            var validUsers = _context.Users.AsNoTracking()
                .Where(_ => _.SiteId == criterion.SiteId && !_.IsDeleted);

            if (criterion.IsFirstTimeParticipant)
            {
                validUsers = validUsers.Where(_ => _.IsFirstTime);
            }

            if (criterion.BranchId.HasValue)
            {
                validUsers = validUsers.Where(_ => _.BranchId == criterion.BranchId.Value);
            }
            else if (criterion.SystemId.HasValue)
            {
                validUsers = validUsers.Where(_ => _.SystemId == criterion.SystemId.Value);
            }

            if (criterion.ProgramId.HasValue)
            {
                validUsers = validUsers.Where(_ => _.ProgramId == criterion.ProgramId.Value);
            }

            return await (from vendorCodes in DbSet.AsNoTracking().Where(_ => _.UserId.HasValue
                            && _.SiteId == criterion.SiteId
                            && !_.ReassignedByUserId.HasValue)
                          join users in validUsers
                          on vendorCodes.UserId equals users.Id
                          select vendorCodes)
                          .ProjectToType<VendorCode>()
                          .ToListAsync();
        }

        public async Task<ICollection<VendorCode>> GetHoldSlipsAsync(string packingSlipNumber)
        {
            var codes = await DbSet
                .Where(_ => _.PackingSlip == packingSlipNumber
                    && _.IsMissing != true
                    && _.IsDamaged != true)
                .AsNoTracking()
                .ProjectToType<VendorCode>()
                .ToListAsync();

            var userIds = codes.Select(_ => _.UserId).Union(codes.Select(_ => _.AssociatedUserId));

            var users = _context
                .Users
                .AsNoTracking()
                .Where(_ => userIds.Contains(_.Id))
                .Select(_ => new
                {
                    _.FirstName,
                    _.HouseholdHeadUserId,
                    _.Id,
                    _.IsDeleted,
                    _.LastName,
                    _.ProgramId
                });

            var groups = await _context
                .GroupInfos
                .AsNoTracking()
                .ToDictionaryAsync(k => k.UserId, v => v.Name);

            var programs = await _context
                .Programs
                .AsNoTracking()
                .ToDictionaryAsync(k => k.Id, v => v.Name);

            foreach (var code in codes.Where(_ => _.UserId.HasValue
                || (!_.UserId.HasValue && _.AssociatedUserId.HasValue)))
            {
                var userId = code.UserId ?? code.AssociatedUserId;
                var user = await users.SingleOrDefaultAsync(_ => _.Id == userId);
                if (user == null)
                {
                    code.IsUserValid = false;
                }
                else
                {
                    code.IsUserValid = !user.IsDeleted;
                    code.FirstName = user.FirstName;
                    code.LastName = user.LastName;
                    code.ProgramName = programs.TryGetValue(user.ProgramId, out string programValue)
                        ? programValue
                        : null;
                    code.GroupName = groups.TryGetValue(user.Id, out string groupValue)
                        ? groupValue
                        : null;
                    if (code.GroupName == null && user.HouseholdHeadUserId.HasValue)
                    {
                        code.GroupName = groups.TryGetValue(user.HouseholdHeadUserId.Value,
                            out string headUserGroup)
                           ? headUserGroup
                           : null;
                    }
                }
            }

            return codes.Where(_ => _.IsUserValid).ToList();
        }

        public async Task<ICollection<VendorCodeItemStatus>>
            GetOrderedNotShipped(int vendorCodeTypeId, int? systemId, int? branchId)
        {
            var orderedNotShipped = DbSet.AsNoTracking()
                .Where(_ => _.VendorCodeTypeId == vendorCodeTypeId
                    && _.OrderDate.HasValue
                    && !_.ShipDate.HasValue)
                .Join(_context.Users.AsNoTracking(),
                    v => v.UserId,
                    u => u.Id,
                    (v, u) => new VendorCodeItemStatus
                    {
                        Code = v.Code,
                        DeliveryBranchId = v.BranchId,
                        FirstName = u.FirstName,
                        IsUserDeleted = u.IsDeleted,
                        LastName = u.LastName,
                        OrderDate = v.OrderDate,
                        OrderDetails = v.Details,
                        ReassignedAt = v.ReassignedAt,
                        ReassignedBy = v.ReassignedByUserId,
                        ReassignedFor = v.ReasonForReassignment,
                        UserId = u.Id,
                        Username = u.Username
                    })
                .Where(_ => !_.IsUserDeleted);

            List<int> limitToBranches = null;

            if (branchId.HasValue)
            {
                limitToBranches = new List<int> { branchId.Value };
            }
            else if (systemId.HasValue)
            {
                limitToBranches = await _context.Branches
                    .AsNoTracking()
                    .Where(_ => _.SystemId == systemId.Value)
                    .Select(_ => _.Id)
                    .ToListAsync();
            }

            if (limitToBranches != null)
            {
                orderedNotShipped = orderedNotShipped
                    .Where(_ => limitToBranches.Contains(_.DeliveryBranchId.Value));
            }

            return await orderedNotShipped.ToListAsync();
        }

        public async Task<ICollection<VendorCode>> GetPendingHouseholdCodes(int headOfHouseholdId)
        {
            var householdUsers = _context.Users
                .AsNoTracking()
                .Where(_ => _.Id == headOfHouseholdId
                    || _.HouseholdHeadUserId == headOfHouseholdId);

            return await DbSet.AsNoTracking()
                .Where(_ => _.UserId.HasValue && _.IsDonated == null && _.IsEmailAward == null)
                .Join(householdUsers,
                    vendorCode => vendorCode.UserId,
                    user => user.Id,
                    (vendorCode, _) => vendorCode)
                .ProjectToType<VendorCode>()
                .ToListAsync();
        }

        public async Task<IList<ReportVendorCodePending>> GetPendingPrizesPickupBranch()
        {
            var shippedNotArrived = await DbSet
                .AsNoTracking()
                .Where(_ => _.ShipDate != null
                    && _.ArrivalDate == null
                    && !_.ReassignedByUserId.HasValue)
                .GroupBy(_ => _.BranchId)
                .Select(_ => new { BranchId = _.Key, Count = _.Count() })
                .ToDictionaryAsync(k => k.BranchId, v => v.Count);

            var orderedNotShipped = await DbSet
                .AsNoTracking()
                .Where(_ => _.OrderDate != null
                    && _.ShipDate == null
                    && !_.ReassignedByUserId.HasValue)
                .GroupBy(_ => _.BranchId)
                .Select(_ => new { BranchId = _.Key, Count = _.Count() })
                .ToDictionaryAsync(k => k.BranchId, v => v.Count);

            var branches = await _context.Branches
                .AsNoTracking()
                .Include(_ => _.System)
                .Select(_ => new ReportVendorCodePending
                {
                    BranchId = _.Id,
                    Name = _.Name,
                    SystemName = _.System.Name,
                })
                .OrderBy(_ => _.SystemName)
                .ThenBy(_ => _.Name)
                .ToListAsync();

            foreach (var branch in branches)
            {
                branch.OrderedNotShipped = orderedNotShipped.ContainsKey(branch.BranchId)
                    ? orderedNotShipped[branch.BranchId]
                    : 0;
                branch.ShippedNotArrived = shippedNotArrived.ContainsKey(branch.BranchId)
                    ? shippedNotArrived[branch.BranchId]
                    : 0;
            }

            return branches;
        }

        public async Task<ICollection<VendorCode>> GetRemainingPrizesForBranchAsync(int branchId)
        {
            var remainingPrizes = _context.PrizeWinners.Where(_ => !_.RedeemedAt.HasValue);

            return await DbSet
                .Where(_ => _.BranchId == branchId && _.UserId.HasValue)
                .Join(remainingPrizes,
                    vendorCode => vendorCode.Id,
                    prize => prize.VendorCodeId,
                    (vendorCode, _) => vendorCode)
                .AsNoTracking()
                .OrderBy(_ => _.ArrivalDate)
                .ThenBy(_ => _.Details)
                .ProjectToType<VendorCode>()
                .ToListAsync();
        }

        public async Task<ICollection<VendorCodeItemStatus>>
            GetShippedNotArrived(int vendorCodeTypeId, int? systemId, int? branchId)
        {
            var shippedNotArrived = DbSet.AsNoTracking()
                .Where(_ => _.VendorCodeTypeId == vendorCodeTypeId
                    && _.ShipDate.HasValue
                    && !_.ArrivalDate.HasValue)
                .Join(_context.Users.AsNoTracking(),
                    v => v.UserId,
                    u => u.Id,
                    (v, u) => new VendorCodeItemStatus
                    {
                        ArrivalDate = v.ArrivalDate,
                        Code = v.Code,
                        DeliveryBranchId = v.BranchId,
                        FirstName = u.FirstName,
                        IsUserDeleted = u.IsDeleted,
                        LastName = u.LastName,
                        OrderDate = v.OrderDate,
                        OrderDetails = v.Details,
                        ReassignedAt = v.ReassignedAt,
                        ReassignedBy = v.ReassignedByUserId,
                        ReassignedFor = v.ReasonForReassignment,
                        ShipDate = v.ShipDate,
                        UserId = u.Id,
                        Username = u.Username
                    })
                .Where(_ => !_.IsUserDeleted);

            List<int> limitToBranches = null;

            if (branchId.HasValue)
            {
                limitToBranches = new List<int> { branchId.Value };
            }
            else if (systemId.HasValue)
            {
                limitToBranches = await _context.Branches
                    .AsNoTracking()
                    .Where(_ => _.SystemId == systemId.Value)
                    .Select(_ => _.Id)
                    .ToListAsync();
            }

            if (limitToBranches != null)
            {
                shippedNotArrived = shippedNotArrived
                    .Where(_ => limitToBranches.Contains(_.DeliveryBranchId.Value));
            }

            return await shippedNotArrived.ToListAsync();
        }

        public async Task<VendorCodeStatus> GetStatusAsync(int vendorCodeTypeId)
        {
            var all = DbSet.AsNoTracking().Where(_ => _.VendorCodeTypeId == vendorCodeTypeId);

            var allCount = await all.CountAsync();

            if (allCount == 0)
            {
                return new VendorCodeStatus();
            }

            var assigned = all.Join(_context.Users,
                    v => v.UserId,
                    u => u.Id,
                    (v, u) => new { v, u })
                .Where(_ => _.v.IsAssigned && !_.u.IsDeleted)
                .Select(_ => _.v);

            var emailAwards = assigned.Where(_ => !_.ReassignedByUserId.HasValue
                && _.IsEmailAward == true
                && (!_.IsDonated.HasValue || _.IsDonated != true));
            var vendorAwards = assigned.Where(_ => !_.ReassignedByUserId.HasValue
                && (!_.IsEmailAward.HasValue || _.IsEmailAward != true)
                && (!_.IsDonated.HasValue || _.IsDonated != true));

            return new VendorCodeStatus
            {
                All = allCount,
                Arrived = await vendorAwards.CountAsync(_ => _.ArrivalDate != null),
                AssignedCodes = await assigned.CountAsync(_ => !_.ReassignedByUserId.HasValue),
                Donated = await assigned.CountAsync(_ => !_.ReassignedByUserId.HasValue
                    && _.IsDonated == true),
                IsConfigured = true,
                EmailAwardSelected = await emailAwards.CountAsync(),
                EmailAwardDownloadedInReport = await emailAwards
                    .CountAsync(_ => _.EmailAwardReported != null),
                EmailAwardPendingDownload = await emailAwards
                    .CountAsync(_ => _.EmailAwardReported == null),
                EmailAwardSent = await emailAwards.CountAsync(_ => _.EmailAwardSent != null),
                NoStatus = await vendorAwards.CountAsync(_ => _.ArrivalDate == null
                    && _.ShipDate == null
                    && _.OrderDate == null),
                Ordered = await vendorAwards.CountAsync(_ => _.OrderDate != null),
                ReassignedCodes = await assigned.CountAsync(_ => _.ReassignedByUserId.HasValue),
                Shipped = await vendorAwards.CountAsync(_ => _.ShipDate != null),
                UnusedCodes = await all.CountAsync(_ => !_.ReassignedByUserId.HasValue
                    && !_.IsAssigned),
                VendorCodeTypeId = vendorCodeTypeId,
                VendorSelected = await vendorAwards.CountAsync()
            };
        }

        public async Task<ICollection<VendorTitlesOnOrder>>
            GetTitlesOnOrderAsync(int vendorCodeTypeId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.VendorCodeTypeId == vendorCodeTypeId
                        && _.OrderDate.HasValue
                        && !_.ShipDate.HasValue
                        && !_.ArrivalDate.HasValue)
                .Join(_context.Users.AsNoTracking(),
                    v => v.UserId,
                    u => u.Id,
                    (v, u) => new
                    {
                        u.IsDeleted,
                        v.Details,
                        OrderDate = v.OrderDate.Value,
                    })
                .Where(_ => !_.IsDeleted)
                .GroupBy(_ => _.Details)
                .Select(_ => new VendorTitlesOnOrder
                {
                    Count = _.Count(),
                    Details = _.Key,
                    EarliestDate = _.Min(od => od.OrderDate),
                    LatestDate = _.Max(od => od.OrderDate)
                })
                .ToListAsync();
        }

        public async Task<ICollection<VendorCodeEmailAward>>
            GetUnreportedEmailAwardCodes(int siteId, int vendorCodeTypeId)
        {
            return await _context.Users
                .AsNoTracking()
                .Join(DbSet.Where(_ => _.SiteId == siteId
                        && _.VendorCodeTypeId == vendorCodeTypeId
                        && _.IsEmailAward == true
                        && !_.EmailAwardReported.HasValue),
                    user => user.Id,
                    vendorCode => vendorCode.UserId,
                    (user, vendorcode) => new VendorCodeEmailAward
                    {
                        VendorCodeId = vendorcode.Id,
                        UserId = user.Id,
                        Name = user.FirstName + " " + user.LastName,
                        Email = vendorcode.EmailAwardAddress
                    })
                .ToListAsync();
        }

        public async Task<VendorCode> GetUserVendorCode(int userId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .OrderByDescending(_ => _.CreatedAt)
                .ProjectToType<VendorCode>()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<VendorCode>> GetUserVendorCodes(int userId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .OrderByDescending(_ => _.CreatedAt)
                .ProjectToType<VendorCode>()
                .ToListAsync();
        }
    }
}
