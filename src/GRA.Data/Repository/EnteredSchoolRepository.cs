using System;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        public async Task ConvertSchoolAsync(int userId, int enteredSchoolId, int schoolId)
        {
            var users = await _context.Users
                .Where(_ => _.EnteredSchoolId == enteredSchoolId)
                .ToListAsync();

            foreach(var user in users)
            {
                user.SchoolId = schoolId;
                user.EnteredSchoolId = default(int?);
            }
            _context.Users.UpdateRange(users);

            await RemoveSaveAsync(userId, enteredSchoolId);
        }
    }
}