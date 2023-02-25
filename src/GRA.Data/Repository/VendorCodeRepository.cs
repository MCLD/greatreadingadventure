using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
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

        public async Task<ICollection<VendorCode>> GetByPackingSlipAsync(long packingSlipNumber)
        {
            var codes = await DbSet
                .Where(_ => _.PackingSlip == packingSlipNumber)
                .AsNoTracking()
                .ProjectTo<VendorCode>(_mapper.ConfigurationProvider)
                .ToListAsync();

            foreach (var code in codes.Where(_ => _.UserId.HasValue))
            {
                code.IsUserValid = _context.Users
                    .AsNoTracking()
                    .Any(_ => _.Id == code.UserId.Value && !_.IsDeleted);
            }

            return codes;
        }

        public async Task<ICollection<VendorCode>> GetEarnedCodesAsync(ReportCriterion criterion)
        {
            // Includes deleted users
            var validUsers = _context.Users.AsNoTracking()
                .Where(_ => _.SiteId == criterion.SiteId);

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

            return await (from vendorCodes in DbSet.AsNoTracking()
                            .Where(_ => _.UserId.HasValue && _.SiteId == criterion.SiteId)
                          join users in validUsers
                          on vendorCodes.UserId equals users.Id
                          select vendorCodes)
                          .ProjectTo<VendorCode>(_mapper.ConfigurationProvider)
                          .ToListAsync();
        }

        public async Task<ICollection<VendorCode>> GetPendingHouseholdCodes(int headOfHouseholdId)
        {
            var householdUsers = _context.Users
                .AsNoTracking()
                .Where(_ => _.Id == headOfHouseholdId || _.HouseholdHeadUserId == headOfHouseholdId);

            return await DbSet.AsNoTracking()
                .Where(_ => _.UserId.HasValue && _.IsDonated == null && _.IsEmailAward == null)
                .Join(householdUsers,
                    vendorCode => vendorCode.UserId,
                    user => user.Id,
                    (vendorCode, _) => vendorCode)
                .ProjectTo<VendorCode>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IList<ReportVendorCodePending>> GetPendingPrizesPickupBranch()
        {
            var shippedNotArrived = await DbSet
                .AsNoTracking()
                .Where(_ => _.ShipDate != null && _.ArrivalDate == null)
                .GroupBy(_ => _.BranchId)
                .Select(_ => new { BranchId = _.Key, Count = _.Count() })
                .ToDictionaryAsync(k => k.BranchId, v => v.Count);

            var orderedNotShipped = await DbSet
                .AsNoTracking()
                .Where(_ => _.OrderDate != null && _.ShipDate == null)
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
                .ProjectTo<VendorCode>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<VendorCodeStatus> GetStatusAsync()
        {
            var all = DbSet.AsNoTracking();

            if (!await all.AnyAsync())
            {
                return new VendorCodeStatus();
            }

            var assigned = all.Where(_ => _.IsAssigned);
            var emailAwards = assigned.Where(_ => _.IsEmailAward == true && _.IsDonated != true);
            var vendorAwards = assigned.Where(_ => _.IsEmailAward != true && _.IsDonated != true);

            return new VendorCodeStatus
            {
                AssignedCodes = await assigned.CountAsync(),
                Donated = await assigned.CountAsync(_ => _.IsDonated == true),
                IsConfigured = true,
                EmailAwardSelected = await emailAwards.CountAsync(),
                EmailAwardDownloadedInReport = await emailAwards
                    .CountAsync(_ => _.EmailAwardReported != null),
                EmailAwardPendingDownload = await emailAwards
                    .CountAsync(_ => _.EmailAwardReported == null),
                EmailAwardSent = await emailAwards.CountAsync(_ => _.EmailAwardSent != null),
                NoStatus = await vendorAwards.CountAsync(_ => _.ShipDate == null
                    && _.OrderDate == null),
                Ordered = await vendorAwards.CountAsync(_ => _.OrderDate != null),
                Shipped = await vendorAwards.CountAsync(_ => _.ShipDate != null),
                UnusedCodes = await all.CountAsync(_ => !_.IsAssigned),
                VendorSelected = await vendorAwards.CountAsync()
            };
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
                .ProjectTo<VendorCode>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
    }
}
