using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.EmailManagement;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageBulkEmails)]
    public class EmailManagementController : Base.MCController
    {
        private readonly ILogger _logger;
        private readonly EmailManagementService _emailManagementService;
        private readonly EmailService _emailService;
        private readonly UserService _userService;

        public EmailManagementController(ServiceFacade.Controller context,
            ILogger<EmailManagementController> logger,
            EmailManagementService emailManagementService,
            EmailService emailService,
            UserService userService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailManagementService = emailManagementService
                ?? throw new ArgumentNullException(nameof(emailManagementService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            PageTitle = "Email Management";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var filter = new BaseFilter();
            var templateList
                = await _emailManagementService.GetPaginatedEmailTemplateListAsync(filter);
            var paginateModel = new PaginateViewModel
            {
                ItemCount = templateList.Count,
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

            var currentUser = await _userService.GetDetails(GetActiveUserId());

            return View(new EmailIndexViewModel
            {
                PaginateModel = paginateModel,
                EmailTemplates = templateList.Data,
                SubscribedParticipants = await _emailManagementService.GetSubscriberCount(),
                DefaultTestEmail = currentUser?.Email
            });
        }

        public async Task<IActionResult> Create()
        {
            PageTitle = "Create Email";
            var site = await GetCurrentSiteAsync();
            var viewModel = new EmailDetailViewModel
            {
                Action = nameof(Create),
                EmailTemplate = new EmailTemplate(),
            };
            viewModel.EmailTemplate.FromAddress = site.FromEmailAddress;
            viewModel.EmailTemplate.FromName = site.FromEmailName;
            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmailDetailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var newTemplate
                    = await _emailManagementService.CreateEmailTemplate(viewModel.EmailTemplate);
                return RedirectToAction(nameof(EmailManagementController.Edit),
                    new { id = newTemplate.Id });
            }
            PageTitle = "Create Email";
            return View("Detail", viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            PageTitle = "Edit Email";
            var viewModel = new EmailDetailViewModel
            {
                Action = nameof(Edit),
                EmailTemplate = await _emailService.GetEmailTemplate(id)
            };
            if (viewModel.EmailTemplate == null)
            {
                ShowAlertDanger($"Could not find email template {id}");
                RedirectToAction(nameof(EmailManagementController.Index));
            }
            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmailDetailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _emailManagementService.EditEmailTemplate(viewModel.EmailTemplate);
                return RedirectToAction(nameof(EmailManagementController.Edit), viewModel.EmailTemplate.Id);
            }
            ShowAlertDanger("Could not update email template");
            PageTitle = "Edit Email";
            return View("Detail", viewModel);
        }

        [HttpPost]
        public IActionResult SendEmailTest(EmailIndexViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.SendTestRecipients))
            {
                ShowAlertDanger("You must supply one or more email addresses in order to send a test.");
                return RedirectToAction(nameof(EmailManagementController.Index));
            }
            else
            {
                _logger.LogInformation("Email test requested by {UserId} for email {EmailId} to {Addresses}",
                     GetActiveUserId(),
                     viewModel.SendTestTemplateId,
                     viewModel.SendTestRecipients);

                ShowAlertDanger("Sending test emails has not been configured yet.");
                return RedirectToAction(nameof(EmailManagementController.Index));
            }
        }

        [HttpPost]
        public IActionResult SendEmail(EmailIndexViewModel viewModel)
        {
            if (!string.Equals(viewModel.SendValidation,
                    "YES",
                    StringComparison.InvariantCultureIgnoreCase))
            {
                ShowAlertDanger("Emails not sent: you must enter YES in the confirmation field.");
                return RedirectToAction(nameof(EmailManagementController.Index));
            }
            else
            {
                _logger.LogInformation("Email send requested by {UserId} for email {EmailId} with {SendValidation}",
                    GetActiveUserId(),
                    viewModel.SendEmailTemplateId,
                    viewModel.SendValidation);

                ShowAlertDanger("Sending emails has not been configured yet.");
                return RedirectToAction(nameof(EmailManagementController.Index));
            }
        }
    }
}
