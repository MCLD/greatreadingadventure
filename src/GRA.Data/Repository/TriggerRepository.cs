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
using GRA.Domain.Model.Filters;

namespace GRA.Data.Repository
{
    public class TriggerRepository : AuditingRepository<Model.Trigger, Trigger>, ITriggerRepository
    {
        private const string ChallengeIcon = "fa-trophy";
        private const string ProgramIcon = "fa-asterisk";
        private const string TriggerIcon = "fa-gears";

        private const string AchieverDescription = "Achiever Badge";
        private const string ChallengeDescription = "Challenge";
        private const string JoinDescription = "Join Badge";
        private const string QuestionnaireDescription = "Questionnaire Badge";
        private const string TriggerDescription = "Trigger";

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
                CreatedAt = _dateTimeProvider.Now
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
        public async Task<int> CountAsync(TriggerFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<Trigger>> PageAsync(TriggerFilter filter)
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

        private IQueryable<Model.Trigger> ApplyFilters(TriggerFilter filter)
        {
            var triggerList = DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.SiteId == filter.SiteId);

            if (filter.SystemIds?.Any() == true)
            {
                triggerList = triggerList.Where(_ => filter.SystemIds.Contains(_.RelatedSystemId));
            }

            if (filter.BranchIds?.Any() == true)
            {
                triggerList = triggerList.Where(_ => filter.BranchIds.Contains(_.RelatedBranchId));
            }

            if (filter.UserIds?.Any() == true)
            {
                triggerList = triggerList.Where(_ => filter.UserIds.Contains(_.CreatedBy));
            }

            if (filter.ProgramIds?.Any() == true)
            {
                triggerList = triggerList.Where(_ => filter.ProgramIds.Contains(_.LimitToProgramId));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                triggerList = triggerList.Where(_ => _.Name.Contains(filter.Search)
                                                || _.SecretCode.Contains(filter.Search));
            }

            if(filter.SecretCodesOnly == true)
            {
                triggerList = triggerList.Where(_ => !string.IsNullOrWhiteSpace(_.SecretCode));
            }

            return triggerList;
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
            var requirements = new Collection<TriggerRequirement>();

            foreach (var badgeId in trigger.BadgeIds)
            {
                var badge = await _context
                    .Badges
                    .AsNoTracking()
                    .Where(_ => _.Id == badgeId)
                    .SingleOrDefaultAsync();

                var badgeTrigger = await _context.Triggers.AsNoTracking()
                    .Where(_ => _.AwardBadgeId == badgeId)
                    .SingleOrDefaultAsync();
                if (badgeTrigger != null)
                {
                    requirements.Add(new TriggerRequirement()
                    {
                        BadgeId = badgeId,
                        Name = badgeTrigger.Name,
                        Icon = TriggerIcon,
                        IconDescription = TriggerDescription,
                        BadgePath = badge.Filename
                    });
                }
                else
                {
                    var programBadge = await _context.Programs.AsNoTracking()
                        .Where(_ => _.AchieverBadgeId == badgeId || _.JoinBadgeId == badgeId)
                        .FirstOrDefaultAsync();

                    if (programBadge != null)
                    {
                        var requirement = new TriggerRequirement()
                        {
                            BadgeId = badgeId,
                            Icon = ProgramIcon,
                            BadgePath = badge.Filename
                        };
                        if (programBadge.AchieverBadgeId == badgeId)
                        {
                            requirement.Name = programBadge.AchieverBadgeName;
                            requirement.IconDescription = AchieverDescription;
                        }
                        else
                        {
                            requirement.Name = programBadge.JoinBadgeName;
                            requirement.IconDescription = JoinDescription;
                        }
                        requirements.Add(requirement);
                    }
                    else
                    {
                        var questionnareBadge = await _context.Questionnaires.AsNoTracking()
                            .Where(_ => _.BadgeId == badgeId)
                            .FirstOrDefaultAsync();
                        if (questionnareBadge != null)
                        {
                            requirements.Add(new TriggerRequirement()
                            {
                                BadgeId = badgeId,
                                Name = questionnareBadge.BadgeName,
                                Icon = ProgramIcon,
                                IconDescription = QuestionnaireDescription,
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
                    IconDescription = ChallengeDescription,
                    BadgePath = await _context.Badges.AsNoTracking()
                        .Where(_ => _.Id == challenge.BadgeId)
                        .Select(_ => _.Filename)
                        .SingleOrDefaultAsync()
                });
            }

            return requirements.OrderBy(_ => _.Name).ToList();
        }

        public async Task<int> CountRequirementsAsync(TriggerFilter filter)
        {
            return await ApplyRequirementsFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<TriggerRequirement>> PageRequirementsAsync(TriggerFilter filter)
        {
            return await ApplyRequirementsFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ToListAsync();
        }

        private IQueryable<TriggerRequirement> ApplyRequirementsFilters(TriggerFilter filter)
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
                                    IconDescription = ChallengeDescription,
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
                                        IconDescription = TriggerDescription,
                                        BadgePath = badges.Filename
                                    }
                                );

            // Program Join/Achiever and Questionnaire badges
            if (filter.SystemIds == null && filter.BranchIds == null && filter.UserIds == null)
            {
                requirements = requirements.Concat(
                                    from programs in _context.Programs
                                    .Where(_ => _.SiteId == filter.SiteId
                                        && _.JoinBadgeId.HasValue
                                        && _.JoinBadgeName.Contains(filter.Search ?? string.Empty)
                                        && (filter.BadgeIds == null
                                            || !filter.BadgeIds.Contains(_.JoinBadgeId.Value)))
                                    .GroupBy(_ => _.JoinBadgeId).Select(_ => _.First())
                                    join badges in _context.Badges
                                    on programs.JoinBadgeId equals badges.Id
                                    select new TriggerRequirement
                                    {
                                        BadgeId = badges.Id,
                                        Name = programs.JoinBadgeName,
                                        Icon = ProgramIcon,
                                        IconDescription = JoinDescription,
                                        BadgePath = badges.Filename
                                    }
                                )
                                .Concat(
                                    from programs in _context.Programs
                                    .Where(_ => _.SiteId == filter.SiteId
                                        && _.AchieverBadgeId.HasValue
                                        && _.AchieverBadgeName.Contains(filter.Search ?? string.Empty)
                                        && (filter.BadgeIds == null
                                            || !filter.BadgeIds.Contains(_.AchieverBadgeId.Value)))
                                    .GroupBy(_ => _.AchieverBadgeId).Select(_ => _.First())
                                    join badges in _context.Badges
                                    on programs.AchieverBadgeId equals badges.Id
                                    select new TriggerRequirement
                                    {
                                        BadgeId = badges.Id,
                                        Name = programs.AchieverBadgeName,
                                        Icon = ProgramIcon,
                                        IconDescription = AchieverDescription,
                                        BadgePath = badges.Filename
                                    }
                                )
                                .Concat(
                                    from questionnaires in _context.Questionnaires
                                    .Where(_ => _.SiteId == filter.SiteId
                                        && _.BadgeId.HasValue
                                        && _.Name.Contains(filter.Search ?? string.Empty)
                                        && (filter.BadgeIds == null
                                            || !filter.BadgeIds.Contains(_.BadgeId.Value)))
                                    .GroupBy(_ => _.BadgeId).Select(_ => _.First())
                                    join badges in _context.Badges
                                    on questionnaires.BadgeId equals badges.Id
                                    select new TriggerRequirement
                                    {
                                        BadgeId = badges.Id,
                                        Name = questionnaires.BadgeName,
                                        Icon = ProgramIcon,
                                        IconDescription = QuestionnaireDescription,
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

            if (trigger.BadgeIds != null || trigger.ChallengeIds != null)
            {
                if (trigger.BadgeIds != null)
                {
                    foreach (var badgeId in trigger.BadgeIds)
                    {
                        var triggerBadge = new GRA.Data.Model.TriggerBadge()
                        {
                            BadgeId = badgeId,
                            TriggerId = newTrigger.Id
                        };
                        await _context.TriggerBadges.AddAsync(triggerBadge);
                    }
                }

                if (trigger.ChallengeIds != null)
                {
                    foreach (var challengeId in trigger.ChallengeIds)
                    {
                        var triggerChallenge = new GRA.Data.Model.TriggerChallenge()
                        {
                            ChallengeId = challengeId,
                            TriggerId = newTrigger.Id
                        };
                        await _context.TriggerChallenges.AddAsync(triggerChallenge);
                    }
                }
                await _context.SaveChangesAsync();
            }

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

        public async Task<bool> SecretCodeInUseAsync(int siteId, string secretCode)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.SecretCode == secretCode)
                .AnyAsync();
        }

        public async Task<Trigger> GetByBadgeIdAsync(int badgeId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.AwardBadgeId == badgeId)
                .ProjectTo<Trigger>()
                .FirstOrDefaultAsync();
        }

        public async Task RemoveUserTriggerAsync(int userId, int triggerId)
        {
            var userTrigger = await _context.UserTriggers
                .Where(_ => _.UserId == userId && _.TriggerId == triggerId)
                .SingleOrDefaultAsync();

            if (userTrigger != null)
            {
                _context.UserTriggers.Remove(userTrigger);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ICollection<Trigger>> GetTriggersAwardingBundleAsync(int bundleId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.AwardAvatarBundleId == bundleId)
                .ProjectTo<Trigger>()
                .ToListAsync();
        }

        public async Task<bool> BundleIsInUseAsync(int bundleId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.AwardAvatarBundleId == bundleId)
                .ProjectTo<Trigger>()
                .AnyAsync();
        }
    }
}
