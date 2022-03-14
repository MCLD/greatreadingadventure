﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.EmailManagement;
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
        private const string CannotUpdateAlreadySent
            = "This template cannot be edited - bulk emails were already sent to participants utilizing it.";

        private const string SubscribedParticipants = "SubscribedParticipants";
        private readonly EmailManagementService _emailManagementService;
        private readonly EmailReminderService _emailReminderService;
        private readonly EmailService _emailService;
        private readonly JobService _jobService;
        private readonly LanguageService _languageService;
        private readonly ILogger _logger;
        private readonly UserService _userService;

        public EmailManagementController(ServiceFacade.Controller context,
            ILogger<EmailManagementController> logger,
            EmailManagementService emailManagementService,
            EmailService emailService,
            EmailReminderService emailReminderService,
            JobService jobService,
            LanguageService languageService,
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
            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            PageTitle = "Email Management";
        }

        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> Addresses()
        {
            return await ShowAddressView(null);
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> CreateTemplate()
        {
            var defaultLanguageId = await _languageService.GetDefaultLanguageIdAsync();
            var languageList = await _languageService.GetActiveAsync();
            var emailBases = await _emailManagementService.GetEmailBasesAsync();

            var languageSelect = new SelectList(languageList,
                    nameof(Language.Id),
                    nameof(Language.Description),
                    defaultLanguageId);

            foreach (var languageOption in languageSelect)
            {
                if (languageOption.Value != defaultLanguageId.ToString(CultureInfo.InvariantCulture))
                {
                    languageOption.Disabled = true;
                }
            }
            return View("Details", new DetailsViewModel
            {
                Action = nameof(CreateTemplate),
                EmailBases = new SelectList(emailBases,
                    nameof(EmailBase.Id),
                    nameof(EmailBase.Name)),
                Footer = "You are receiving this email becuase you are subscribed to news updates for {{Sitename}}. You can [unsubscribe instantly at any time]({{UnsubscribeLink}})",
                Languages = languageSelect,
                LanguageId = defaultLanguageId,
                Title = "{{Sitename}}"
            });
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> CreateTemplate(DetailsViewModel detailsViewModel)
        {
            if (detailsViewModel == null)
            {
                ShowAlertWarning("Unable to create template.");
                return RedirectToAction(nameof(Index));
            }

            string insertProblem = null;
            if (ModelState.IsValid)
            {
                var emailTemplate = new DirectEmailTemplate
                {
                    Description = detailsViewModel.TemplateDescription?.Trim(),
                    EmailBaseId = detailsViewModel.EmailBaseId,
                    DirectEmailTemplateText = new DirectEmailTemplateText
                    {
                        BodyCommonMark = detailsViewModel.BodyCommonMark?.Trim(),
                        Footer = detailsViewModel.Footer?.Trim(),
                        LanguageId = detailsViewModel.LanguageId,
                        Preview = detailsViewModel.Preview?.Trim(),
                        Subject = detailsViewModel.Subject?.Trim(),
                        Title = detailsViewModel.Title?.Trim()
                    }
                };

                try
                {
                    var insertedId = await _emailManagementService.AddTemplateAsync(emailTemplate);

                    ShowAlertSuccess($"Successfully created template: {detailsViewModel.TemplateDescription}");

                    return RedirectToAction(nameof(EditTemplate), new
                    {
                        templateId = insertedId,
                        languageId = detailsViewModel.LanguageId
                    });
                }
                catch (GraException gex)
                {
                    insertProblem = gex.Message;
                }
            }

            var issues = new StringBuilder("There were issues with your submission:<ul>");
            if (!string.IsNullOrEmpty(insertProblem))
            {
                issues.Append("<li>").Append(insertProblem).AppendLine("</li>");
            }
            foreach (var key in ModelState.Keys)
            {
                issues.Append("<li>").Append(ModelState[key]).AppendLine("</li>");
            }
            issues.Append("</ul>");
            ShowAlertWarning(issues.ToString());
            return View("Details", detailsViewModel);
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> EditTemplate(int templateId, int languageId)
        {
            var languageList = await _languageService.GetActiveAsync();
            var emailBases = await _emailManagementService.GetEmailBasesAsync();

            if (languageList.SingleOrDefault(_ => _.Id == languageId) == null)
            {
                ShowAlertWarning($"Could not find language id {languageId}");
                return RedirectToAction(nameof(Index));
            }

            var template = await _emailManagementService.GetTemplateAsync(templateId, languageId);

            if (template == null)
            {
                ShowAlertWarning($"Could not find template id {templateId}");
                return RedirectToAction(nameof(Index));
            }

            var currentUser = await _userService.GetDetails(GetActiveUserId());

            if (template.IsDisabled)
            {
                ShowAlertInfo(CannotUpdateAlreadySent);
            }

            return View("Details", new DetailsViewModel
            {
                Action = nameof(EditTemplate),
                BodyCommonMark = template.DirectEmailTemplateText?.BodyCommonMark,
                DefaultTestEmail = currentUser?.Email,
                EmailBaseId = template.EmailBaseId,
                EmailBases = new SelectList(emailBases,
                    nameof(EmailBase.Id),
                    nameof(EmailBase.Name)),
                EmailTemplateId = template.Id,
                Footer = template.DirectEmailTemplateText?.Footer,
                IsDisabled = template.IsDisabled,
                Languages = new SelectList(languageList,
                    nameof(Language.Id),
                    nameof(Language.Description),
                    languageId),
                LanguageId = languageId,
                Preview = template.DirectEmailTemplateText?.Preview,
                Subject = template.DirectEmailTemplateText?.Subject,
                TemplateDescription = template.Description,
                Title = template.DirectEmailTemplateText?.Title
            });
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> EditTemplate(DetailsViewModel detailsViewModel)
        {
            if (detailsViewModel == null)
            {
                ShowAlertWarning("Could not find template to update.");
                return RedirectToAction(nameof(Index));
            }

            var currentTemplate = await _emailManagementService
                .GetTemplateAsync(detailsViewModel.EmailTemplateId, detailsViewModel.LanguageId);

            if (currentTemplate.IsDisabled)
            {
                ShowAlertWarning(CannotUpdateAlreadySent);
                return RedirectToAction(nameof(EditTemplate), new
                {
                    templateId = detailsViewModel.EmailTemplateId,
                    languageId = detailsViewModel.LanguageId
                });
            }

            string updateProblem = null;
            if (ModelState.IsValid)
            {
                var emailTemplate = new DirectEmailTemplate
                {
                    Id = detailsViewModel.EmailTemplateId,
                    Description = detailsViewModel.TemplateDescription?.Trim(),
                    EmailBaseId = detailsViewModel.EmailBaseId,
                    DirectEmailTemplateText = new DirectEmailTemplateText
                    {
                        BodyCommonMark = detailsViewModel.BodyCommonMark?.Trim(),
                        Footer = detailsViewModel.Footer?.Trim(),
                        LanguageId = detailsViewModel.LanguageId,
                        Preview = detailsViewModel.Preview?.Trim(),
                        Subject = detailsViewModel.Subject?.Trim(),
                        Title = detailsViewModel.Title?.Trim()
                    }
                };

                try
                {
                    await _emailManagementService.UpdateTemplateAsync(emailTemplate);

                    ShowAlertSuccess($"Successfully updated template: {detailsViewModel.TemplateDescription}");

                    return RedirectToAction(nameof(EditTemplate), new
                    {
                        templateId = emailTemplate.Id,
                        languageId = detailsViewModel.LanguageId
                    });
                }
                catch (GraException gex)
                {
                    updateProblem = gex.Message;
                }
            }

            var issues = new StringBuilder("There were issues with your submission:<ul>");
            if (!string.IsNullOrEmpty(updateProblem))
            {
                issues.Append("<li>").Append(updateProblem).AppendLine("</li>");
            }
            foreach (var key in ModelState.Keys)
            {
                issues.Append("<li>").Append(ModelState[key]).AppendLine("</li>");
            }
            issues.Append("</ul>");
            ShowAlertWarning(issues.ToString());
            return View("Details", detailsViewModel);
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
                var languages = (await _languageService.GetActiveAsync())
                    .ToDictionary(k => k.Name, v => v.Id);

                foreach (var emailReminder in emailReminders)
                {
                    if (string.IsNullOrEmpty(emailReminder.Email))
                    {
                        issues.Add("Record {recordNumber} is missing an email address.");
                    }

                    emailReminder.SignUpSource = viewModel.SignUpSource;

                    if (!string.IsNullOrEmpty(emailReminder.LanguageName)
                        && languages.ContainsKey(emailReminder.LanguageName))
                    {
                        emailReminder.LanguageId = languages[emailReminder.LanguageName];
                    }

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

            var viewModel = new EmailIndexViewModel
            {
                ItemCount = templateList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };

            if (viewModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = viewModel.LastPage ?? 1
                    });
            }

            var currentUser = await _userService.GetDetails(GetActiveUserId());
            var addressTypes = new List<SelectListItem>();
            var defaultLanguage = await _languageService.GetDefaultLanguageIdAsync();

            var isAnyoneSubscribed = await _emailManagementService.IsAnyoneSubscribedAsync();

            var languageList = await _languageService.GetActiveAsync();

            viewModel.EmailTemplates = templateList.Data;
            viewModel.IsAdmin = currentUser?.IsAdmin == true;
            viewModel.IsAnyoneSubscribed = isAnyoneSubscribed;
            viewModel.LanguageNames = languageList.ToDictionary(k => k.Id, v => v.Description);

            if (!isAnyoneSubscribed)
            {
                ShowAlertWarning("There are no subscribed participants or interested parties to send an email to.");
            }

            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> SelectList(int templateId, string listName)
        {
            var defaultLanguageId = await _languageService.GetDefaultLanguageIdAsync();
            var languageList = await _languageService.GetActiveAsync();
            var languages = languageList.ToDictionary(k => k.Id, v => v.Name);

            var viewModel = new SendEmailViewModel
            {
                SelectedList = listName,
                Template = await _emailManagementService.GetTemplateStatusAsync(templateId)
            };

            viewModel.IsForSubscribers
                = viewModel.Template.LanguageUnsub.Any(_ => _.Value == false);

            if (!viewModel.IsForSubscribers)
            {
                return RedirectToAction(nameof(Send), new { templateId });
            }

            // check update subscribers for languages
            // key = list, value = dictionary language id, subscribers
            var listsAndLanguages = await _emailManagementService
                .GetEmailListsAsync(defaultLanguageId);

            viewModel.RegisteredLanguages =
                listsAndLanguages[listName]
                .ToDictionary(k => languageList.SingleOrDefault(_ => _.Id == k.Key).Description,
                    v => v.Value);

            var templateLanguageIds = viewModel.Template.LanguageUnsub.Select(_ => _.Key);

            viewModel.TemplateLanguages = templateLanguageIds
                .Select(_ => languageList.SingleOrDefault(__ => __.Id == _).Description);

            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> Send(int templateId)
        {
            var defaultLanguageId = await _languageService.GetDefaultLanguageIdAsync();
            var languageList = await _languageService.GetActiveAsync();
            var languages = languageList.ToDictionary(k => k.Id, v => v.Name);

            var viewModel = new SendEmailViewModel
            {
                Template = await _emailManagementService.GetTemplateStatusAsync(templateId)
            };

            // if any footer doesn't have the unsubscribe link then it's not for participants
            viewModel.IsForSubscribers
                = viewModel.Template.LanguageUnsub.Any(_ => _.Value == false);

            // if some footers have unsub link and some don't then we're in conflict
            viewModel.IsMixedFooter = viewModel.IsForSubscribers
                && viewModel.Template.LanguageUnsub.Any(_ => _.Value == true);

            if (!viewModel.IsMixedFooter)
            {
                if (viewModel.IsForSubscribers)
                {
                    // check update subscribers for languages
                    // key = list, value = dictionary language id, subscribers
                    var listsAndLanguages = await _emailManagementService
                        .GetEmailListsAsync(defaultLanguageId);

                    var languageSelect = listsAndLanguages
                        .Select(_ => new SelectListItem
                        {
                            Text = $"{_.Key} ({_.Value.Sum(_ => _.Value)} subscribers)",
                            Value = _.Key
                        });

                    viewModel.SubscriptionLists = new SelectList(languageSelect,
                       nameof(SelectListItem.Value),
                       nameof(SelectListItem.Text));
                }
                else
                {
                    // check participants for languages
                    // key = language id, value = count
                    var participantLanguageIds = await _userService
                        .GetSubscribedLanguageCountAsync();

                    viewModel.RegisteredLanguages = participantLanguageIds
                        .ToDictionary(k => languageList.SingleOrDefault(_ => _.Id == k.Key)
                            .Description,
                            v => v.Value);

                    var templateLanguageIds = viewModel.Template.LanguageUnsub.Select(_ => _.Key);

                    viewModel.TemplateLanguages = templateLanguageIds
                        .Select(_ => languageList.SingleOrDefault(__ => __.Id == _).Description);
                }
            }
            else
            {
                ShowAlertDanger("Some languages for this template have an {{UnsubscribeLink}} in the footer and some do not. You can only send an email to subscribed addresses or participants, not both.");
            }

            // check participant/subscriber lists for languages
            // check email for translations, notify
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = Policy.SendBulkEmails)]
        public async Task<IActionResult> SendEmailTest(DetailsViewModel viewModel)
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
                    viewModel.EmailTemplateId,
                    viewModel.SendTestRecipients);

                var jobToken = await _jobService.CreateJobAsync(new Job
                {
                    JobType = JobType.SendBulkEmails,
                    SerializedParameters = JsonSerializer
                        .Serialize(new JobDetailsSendBulkEmails
                        {
                            EmailTemplateId = viewModel.EmailTemplateId,
                            TestLanguageId = viewModel.LanguageId,
                            To = viewModel.SendTestRecipients,
                            UnsubscribeBase = UnsubBase()
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
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> SendParticipants(int templateId)
        {
            _logger.LogInformation("Email send to participants by {UserId} for templated id {TemplateId}",
                GetActiveUserId(),
                templateId);

            var jobToken = await _jobService.CreateJobAsync(new Job
            {
                JobType = JobType.SendBulkEmails,
                SerializedParameters = JsonSerializer
                    .Serialize(new JobDetailsSendBulkEmails
                    {
                        EmailTemplateId = templateId,
                        UnsubscribeBase = UnsubBase()
                    })
            });

            return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
            {
                CancelUrl = Url.Action(nameof(Index)),
                JobToken = jobToken.ToString(),
                PingSeconds = 5,
                SuccessRedirectUrl = "",
                SuccessUrl = Url.Action(nameof(Index)),
                Title = "Sending email to participants..."
            });
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageBulkEmails)]
        public async Task<IActionResult> SendSubscribers(int templateId, string listName)
        {
            _logger.LogInformation("Email to subscribers by {UserId} for template id {TemplateId} to list {ListName}",
                GetActiveUserId(),
                templateId,
                listName);

            var jobToken = await _jobService.CreateJobAsync(new Job
            {
                JobType = JobType.SendBulkEmails,
                SerializedParameters = JsonSerializer
                    .Serialize(new JobDetailsSendBulkEmails
                    {
                        EmailTemplateId = templateId,
                        MailingList = listName,
                    })
            });

            return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
            {
                CancelUrl = Url.Action(nameof(Index)),
                JobToken = jobToken.ToString(),
                PingSeconds = 5,
                SuccessRedirectUrl = "",
                SuccessUrl = Url.Action(nameof(Index)),
                Title = "Sending email to subscribers..."
            });
        }

        private async Task<IActionResult> ShowAddressView(EmailAddressesViewModel viewModel)
        {
            var emailAddressesViewModel = viewModel ?? new EmailAddressesViewModel();

            if (!emailAddressesViewModel.HasSources)
            {
                var defaultLanguageId = await _languageService.GetDefaultLanguageIdAsync();
                var allEmailReminders = await _emailManagementService
                    .GetEmailListsAsync(defaultLanguageId);

                var selectListMailingLists = allEmailReminders.Select(_ =>
                    new SelectListItem
                    {
                        Text = $"{_.Key} ({_.Value.Values.Sum()} subscribers across {_.Value.Values.Count} language{(_.Value.Values.Count == 1 ? null : 's')})",
                        Value = _.Key
                    });

                emailAddressesViewModel.SignUpSources = new SelectList(selectListMailingLists,
                    nameof(SelectListItem.Value),
                    nameof(SelectListItem.Text));
                emailAddressesViewModel.HasSources = allEmailReminders.Count > 0;
            }
            return View(emailAddressesViewModel);
        }

        private string UnsubBase()
        {
            return Url.Action(nameof(Controllers.HomeController.Unsubscribe),
                Controllers.HomeController.Name,
                null,
                HttpContext.Request.Scheme);
        }
    }
}
