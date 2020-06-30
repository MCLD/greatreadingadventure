using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.EmailManagement;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using GRA.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageBulkEmails)]
    public class EmailManagementController : Base.MCController
    {
        private readonly ILogger _logger;
        private readonly EmailManagementService _emailManagementService;
        private readonly EmailService _emailService;
        private readonly EmailReminderService _emailReminderService;
        private readonly JobService _jobService;
        private readonly UserService _userService;

        private const string SubscribedParticipants = "SubscribedParticipants";

        public EmailManagementController(ServiceFacade.Controller context,
            ILogger<EmailManagementController> logger,
            EmailManagementService emailManagementService,
            EmailService emailService,
            EmailReminderService emailReminderService,
            JobService jobService,
            UserService userService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailManagementService = emailManagementService
                ?? throw new ArgumentNullException(nameof(emailManagementService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _emailReminderService = emailReminderService
                ?? throw new ArgumentNullException(nameof(emailReminderService));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
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
            int subscribedParticipants = await _emailManagementService.GetSubscriberCount();
            var addressTypes = new List<SelectListItem>();
            var emailLists = await _emailManagementService.GetEmailListsAsync();
            foreach (var emailList in emailLists.Where(_ => _.Count > 0))
            {
                var signupsourcedata = new SelectListItem
                {
                    Text = $"{emailList.Data} ({emailList.Count} subscribed)",
                    Value = emailList.Data
                };
                addressTypes.Add(signupsourcedata);
            }
            if (subscribedParticipants > 0)
            {
                addressTypes.Add(new SelectListItem
                {
                    Text = $"Subscribed participants ({subscribedParticipants} subscribed)",
                    Value = SubscribedParticipants
                });
            }
            var addressSelectList = new SelectList(addressTypes, "Value", "Text");

            var viewModel = new EmailIndexViewModel
            {
                PaginateModel = paginateModel,
                EmailTemplates = templateList.Data,
                SubscribedParticipants = subscribedParticipants,
                DefaultTestEmail = currentUser?.Email,
                IsAdmin = currentUser?.IsAdmin == true,
                AddressTypes = addressSelectList
            };

            if (!string.IsNullOrEmpty(viewModel.SendButtonDisabled))
            {
                ShowAlertWarning("There are no subscribed participants or interested parties to send an email to.");
            }

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
                return RedirectToAction(nameof(EmailManagementController.Edit),
                    viewModel.EmailTemplate.Id);
            }
            ShowAlertDanger("Could not update email template");
            PageTitle = "Edit Email";
            return View("Detail", viewModel);
        }

        [HttpPost]
        [Authorize(Policy = Policy.SendBulkEmails)]
        public async Task<IActionResult> SendEmailTest(EmailIndexViewModel viewModel)
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

                var jobToken = await _jobService.CreateJobAsync(new Job
                {
                    JobType = JobType.SendBulkEmails,
                    SerializedParameters = JsonConvert
                        .SerializeObject(new JobDetailsSendBulkEmails
                        {
                            EmailTemplateId = viewModel.SendTestTemplateId,
                            To = viewModel.SendTestRecipients
                        })
                });

                return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
                {
                    CancelUrl = Url.Action(nameof(Index)),
                    JobToken = jobToken.ToString(),
                    PingSeconds = 5,
                    SuccessRedirectUrl = "",
                    SuccessUrl = Url.Action(nameof(Index)),
                    Title = "Sending test emails..."
                });
            }
        }

        [HttpPost]
        [Authorize(Policy = Policy.SendBulkEmails)]
        public async Task<IActionResult> SendEmail(EmailIndexViewModel viewModel)
        {
            if (!string.Equals(viewModel.SendValidation,
                    "YES",
                    StringComparison.Ordinal))
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

                var jobToken = await _jobService.CreateJobAsync(new Job
                {
                    JobType = JobType.SendBulkEmails,
                    SerializedParameters = JsonConvert
                        .SerializeObject(new JobDetailsSendBulkEmails
                        {
                            EmailTemplateId = viewModel.SendEmailTemplateId,
                            MailingList = viewModel.EmailList == SubscribedParticipants
                                ? null
                                : viewModel.EmailList
                        })
                });

                return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
                {
                    CancelUrl = Url.Action(nameof(Index)),
                    JobToken = jobToken.ToString(),
                    PingSeconds = 5,
                    SuccessRedirectUrl = "",
                    SuccessUrl = Url.Action(nameof(Index)),
                    Title = "Sending emails..."
                });
            }
        }

        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> Addresses()
        {
            return await ShowAddressView(null);
        }

        private async Task<IActionResult> ShowAddressView(EmailAddressesViewModel viewModel)
        {
            var emailAddressesViewModel = viewModel ?? new EmailAddressesViewModel();

            if (!emailAddressesViewModel.HasSources)
            {
                var allEmailReminders = await _emailManagementService.GetEmailListsAsync();

                var selectListMailingLists = allEmailReminders.Select(_ =>
                    new SelectListItem
                    {
                        Text = $"{_.Data} ({_.Count})",
                        Value = _.Data
                    });
                emailAddressesViewModel.SignUpSources = new SelectList(selectListMailingLists,
                    nameof(SelectListItem.Value),
                    nameof(SelectListItem.Text));
                emailAddressesViewModel.HasSources = allEmailReminders.Count > 0;
            }
            return View(emailAddressesViewModel);
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> ExportAddresses(EmailAddressesViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.SignUpSource))
            {
                ModelState.AddModelError(nameof(viewModel.SignUpSource),
                    "Please select a list to export.");
                return await ShowAddressView(viewModel);
            }
            var json = JsonConvert.SerializeObject(await _emailReminderService
                .GetAllSubscribersAsync(viewModel.SignUpSource));

            var filename = $"EmailList-{viewModel.SignUpSource}.json";

            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms))
                {
                    await writer.WriteAsync(json);
                    await writer.FlushAsync();
                    writer.Close();
                    return File(ms.ToArray(),
                        "text/json",
                        FileUtility.EnsureValidFilename(filename));
                }
            }
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> ImportAddresses(EmailAddressesViewModel viewModel)
        {
            if (viewModel?.UploadedFile == null)
            {
                ShowAlertDanger("You must upload a JSON file of email records.");
                ModelState.AddModelError(nameof(viewModel.UploadedFile),
                    "A .json file is required.");
                return RedirectToAction(nameof(EmailManagementController.Addresses));
            }

            using (var reader = new StreamReader(viewModel.UploadedFile.OpenReadStream()))
            {
                var issues = new List<string>();
                int recordNumber = 1;
                var success = 0;
                try
                {
                    var jsonString = await reader.ReadToEndAsync();
                    foreach (var emailReminder in JsonConvert
                        .DeserializeObject<ICollection<EmailReminder>>(jsonString))
                    {
                        try
                        {
                            if (await _emailReminderService
                                .ImportEmailToListAsync(GetActiveUserId(), emailReminder))
                            {
                                success++;
                            }
                        }
#pragma warning disable CA1031 // Do not catch general exception types
                        catch (Exception ex)
                        {
                            _logger.LogError("Issue in {Filename} on record {RecordNumber}: {ErrorMessage}",
                                viewModel.UploadedFile.FileName,
                                recordNumber,
                                ex.Message);
                            issues.Add($"Issue in item {recordNumber}: {ex.Message}");
                        }
#pragma warning restore CA1031 // Do not catch general exception types

                        if (recordNumber % 50 == 0)
                        {
                            await _emailReminderService.SaveImportAsync();
                        }

                        recordNumber++;
                    }
                    await _emailReminderService.SaveImportAsync();
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
                {
                    _logger.LogError("Import failed for {Filename} on record {RecordNumber}: {ErrorMessage}",
                        viewModel.UploadedFile.FileName,
                        recordNumber,
                        ex.Message);
                    ShowAlertDanger($"Failed to import addresses: {ex.Message}");
                    return RedirectToAction(nameof(EmailManagementController.Addresses));
                }
#pragma warning restore CA1031 // Do not catch general exception types

                var response = new StringBuilder();
                if (success > 0)
                {
                    response.Append("Successfully imported <strong>")
                        .Append(success)
                        .Append(" addresses</strong>.");
                }
                else
                {
                    response.Append("All addresses were already present on the list.");
                }

                if (issues.Count == 0)
                {
                    ShowAlertSuccess(response.ToString());
                }
                else
                {
                    response.Append(" The following issues occurred with the import:<ul>");
                    foreach (var issue in issues)
                    {
                        response.Append("<li>").Append(issue).Append("</li>");
                    }
                    response.Append("</ul>");
                    ShowAlertWarning(response.ToString());
                }

                return RedirectToAction(nameof(EmailManagementController.Addresses));
            }
        }
    }
}
