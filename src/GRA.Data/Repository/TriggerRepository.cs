using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;
using System;
using GRA.Domain.Repository.Extensions;
using System.Collections.ObjectModel;
using System.Collections;
using GRA.Domain.Model.Filters;

namespace GRA.Data.Repository
{
    public class TriggerRepository : AuditingRepository<Model.Trigger, Trigger>, ITriggerRepository
    {
        private const string ChallengeIcon = "fa-trophy";
        private const string ProgramIcon = "fa-asterisk";
        private const string TriggerIcon = "fa-gears";

        public TriggerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<TriggerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task AddTriggerActivationAsync(int userId, int triggerId)
        {
            _context.UserTriggers.Add(new Model.UserTrigger
            {
                UserId = userId,
                TriggerId = triggerId,
                CreatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        public async Task<DateTime?> CheckTriggerActivationAsync(int userId, int triggerId)
        {
            var trigger = await _context.UserTriggers
                .AsNoTracking()
                .Where(_ => _.UserId == userId && _.TriggerId == triggerId)
                .SingleOrDefaultAsync();
            if (trigger == null)
            {
                return null;
            }
            else
            {
                return trigger.CreatedAt;
            }
        }

        public override async Task<Trigger> GetByIdAsync(int id)
        {
            return await DbSet
                .AsNoTracking()
                .Include(_ => _.AwardBadge)
                .Include(_ => _.RequiredBadges)
                .Include(_ => _.RequiredChallenges)
                .Where(_ => _.Id == id)
                .ProjectTo<Trigger>()
                .SingleOrDefaultAsync();
        }

        // honors site id, skip, and take
        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<Trigger>> PageAsync(BaseFilter filter)
        {
            var triggerList = await ApplyFilters(filter)
                .ApplyPagination(filter)
                .ProjectTo<Trigger>()
                .ToListAsync();

            foreach (var trigger in triggerList)
            {
                trigger.HasDependents = await HasDependentsAsync(trigger.Id);
            }

            return triggerList;
        }

        private IQueryable<Model.Trigger> ApplyFilters(BaseFilter filter)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.SiteId == filter.SiteId);
        }

        public async Task<Trigger> GetByCodeAsync(int siteId, string secretCode)
        {
            secretCode = secretCode.Trim().ToLower();
            var codeTrigger = await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId
                    && _.IsDeleted == false
                    && _.SecretCode == secretCode)
                .SingleOrDefaultAsync();

            if (codeTrigger == null)
            {
                return null;
            }
            return _mapper.Map<Trigger>(codeTrigger);
        }

        public async Task<ICollection<Trigger>> GetTriggersAsync(int userId)
        {
            // get user details for filtering triggers
            var user = await _context.Users
                .AsNoTracking()
                .Where(_ => _.Id == userId)
                .SingleOrDefaultAsync();

            // already earned triggers to exclude
            var alreadyEarnedTriggerIds = _context.UserTriggers
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.TriggerId);

            // monster trigger query
            var triggers = await DbSet
                .AsNoTracking()
                .Include("RequiredBadges")
                .Include("RequiredChallenges")
                .Where(_ => _.SiteId == user.SiteId
                    && _.IsDeleted == false
                    && !alreadyEarnedTriggerIds.Contains(_.Id)
                    && (_.LimitToSystemId == null || _.LimitToSystemId == user.SystemId)
                    && (_.LimitToBranchId == null || _.LimitToBranchId == user.BranchId)
                    && (_.LimitToProgramId == null || _.LimitToProgramId == user.ProgramId)
                    && (_.Points == 0 || _.Points <= user.PointsEarned)
                    && string.IsNullOrEmpty(_.SecretCode))
                .OrderBy(_ => _.Points)
                .ThenBy(_ => _.AwardPoints)
                .ToListAsync();

            // create a list of triggers to remove based on badge and challenge earnings
            var itemsToRemove = new List<Model.Trigger>();

            // get a list of triggers that fire based on badge or challenge earnings
            var itemTriggers = triggers.Where(_ => _.ItemsRequired > 0);

            if (itemTriggers.Count() > 0)
            {
                // get the user's badges
                var userBadgeIds = _context.UserBadges
                    .AsNoTracking()
                    .Where(_ => _.UserId == userId)
                    .Select(_ => _.BadgeId);

                // get the user's challenges
                var userChallengeIds = _context.UserLogs
                    .AsNoTracking()
                    .Where(_ => _.UserId == userId && _.ChallengeId != null)
                    .Select(_ => _.ChallengeId.Value);

                foreach (var eligibleTrigger in itemTriggers)
                {
                    int itemsCompleted = 0;

                    // get the number of completed badges
                    if (eligibleTrigger.RequiredBadges != null 
                        && eligibleTrigger.RequiredBadges.Count > 0)
                    {
                        itemsCompleted += eligibleTrigger.RequiredBadges
                            .Select(_ => _.BadgeId).Intersect(userBadgeIds).Count();
                    }

                    // get the number of completed challenges
                    if (eligibleTrigger.RequiredChallenges != null
                        && eligibleTrigger.RequiredChallenges.Count > 0)
                    {
                        itemsCompleted += eligibleTrigger.RequiredChallenges
                            .Select(_ => _.ChallengeId).Intersect(userChallengeIds).Count();
                    }

                    // remove the trigger if not enough items completed
                    if (itemsCompleted < eligibleTrigger.ItemsRequired)
                    {
                        itemsToRemove.Add(eligibleTrigger);
                    }
                }
            }

            // return all the triggers that should be awarded to the user
            return _mapper.Map<ICollection<Trigger>>(triggers.Except(itemsToRemove));
        }

        public async Task<ICollection<TriggerRequirement>> GetTriggerRequirmentsAsync(Trigger trigger)
        {
            Collection<TriggerRequirement> requirements = new Collection<TriggerRequirement>();

            foreach (var badgeId in trigger.BadgeIds)
            {
                var badge = await _context
                    .Badges
                    .AsNoTracking()
                    .Where(_ => _.Id == badgeId)
                    .SingleOrDefaultAsync();

                var badgeTrigger = await _context.Triggers.AsNoTracking().Where(_ => _.AwardBadgeId == badgeId).SingleOrDefaultAsync();
                if (badgeTrigger != null)
                {
                    requirements.Add(new TriggerRequirement()
                    {
                        BadgeId = badgeId,
                        Name = badgeTrigger.Name,
                        Icon = TriggerIcon,
                        BadgePath = badge.Filename
                    });
                }
                else
                {
                    requirements.Add(new TriggerRequirement()
                    {
                        BadgeId = badgeId,
                        Name = "Unknown",
                        Icon = "",
                        BadgePath = badge.Filename
                    });
                }
            }

            foreach (var challengeId in trigger.ChallengeIds)
            {
                var challenge = await _context
                    .Challenges
                    .AsNoTracking()
                    .Where(_ => _.Id == challengeId)
                    .SingleOrDefaultAsync();
                requirements.Add(new TriggerRequirement()
                {
                    ChallengeId = challengeId,
                    Name = challenge.Name,
                    Icon = ChallengeIcon,
                    BadgePath = await _context.Badges.AsNoTracking()
                        .Where(_ => _.Id == challenge.BadgeId)
                        .Select(_ => _.Filename)
                        .SingleOrDefaultAsync()
                });
            }

            return requirements.OrderBy(_ => _.Name).ToList();
        }

        public async Task<int> CountRequirementsAsync(BaseFilter filter)
        {
            return await ApplyRequirementsFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<TriggerRequirement>> PageRequirementsAsync(BaseFilter filter)
        {
            return await ApplyRequirementsFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ToListAsync();
        }

        private IQueryable<TriggerRequirement> ApplyRequirementsFilters(BaseFilter filter)
        {
            // Badge and Trigger lists
            var requirements = (from challenges in _context.Challenges
                                    .Where(_ => _.SiteId == filter.SiteId
                                        && _.IsDeleted == false
                                        && _.IsActive
                                        && _.Name.Contains(filter.Search ?? string.Empty)
                                        && (filter.SystemIds == null
                                            || filter.SystemIds.Contains(_.RelatedSystemId))
                                        && (filter.BranchIds == null
                                            || filter.BranchIds.Contains(_.RelatedBranchId))
                                        && (filter.UserIds == null
                                            || filter.UserIds.Contains(_.CreatedBy))
                                        && (filter.ChallengeIds == null
                                            || !filter.ChallengeIds.Contains(_.Id)))
                                from badges in _context.Badges
                                    .Where(_ => _.Id == challenges.BadgeId)
                                    .DefaultIfEmpty()
                                select new TriggerRequirement
                                {
                                    ChallengeId = challenges.Id,
                                    Name = challenges.Name,
                                    Icon = ChallengeIcon,
                                    BadgePath = badges.Filename
                                }
                                )
                                .Concat(
                                    from triggers in _context.Triggers
                                    .Where(_ => _.SiteId == filter.SiteId
                                        && _.IsDeleted == false
                                        && _.Name.Contains(filter.Search ?? string.Empty)
                                        && (filter.SystemIds == null
                                            || filter.SystemIds.Contains(_.RelatedSystemId))
                                        && (filter.BranchIds == null
                                            || filter.BranchIds.Contains(_.RelatedBranchId))
                                        && (filter.UserIds == null
                                            || filter.UserIds.Contains(_.CreatedBy))
                                        && (filter.BadgeIds == null
                                            || !filter.BadgeIds.Contains(_.AwardBadgeId)))
                                    join badges in _context.Badges
                                    on triggers.AwardBadgeId equals badges.Id
                                    select new TriggerRequirement
                                    {
                                        BadgeId = badges.Id,
                                        Name = triggers.Name,
                                        Icon = TriggerIcon,
                                        BadgePath = badges.Filename
                                    }
                                );

            // Program Join and Achiever badges
            if (filter.SystemIds == null && filter.BranchIds == null && filter.UserIds == null)
            {
                requirements = requirements.Concat(
                                    from programs in _context.Programs
                                    .Where(_ => _.SiteId == filter.SiteId
                                        && _.JoinBadgeId.HasValue
                                        && _.Name.Contains(filter.Search ?? string.Empty)
                                        && (filter.BadgeIds == null
                                            || !filter.BadgeIds.Contains(_.JoinBadgeId.Value)))
                                    join badges in _context.Badges
                                    on programs.JoinBadgeId equals badges.Id
                                    select new TriggerRequirement
                                    {
                                        BadgeId = badges.Id,
                                        Name = programs.Name + " Join Badge",
                                        Icon = ProgramIcon,
                                        BadgePath = badges.Filename
                                    }
                                )
                                .Concat(
                                    from programs in _context.Programs
                                    .Where(_ => _.SiteId == filter.SiteId
                                        && _.AchieverBadgeId.HasValue
                                        && _.Name.Contains(filter.Search ?? string.Empty)
                                        && (filter.BadgeIds == null
                                            || !filter.BadgeIds.Contains(_.AchieverBadgeId.Value)))
                                    join badges in _context.Badges
                                    on programs.AchieverBadgeId equals badges.Id
                                    select new TriggerRequirement
                                    {
                                        BadgeId = badges.Id,
                                        Name = programs.Name + " Achiever Badge",
                                        Icon = ProgramIcon,
                                        BadgePath = badges.Filename
                                    }
                                );
            }

            return requirements;
        }

        public async Task<bool> CodeExistsAsync(int siteId, string secretCode, int? triggerId = null)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId
                    && _.IsDeleted == false
                    && _.Id != triggerId
                    && _.SecretCode == secretCode)
                .AnyAsync();
        }

        public override async Task<Trigger> AddSaveAsync(int userId, Trigger trigger)
        {
            var newTrigger = await base.AddSaveAsync(userId, trigger);

            foreach (var badgeId in trigger.BadgeIds)
            {
                var triggerBadge = new GRA.Data.Model.TriggerBadge()
                {
                    BadgeId = badgeId,
                    TriggerId = newTrigger.Id
                };
                await _context.TriggerBadges.AddAsync(triggerBadge);
            }
            foreach (var challengeId in trigger.ChallengeIds)
            {
                var triggerChallenge = new GRA.Data.Model.TriggerChallenge()
                {
                    ChallengeId = challengeId,
                    TriggerId = newTrigger.Id
                };
                await _context.TriggerChallenges.AddAsync(triggerChallenge);
            }
            await _context.SaveChangesAsync();

            return newTrigger;
        }

        public override async Task<Trigger> UpdateSaveAsync(int userId, Trigger trigger)
        {
            var updatedTrigger = await base.UpdateSaveAsync(userId, trigger);

            // update TriggerBadge list
            var thisTriggerBadges = _context.TriggerBadges.Where(_ => _.TriggerId == trigger.Id);
            var badgesToAdd = trigger.BadgeIds.Where(_ =>
                !thisTriggerBadges.Select(b => b.BadgeId).Contains(_));
            var badgesToRemove = thisTriggerBadges.Where(_ => !trigger.BadgeIds.Contains(_.BadgeId));
            foreach (var badgeId in badgesToAdd)
            {
                var triggerBadge = new GRA.Data.Model.TriggerBadge()
                {
                    BadgeId = badgeId,
                    TriggerId = trigger.Id
                };
                await _context.TriggerBadges.AddAsync(triggerBadge);
            }
            _context.TriggerBadges.RemoveRange(badgesToRemove);

            // update TriggerChallenge list
            var thisTriggerChallenges = _context.TriggerChallenges
                .Where(_ => _.TriggerId == trigger.Id);
            var challengesToAdd = trigger.ChallengeIds.Where(_ =>
                !thisTriggerChallenges.Select(c => c.ChallengeId).Contains(_));
            var challengesToRemove = thisTriggerChallenges
                .Where(_ => !trigger.ChallengeIds.Contains(_.ChallengeId));
            foreach (var challengeId in challengesToAdd)
            {
                var triggerChallenge = new GRA.Data.Model.TriggerChallenge()
                {
                    ChallengeId = challengeId,
                    TriggerId = trigger.Id
                };
                await _context.TriggerChallenges.AddAsync(triggerChallenge);
            }
            _context.TriggerChallenges.RemoveRange(challengesToRemove);

            await _context.SaveChangesAsync();

            return updatedTrigger;
        }

        public async Task<bool> HasDependentsAsync(int triggerId)
        {
            return await (from triggerBadges in _context.TriggerBadges
                          join trigger in DbSet.Where(_ => _.Id == triggerId)
                          on triggerBadges.BadgeId equals trigger.AwardBadgeId
                          select triggerBadges)
                          .AnyAsync();
        }

        public async Task<ICollection<Trigger>> GetTriggerDependentsAsync(int triggerBadgeId)
        {
            return await _context.TriggerBadges
                .AsNoTracking()
                .Include(_ => _.Trigger)
                .Where(_ => _.BadgeId == triggerBadgeId)
                .Select(_ => _.Trigger)
                .ProjectTo<Trigger>()
                .ToListAsync();
        }

        public async Task<ICollection<Trigger>> GetChallengeDependentsAsync(int challengeId)
        {
            return await _context.TriggerChallenges
                .AsNoTracking()
                .Where(_ => _.ChallengeId == challengeId)
                .Select(_ => _.Trigger)
                .ProjectTo<Trigger>()
                .ToListAsync();
        }
    }
}
