using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.EmailManagement;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageBulkEmails)]
    public class EmailManagementController : Base.MCController
    {
        private readonly EmailManagementService _emailManagementService;
        private readonly EmailService _emailService;

        public EmailManagementController(ServiceFacade.Controller context,
            EmailManagementService emailManagementService,
            EmailService emailService)
            : base(context)
        {
            _emailManagementService = emailManagementService ?? throw new ArgumentNullException(nameof(emailManagementService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            PageTitle = "Email Management";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var filter = new BaseFilter();
            var templateList = await _emailManagementService.GetPaginatedEmailTemplateListAsync(filter);
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
            var viewModel = new EmailIndexViewModel
            {
                PaginateModel = paginateModel,
                EmailTemplates = templateList.Data
            };
            return View(viewModel);
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
                var newTemplate = await _emailManagementService.CreateEmailTemplate(viewModel.EmailTemplate);
                return RedirectToAction(nameof(EmailManagementController.Edit), new { id = newTemplate.Id });
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
            if(viewModel.EmailTemplate == null)
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
                return RedirectToAction(nameof(EmailManagementController.Edit),viewModel.EmailTemplate.Id);
            }
            ShowAlertDanger("Could not update email template");
            PageTitle = "Edit Email";
            return View("Detail", viewModel);
        }

        [HttpPost]
        public IActionResult SendEmailTest()
        {
            ShowAlertDanger("Sending test emails has not been configured yet.");
            return RedirectToAction(nameof(EmailManagementController.Index));
        }

        [HttpPost]
        public IActionResult SendEmail()
        {
            ShowAlertDanger("Sending emails has not been configured yet.");
            return RedirectToAction(nameof(EmailManagementController.Index));
        }
    }
}
