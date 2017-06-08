using System;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class EnteredSchoolRepository
        : AuditingRepository<Model.EnteredSchool, EnteredSchool>,
        IEnteredSchoolRepository
    {
        public EnteredSchoolRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<EnteredSchoolRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task ConvertSchoolAsync(EnteredSchool enteredSchool, int schoolId)
        {
            var identicalEnteredSchools = await  DbSet.AsNoTracking()
                .Where(_ => _.SchoolDistrictId == enteredSchool.SchoolDistrictId
                && _.Name.Trim() == enteredSchool.Name.Trim())
                .ToListAsync();

            var users = await _context.Users
                .Where(_ => identicalEnteredSchools
                    .Select(s => s.Id)
                    .Contains(_.EnteredSchoolId.GetValueOrDefault()))
                .ToListAsync();

            foreach (var user in users)
            {
                user.SchoolId = schoolId;
                user.EnteredSchoolId = default(int?);
            }

            _context.Users.UpdateRange(users);
            DbSet.RemoveRange(identicalEnteredSchools);
            await _context.SaveChangesAsync();
        }

        public async Task<DataWithCount<ICollection<EnteredSchool>>> GetPaginatedListAsync(int siteId,
            int skip,
            int take)
        {
            var schoolList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId);

            return new DataWithCount<ICollection<EnteredSchool>>()
            {
                Data = await schoolList
                    .OrderBy(_ => _.Name)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<EnteredSchool>()
                    .ToListAsync(),
                Count = await schoolList.CountAsync()
            };
        }
    }
}