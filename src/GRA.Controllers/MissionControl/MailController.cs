using GRA.Controllers.ViewModel.MissionControl.Mail;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;

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
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Mail";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);
            var mailList = await _mailService.GetAllUnrepliedPaginatedAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = mailList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            MailListViewModel viewModel = new MailListViewModel()
            {
                Mail = mailList.Data,
                PaginateModel = paginateModel,
                CanDelete = UserHasPermission(Permission.DeleteAnyMail)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ViewAll(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);
            var mailList = await _mailService.GetAllPaginatedAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = mailList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            MailListViewModel viewModel = new MailListViewModel()
            {
                Mail = mailList.Data,
                PaginateModel = paginateModel,
                CanDelete = UserHasPermission(Permission.DeleteAnyMail)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var mail = await _mailService.GetDetails(id);
                if (mail.ToUserId == null && mail.IsNew)
                {
                    await _mailService.MCMarkAsReadAsync(id);
                }

                string participantLink = string.Empty;
                string participantName = string.Empty;
                int from = mail.ToUserId ?? mail.FromUserId;
                if (from > 0)
                {
                    var participant = await _userService.GetDetails(from);

                    participantLink = Url.Action("Detail", "Participants", new { id = participant.Id });
                    participantName = participant.FirstName;

                    if (!string.IsNullOrWhiteSpace(participant.Username))
                    {
                        participantName += $" ({participant.Username})";
                    }
                }

                MailDetailViewModel viewModel = new MailDetailViewModel()
                {
                    Mail = mail,
                    SentMessage = (mail.ToUserId == null ? "from" : "to"),
                    ParticipantLink = participantLink,
                    ParticipantName = participantName,
                    CanDelete = UserHasPermission(Permission.DeleteAnyMail),
                    CanMail = UserHasPermission(Permission.MailParticipants)
                };
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
                return RedirectToAction("Detail", new { id = id });
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
                    var participant = await _userService.GetDetails(mail.FromUserId);
                    string participantLink =
                        Url.Action("Detail", "Participants", new { id = participant.Id });
                    string participantName = participant.FullName;
                    if (!string.IsNullOrEmpty(participant.Username))
                    {
                        participantName += $" ({participant.Username})";
                    }

                    MailReplyViewModel viewModel = new MailReplyViewModel()
                    {
                        Subject = $"Re: {mail.Subject}",
                        InReplyToId = mail.Id,
                        InReplyToSubject = mail.Subject,
                        ParticipantLink = participantLink,
                        ParticipantName = participantName
                    };

                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Detail", new { id = id });
                }
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view mail: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy.DeleteAnyMail)]
        [HttpPost]
        public async Task<IActionResult> Reply(MailReplyViewModel model)
        {
            if (model.InReplyToId == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                Mail mail = new Mail()
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

        [Authorize(Policy.MailParticipants)]
        [HttpGet]
        public IActionResult SendBroadcast()
        {
            return View();
        }

        [Authorize(Policy.MailParticipants)]
        [HttpPost]
        public async Task<IActionResult> SendBroadcast(SendBroadcastViewModel viewModel)
        {
            var mail = new Mail
            {
                Body = viewModel.Body,
                Subject = viewModel.Subject
            };

            int mailsSent = await _mailService.MCSendBroadcastAsync(mail);

            ShowAlertSuccess($"{mailsSent} broadcast messages sent.", "envelope");
            return RedirectToAction("Index");
        }
    }
}
