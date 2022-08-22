using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Mail;
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
    [Authorize(Policy = Policy.ReadAllMail)]
    public class MailController : Base.MCController
    {
        private readonly ILogger<MailController> _logger;
        private readonly MailService _mailService;
        private readonly UserService _userService;
        public MailController(ILogger<MailController> logger,
            ServiceFacade.Controller context,
            MailService mailService,
            UserService userService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            PageTitle = "Mail";
        }

        private void AddNoticeIfDisabled()
        {
            if (!HttpContext.Items.ContainsKey(ItemKey.ShowMail)
                || HttpContext.Items[ItemKey.ShowMail] as bool? != true)
            {
                ShowAlertDanger("Mail is disabled in site settings, participants will not see it.");
            }
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            AddNoticeIfDisabled();

            const int take = 15;
            int skip = take * (page - 1);
            var mailList = await _mailService.GetAllUnrepliedPaginatedAsync(skip, take);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = mailList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };

            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new MailListViewModel
            {
                Mail = mailList.Data,
                PaginateModel = paginateModel,
                CanDelete = UserHasPermission(Permission.DeleteAnyMail)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ViewAll(int page = 1)
        {
            AddNoticeIfDisabled();

            const int take = 15;
            int skip = take * (page - 1);
            var mailList = await _mailService.GetAllPaginatedAsync(skip, take);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = mailList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };

            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new MailListViewModel
            {
                Mail = mailList.Data,
                PaginateModel = paginateModel,
                CanDelete = UserHasPermission(Permission.DeleteAnyMail)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            AddNoticeIfDisabled();

            try
            {
                var mail = await _mailService.GetDetails(id);
                if (mail.ToUserId == null)
                {
                    if (mail.IsNew)
                    {
                        await _mailService.MCMarkAsReadAsync(id);
                    }
                }
                else
                {
                    mail.Body = CommonMark.CommonMarkConverter.Convert(mail.Body);
                }

                var thread = await _mailService.GetThreadAsync(mail.ThreadId ?? mail.Id);

                var viewModel = new MailDetailViewModel
                {
                    Mail = mail,
                    MailThread = thread,
                    SentMessage = (mail.ToUserId == null ? "from" : "to"),
                    CanDelete = UserHasPermission(Permission.DeleteAnyMail),
                    CanMail = UserHasPermission(Permission.MailParticipants)
                };

                int from = mail.ToUserId ?? mail.FromUserId;
                if (from > 0)
                {
                    viewModel.User = await _userService.GetDetailsByPermission(from);
                }
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view mail: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy.MailParticipants)]
        [HttpPost]
        public async Task<IActionResult> MarkHandled(int id)
        {
            try
            {
                await _mailService.MarkHandled(id);
                AlertSuccess = "Marked as handled";
                return RedirectToAction("Index");
            }
            catch (GraException gex)
            {
                AlertInfo = gex.Message;
                return RedirectToAction("Detail", new { id });
            }
        }

        [Authorize(Policy.MailParticipants)]
        public async Task<IActionResult> Reply(int id)
        {
            try
            {
                var mail = await _mailService.GetDetails(id);
                if (mail.ToUserId == null)
                {
                    var participant = await _userService.GetDetailsByPermission(mail.FromUserId);
                    string participantLink =
                        Url.Action("Detail", "Participants", new { id = participant.Id });
                    string participantName = participant.FullName;
                    if (!string.IsNullOrEmpty(participant.Username))
                    {
                        participantName += $" ({participant.Username})";
                    }

                    var viewModel = new MailReplyViewModel
                    {
                        Subject = $"Re: {mail.Subject}",
                        InReplyToId = mail.Id,
                        InReplyToSubject = mail.Subject,
                        InReplyToBody = mail.Body,
                        ParticipantLink = participantLink,
                        ParticipantName = participantName
                    };

                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Detail", new { id });
                }
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view mail: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy.MailParticipants)]
        [HttpPost]
        public async Task<IActionResult> Reply(MailReplyViewModel model)
        {
            if (model == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (model.InReplyToId == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                var mail = new Mail
                {
                    Subject = model.Subject,
                    Body = model.Body,
                    InReplyToId = model.InReplyToId
                };
                await _mailService.MCSendReplyAsync(mail);
                AlertSuccess = $"Reply \"<strong>{mail.Subject}</strong>\" sent";
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [Authorize(Policy.DeleteAnyMail)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _mailService.RemoveAsync(id);
            return RedirectToAction("Index");
        }

        [Authorize(Policy.SendBroadcastMail)]
        public async Task<IActionResult> Broadcasts(bool upcoming = true, int page = 1)
        {
            AddNoticeIfDisabled();

            var filter = new BroadcastFilter(page)
            {
                Upcoming = upcoming
            };

            var broadcastList = await _mailService.PageBroadcastsAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = broadcastList.Count,
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

            var viewModel = new BroadcastListViewModel
            {
                Broadcasts = broadcastList.Data,
                PaginateModel = paginateModel,
                Upcoming = upcoming
            };

            return View(viewModel);
        }

        [Authorize(Policy.SendBroadcastMail)]
        public IActionResult BroadcastCreate()
        {
            AddNoticeIfDisabled();

            var viewModel = new BroadcastDetailViewModel
            {
                Action = "Create"
            };

            PageTitle = "Create Broadcast";
            return View("BroadcastDetail", viewModel);
        }

        [Authorize(Policy.SendBroadcastMail)]
        [HttpPost]
        public async Task<IActionResult> BroadcastCreate(BroadcastDetailViewModel model)
        {
            if (model == null)
            {
                return RedirectToAction(nameof(BroadcastCreate));
            }

            if (!model.SendNow)
            {
                if (model.Broadcast.SendAt == null)
                {
                    ModelState.AddModelError("Broadcast.SendAt", "Please select a date to send the Broadcast.");
                }
                else if (model.Broadcast.SendAt < _dateTimeProvider.Now)
                {
                    ModelState.AddModelError("Broadcast.SendAt", "Please select a date not in the past.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.SendNow)
                    {
                        model.Broadcast.SendAt = _dateTimeProvider.Now;
                    }

                    await _mailService.AddBroadcastAsync(model.Broadcast);
                    ShowAlertSuccess($"Broadcast {model.Broadcast.Subject} successfully scheduled.");

                    return RedirectToAction("Broadcasts",
                        new { Upcoming = (model.SendNow ? "False" : null) });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to create broadcast: ", gex);
                }
            }
            return View("BroadcastDetail", model);
        }

        [Authorize(Policy.SendBroadcastMail)]
        public async Task<IActionResult> BroadcastEdit(int id)
        {
            var viewModel = new BroadcastDetailViewModel
            {
                Broadcast = await _mailService.GetBroadcastByIdAsync(id),
                Action = "Edit"
            };

            if (viewModel.Broadcast.SendAt <= _dateTimeProvider.Now)
            {
                viewModel.Sent = true;
                PageTitle = "View Broadcast";
            }
            else
            {
                PageTitle = "Edit Broadcast";
            }

            return View("BroadcastDetail", viewModel);
        }

        [Authorize(Policy.SendBroadcastMail)]
        [HttpPost]
        public async Task<IActionResult> BroadcastEdit(BroadcastDetailViewModel model)
        {
            if (model == null)
            {
                return RedirectToAction(nameof(Broadcasts));
            }

            if (!model.SendNow)
            {
                if (model.Broadcast.SendAt == null)
                {
                    ModelState.AddModelError("Broadcast.SendAt", "Please select a date to send the Broadcast.");
                }
                else if (model.Broadcast.SendAt < _dateTimeProvider.Now)
                {
                    ModelState.AddModelError("Broadcast.SendAt", "Please select a date not in the past.");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.SendNow)
                    {
                        model.Broadcast.SendAt = _dateTimeProvider.Now;
                    }

                    await _mailService.EditBroadcastAsync(model.Broadcast);
                    ShowAlertSuccess($"Broadcast {model.Broadcast.Subject} successfully edited.");

                    return RedirectToAction("Broadcasts",
                        new { Upcoming = (model.SendNow ? "False" : null) });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to edit broadcast: ", gex);
                }
            }
            return View("BroadcastDetail", model);
        }

        [Authorize(Policy.SendBroadcastMail)]
        [HttpPost]
        public async Task<IActionResult> BroadcastDelete(int id)
        {
            try
            {
                await _mailService.RemoveBroadcastAsync(id);
                ShowAlertSuccess("Broadcast successfully deleted!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete broadcast: ", gex.Message);
            }
            return RedirectToAction("Broadcasts");
        }
    }
}
