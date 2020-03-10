using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.EmailManagement;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
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
            _emailReminderService = emailReminderService ?? throw new ArgumentNullException(nameof(emailReminderService));
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
            var addressTypes = new List<object>();
            var signupData = (await _emailManagementService.GetAllEmailRemindersAsync())
                .GroupBy(_ => _.SignUpSource)
                .Select(_ => new
                {
                    DisplayText = _.Key + " (" + _.Distinct().Count().ToString() + ")",
                    Value = _.Key
                });
            foreach (var type in signupData)
            {
                addressTypes.Add(type);
            }
            addressTypes.Add(new {
                DisplayText = "Subscribed Participants (" + subscribedParticipants.ToString() + ")",
                Value = "Subscribed"
            });
            var addressSelectList = new SelectList(addressTypes, "Value", "DisplayText");

            return View(new EmailIndexViewModel
            {
                PaginateModel = paginateModel,
                EmailTemplates = templateList.Data,
                SubscribedParticipants =subscribedParticipants,
                DefaultTestEmail = currentUser?.Email,
                IsAdmin = currentUser.IsAdmin,
                AddressTypes = addressSelectList
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
                            EmailTemplateId = viewModel.SendEmailTemplateId
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
            var user = await _userService.GetDetails(GetActiveUserId());
            var signupData = (await _emailManagementService.GetAllEmailRemindersAsync())
                .GroupBy(_ => _.SignUpSource)
                .Select(_ => new
                {
                    DisplayText = _.Key + " (" + _.Distinct().Count().ToString() + ")",
                    Value = _.Key
                });
            var viewModel = new EmailAddressesViewModel
            {
                SignUpSources = new SelectList(signupData, "Value", "DisplayText")
            };
            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> ExportAddresses(EmailAddressesViewModel viewModel)
        {
            var user = await _userService.GetDetails(GetActiveUserId());
            if (user.IsAdmin)
            {
                var signupData = await _emailManagementService.GetAllEmailRemindersAsync();
                if (string.IsNullOrEmpty(viewModel.SignUpSource))
                {
                    ShowAlertDanger("Could not export Email Reminders");
                    ModelState.AddModelError(nameof(viewModel.SignUpSource), "The signup source is required.");
                    viewModel.SignUpSources = new SelectList(signupData, "Value", "DisplayText");
                    return View("Addresses", viewModel);
                }
                var emailReminders = await _emailManagementService.GetEmailRemindersBySignUpSourceAsync(viewModel.SignUpSource);
                var json = JsonConvert.SerializeObject(emailReminders);
                using(var ms = new MemoryStream()) {
                    using (var writer = new StreamWriter(ms))
                    {
                        writer.WriteLine(json);
                        writer.Flush();
                        writer.Close();
                        return File(ms.ToArray(), "text/plain", $"Email_Reminders_{viewModel.SignUpSource}.txt");
                    }
                }
            }
            ShowAlertDanger("Page not found.");
            return RedirectToAction(nameof(EmailManagementController.Index));
        }
        
        [HttpPost]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> ImportAddresses(EmailAddressesViewModel viewModel)
        {
            var user = await _userService.GetDetails(GetActiveUserId());
            if (user.IsAdmin)
            {
                if (viewModel.UploadedFile == null)
                {
                    ShowAlertDanger("Could not import Email Reminders");
                    ModelState.AddModelError(nameof(viewModel.UploadedFile), "A .txt file is required.");
                    return RedirectToAction(nameof(EmailManagementController.Addresses));
                }
                using (var reader = new StreamReader(viewModel.UploadedFile.OpenReadStream()))
                {
                    var success = 0;
                    try
                    {
                        var jsonString = await reader.ReadToEndAsync();
                        var addressList = JsonConvert.DeserializeObject<ICollection<EmailReminder>>(jsonString);
                        foreach (var emailReminder in addressList)
                        {
                            var imported = await _emailReminderService.AddImportEmailReminderAsync(emailReminder);
                            if (imported)
                            {
                                success++;
                            }
                        }
                    }
                    catch
                    {
                        ShowAlertDanger($"Failed to import email reminders: Malformed Json data.");
                        return RedirectToAction(nameof(EmailManagementController.Addresses));
                    }
                    if (success >0)
                    {
                        ShowAlertSuccess($"Successfully imported ({success}) email reminders");
                    }
                    else
                    {
                        ShowAlertDanger($"Failed to import email reminders: All objects already exist");
                    }
                    return RedirectToAction(nameof(EmailManagementController.Addresses));
                }
            }
            ShowAlertDanger("Page not found.");
            return RedirectToAction(nameof(EmailManagementController.Index));
        }
    }
}
