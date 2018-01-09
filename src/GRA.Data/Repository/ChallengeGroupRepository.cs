using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class ChallengeGroupRepository
        : AuditingRepository<Model.ChallengeGroup, Domain.Model.ChallengeGroup>,
        IChallengeGroupRepository
    {
        public ChallengeGroupRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<ChallengeGroupRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public override async Task<ChallengeGroup> GetByIdAsync(int id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id)
                .ProjectTo<ChallengeGroup>()
                .SingleOrDefaultAsync();
        }

        public async Task<ChallengeGroup> GetActiveByIdAsync(int id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id 
                    && _.ChallengeGroupChallenges.Any(c => c.Challenge.IsActive == true
                       && c.Challenge.IsDeleted == false))
                .ProjectTo<ChallengeGroup>()
                .SingleOrDefaultAsync();
        }

        public async Task<ChallengeGroup> GetActiveByStubAsync(int siteId, string stub)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.Stub == stub
                    && _.ChallengeGroupChallenges.Any(c => c.Challenge.IsActive == true 
                        && c.Challenge.IsDeleted == false))
                .ProjectTo<ChallengeGroup>()
                .SingleOrDefaultAsync();
        }

        public async Task<List<ChallengeGroup>> GetByChallengeId(int siteId, int challengeId)
        {
            return await DbSet
                .AsNoTracking()
                .Include(_ => _.ChallengeGroupChallenges)
                .Where(_ => _.SiteId == siteId
                    && _.ChallengeGroupChallenges.Select(c => c.ChallengeId).Contains(challengeId))
                .ProjectTo<ChallengeGroup>()
                .ToListAsync();
        }

        public async Task<int> CountAsync(ChallengeGroupFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<IEnumerable<ChallengeGroup>> PageAsync(ChallengeGroupFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<ChallengeGroup>()
                .ToListAsync();
        }

        public IQueryable<Model.ChallengeGroup> ApplyFilters(ChallengeGroupFilter filter)
        {
            var challengeGroupList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (filter.ChallengeGroupIds?.Count > 0)
            {
                challengeGroupList = challengeGroupList
                    .Where(_ => filter.ChallengeGroupIds.Contains(_.Id) == false);
            }

            if (filter.ActiveGroups.HasValue)
            {
                var inactiveChallengeIds = _context.Challenges
                    .AsNoTracking()
                    .Where(_ => _.IsActive == false || _.IsDeleted == true)
                    .Select(_ => _.Id);

                challengeGroupList = challengeGroupList
                    .Where(_ => 
                        _.ChallengeGroupChallenges
                            .Where(c => inactiveChallengeIds.Contains(c.ChallengeId) == false)
                            .Select(c => c.ChallengeGroupId)
                        .Contains(_.Id) == filter.ActiveGroups.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                challengeGroupList = challengeGroupList.Where(_ => _.Name.Contains(filter.Search));
            }

            return challengeGroupList;
        }

        public async Task<ChallengeGroup> AddSaveAsync(int userId,
            ChallengeGroup challengeGroup, IEnumerable<int> challengeIds)
        {
            var newChallengeGroup = await base.AddSaveAsync(userId, challengeGroup);
            if (challengeIds.Count() > 0)
            {
                var time = _dateTimeProvider.Now;
                var challengeGroupChallengeList = new List<Model.ChallengeGroupChallenge>();
                foreach (var challengeId in challengeIds)
                {
                    challengeGroupChallengeList.Add(new Model.ChallengeGroupChallenge()
                    {
                        ChallengeGroupId = newChallengeGroup.Id,
                        ChallengeId = challengeId,
                        CreatedAt = time,
                        CreatedBy = userId
                    });
                }
                await _context.ChallengeGroupChallenges.AddRangeAsync(challengeGroupChallengeList);
                await _context.SaveChangesAsync();
            }

            return newChallengeGroup;
        }

        public async Task<ChallengeGroup> UpdateSaveAsync(int userId, ChallengeGroup challengeGroup,
            IEnumerable<int> challengesToAdd, IEnumerable<int> challengesToRemove)
        {
            await base.UpdateAsync(userId, challengeGroup);

            if (challengesToAdd?.Count() > 0)
            {
                var time = _dateTimeProvider.Now;
                var challengeGroupChallengeList = new List<Model.ChallengeGroupChallenge>();
                foreach (var challengeId in challengesToAdd)
                {
                    challengeGroupChallengeList.Add(new Model.ChallengeGroupChallenge()
                    {
                        ChallengeGroupId = challengeGroup.Id,
                        ChallengeId = challengeId,
                        CreatedAt = time,
                        CreatedBy = userId
                    });
                }
                await _context.ChallengeGroupChallenges.AddRangeAsync(challengeGroupChallengeList);
            }
            if (challengesToRemove?.Count() > 0)
            {
                var removeList = _context.ChallengeGroupChallenges
                    .Where(_ => _.ChallengeGroupId == challengeGroup.Id
                        && challengesToRemove.Contains(_.ChallengeId));
                _context.ChallengeGroupChallenges.RemoveRange(removeList);
            }

            await _context.SaveChangesAsync();
            return challengeGroup;
        }

        public override async Task RemoveSaveAsync(int userId, int challengeGroupId)
        {
            var challengeGroupChallenges = await _context.ChallengeGroupChallenges
                .Where(_ => _.ChallengeGroupId == challengeGroupId)
                .ToListAsync();
            _context.ChallengeGroupChallenges.RemoveRange(challengeGroupChallenges);

            await base.RemoveSaveAsync(userId, challengeGroupId);
        }

        public async Task<bool> StubInUseAsync(int siteId, string stub)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.Stub == stub)
                .AnyAsync();
        }
    }
}
