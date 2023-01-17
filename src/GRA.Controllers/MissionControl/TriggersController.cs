using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Triggers;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageTriggers)]
    public class TriggersController : Base.MCController
    {
        private readonly AttachmentService _attachmentService;
        private readonly AvatarService _avatarService;
        private readonly BadgeService _badgeService;
        private readonly EventService _eventService;
        private readonly ILogger<TriggersController> _logger;
        private readonly SiteService _siteService;
        private readonly TriggerService _triggerService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        public TriggersController(ILogger<TriggersController> logger,
            ServiceFacade.Controller context,
            AvatarService avatarService,
            BadgeService badgeService,
            AttachmentService attachmentService,
            EventService eventService,
            SiteService siteService,
            TriggerService triggerService,
            VendorCodeService vendorCodeService,
            UserService userService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _avatarService = avatarService
                ?? throw new ArgumentNullException(nameof(avatarService));
            _badgeService = badgeService ?? throw new ArgumentNullException(nameof(badgeService));
            _attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(attachmentService));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _triggerService = triggerService
                ?? throw new ArgumentNullException(nameof(triggerService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            PageTitle = "Triggers";
        }

        public static string Name
        { get { return "Triggers"; } }

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
                EditAttachment = UserHasPermission(Permission.TriggerAttachments),
                EditMail = UserHasPermission(Permission.ManageTriggerMail),
                EditVendorCode = UserHasPermission(Permission.ManageVendorCodes),
                SystemList = new SelectList(await _siteService.GetSystemList(), "Id", "Name"),
                BranchList = new SelectList(await _siteService.GetAllBranches(), "Id", "Name"),
                ProgramList = new SelectList(await _siteService.GetProgramList(), "Id", "Name"),
                IgnorePointLimits = UserHasPermission(Permission.IgnorePointLimits),
                MaxPointLimit = await _triggerService.GetMaximumAllowedPointsAsync(GetCurrentSiteId())
            };
            if (viewModel.MaxPointLimit.HasValue)
            {
                viewModel.MaxPointsMessage = $"(Up to {viewModel.MaxPointLimit.Value} points)";
            }

            if (viewModel.EditVendorCode)
            {
                viewModel.VendorCodeTypeList = new SelectList(
                    await _vendorCodeService.GetTypeAllAsync(), "Id", "Description");
            }
            if (viewModel.EditAvatarBundle)
            {
                viewModel.UnlockableAvatarBundleList = new SelectList(
                    await _avatarService.GetAllBundlesAsync(true), "Id", "Name");
            }

            PageTitle = "Create Trigger";
            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TriggersDetailViewModel model)
        {
            byte[] badgeBytes = null;
            byte[] attachmentBytes = null;

            var badgeRequiredList = new List<int>();
            var challengeRequiredList = new List<int>();

            model.IgnorePointLimits = UserHasPermission(Permission.IgnorePointLimits);
            model.MaxPointLimit = await _triggerService
                .GetMaximumAllowedPointsAsync(GetCurrentSiteId());
            if (!model.IgnorePointLimits
                && model.MaxPointLimit.HasValue
                && model.Trigger.AwardPoints > model.MaxPointLimit)
            {
                ModelState.AddModelError("Trigger.AwardPoints",
                    $"You may award up to {model.MaxPointLimit} points.");
            }
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
            if (string.IsNullOrWhiteSpace(model.BadgeAltText))
            {
                ModelState.AddModelError("BadgeAltText", "The badge's alternative text is required.");
            }
            if (string.IsNullOrWhiteSpace(model.BadgeMakerImage) && model.BadgeUploadImage == null)
            {
                ModelState.AddModelError("BadgePath", "A badge is required.");
            }
            else if (model.BadgeUploadImage != null
                    && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker))
            {
                if (!ValidImageExtensions.Contains(
                    Path.GetExtension(model.BadgeUploadImage.FileName).ToLowerInvariant()))
                {
                    ModelState.AddModelError("BadgeUploadImage", $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                }
                if (model.BadgeUploadImage != null)
                {
                    try
                    {
                        using (var ms = new MemoryStream())
                        {
                            await model.BadgeUploadImage.CopyToAsync(ms);
                            badgeBytes = ms.ToArray();
                        }
                        await _badgeService.ValidateBadgeImageAsync(badgeBytes);
                    }
                    catch (GraException gex)
                    {
                        ModelState.AddModelError("BadgeUploadImage", gex.Message);
                    }
                }
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

            if (model.AwardsPrize && string.IsNullOrWhiteSpace(model.Trigger.AwardPrizeName))
            {
                ModelState.AddModelError("Trigger.AwardPrizeName", "The Prize Name field is required.");
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

            if (model.AwardsAttachment)
            {
                if (model.AttachmentUploadFile == null)
                {
                    ModelState.AddModelError("AttachmentUploadFile", "An attachment is required.");
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
                        model.Trigger.SecretCode = model
                            .Trigger.SecretCode
                            .Trim()
                            .ToLowerInvariant();
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
                    if (!model.AwardsAttachment)
                    {
                        model.Trigger.AwardAttachmentFilename = "";
                        model.Trigger.AwardAttachmentId = null;
                    }

                    if (model.BadgeUploadImage != null
                        || !string.IsNullOrWhiteSpace(model.BadgeMakerImage))
                    {
                        string filename;
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage)
                            && (model.BadgeUploadImage == null || model.UseBadgeMaker))
                        {
                            var badgeString = model.BadgeMakerImage.Split(',').Last();
                            badgeBytes = Convert.FromBase64String(badgeString);
                            filename = "badge.png";
                        }
                        else
                        {
                            if (badgeBytes == null)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    await model.BadgeUploadImage.CopyToAsync(ms);
                                    badgeBytes = ms.ToArray();
                                }
                            }
                            filename = Path.GetFileName(model.BadgeUploadImage.FileName);
                        }
                        var newBadge = new Badge
                        {
                            Filename = filename,
                            AltText = model.BadgeAltText
                        };
                        var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                        model.Trigger.AwardBadgeId = badge.Id;
                    }

                    if (model.AttachmentUploadFile != null)
                    {
                        string fileName;
                        using (var ms = new MemoryStream())
                        {
                            await model.AttachmentUploadFile.CopyToAsync(ms);
                            attachmentBytes = ms.ToArray();
                        }
                        fileName = Path.GetFileName(model.AttachmentUploadFile.FileName);

                        var newAttachment = new Attachment
                        {
                            FileName = fileName
                        };
                        var attachment = await _attachmentService.AddAttachmentAsync(newAttachment, attachmentBytes);
                        model.Trigger.AwardAttachmentId = attachment.Id;
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
            model.SystemList = new SelectList(await _siteService.GetSystemList(), "Id", "Name");
            if (model.Trigger.LimitToSystemId.HasValue)
            {
                model.BranchList = new SelectList(
                    await _siteService.GetBranches(model.Trigger.LimitToSystemId.Value), "Id", "Name");
            }
            else
            {
                model.BranchList = new SelectList(await _siteService.GetAllBranches(), "Id", "Name");
            }
            model.ProgramList = new SelectList(await _siteService.GetProgramList(), "Id", "Name");
            model.TriggerRequirements = await _triggerService
                .GetRequirementsByIdsAsync(badgeRequiredList, challengeRequiredList);
            foreach (var requirement in model.TriggerRequirements)
            {
                requirement.BadgePath = _pathResolver.ResolveContentPath(requirement.BadgePath);
            }
            if (model.EditVendorCode)
            {
                model.VendorCodeTypeList = new SelectList(
                    await _vendorCodeService.GetTypeAllAsync(), "Id", "Description");
            }
            if (model.EditAvatarBundle)
            {
                model.UnlockableAvatarBundleList = new SelectList(
                    await _avatarService.GetAllBundlesAsync(true), "Id", "Name");
            }

            if (model.MaxPointLimit.HasValue)
            {
                model.MaxPointsMessage = $"(Up to {model.MaxPointLimit.Value} points)";
            }

            PageTitle = "Create Trigger";
            return View("Detail", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var requireSecretCode = await GetSiteSettingBoolAsync(
                SiteSettingKey.Events.RequireBadge);
                if (requireSecretCode)
                {
                    var relatedEvents = await _eventService.GetRelatedEventsForTriggerAsync(id);
                    if (relatedEvents.Count > 0)
                    {
                        ShowAlertWarning("Unable to delete trigger: Trigger has related events");
                        return RedirectToAction("Index");
                    }
                }

                await _triggerService.RemoveAsync(id);
                ShowAlertSuccess("Trigger deleted.");
            }
            catch (GraException gex)
            {
                if (gex.Data?.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (DictionaryEntry trigger in gex.Data)
                    {
                        sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                            "<a href=\"{0}\" target=\"_blank\">{1}</a>, ",
                            Url.Action(nameof(TriggersController.Edit),
                                new { controller = TriggersController.Name, id = trigger.Key }),
                            trigger.Value);
                    }
                    ShowAlertWarning("Unable to delete event due to these trigger(s): ",
                        sb.ToString().Trim(' ').Trim(','));
                }
                else
                {
                    ShowAlertWarning("Unable to delete trigger: ", gex.Message);
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var trigger = await _triggerService.GetByIdAsync(id);

            if (trigger == null)
            {
                ShowAlertWarning($"Could not find trigger id {id}, possibly it has been deleted.");
                return RedirectToAction("Index");
            }
            var site = await GetCurrentSiteAsync();
            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            var badge = await _badgeService.GetByIdAsync(trigger.AwardBadgeId);
            Attachment attachment = null;
            if (trigger.AwardAttachmentId != null)
            {
                attachment = await _attachmentService.GetByIdAsync(trigger.AwardAttachmentId.Value);
            }

            var viewModel = new TriggersDetailViewModel
            {
                Trigger = trigger,
                CreatedByName = await _userService.GetUsersNameByIdAsync(trigger.CreatedBy),
                CanViewParticipants = UserHasPermission(Permission.ViewParticipantDetails),
                Action = "Edit",
                IsSecretCode = !string.IsNullOrWhiteSpace(trigger.SecretCode),
                BadgeMakerUrl = GetBadgeMakerUrl(siteUrl, site.FromEmailAddress),
                UseBadgeMaker = true,
                EditAvatarBundle = UserHasPermission(Permission.ManageAvatars),
                EditAttachment = UserHasPermission(Permission.TriggerAttachments),
                EditMail = UserHasPermission(Permission.ManageTriggerMail),
                EditVendorCode = UserHasPermission(Permission.ManageVendorCodes),
                AwardsMail = !string.IsNullOrWhiteSpace(trigger.AwardMailSubject),
                AwardsPrize = !string.IsNullOrWhiteSpace(trigger.AwardPrizeName),
                AwardsAttachment = !string.IsNullOrWhiteSpace(trigger.AwardAttachmentFilename),
                DependentTriggers = await _triggerService.GetDependentsAsync(trigger.AwardBadgeId),
                TriggerRequirements = await _triggerService.GetTriggerRequirementsAsync(trigger),
                BadgeRequiredList = string.Join(",", trigger.BadgeIds),
                ChallengeRequiredList = string.Join(",", trigger.ChallengeIds),
                SystemList = new SelectList(await _siteService.GetSystemList(), "Id", "Name"),
                ProgramList = new SelectList(await _siteService.GetProgramList(), "Id", "Name"),
                IgnorePointLimits = UserHasPermission(Permission.IgnorePointLimits),
                MaxPointLimit =
                    await _triggerService.GetMaximumAllowedPointsAsync(GetCurrentSiteId()),
                BadgeAltText = badge.AltText
            };
            if (viewModel?.MaxPointLimit != null)
            {
                viewModel.MaxPointsMessage = $"(Up to {viewModel.MaxPointLimit.Value} points)";
            }
            if (trigger.AwardPoints > viewModel.MaxPointLimit)
            {
                viewModel.MaxPointsWarningMessage = $"This Trigger exceeds the maximum of {viewModel.MaxPointLimit.Value} points per required task. Only Administrators can edit the points awarded.";
            }
            if (viewModel.EditVendorCode)
            {
                viewModel.VendorCodeTypeList = new SelectList(
                    await _vendorCodeService.GetTypeAllAsync(), "Id", "Description");
            }
            else if (viewModel.Trigger.AwardVendorCodeTypeId.HasValue)
            {
                viewModel.VendorCodeType = (await _vendorCodeService
                    .GetTypeById(viewModel.Trigger.AwardVendorCodeTypeId.Value)).Description;
            }

            if (viewModel.EditAvatarBundle)
            {
                viewModel.UnlockableAvatarBundleList = new SelectList(
                    await _avatarService.GetAllBundlesAsync(true), "Id", "Name");
            }
            else if (viewModel.Trigger.AwardAvatarBundleId.HasValue)
            {
                viewModel.UnlockableAvatarBundle = (await _avatarService
                    .GetBundleByIdAsync(viewModel.Trigger.AwardAvatarBundleId.Value)).Name;
            }

            if (viewModel.Trigger.LimitToSystemId.HasValue)
            {
                viewModel.BranchList = new SelectList(
                    await _siteService.GetBranches(viewModel.Trigger.LimitToSystemId.Value), "Id", "Name");
            }
            else
            {
                viewModel.BranchList = new SelectList(await _siteService.GetAllBranches(), "Id", "Name");
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
            if (attachment != null)
            {
                viewModel.Trigger.AwardAttachmentFilename = _pathResolver.ResolveContentPath(attachment.FileName);
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
            byte[] badgeBytes = null;
            byte[] attachmentBytes = null;

            var badgeRequiredList = new List<int>();
            var challengeRequiredList = new List<int>();

            model.IgnorePointLimits = UserHasPermission(Permission.IgnorePointLimits);
            model.MaxPointLimit =
                await _triggerService.GetMaximumAllowedPointsAsync(GetCurrentSiteId());
            if (!model.IgnorePointLimits
                && model.MaxPointLimit.HasValue
                && model.Trigger.AwardPoints > model.MaxPointLimit)
            {
                ModelState.AddModelError("Trigger.AwardPoints",
                    $"You may award up to {model.MaxPointLimit} points.");
            }
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
                    && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker))
            {
                if (!ValidImageExtensions.Contains(
                    Path.GetExtension(model.BadgeUploadImage.FileName).ToLowerInvariant()))
                {
                    ModelState.AddModelError("BadgeUploadImage", $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                }
                if (model.BadgeUploadImage != null)
                {
                    try
                    {
                        using (var ms = new MemoryStream())
                        {
                            await model.BadgeUploadImage.CopyToAsync(ms);
                            badgeBytes = ms.ToArray();
                        }
                        await _badgeService.ValidateBadgeImageAsync(badgeBytes);
                    }
                    catch (GraException gex)
                    {
                        ModelState.AddModelError("BadgeUploadImage", gex.Message);
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(model.BadgeAltText))
            {
                ModelState.AddModelError("BadgeAltText", "The badge's alternative text is required.");
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
            if (model.AwardsPrize && string.IsNullOrWhiteSpace(model.Trigger.AwardPrizeName))
            {
                ModelState.AddModelError("Trigger.AwardPrizeName", "The Prize Name field is required.");
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
            if (model.AwardsAttachment && model.AttachmentUploadFile == null && !model.Trigger.AwardAttachmentId.HasValue)
            {
                ModelState.AddModelError("AttachmentUploadFile", "An attachment is required.");
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
                        model.Trigger.SecretCode = model
                            .Trigger
                            .SecretCode
                            .Trim()
                            .ToLowerInvariant();
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
                    if (!model.AwardsAttachment && model.EditAttachment)
                    {
                        model.Trigger.AwardAttachmentFilename = "";
                        model.Trigger.AwardAttachmentId = null;
                    }
                    var existing = await _badgeService
                        .GetByIdAsync(model.Trigger.AwardBadgeId);
                    string fileName;
                    if (model.BadgeUploadImage != null
                        || !string.IsNullOrWhiteSpace(model.BadgeMakerImage))
                    {
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage)
                            && (model.BadgeUploadImage == null || model.UseBadgeMaker))
                        {
                            var badgeString = model.BadgeMakerImage.Split(',').Last();
                            badgeBytes = Convert.FromBase64String(badgeString);
                            fileName = "badge.png";
                        }
                        else
                        {
                            if (badgeBytes == null)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    await model.BadgeUploadImage.CopyToAsync(ms);
                                    badgeBytes = ms.ToArray();
                                }
                            }
                            fileName = Path.GetFileName(model.BadgeUploadImage.FileName);
                        }
                        if (model.Trigger.AwardBadgeId == null)
                        {
                            var newBadge = new Badge
                            {
                                Filename = fileName,
                                AltText = model.BadgeAltText
                            };
                            var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                            model.Trigger.AwardBadgeId = badge.Id;
                        }
                        else
                        {
                            if (!string.Equals(existing.AltText, model.BadgeAltText,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                existing.AltText = model.BadgeAltText;
                                await _badgeService.ReplaceBadgeFileAsync(existing, null, null);
                            }
                            existing.Filename = Path.GetFileName(model.BadgePath);
                            existing.AltText = model.BadgeAltText;
                            await _badgeService.ReplaceBadgeFileAsync(existing,
                                badgeBytes,
                                fileName);
                        }
                    }
                    else if (!string.Equals(existing.AltText, model.BadgeAltText,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        existing.AltText = model.BadgeAltText;
                        await _badgeService.ReplaceBadgeFileAsync(existing, null, null);
                    }

                    if (model.AttachmentUploadFile != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await model.AttachmentUploadFile.CopyToAsync(ms);
                            attachmentBytes = ms.ToArray();
                        }

                        if (model.Trigger.AwardAttachmentId != null)
                        {
                            var existingAttachment = await _attachmentService
                            .GetByIdAsync(model.Trigger.AwardAttachmentId.Value);

                            await _attachmentService.ReplaceAttachmentFileAsync(existingAttachment, attachmentBytes);
                        }
                        else
                        {
                            var attachment = new Attachment
                            {
                                FileName = Path.GetFileName(model.AttachmentUploadFile.FileName)
                            };

                            attachment = await _attachmentService.AddAttachmentAsync(attachment, attachmentBytes);
                            model.Trigger.AwardAttachmentId = attachment.Id;
                        }
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
            model.SystemList = new SelectList(await _siteService.GetSystemList(), "Id", "Name");
            if (model.Trigger.LimitToSystemId.HasValue)
            {
                model.BranchList = new SelectList(
                    await _siteService.GetBranches(model.Trigger.LimitToSystemId.Value), "Id", "Name");
            }
            else
            {
                model.BranchList = new SelectList(await _siteService.GetAllBranches(), "Id", "Name");
            }
            model.ProgramList = new SelectList(await _siteService.GetProgramList(), "Id", "Name");
            model.TriggerRequirements = await _triggerService
                .GetRequirementsByIdsAsync(badgeRequiredList, challengeRequiredList);
            foreach (var requirement in model.TriggerRequirements)
            {
                requirement.BadgePath = _pathResolver.ResolveContentPath(requirement.BadgePath);
            }

            if (model.EditVendorCode)
            {
                model.VendorCodeTypeList = new SelectList(
                    await _vendorCodeService.GetTypeAllAsync(), "Id", "Description");
            }
            else if (model.Trigger.AwardVendorCodeTypeId.HasValue)
            {
                model.VendorCodeType = (await _vendorCodeService
                    .GetTypeById(model.Trigger.AwardVendorCodeTypeId.Value)).Description;
            }

            if (model.EditAvatarBundle)
            {
                model.UnlockableAvatarBundleList = new SelectList(
                    await _avatarService.GetAllBundlesAsync(true), "Id", "Name");
            }
            else if (model.Trigger.AwardAvatarBundleId.HasValue)
            {
                model.UnlockableAvatarBundle = (await _avatarService
                    .GetBundleByIdAsync(model.Trigger.AwardAvatarBundleId.Value)).Name;
            }

            if (model.MaxPointLimit.HasValue)
            {
                model.MaxPointsMessage = $"(Up to {model.MaxPointLimit.Value} points)";

                var currentTrigger = await _triggerService.GetByIdAsync(model.Trigger.Id);
                if (currentTrigger.AwardPoints > model.MaxPointLimit.Value)
                {
                    model.MaxPointsWarningMessage = $"This Trigger exceeds the maximum of {model.MaxPointLimit.Value} points per required task. Only Administrators can edit the points awarded.";
                }
            }

            PageTitle = $"Edit Trigger - {model.Trigger.Name}";
            return View("Detail", model);
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
            if (paginateModel.PastMaxPage)
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
                var graEvent = (await _eventService.GetRelatedEventsForTriggerAsync(trigger.Id))
                    .FirstOrDefault();
                if (graEvent != null)
                {
                    trigger.RelatedEventId = graEvent.Id;
                    trigger.RelatedEventName = graEvent.Name;
                }
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
                    .SingleOrDefault(_ => _.Id == branch.SystemId)?.Name;
                viewModel.BranchList = (await _siteService.GetBranches(branch.SystemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Branch";
            }
            else if (systemId.HasValue)
            {
                viewModel.SystemName = systemList
                    .SingleOrDefault(_ => _.Id == systemId.Value)?.Name;
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
    }
}
