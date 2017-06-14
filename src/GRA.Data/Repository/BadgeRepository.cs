using GRA.Domain.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class BadgeRepository
        : AuditingRepository<Model.Badge, Domain.Model.Badge>, IBadgeRepository
    {
        public BadgeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<BadgeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task AddUserBadge(int userId, int badgeId)
        {
            if (!await UserHasBadge(userId, badgeId))
            {
                _context.UserBadges.Add(new Model.UserBadge
                {
                    UserId = userId,
                    BadgeId = badgeId,
                    CreatedAt = _dateTimeProvider.Now
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCountForUserAsync(int userId)
        {
            return await _context.UserBadges
                .AsNoTracking()
                .Include(_ => _.Badge)
                .Where(_ => _.UserId == userId)
                .CountAsync();
        }

        public async Task<IEnumerable<Badge>> PageForUserAsync(int userId, int skip, int take)
        {
            return await _context.UserBadges
                .AsNoTracking()
                .Include(_ => _.Badge)
                .Where(_ => _.UserId == userId)
                .OrderByDescending(_ => _.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(_ => _.Badge)
                .ProjectTo<Badge>()
                .ToListAsync();
        }

        public async Task<bool> UserHasBadge(int userId, int badgeId)
        {
            return null != await _context.UserBadges
                .Where(_ => _.UserId == userId && _.BadgeId == badgeId)
                .SingleOrDefaultAsync();
        }

        public async Task RemoveUserBadgeAsync(int userId, int badgeId)
        {
            var userBadge = await _context.UserBadges
                .Where(_ => _.UserId == userId && _.BadgeId == badgeId)
                .SingleOrDefaultAsync();

            if (userBadge != null)
            {
                _context.UserBadges.Remove(userBadge);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GetBadgeNameAsync(int badgeId)
        {
            var trigger = await _context.Triggers
                .AsNoTracking()
                .Where(_ => _.AwardBadgeId == badgeId)
                .ToListAsync();

            if (trigger.Count > 0)
            {
                return string.Join(", ", trigger.Select(_ => _.Name));
            }

            var programs = await _context.Programs
                .AsNoTracking()
                .Where(_ => _.JoinBadgeId == badgeId || _.AchieverBadgeId == badgeId)
                .ToListAsync();

            if (programs.Count > 0)
            {
                var joined = new List<string>();
                var achiever = new List<string>();
                foreach (var program in programs)
                {
                    if (program.JoinBadgeId == badgeId)
                    {
                        joined.Add(program.Name);
                    }
                    else
                    {
                        achiever.Add(program.Name);
                    }
                }
                string name = null;
                if (joined.Count > 0)
                {
                    name = $"Joined {string.Join(", ", joined)}";
                    if (achiever.Count > 0)
                    {
                        name += $"; achiever status in {string.Join(", ", achiever)}";
                    }
                }
                else
                {
                    name = $"Achiever status in {string.Join(", ", achiever)}";
                }
                return name;
            }

            var questionnaire = await _context.Questionnaires
                .AsNoTracking()
                .Where(_ => _.BadgeId == badgeId)
                .ToListAsync();

            if (questionnaire.Count() > 0)
            {
                string completed = questionnaire.Count() == 1
                    ? "Completed questionnaire: "
                    : "Completed questionnaire(s): ";
                return $"{completed} {string.Join(", ", questionnaire.Select(_ => _.Name))}";
            }

            return $"Badge id {badgeId}";
        }
    }
}
