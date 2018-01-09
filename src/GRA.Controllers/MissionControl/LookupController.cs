using GRA.Controllers.ViewModel.MissionControl.Lookup;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.AccessMissionControl)]
    public class LookupController : Base.MCController
    {
        private readonly ILogger<LookupController> _logger;
        private readonly ChallengeService _challengeService;
        private readonly TriggerService _triggerService;

        public LookupController(ILogger<LookupController> logger,
             ServiceFacade.Controller context,
             ChallengeService challengeService,
             TriggerService triggerService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _challengeService = Require.IsNotNull(challengeService, nameof(challengeService));
            _triggerService = Require.IsNotNull(triggerService, nameof(triggerService));
        }

        [HttpPost]
        public async Task<JsonResult> SecretCodeInUse(string secretCode)
        {
            return Json(await _triggerService.SecretCodeInUseAsync(secretCode));
        }

        public async Task<IActionResult> GetRequirementList(string badgeIds,
            string challengeIds,
            string scope,
            string search,
            int page = 1,
            int? thisBadge = null)
        {
            var filter = new BaseFilter(page, 10)
            {
                Search = search
            };

            var badgeList = new List<int>();
            if (thisBadge.HasValue)
            {
                badgeList.Add(thisBadge.Value);
            }
            if (!string.IsNullOrWhiteSpace(badgeIds))
            {
                badgeList.AddRange(badgeIds.Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .ToList());
            }
            if (badgeList.Count > 0)
            {
                filter.BadgeIds = badgeList;
            }

            if (!string.IsNullOrWhiteSpace(challengeIds))
            {
                filter.ChallengeIds = challengeIds.Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .ToList();
            }
            switch (scope)
            {
                case ("System"):
                    filter.SystemIds = new List<int> { GetId(ClaimType.SystemId) };
                    break;
                case ("Branch"):
                    filter.BranchIds = new List<int> { GetId(ClaimType.BranchId) };
                    break;
                case ("Mine"):
                    filter.UserIds = new List<int> { GetId(ClaimType.UserId) };
                    break;
                default:
                    break;
            }

            var requirements = await _triggerService.PageRequirementAsync(filter);
            var paginateModel = new PaginateViewModel
            {
                ItemCount = requirements.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            var viewModel = new RequirementListViewModel
            {
                Requirements = requirements.Data,
                PaginateModel = paginateModel
            };
            foreach (var requirement in requirements.Data)
            {
                if (!string.IsNullOrWhiteSpace(requirement.BadgePath))
                {
                    requirement.BadgePath = _pathResolver.ResolveContentPath(requirement.BadgePath);
                }
            }

            return PartialView("_RequirementsPartial", viewModel);
        }

        [Authorize(Policy = Policy.ViewAllChallenges)]
        public async Task<IActionResult> GetChallengeList(string challengeIds,
            string scope,
            string search,
            bool showActive = false,
            int page = 1)
        {
            var filter = new ChallengeFilter(page, 10)
            {
                IsActive = true,
                Search = search
            };

            if (!string.IsNullOrWhiteSpace(challengeIds))
            {
                filter.ChallengeIds = challengeIds.Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .ToList();
            }

            switch (scope.ToLower())
            {
                case ("system"):
                    filter.SystemIds = new List<int> { GetId(ClaimType.SystemId) };
                    break;
                case ("branch"):
                    filter.BranchIds = new List<int> { GetId(ClaimType.BranchId) };
                    break;
                case ("mine"):
                    filter.UserIds = new List<int> { GetId(ClaimType.UserId) };
                    break;
                default:
                    break;
            }

            var challengeList = await _challengeService.MCGetPaginatedChallengeListAsync(filter);
            var paginateModel = new PaginateViewModel
            {
                ItemCount = challengeList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            var viewModel = new ChallengesListViewModel
            {
                Challenges = challengeList.Data,
                PaginateModel = paginateModel,
                CanEditChallenges = UserHasPermission(Permission.EditChallenges),
                ShowActive = showActive
            };

            foreach (var challenge in viewModel.Challenges)
            {
                if (!string.IsNullOrWhiteSpace(challenge.BadgeFilename))
                {
                    challenge.BadgeFilename = _pathResolver.ResolveContentPath(
                        challenge.BadgeFilename);
                }
            }

            return PartialView("_ChallengeListPartial", viewModel);
        }

        [Authorize(Policy = Policy.ViewAllChallenges)]
        public async Task<IActionResult> GetChallengeGroupList(string challengeGroupIds,
            string search,
            int page = 1)
        {
            var filter = new ChallengeGroupFilter(page, 10)
            {
                ActiveGroups = true,
                Search = search
            };

            if (!string.IsNullOrWhiteSpace(challengeGroupIds))
            {
                filter.ChallengeGroupIds = challengeGroupIds.Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .ToList();
            }

            var challengeGroupList = await _challengeService.GetPaginatedGroupListAsync(filter);
            var paginateModel = new PaginateViewModel
            {
                ItemCount = challengeGroupList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            var viewModel = new ChallengeGroupsListViewModel
            {
                ChallengeGroups = challengeGroupList.Data,
                PaginateModel = paginateModel,
                CanEditGroups = UserHasPermission(Permission.EditChallengeGroups),
            };

            return PartialView("_ChallengeGroupListPartial", viewModel);
        }
    }
}
