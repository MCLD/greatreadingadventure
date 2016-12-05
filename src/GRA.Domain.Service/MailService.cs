using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class MailService : Abstract.BaseUserService<MailService>
    {
        private IMailRepository _mailRepository;
        public MailService(ILogger<MailService> logger,
            IUserContextProvider userContextProvider,
            IMailRepository mailRepository) : base(logger, userContextProvider)
        {
            _mailRepository = Require.IsNotNull(mailRepository, nameof(mailRepository));
        }

        public async Task<int> GetUserUnreadCountAsync()
        {
            var userId = GetClaimId(ClaimType.UserId);
            return await _mailRepository.GetUserUnreadCountAsync(userId);
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetUserPaginatedAsync(int skip,
            int take)
        {
            var userId = GetClaimId(ClaimType.UserId);
            var dataTask = _mailRepository.PageUserAsync(userId, skip, take);
            var countTask = _mailRepository.GetUserCountAsync(userId);
            await Task.WhenAll(dataTask, countTask);
            return new DataWithCount<IEnumerable<Mail>>
            {
                Data = dataTask.Result,
                Count = countTask.Result
            };
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetUserPaginatedAsync(
            int getMailForUserId,
            int skip,
            int take)
        {
            if (HasPermission(Permission.ReadAllMail))
            {
                var dataTask = _mailRepository.PageUserAsync(getMailForUserId, skip, take);
                var countTask = _mailRepository.GetUserCountAsync(getMailForUserId);
                await Task.WhenAll(dataTask, countTask);
                return new DataWithCount<IEnumerable<Mail>>
                {
                    Data = dataTask.Result,
                    Count = countTask.Result
                };
            }
            else
            {
                var requestingUser = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {requestingUser} doesn't have permission to view messages for {getMailForUserId}.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<Mail> GetDetails(int mailId)
        {
            var userId = GetClaimId(ClaimType.UserId);
            bool canReadAll = HasPermission(Permission.ReadAllMail);
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail.FromUserId == userId || mail.ToUserId == userId || canReadAll)
            {
                return mail;
            }
            _logger.LogError($"User {userId} doesn't have permission to view details for message {mailId}.");
            throw new Exception("Permission denied.");
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetAllPaginatedAsync(int skip,
            int take)
        {
            int siteId = GetClaimId(ClaimType.SiteId);
            if (HasPermission(Permission.ReadAllMail))
            {
                var dataTask = _mailRepository.PageAllAsync(siteId, skip, take);
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
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to get all mails.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetAllUnreadPaginatedAsync(int skip,
            int take)
        {
            if (HasPermission(Permission.ReadAllMail))
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
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to get all unread mails.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task MarkAsReadAsync(int mailId)
        {
            var userId = GetClaimId(ClaimType.UserId);
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail.FromUserId == userId || mail.ToUserId == userId)
            {
                await _mailRepository.MarkAsReadAsync(mailId);
                return;
            }
            _logger.LogError($"User {userId} doesn't have permission mark mail {mailId} as read.");
            throw new Exception("Permission denied.");
        }

        public async Task<Mail> SendAsync(Mail mail)
        {
            if (mail.ToUserId == null
               || HasPermission(Permission.MailParticipants))
            {
                mail.FromUserId = GetClaimId(ClaimType.UserId);
                mail.IsNew = true;
                mail.IsDeleted = false;
                mail.SiteId = GetClaimId(ClaimType.SiteId);
                return await _mailRepository.AddSaveAsync(mail.FromUserId, mail);
            }
            else
            {
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to send a mail to {mail.ToUserId}.");
                throw new Exception("Permission denied");
            }
        }

        public async Task<Mail> SendReplyAsync(Mail mail, int inReplyToId)
        {
            if (mail.ToUserId == null
               || HasPermission(Permission.MailParticipants))
            {
                var inReplyToMail = await _mailRepository.GetByIdAsync(inReplyToId);
                mail.InReplyToId = inReplyToId;
                mail.ThreadId = inReplyToMail.ThreadId ?? inReplyToId;
                mail.FromUserId = GetClaimId(ClaimType.UserId);
                mail.IsNew = true;
                mail.IsDeleted = false;
                mail.SiteId = GetClaimId(ClaimType.SiteId);
                return await _mailRepository.AddSaveAsync(mail.FromUserId, mail);
            }
            else
            {
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to send a mail to {mail.ToUserId}.");
                throw new Exception("Permission denied");
            }
        }


        public async Task RemoveAsync(int mailId)
        {
            var userId = GetClaimId(ClaimType.UserId);
            bool canDeleteAll = HasPermission(Permission.DeleteAnyMail);
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail.FromUserId == userId || mail.ToUserId == userId || canDeleteAll)
            {
                await _mailRepository.RemoveSaveAsync(userId, mailId);
                return;
            }
            _logger.LogError($"User {userId} doesn't have permission remove mail {mailId}.");
            throw new Exception("Permission denied.");
        }
    }
}
