using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.Mail;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    [Authorize]
    public class MailController : Base.UserController
    {
        private readonly ILogger<MailController> _logger;
        private readonly MailService _mailService;

        public static string Name { get { return "Mail"; } }

        public MailController(ILogger<MailController> logger,
            ServiceFacade.Controller context,
            MailService mailService) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            PageTitle = "Mail";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int take = 15;
            int skip = take * (page - 1);
            var mailList = await _mailService.GetUserInboxPaginatedAsync(skip, take);

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
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Read(int id)
        {
            try
            {
                var mail = await _mailService.GetParticipantMailAsync(id);
                mail.Body = CommonMark.CommonMarkConverter.Convert(mail.Body);
                if (mail.IsNew)
                {
                    await _mailService.MarkAsReadAsync(id);
                    HttpContext.Items[ItemKey.UnreadCount] =
                        (int)HttpContext.Items[ItemKey.UnreadCount] - 1;
                }
                return View(mail);
            }
            catch (GraException gex)
            {
                ShowAlertWarning(_sharedLocalizer[ErrorMessages.MailUnableToRead, gex.Message]);
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Reply(int id)
        {
            try
            {
                var mail = await _mailService.GetParticipantMailAsync(id);
                var viewModel = new MailCreateViewModel
                {
                    Subject
                        = _sharedLocalizer[Annotations.Interface.MailReplyPrefix, mail.Subject],
                    InReplyToId = mail.Id,
                    InReplyToSubject = mail.Subject,
                };
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning(_sharedLocalizer[ErrorMessages.MailUnableToReply, gex.Message]);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reply(MailCreateViewModel model)
        {
            if (model.InReplyToId == null)
            {
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                var mail = new Mail
                {
                    Subject = model.Subject,
                    Body = model.Body,
                    InReplyToId = model.InReplyToId
                };
                await _mailService.SendReplyAsync(mail);
                AlertSuccess = _sharedLocalizer[Annotations.Interface.MailSent, mail.Subject];
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(MailCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mail = new Mail
                {
                    Subject = model.Subject,
                    Body = model.Body
                };
                await _mailService.SendAsync(mail);
                AlertSuccess = _sharedLocalizer[Annotations.Interface.MailSent, mail.Subject];
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _mailService.RemoveAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Problem with user {GetActiveUserId} deleting mail id {id}: {Message}",
                    GetActiveUserId(),
                    id,
                    ex.Message);
                AlertWarning = _sharedLocalizer[ErrorMessages.MailUnableToDelete];
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
