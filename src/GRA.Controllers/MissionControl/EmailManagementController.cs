using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.EmailManagement;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;
using GRA.Domain.Service;
using GRA.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageBulkEmails)]
    public class EmailManagementController : Base.MCController
    {
        private const string SubscribedParticipants = "SubscribedParticipants";
        private readonly EmailManagementService _emailManagementService;
        private readonly EmailReminderService _emailReminderService;
        private readonly EmailService _emailService;
        private readonly JobService _jobService;
        private readonly ILogger _logger;
        private readonly UserService _userService;

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

        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> Addresses()
        {
            return await ShowAddressView(null);
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

        [HttpGet]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "The File() method handles the stream disposal for us.")]
        public async Task<IActionResult> ExportAddresses(EmailAddressesViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel?.SignUpSource))
            {
                ModelState.AddModelError(nameof(viewModel.SignUpSource),
                    "Please select a list to export.");
                return await ShowAddressView(viewModel);
            }

            var subscribers = await _emailReminderService
                .ExportSubscribersAsync(viewModel.SignUpSource);

            if (!subscribers.Any())
            {
                ShowAlertWarning("Unable to prepare export: no subscribers to that list.");
                return await ShowAddressView(viewModel);
            }

            var user = await _userService.GetDetails(GetActiveUserId());
            var site = await GetCurrentSiteAsync();
            var link = await _siteLookupService.GetSiteLinkAsync(site.Id);

            var filename = $"EmailList-{viewModel.SignUpSource}.json";

            var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, new EmailListExport
            {
                Addresses = subscribers,
                ExportedAt = _dateTimeProvider.Now,
                ExportedBy = $"{user.FullName} ({user.Email ?? user.Username})",
                Source = $"{site.Name} ({link})",
                Version = 2
            });
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms,
                "text/json",
                FileUtility.EnsureValidFilename(filename));
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

            var issues = new List<string>();
            int recordNumber = 1;
            var success = 0;
            string successMessage;

            var stream = viewModel.UploadedFile.OpenReadStream();

            var emailReminders = new List<EmailReminder>();
            EmailListImport emailListImport = null;

            try
            {
                emailListImport = await JsonSerializer
                    .DeserializeAsync<EmailListImport>(stream);
            }
            catch (JsonException)
            {
            }

            if (emailListImport != null)
            {
                // Version 2+
                emailReminders.AddRange(emailListImport.Addresses);
                successMessage = $"Import from <strong>{emailListImport.Source}</strong> complete.";
            }
            else
            {
                // Version 1
                stream.Seek(0, SeekOrigin.Begin);
                try
                {
                    emailReminders.AddRange(await JsonSerializer
                        .DeserializeAsync<IEnumerable<EmailReminder>>(stream));
                }
                catch (JsonException jex)
                {
                    ShowAlertWarning($"Unable to import file: {jex.Message}");
                    _logger.LogError(jex,
                        "Unable to import mailing list file: {ErrorMessage}",
                        jex.Message);
                    return RedirectToAction(nameof(Addresses));
                }
                successMessage = "Import complete.";
            }

            if (emailReminders.Any())
            {
                foreach (var emailReminder in emailReminders)
                {
                    if (string.IsNullOrEmpty(emailReminder.Email))
                    {
                        issues.Add("Record {recordNumber} is missing an email address.");
                    }

                    emailReminder.SignUpSource = viewModel.SignUpSource;

                    success += await _emailReminderService
                        .ImportEmailToListAsync(GetActiveUserId(), emailReminder) ? 1 : 0;

                    recordNumber++;

                    if (success % 40 == 0)
                    {
                        await _emailReminderService.SaveImportAsync();
                    }
                }
                await _emailReminderService.SaveImportAsync();
            }

            var response = new StringBuilder(successMessage).Append(' ');
            if (success > 0)
            {
                response.Append("Successfully imported <strong>")
                    .Append(success)
                    .Append(" addresses</strong> to list ")
                    .Append(viewModel.SignUpSource)
                    .Append('.');
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
                    SerializedParameters = JsonSerializer
                        .Serialize(new JobDetailsSendBulkEmails
                        {
                            EmailTemplateId = viewModel.SendEmailTemplateId,
                            MailingList = viewModel.EmailList == SubscribedParticipants
                                ? null
                                : viewModel.EmailList,
                            SendToParticipantsToo = viewModel.SendToParticipantsToo
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
                    SerializedParameters = JsonSerializer
                        .Serialize(new JobDetailsSendBulkEmails
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
    }
}
