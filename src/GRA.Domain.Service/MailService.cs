using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class MailService : Abstract.BaseService<MailService>
    {
        private IMailRepository _mailRepository;
        public MailService(ILogger<MailService> logger,
            IMailRepository mailRepository) : base(logger)
        {
            _mailRepository = Require.IsNotNull(mailRepository, nameof(mailRepository));
        }

        public async Task<int> GetUserUnreadCountAsync(ClaimsPrincipal user)
        {
            var userId = GetId(user, ClaimType.UserId);
            return await _mailRepository.GetUserUnreadCountAsync(userId);
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetUserPaginatedAsync(
            ClaimsPrincipal user,
            int skip,
            int take)
        {
            var userId = GetId(user, ClaimType.UserId);
            var dataTask = _mailRepository.PageUserAsync(userId, skip, take);
            var countTask = _mailRepository.GetUserCountAsync(userId);
            await Task.WhenAll(dataTask, countTask);
            return new DataWithCount<IEnumerable<Mail>>
            {
                Data = dataTask.Result,
                Count = countTask.Result
            };
        }

        public async Task<Mail> GetDetails(ClaimsPrincipal user, int mailId)
        {
            var userId = GetId(user, ClaimType.UserId);
            bool canReadAll = UserHasPermission(user, Permission.ReadAllMail);
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if(mail.FromUserId == userId || mail.ToUserId == userId || canReadAll)
            {
                return mail;
            }
            logger.LogError($"User {userId} doesn't have permission to view details for message {mailId}.");
            throw new Exception("Permission denied.");
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetAllPaginatedAsync(
            ClaimsPrincipal user,
            int skip,
            int take)
        {
            if (UserHasPermission(user, Permission.ReadAllMail))
            {
                var dataTask = _mailRepository.PageAllAsync(skip, take);
                var countTask = _mailRepository.GetAllCountAsync();
                await Task.WhenAll(dataTask, countTask);
                return new DataWithCount<IEnumerable<Mail>>
                {
                    Data = dataTask.Result,
                    Count = countTask.Result
                };
            }
            else
            {
                var userId = GetId(user, ClaimType.UserId);
                logger.LogError($"User {userId} doesn't have permission to get all mails.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetAllUnreadPaginatedAsync(
            ClaimsPrincipal user,
            int skip,
            int take)
        {
            if (UserHasPermission(user, Permission.ReadAllMail))
            {
                var dataTask = _mailRepository.PageAdminUnreadAsync(skip, take);
                var countTask = _mailRepository.GetAdminUnreadCountAsync();
                await Task.WhenAll(dataTask, countTask);
                return new DataWithCount<IEnumerable<Mail>>
                {
                    Data = dataTask.Result,
                    Count = countTask.Result
                };
            }
            else
            {
                var userId = GetId(user, ClaimType.UserId);
                logger.LogError($"User {userId} doesn't have permission to get all unread mails.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task MarkAsReadAsync(ClaimsPrincipal user, int mailId)
        {
            var userId = GetId(user, ClaimType.UserId);
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail.FromUserId == userId || mail.ToUserId == userId)
            {
                await _mailRepository.MarkAsReadAsync(mailId);
                return;
            }
            logger.LogError($"User {userId} doesn't have permission mark mail {mailId} as read.");
            throw new Exception("Permission denied.");
        }

        public async Task<Mail> SendAsync(ClaimsPrincipal user, Mail mail)
        {
            if(mail.ToUserId == default(int)
               || UserHasPermission(user, Permission.MailParticipants))
            {
                mail.FromUserId = GetId(user, ClaimType.UserId);
                mail.IsNew = true;
                mail.IsDeleted = false;
                mail.SiteId = GetId(user, ClaimType.SiteId);
                return await _mailRepository.AddSaveAsync(mail.FromUserId, mail);
            }
            else
            {
                var userId = GetId(user, ClaimType.UserId);
                logger.LogError($"User {userId} doesn't have permission to send a mail to {mail.ToUserId}.");
                throw new Exception("Permission denied");
            }
        }

        public async Task RemoveAsync(ClaimsPrincipal user, int mailId)
        {
            var userId = GetId(user, ClaimType.UserId);
            bool canDeleteAll = UserHasPermission(user, Permission.DeleteAnyMail);
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail.FromUserId == userId || mail.ToUserId == userId || canDeleteAll)
            {
                await _mailRepository.RemoveSaveAsync(userId, mailId);
                return;
            }
            logger.LogError($"User {userId} doesn't have permission remove mail {mailId}.");
            throw new Exception("Permission denied.");
        }
    }
}
