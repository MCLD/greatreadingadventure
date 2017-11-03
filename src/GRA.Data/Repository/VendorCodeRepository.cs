using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                        && _.VendorCodeTypeId == vendorCodeTypeId)
                    .FirstOrDefaultAsync();

                if (unusedCode == null)
                {
                    _logger.LogCritical($"No available vendor codes of type {vendorCodeTypeId} to assign to {userId}.");
                    throw new Exception("No available vendor code to assign.");
                }

                unusedCode.UserId = userId;

                tries++;
                try
                {
                    await _context.SaveChangesAsync();
                    success = true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Exception trying to update vendor code id {unusedCode.Id}, trying for user {user.Id} again ({tries} tries): {ex.Message}");
                    await Task.Delay(100);
                    await _context.Entry(unusedCode).ReloadAsync();
                }
            }

            if (!success)
            {
                _logger.LogCritical($"Ultimately unsuccessful assigning vendor code type {vendorCodeTypeId} to {user.Id}");
                throw new Exception($"Unable to assign vendor code type {vendorCodeTypeId} to user {user.Id}");
            }

            return await GetByIdAsync(unusedCode.Id);
        }

        public async Task<VendorCode> GetUserVendorCode(int userId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .OrderByDescending(_ => _.CreatedAt)
                .ProjectTo<VendorCode>()
                .FirstOrDefaultAsync();
        }

        public async Task<VendorCode> GetByCode(string code)
        {
            var vendorCode = await DbSet.AsNoTracking()
                .Where(_ => _.Code == code)
                .OrderByDescending(_ => _.CreatedAt)
                .FirstOrDefaultAsync();
            return _mapper.Map<VendorCode>(vendorCode);
        }

        public async Task<ICollection<VendorCode>> GetEarnedCodesAsync(ReportCriterion criterion)
        {
            // Includes deleted users
            var validUsers = _context.Users.AsNoTracking()
                .Where(_ => _.SiteId == criterion.SiteId);

            if (criterion.BranchId.HasValue)
            {
                validUsers = validUsers.Where(_ => _.BranchId == criterion.BranchId.Value);
            }
            else if (criterion.SystemId.HasValue)
            {
                validUsers = validUsers.Where(_ => _.SystemId == criterion.SystemId.Value);
            }

            return await (from vendorCodes in DbSet.AsNoTracking()
                            .Where(_ => _.UserId.HasValue && _.SiteId == criterion.SiteId)
                          join users in validUsers
                          on vendorCodes.UserId equals users.Id
                          select vendorCodes)
                          .ProjectTo<VendorCode>()
                          .ToListAsync();
        }
    }
}
