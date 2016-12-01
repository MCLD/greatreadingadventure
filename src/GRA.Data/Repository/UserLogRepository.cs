using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class UserLogRepository
        : AuditingRepository<Model.UserLog, Domain.Model.UserLog>, IUserLogRepository
    {
        public UserLogRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<UserLogRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<UserLog>> PageHistoryAsync(int userId, int skip, int take)
        {
            var userLogs = await DbSet
               .AsNoTracking()
               .Where(_ => _.UserId == userId
                      && _.IsDeleted == false)
               .OrderBy(_ => _.CreatedAt)
               .Skip(skip)
               .Take(take)
               .ProjectTo<UserLog>()
               .ToListAsync();

            foreach (var userLog in userLogs)
            {
                if (userLog.ChallengeId != null)
                {
                    var challenge = context.Challenges
                        .AsNoTracking()
                        .Where(_ => _.Id == userLog.ChallengeId)
                        .SingleOrDefault();
                    userLog.Description = $"Completed challenge: {challenge.Name}";
                }
                else
                {
                    // used a point translation
                    var translation = context.PointTranslations
                        .AsNoTracking()
                        .Where(_ => _.Id == userLog.PointTranslationId)
                        .SingleOrDefault();
                    if(translation.TranslationDescriptionPastTense.Contains("{0}"))
                    {
                        userLog.Description = string.Format(
                            translation.TranslationDescriptionPastTense,
                            userLog.ActivityEarned);
                    }
                    else
                    {
                        userLog.Description = translation.TranslationDescriptionPastTense;
                    }
                }
            }

            return userLogs;
        }

        public async Task<int> GetHistoryItemCountAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId
                       && _.IsDeleted == false)
                .CountAsync();
        }

        public async override Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await DbSet
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .SingleAsync();
            entity.IsDeleted = true;
            await base.UpdateAsync(userId, entity, null);
            await base.SaveAsync();
        }
    }
}
