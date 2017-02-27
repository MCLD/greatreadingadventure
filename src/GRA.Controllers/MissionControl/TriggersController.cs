using GRA.Controllers.ViewModel.Shared;
using GRA.Controllers.ViewModel.MissionControl.Triggers;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Collections.Generic;
using System;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageEvents)]
    public class TriggersController : Base.MCController
    {
        private readonly ILogger<TriggersController> _logger;
        private readonly BadgeService _badgeService;
        private readonly SiteService _siteService;
        private readonly TriggerService _triggerService;
        private readonly VendorCodeService _vendorCodeService;
        public TriggersController(ILogger<TriggersController> logger,
            ServiceFacade.Controller context,
            BadgeService badgeService,
            SiteService siteService,
            TriggerService triggerService,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _badgeService = Require.IsNotNull(badgeService, nameof(badgeService));
            _siteService = Require.IsNotNull(siteService, nameof(SiteService));
            _triggerService = Require.IsNotNull(triggerService, nameof(triggerService));
            _vendorCodeService = Require.IsNotNull(vendorCodeService, nameof(vendorCodeService));
            PageTitle = "Triggers";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            Domain.Model.Filter filter = new Domain.Model.Filter(page);

            var triggerList = await _triggerService.GetPaginatedListAsync(filter);

            foreach (var trigger in triggerList.Data)
            {
                trigger.AwardBadgeFilename =
                    _pathResolver.ResolveContentPath(trigger.AwardBadgeFilename);
            }

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = triggerList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };

            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            TriggersListViewModel viewModel = new TriggersListViewModel()
            {
                Triggers = triggerList.Data,
                PaginateModel = paginateModel,
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var site = await GetCurrentSiteAsync();
            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            TriggersDetailViewModel viewModel = new TriggersDetailViewModel()
            {
                Action = "Create",
                IsSecretCode = true,
                BadgeMakerUrl = GetBadgeMakerUrl(siteUrl, site.FromEmailAddress),
                UseBadgeMaker = true,
                EditVendorCode = UserHasPermission(Permission.ManageVendorCodes),
                SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name"),
                BranchList = new SelectList((await _siteService.GetAllBranches()), "Id", "Name"),
                ProgramList = new SelectList((await _siteService.GetProgramList()), "Id", "Name"),
                VendorCodeTypeList = new SelectList(
                    (await _vendorCodeService.GetTypeAllAsync()), "Id", "Description")
            };

            PageTitle = "Create Trigger";
            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TriggersDetailViewModel model)
        {
            List<int> badgeRequiredList = new List<int>();
            List<int> challengeRequiredList = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.BadgeRequiredList))
            {
                badgeRequiredList = model.BadgeRequiredList
                    .Replace("<", "")
                    .Split('>')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(Int32.Parse)
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(model.ChallengeRequiredList))
            {
                challengeRequiredList = model.ChallengeRequiredList
                .Replace("<", "")
                .Split('>')
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .Select(Int32.Parse)
                .ToList();
            }
            var requirementCount = badgeRequiredList.Count + challengeRequiredList.Count;

            if (string.IsNullOrWhiteSpace(model.BadgeMakerImage) && model.BadgeUploadImage == null)
            {
                ModelState.AddModelError("BadgePath", "A badge is required.");
            }
            else if (model.BadgeUploadImage != null
                && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker)
                && (Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpeg"
                    && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".png"))
            {
                ModelState.AddModelError("BadgeUploadImage", "Please use a .jpg or .png image.");
            }
            if (!model.IsSecretCode)
            {
                if ((!model.Trigger.Points.HasValue || model.Trigger.Points < 1)
                    && requirementCount < 1)
                {
                    ModelState.AddModelError("TriggerRequirements", "Points or a Challenge/Trigger item is required.");
                }
                else if ((!model.Trigger.ItemsRequired.HasValue || model.Trigger.ItemsRequired < 1)
                    && requirementCount >= 1)
                {
                    ModelState.AddModelError("Trigger.ItemsRequired", "Please enter how many of the Challenge/Trigger item are required.");
                }
                else if (model.Trigger.ItemsRequired > requirementCount)
                {
                    ModelState.AddModelError("Trigger.ItemsRequired", "Items Required can not be greater than the number of Challenge/Trigger items.");
                }
            }
            else if (string.IsNullOrWhiteSpace(model.Trigger.SecretCode))
            {
                ModelState.AddModelError("Trigger.SecretCode", "The Secret Code field is required.");
            }
            else if (await _triggerService.CodeExistsAsync(model.Trigger.SecretCode))
            {
                ModelState.AddModelError("Trigger.SecretCode", "That Secret Code already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.IsSecretCode)
                    {
                        model.Trigger.Points = null;
                        model.Trigger.ItemsRequired = null;
                        model.Trigger.LimitToSystemId = null;
                        model.Trigger.LimitToBranchId = null;
                        model.Trigger.LimitToProgramId = null;
                        model.Trigger.SecretCode = model.Trigger.SecretCode.Trim().ToLower();
                        model.Trigger.BadgeIds = new List<int>();
                        model.Trigger.ChallengeIds = new List<int>();
                    }
                    else
                    {
                        model.Trigger.SecretCode = null;
                        model.Trigger.BadgeIds = badgeRequiredList;
                        model.Trigger.ChallengeIds = challengeRequiredList;
                    }

                    if (model.BadgeUploadImage != null
                        || !string.IsNullOrWhiteSpace(model.BadgeMakerImage))
                    {
                        byte[] badgeBytes;
                        string filename;
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage) &&
                            (model.BadgeUploadImage != null || model.UseBadgeMaker))
                        {
                            var badgeString = model.BadgeMakerImage.Split(',').Last();
                            badgeBytes = Convert.FromBase64String(badgeString);
                            filename = "badge.png";
                        }
                        else
                        {
                            using (var fileStream = model.BadgeUploadImage.OpenReadStream())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    fileStream.CopyTo(ms);
                                    badgeBytes = ms.ToArray();
                                }
                            }
                            filename = Path.GetFileName(model.BadgeUploadImage.FileName);
                        }
                        Badge newBadge = new Badge()
                        {
                            Filename = filename
                        };
                        var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                        model.Trigger.AwardBadgeId = badge.Id;
                    }
                    var trigger = await _triggerService.AddAsync(model.Trigger);
                    ShowAlertSuccess($"Trigger '<strong>{trigger.Name}</strong>' was successfully created");
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Unable to add trigger: ", gex.Message);
                }
            }
            model.Action = "Create";
            model.SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name");
            if (model.Trigger.LimitToSystemId.HasValue)
            {
                model.BranchList = new SelectList(
                    (await _siteService.GetBranches(model.Trigger.LimitToSystemId.Value)), "Id", "Name");
            }
            else
            {
                model.BranchList = new SelectList((await _siteService.GetAllBranches()), "Id", "Name");
            }
            model.ProgramList = new SelectList((await _siteService.GetProgramList()), "Id", "Name");
            model.VendorCodeTypeList = new SelectList(
                    (await _vendorCodeService.GetTypeAllAsync()), "Id", "Description");
            model.TriggerRequirements = await _triggerService
                .GetRequirementsByIdsAsync(badgeRequiredList, challengeRequiredList);
            foreach (var requirement in model.TriggerRequirements)
            {
                requirement.BadgePath = _pathResolver.ResolveContentPath(requirement.BadgePath);
            }
            PageTitle = "Create Trigger";
            return View("Detail", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var trigger = await _triggerService.GetByIdAsync(id);
            var site = await GetCurrentSiteAsync();
            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            TriggersDetailViewModel viewModel = new TriggersDetailViewModel()
            {
                Trigger = trigger,
                Action = "Edit",
                IsSecretCode = !string.IsNullOrWhiteSpace(trigger.SecretCode),
                BadgeMakerUrl = GetBadgeMakerUrl(siteUrl, site.FromEmailAddress),
                UseBadgeMaker = true,
                EditVendorCode = UserHasPermission(Permission.ManageVendorCodes),
                DependentTriggers = await _triggerService.GetDependentsAsync(trigger.AwardBadgeId),
                TriggerRequirements = await _triggerService.GetTriggerRequirementsAsync(trigger),
                BadgeRequiredList = string.Join("", trigger.BadgeIds
                    .Select(_ => "<" + _.ToString() + ">")),
                ChallengeRequiredList = string.Join("", trigger.ChallengeIds
                .Select(_ => "<" + _.ToString() + ">")),
                SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name"),
                ProgramList = new SelectList((await _siteService.GetProgramList()), "Id", "Name"),
                VendorCodeTypeList = new SelectList(
                    (await _vendorCodeService.GetTypeAllAsync()), "Id", "Description")
            };
            if (viewModel.Trigger.LimitToSystemId.HasValue)
            {
                viewModel.BranchList = new SelectList(
                    (await _siteService.GetBranches(viewModel.Trigger.LimitToSystemId.Value)), "Id", "Name");
            }
            else
            {
                viewModel.BranchList = new SelectList((await _siteService.GetAllBranches()), "Id", "Name");
            }
            foreach (var requirement in viewModel.TriggerRequirements)
            {
                if (!string.IsNullOrWhiteSpace(requirement.BadgePath))
                {
                    requirement.BadgePath = _pathResolver.ResolveContentPath(requirement.BadgePath);
                }
            }
            if (!string.IsNullOrWhiteSpace(viewModel.Trigger.AwardBadgeFilename))
            {
                viewModel.BadgePath = _pathResolver.ResolveContentPath(viewModel.Trigger.AwardBadgeFilename);
            }
            PageTitle = $"Edit Trigger - {viewModel.Trigger.Name}";
            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TriggersDetailViewModel model)
        {
            List<int> badgeRequiredList = new List<int>();
            List<int> challengeRequiredList = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.BadgeRequiredList))
            {
                badgeRequiredList = model.BadgeRequiredList
                    .Replace("<", "")
                    .Split('>')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(Int32.Parse)
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(model.ChallengeRequiredList))
            {
                challengeRequiredList = model.ChallengeRequiredList
                .Replace("<", "")
                .Split('>')
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .Select(Int32.Parse)
                .ToList();
            }
            var requirementCount = badgeRequiredList.Count + challengeRequiredList.Count;

            if (model.BadgeUploadImage != null
                && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker)
                && (Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpeg"
                    && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".png"))
            {
                ModelState.AddModelError("BadgeImage", "Please use a .jpg or .png image.");
            }

            if (!model.IsSecretCode)
            {
                if ((!model.Trigger.Points.HasValue || model.Trigger.Points < 1)
                    && requirementCount < 1)
                {
                    ModelState.AddModelError("TriggerRequirements", "Points or a Challenge/Trigger item is required.");
                }
                else if ((!model.Trigger.ItemsRequired.HasValue || model.Trigger.ItemsRequired < 1)
                    && requirementCount >= 1)
                {
                    ModelState.AddModelError("Trigger.ItemsRequired", "Please enter how many of the Challenge/Trigger item are required.");
                }
                else if (model.Trigger.ItemsRequired > requirementCount)
                {
                    ModelState.AddModelError("Trigger.ItemsRequired", "Items Required can not be greater than the number of Challenge/Trigger items.");
                }
            }
            else if (string.IsNullOrWhiteSpace(model.Trigger.SecretCode))
            {
                ModelState.AddModelError("Trigger.SecretCode", "The Secret Code field is required.");
            }
            else if (await _triggerService.CodeExistsAsync(model.Trigger.SecretCode, model.Trigger.Id))
            {
                ModelState.AddModelError("Trigger.SecretCode", "That Secret Code already exists.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.IsSecretCode)
                    {
                        model.Trigger.Points = null;
                        model.Trigger.ItemsRequired = null;
                        model.Trigger.LimitToSystemId = null;
                        model.Trigger.LimitToBranchId = null;
                        model.Trigger.LimitToProgramId = null;
                        model.Trigger.SecretCode = model.Trigger.SecretCode.Trim().ToLower();
                        model.Trigger.BadgeIds = new List<int>();
                        model.Trigger.ChallengeIds = new List<int>();
                    }
                    else
                    {
                        model.Trigger.SecretCode = null;
                        model.Trigger.BadgeIds = badgeRequiredList;
                        model.Trigger.ChallengeIds = challengeRequiredList;
                    }
                    if (model.BadgeUploadImage != null
                        || !string.IsNullOrWhiteSpace(model.BadgeMakerImage))
                    {
                        byte[] badgeBytes;
                        string filename;
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage) &&
                            (model.BadgeUploadImage != null || model.UseBadgeMaker))
                        {
                            var badgeString = model.BadgeMakerImage.Split(',').Last();
                            badgeBytes = Convert.FromBase64String(badgeString);
                            filename = "badge.png";
                        }
                        else
                        {
                            using (var fileStream = model.BadgeUploadImage.OpenReadStream())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    fileStream.CopyTo(ms);
                                    badgeBytes = ms.ToArray();
                                }
                            }
                            filename = Path.GetFileName(model.BadgeUploadImage.FileName);
                        }
                        var existing = await _badgeService
                                    .GetByIdAsync(model.Trigger.AwardBadgeId);
                        existing.Filename = Path.GetFileName(model.BadgePath);
                        await _badgeService.ReplaceBadgeFileAsync(existing, badgeBytes);
                    }
                    var savedtrigger = await _triggerService.UpdateAsync(model.Trigger);
                    ShowAlertSuccess($"Trigger '<strong>{savedtrigger.Name}</strong>' was successfully modified");
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Unable to edit trigger: ", gex.Message);
                }
            }

            model.Action = "Edit";
            model.DependentTriggers = await _triggerService.GetDependentsAsync(model.Trigger.Id);
            model.SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name");
            if (model.Trigger.LimitToSystemId.HasValue)
            {
                model.BranchList = new SelectList(
                    (await _siteService.GetBranches(model.Trigger.LimitToSystemId.Value)), "Id", "Name");
            }
            else
            {
                model.BranchList = new SelectList((await _siteService.GetAllBranches()), "Id", "Name");
            }
            model.ProgramList = new SelectList((await _siteService.GetProgramList()), "Id", "Name");
            model.VendorCodeTypeList = new SelectList(
                    (await _vendorCodeService.GetTypeAllAsync()), "Id", "Description");
            model.TriggerRequirements = await _triggerService
                .GetRequirementsByIdsAsync(badgeRequiredList, challengeRequiredList);
            foreach (var requirement in model.TriggerRequirements)
            {
                requirement.BadgePath = _pathResolver.ResolveContentPath(requirement.BadgePath);
            }
            PageTitle = $"Edit Trigger - {model.Trigger.Name}";
            return View("Detail", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _triggerService.RemoveAsync(id);
                ShowAlertSuccess("Trigger deleted.");
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to delete trigger: ", gex.Message);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetRequirementList(string badgeIds,
            string challengeIds,
            string scope,
            string search,
            int page = 1,
            int? thisBadge = null)
        {
            Domain.Model.Filter filter = new Domain.Model.Filter(page)
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
                badgeIds = badgeIds.Replace("<", "");
                badgeList.AddRange(badgeIds.Split('>')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(Int32.Parse)
                    .ToList());

            }
            if (badgeList.Count > 0)
            {
                filter.BadgeIds = badgeList;
            }

            if (!string.IsNullOrWhiteSpace(challengeIds))
            {
                challengeIds = challengeIds.Replace("<", "");
                filter.ChallengeIds = challengeIds.Split('>')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(Int32.Parse)
                    .ToList();
            }
            switch (scope)
            {
                case ("System"):
                    filter.SystemIds = new List<int>() { GetId(ClaimType.SystemId) };
                    break;
                case ("Branch"):
                    filter.BranchIds = new List<int>() { GetId(ClaimType.BranchId) };
                    break;
                case ("Mine"):
                    filter.UserIds = new List<int>() { GetId(ClaimType.UserId) };
                    break;
                default:
                    break;
            }

            var requirements = await _triggerService.PageRequirementAsync(filter);
            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = requirements.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            RequirementListViewModel viewModel = new RequirementListViewModel()
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
    }
}
