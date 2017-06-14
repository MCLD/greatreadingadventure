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
using GRA.Domain.Model.Filters;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageEvents)]
    public class TriggersController : Base.MCController
    {
        private readonly ILogger<TriggersController> _logger;
        private readonly BadgeService _badgeService;
        private readonly DynamicAvatarService _dynamicAvatarService;
        private readonly EventService _eventService;
        private readonly SiteService _siteService;
        private readonly TriggerService _triggerService;
        private readonly VendorCodeService _vendorCodeService;
        public TriggersController(ILogger<TriggersController> logger,
            ServiceFacade.Controller context,
            BadgeService badgeService,
            DynamicAvatarService dynamicAvatarService,
            EventService eventService,
            SiteService siteService,
            TriggerService triggerService,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _badgeService = Require.IsNotNull(badgeService, nameof(badgeService));
            _dynamicAvatarService = Require.IsNotNull(dynamicAvatarService, 
                nameof(dynamicAvatarService));
            _eventService = Require.IsNotNull(eventService, nameof(eventService));
            _siteService = Require.IsNotNull(siteService, nameof(SiteService));
            _triggerService = Require.IsNotNull(triggerService, nameof(triggerService));
            _vendorCodeService = Require.IsNotNull(vendorCodeService, nameof(vendorCodeService));
            PageTitle = "Triggers";
        }

        public async Task<IActionResult> Index(string search,
            int? systemId, int? branchId, bool? mine, int? programId, int page = 1)
        {
            var filter = new TriggerFilter(page);

            if (!string.IsNullOrWhiteSpace(search))
            {
                filter.Search = search;
            }

            if (mine == true)
            {
                filter.UserIds = new List<int> { GetId(ClaimType.UserId) };
            }
            else if (branchId.HasValue)
            {
                filter.BranchIds = new List<int> { branchId.Value };
            }
            else if (systemId.HasValue)
            {
                filter.SystemIds = new List<int> { systemId.Value };
            }

            if (programId.HasValue)
            {
                if (programId.Value > 0)
                {
                    filter.ProgramIds = new List<int?> { programId.Value };
                }
                else
                {
                    filter.ProgramIds = new List<int?> { null };
                }
            }

            var triggerList = await _triggerService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel
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

            foreach (var trigger in triggerList.Data)
            {
                trigger.AwardBadgeFilename =
                    _pathResolver.ResolveContentPath(trigger.AwardBadgeFilename);
            }

            var systemList = (await _siteService.GetSystemList())
                .OrderByDescending(_ => _.Id == GetId(ClaimType.SystemId)).ThenBy(_ => _.Name);

            var viewModel = new TriggersListViewModel
            {
                Triggers = triggerList.Data,
                PaginateModel = paginateModel,
                Search = search,
                SystemId = systemId,
                BranchId = branchId,
                ProgramId = programId,
                Mine = mine,
                SystemList = systemList,
                ProgramList = await _siteService.GetProgramList()
            };
            if (mine == true)
            {
                viewModel.BranchList = (await _siteService.GetBranches(GetId(ClaimType.SystemId)))
                        .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                        .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Mine";
            }
            else if (branchId.HasValue)
            {
                var branch = await _siteService.GetBranchByIdAsync(branchId.Value);
                viewModel.BranchName = branch.Name;
                viewModel.SystemName = systemList
                    .Where(_ => _.Id == branch.SystemId).SingleOrDefault().Name;
                viewModel.BranchList = (await _siteService.GetBranches(branch.SystemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Branch";
            }
            else if (systemId.HasValue)
            {
                viewModel.SystemName = systemList
                    .Where(_ => _.Id == systemId.Value).SingleOrDefault().Name;
                viewModel.BranchList = (await _siteService.GetBranches(systemId.Value))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "System";
            }
            else
            {
                viewModel.BranchList = (await _siteService.GetBranches(GetId(ClaimType.SystemId)))
                        .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                        .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "All";
            }
            if (programId.HasValue)
            {
                if (programId.Value > 0)
                {
                    viewModel.ProgramName =
                        (await _siteService.GetProgramByIdAsync(programId.Value)).Name;
                }
                else
                {
                    viewModel.ProgramName = "Not Limited";
                }
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var site = await GetCurrentSiteAsync();
            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            var viewModel = new TriggersDetailViewModel
            {
                Action = "Create",
                IsSecretCode = true,
                BadgeMakerUrl = GetBadgeMakerUrl(siteUrl, site.FromEmailAddress),
                UseBadgeMaker = true,
                EditAvatarBundle = UserHasPermission(Permission.ManageAvatars),
                EditMail = UserHasPermission(Permission.ManageTriggerMail),
                EditVendorCode = UserHasPermission(Permission.ManageVendorCodes),
                SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name"),
                BranchList = new SelectList((await _siteService.GetAllBranches()), "Id", "Name"),
                ProgramList = new SelectList((await _siteService.GetProgramList()), "Id", "Name")
            };

            if (viewModel.EditVendorCode)
            {
                viewModel.VendorCodeTypeList = new SelectList(
                    (await _vendorCodeService.GetTypeAllAsync()), "Id", "Description");
            }
            if (viewModel.EditAvatarBundle)
            {
                viewModel.UnlockableAvatarBundleList = new SelectList(
                    await _dynamicAvatarService.GetAllBundlesAsync(true), "Id", "Name");
            }

            PageTitle = "Create Trigger";
            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TriggersDetailViewModel model)
        {
            var badgeRequiredList = new List<int>();
            var challengeRequiredList = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.BadgeRequiredList))
            {
                badgeRequiredList = model.BadgeRequiredList
                    .Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(model.ChallengeRequiredList))
            {
                challengeRequiredList = model.ChallengeRequiredList
                .Split(',')
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .Select(int.Parse)
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

            if (model.AwardsPrize)
            {
                if (string.IsNullOrWhiteSpace(model.Trigger.AwardPrizeName))
                {
                    ModelState.AddModelError("Trigger.AwardPrizeName", "The Prize Name field is required.");
                }
            }
            if (model.AwardsMail)
            {
                if (string.IsNullOrWhiteSpace(model.Trigger.AwardMailSubject))
                {
                    ModelState.AddModelError("Trigger.AwardMailSubject", "The Mail Subject field is required.");
                }
                if (string.IsNullOrWhiteSpace(model.Trigger.AwardMail))
                {
                    ModelState.AddModelError("Trigger.AwardMail", "The Mail Message field is required.");
                }
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
                    if (!model.AwardsPrize)
                    {
                        model.Trigger.AwardPrizeName = "";
                        model.Trigger.AwardPrizeRedemptionInstructions = "";
                    }
                    if (!model.AwardsMail)
                    {
                        model.Trigger.AwardMailSubject = "";
                        model.Trigger.AwardMail = "";
                    }

                    if (model.BadgeUploadImage != null
                        || !string.IsNullOrWhiteSpace(model.BadgeMakerImage))
                    {
                        byte[] badgeBytes;
                        string filename;
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage) &&
                            (model.BadgeUploadImage == null || model.UseBadgeMaker))
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
                        var newBadge = new Badge
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
            model.TriggerRequirements = await _triggerService
                .GetRequirementsByIdsAsync(badgeRequiredList, challengeRequiredList);
            foreach (var requirement in model.TriggerRequirements)
            {
                requirement.BadgePath = _pathResolver.ResolveContentPath(requirement.BadgePath);
            }
            if (model.EditVendorCode)
            {
                model.VendorCodeTypeList = new SelectList(
                    (await _vendorCodeService.GetTypeAllAsync()), "Id", "Description");
            }
            if (model.EditAvatarBundle)
            {
                model.UnlockableAvatarBundleList = new SelectList(
                    await _dynamicAvatarService.GetAllBundlesAsync(true), "Id", "Name");
            }

            PageTitle = "Create Trigger";
            return View("Detail", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var trigger = await _triggerService.GetByIdAsync(id);
            var site = await GetCurrentSiteAsync();
            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            var viewModel = new TriggersDetailViewModel
            {
                Trigger = trigger,
                Action = "Edit",
                IsSecretCode = !string.IsNullOrWhiteSpace(trigger.SecretCode),
                BadgeMakerUrl = GetBadgeMakerUrl(siteUrl, site.FromEmailAddress),
                UseBadgeMaker = true,
                EditAvatarBundle = UserHasPermission(Permission.ManageAvatars),
                EditMail = UserHasPermission(Permission.ManageTriggerMail),
                EditVendorCode = UserHasPermission(Permission.ManageVendorCodes),
                AwardsMail = !string.IsNullOrWhiteSpace(trigger.AwardMailSubject),
                AwardsPrize = !string.IsNullOrWhiteSpace(trigger.AwardPrizeName),
                DependentTriggers = await _triggerService.GetDependentsAsync(trigger.AwardBadgeId),
                TriggerRequirements = await _triggerService.GetTriggerRequirementsAsync(trigger),
                BadgeRequiredList = string.Join(",", trigger.BadgeIds),
                ChallengeRequiredList = string.Join(",", trigger.ChallengeIds),
                SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name"),
                ProgramList = new SelectList((await _siteService.GetProgramList()), "Id", "Name")
            };

            if (viewModel.EditVendorCode)
            {
                viewModel.VendorCodeTypeList = new SelectList(
                    (await _vendorCodeService.GetTypeAllAsync()), "Id", "Description");
            }
            else if (viewModel.Trigger.AwardVendorCodeTypeId.HasValue)
            {
                viewModel.VendorCodeType = (await _vendorCodeService
                    .GetTypeById(viewModel.Trigger.AwardVendorCodeTypeId.Value)).Description;
            }

            if (viewModel.EditAvatarBundle)
            {
                viewModel.UnlockableAvatarBundleList = new SelectList(
                    await _dynamicAvatarService.GetAllBundlesAsync(true), "Id", "Name");
            }
            else if (viewModel.Trigger.AwardAvatarBundleId.HasValue)
            {
                viewModel.UnlockableAvatarBundle = (await _dynamicAvatarService
                    .GetBundleByIdAsync(viewModel.Trigger.AwardAvatarBundleId.Value)).Name;
            }

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

            if (UserHasPermission(Permission.ManageEvents))
            {
                viewModel.RelatedEvents = await _eventService.GetRelatedEventsForTriggerAsync(id);
            }
            PageTitle = $"Edit Trigger - {viewModel.Trigger.Name}";
            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TriggersDetailViewModel model)
        {
            var badgeRequiredList = new List<int>();
            var challengeRequiredList = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.BadgeRequiredList))
            {
                badgeRequiredList = model.BadgeRequiredList
                    .Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(model.ChallengeRequiredList))
            {
                challengeRequiredList = model.ChallengeRequiredList
                .Split(',')
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .Select(int.Parse)
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
            if (model.AwardsPrize)
            {
                if (string.IsNullOrWhiteSpace(model.Trigger.AwardPrizeName))
                {
                    ModelState.AddModelError("Trigger.AwardPrizeName", "The Prize Name field is required.");
                }
            }
            if (model.AwardsMail)
            {
                if (string.IsNullOrWhiteSpace(model.Trigger.AwardMailSubject))
                {
                    ModelState.AddModelError("Trigger.AwardMailSubject", "The Mail Subject field is required.");
                }
                if (string.IsNullOrWhiteSpace(model.Trigger.AwardMail))
                {
                    ModelState.AddModelError("Trigger.AwardMail", "The Mail Message field is required.");
                }
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
                    if (!model.AwardsPrize)
                    {
                        model.Trigger.AwardPrizeName = "";
                        model.Trigger.AwardPrizeRedemptionInstructions = "";
                    }
                    if (!model.AwardsMail)
                    {
                        model.Trigger.AwardMailSubject = "";
                        model.Trigger.AwardMail = "";
                    }
                    if (model.BadgeUploadImage != null
                        || !string.IsNullOrWhiteSpace(model.BadgeMakerImage))
                    {
                        byte[] badgeBytes;
                        string filename;
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage) &&
                            (model.BadgeUploadImage == null || model.UseBadgeMaker))
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
            model.TriggerRequirements = await _triggerService
                .GetRequirementsByIdsAsync(badgeRequiredList, challengeRequiredList);
            foreach (var requirement in model.TriggerRequirements)
            {
                requirement.BadgePath = _pathResolver.ResolveContentPath(requirement.BadgePath);
            }

            if (model.EditVendorCode)
            {
                model.VendorCodeTypeList = new SelectList(
                    (await _vendorCodeService.GetTypeAllAsync()), "Id", "Description");
            }
            else if (model.Trigger.AwardVendorCodeTypeId.HasValue)
            {
                model.VendorCodeType = (await _vendorCodeService
                    .GetTypeById(model.Trigger.AwardVendorCodeTypeId.Value)).Description;
            }

            if (model.EditAvatarBundle)
            {
                model.UnlockableAvatarBundleList = new SelectList(
                    await _dynamicAvatarService.GetAllBundlesAsync(true), "Id", "Name");
            }
            else if (model.Trigger.AwardAvatarBundleId.HasValue)
            {
                model.UnlockableAvatarBundle = (await _dynamicAvatarService
                    .GetBundleByIdAsync(model.Trigger.AwardAvatarBundleId.Value)).Name;
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
            var filter = new TriggerFilter(page)
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
    }
}
