using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model.Filters;
using System.Linq;

namespace GRA.Domain.Service
{
    public class MailService : Abstract.BaseUserService<MailService>
    {
        private IBroadcastRepository _broadcastRepository;
        private IMailRepository _mailRepository;
        private IUserRepository _userRepository;
        private IMemoryCache _memoryCache;
        public MailService(ILogger<MailService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IBroadcastRepository broadcastRepository,
            IMailRepository mailRepository,
            IUserRepository userRepository,
            IMemoryCache memoryCache) : base(logger, dateTimeProvider, userContextProvider)
        {
            _broadcastRepository = Require.IsNotNull(broadcastRepository,
                nameof(broadcastRepository));
            _mailRepository = Require.IsNotNull(mailRepository, nameof(mailRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _memoryCache = Require.IsNotNull(memoryCache, nameof(memoryCache));
        }

        public async Task<int> GetUserUnreadCountAsync()
        {
            var activeUserId = GetActiveUserId();
            var cacheKey = $"{CacheKey.UserUnreadMailCount}?u{activeUserId}";
            int unreadCount;
            if (!_memoryCache.TryGetValue(cacheKey, out unreadCount))
            {
                await SendUserBroadcastsAsync(activeUserId, true);

                unreadCount = await _mailRepository.GetUserUnreadCountAsync(activeUserId);
                _memoryCache.Set(cacheKey, unreadCount, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return unreadCount;
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetUserInboxPaginatedAsync(int skip,
            int take)
        {
            var activeUserId = GetActiveUserId();
            return new DataWithCount<IEnumerable<Mail>>
            {
                Data = await _mailRepository.PageUserInboxAsync(activeUserId, skip, take),
                Count = await _mailRepository.GetUserInboxCountAsync(activeUserId)
            };
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetUserPaginatedAsync(
            int getMailForUserId,
            int skip,
            int take)
        {
            if (HasPermission(Permission.ReadAllMail))
            {
                return new DataWithCount<IEnumerable<Mail>>
                {
                    Data = await _mailRepository.PageUserAsync(getMailForUserId, skip, take),
                    Count = await _mailRepository.GetUserCountAsync(getMailForUserId)
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
            var activeUserId = GetActiveUserId();
            bool canReadAll = HasPermission(Permission.ReadAllMail);
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail == null)
            {
                throw new GraException("The requested mail could not be accessed or does not exist.");
            }
            if (mail.FromUserId == activeUserId || mail.ToUserId == activeUserId || canReadAll)
            {
                return mail;
            }
            _logger.LogError($"User {activeUserId} doesn't have permission to view details for message {mailId}.");
            throw new GraException("Permission denied.");
        }

        public async Task<List<Mail>> GetThreadAsync(int threadId)
        {
            VerifyPermission(Permission.ReadAllMail);
            return await _mailRepository.GetThreadAsync(threadId);
        }

        public async Task<Mail> GetParticipantMailAsync(int mailId)
        {
            var activeUserId = GetActiveUserId();
            try
            {
                var mail = await _mailRepository.GetByIdAsync(mailId);
                if (mail.ToUserId == activeUserId)
                {
                    return mail;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new GraException("The requested mail could not be accessed or does not exist.");
            }
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetAllPaginatedAsync(int skip,
            int take)
        {
            int siteId = GetClaimId(ClaimType.SiteId);
            if (HasPermission(Permission.ReadAllMail))
            {
                return new DataWithCount<IEnumerable<Mail>>
                {
                    Data = await _mailRepository.PageAllAsync(siteId, skip, take),
                    Count = await _mailRepository.GetAllCountAsync(siteId)
                };
            }
            else
            {
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to get all mails.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<DataWithCount<IEnumerable<Mail>>> GetAllUnrepliedPaginatedAsync(int skip,
            int take)
        {
            if (HasPermission(Permission.ReadAllMail))
            {
                int siteId = GetClaimId(ClaimType.SiteId);
                return new DataWithCount<IEnumerable<Mail>>
                {
                    Data = await _mailRepository.PageAdminUnrepliedAsync(siteId, skip, take),
                    Count = await _mailRepository.GetAdminUnrepliedCountAsync(siteId)
                };
            }
            else
            {
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to get all unread mails.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<int> GetAdminUnreadCountAsync()
        {
            if (HasPermission(Permission.ReadAllMail))
            {
                int siteId = GetClaimId(ClaimType.SiteId);
                var cacheKey = $"{CacheKey.UnhandledMailCount}?s{siteId}";
                int unhandledCount;
                if (!_memoryCache.TryGetValue(cacheKey, out unhandledCount))
                {
                    unhandledCount = await _mailRepository.GetAdminUnrepliedCountAsync(siteId);
                    _memoryCache.Set(cacheKey, unhandledCount, new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
                return unhandledCount;
            }
            else
            {
                var userId = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {userId} doesn't have permission to get unread mail count.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task MarkAsReadAsync(int mailId)
        {
            var activeUserId = GetActiveUserId();
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail.ToUserId == activeUserId)
            {
                await _mailRepository.MarkAsReadAsync(mailId);
                _memoryCache.Remove($"{CacheKey.UserUnreadMailCount}?u{activeUserId}");
                return;
            }
            _logger.LogError($"User {activeUserId} doesn't have permission mark mail {mailId} as read.");
            throw new Exception("Permission denied.");
        }

        public async Task<Mail> SendAsync(Mail mail)
        {
            var siteId = GetClaimId(ClaimType.SiteId);
            var authId = GetClaimId(ClaimType.UserId);
            var activeUserId = GetActiveUserId();
            if (mail.ToUserId == null)
            {
                mail.FromUserId = activeUserId;
                mail.IsNew = true;
                mail.IsDeleted = false;
                mail.SiteId = siteId;

                _memoryCache.Remove($"{CacheKey.UnhandledMailCount}?s{siteId}");

                return await _mailRepository.AddSaveAsync(authId, mail);
            }
            else
            {
                _logger.LogError($"User {activeUserId} doesn't have permission to send a mail to {mail.ToUserId}.");
                throw new Exception("Permission denied");
            }
        }

        public async Task<Mail> SendReplyAsync(Mail mail)
        {
            var siteId = GetClaimId(ClaimType.SiteId);
            var authId = GetClaimId(ClaimType.UserId);
            var activeUserId = GetActiveUserId();
            var inReplyToMail = await _mailRepository.GetByIdAsync(mail.InReplyToId.Value);
            if (inReplyToMail.ToUserId == activeUserId)
            {
                mail.ThreadId = inReplyToMail.ThreadId ?? mail.InReplyToId.Value;
                mail.FromUserId = activeUserId;
                mail.ToUserId = null;
                mail.IsNew = true;
                mail.IsDeleted = false;
                mail.SiteId = siteId;
                if (inReplyToMail.IsRepliedTo == false)
                {
                    inReplyToMail.IsRepliedTo = true;
                    await _mailRepository.UpdateAsync(authId, inReplyToMail);
                }

                _memoryCache.Remove($"{CacheKey.UnhandledMailCount}?s{siteId}");

                return await _mailRepository.AddSaveAsync(authId, mail);
            }
            else
            {
                _logger.LogError($"User {activeUserId} doesn't have permission to reply to a mail sent to {mail.ToUserId}.");
                throw new Exception("Permission Denied");
            }
        }

        public async Task MCMarkAsReadAsync(int mailId)
        {
            if (HasPermission(Permission.ReadAllMail))
            {
                var mail = await _mailRepository.GetByIdAsync(mailId);
                if (mail.ToUserId == null)
                {
                    await _mailRepository.MarkAsReadAsync(mailId);
                    return;
                }
            }
            var authId = GetClaimId(ClaimType.UserId);
            _logger.LogError($"User {authId} doesn't have permission mark mail {mailId} as read.");
            throw new Exception("Permission denied.");
        }

        public async Task<Mail> MCSendAsync(Mail mail)
        {
            var authId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.MailParticipants))
            {
                var user = await _userRepository.GetByIdAsync(mail.ToUserId.Value);
                if (user != null)
                {
                    mail.FromUserId = 0;
                    mail.CanParticipantDelete = true;
                    mail.IsNew = true;
                    mail.IsDeleted = false;
                    mail.SiteId = GetClaimId(ClaimType.SiteId);

                    _memoryCache.Remove($"{CacheKey.UserUnreadMailCount}?u{mail.ToUserId}");

                    return await _mailRepository.AddSaveAsync(authId, mail);
                }
                else
                {
                    throw new GraException("User doesn't exist");
                }
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to send a mail to {mail.ToUserId}.");
                throw new Exception("Permission denied");
            }
        }

        public async Task<Mail> MCSendReplyAsync(Mail mail)
        {
            var authId = GetClaimId(ClaimType.UserId);
            var siteId = GetCurrentSiteId();
            if (mail.InReplyToId != null
               && HasPermission(Permission.MailParticipants))
            {
                var inReplyToMail = await _mailRepository.GetByIdAsync(mail.InReplyToId.Value);
                mail.ThreadId = inReplyToMail?.ThreadId ?? mail.InReplyToId.Value;
                mail.FromUserId = 0;
                mail.ToUserId = inReplyToMail.FromUserId;
                mail.CanParticipantDelete = true;
                mail.IsNew = true;
                mail.IsDeleted = false;
                mail.SiteId = siteId;

                _memoryCache.Remove($"{CacheKey.UserUnreadMailCount}?u{mail.ToUserId}");
                if (inReplyToMail.IsRepliedTo == false)
                {
                    await _mailRepository.MarkAdminReplied(inReplyToMail.Id);
                    _memoryCache.Remove($"{CacheKey.UnhandledMailCount}?s{siteId}");
                }
                return await _mailRepository.AddSaveAsync(authId, mail);
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to reply to mail {mail.InReplyToId}.");
                throw new Exception("Permission denied");
            }
        }

        public async Task MarkHandled(int mailId)
        {
            var authId = GetClaimId(ClaimType.UserId);
            if (HasPermission(Permission.MailParticipants))
            {
                var mail = await _mailRepository.GetByIdAsync(mailId);
                if (mail.ToUserId == null)
                {
                    await _mailRepository.MarkAdminReplied(mailId);
                    var siteId = GetCurrentSiteId();
                    _memoryCache.Remove($"{CacheKey.UnhandledMailCount}?s{siteId}");
                    return;
                }
                else
                {
                    throw new GraException("Cannot mark participant mail as handled");
                }
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to mark mail as handled.");
                throw new Exception("Permission denied");
            }
        }

        public async Task RemoveAsync(int mailId)
        {
            var authId = GetClaimId(ClaimType.UserId);
            var activeId = GetActiveUserId();
            bool canDeleteAll = HasPermission(Permission.DeleteAnyMail);
            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail == null)
            {
                _logger.LogInformation($"User {activeId} tried to remove {mailId} which doesn't exist.");
                return;
            }
            else
            {
                if (mail.ToUserId == activeId || canDeleteAll)
                {
                    if (mail.ToUserId != null)
                    {
                        _memoryCache.Remove($"{CacheKey.UserUnreadMailCount}?u{mail.ToUserId}");
                    }
                    else
                    {
                        var siteId = GetCurrentSiteId();
                        _memoryCache.Remove($"{CacheKey.UnhandledMailCount}?s{siteId}");
                    }
                    await _mailRepository.RemoveSaveAsync(authId, mailId);
                    return;
                }
            }
            _logger.LogError($"User {activeId} doesn't have permission remove mail {mailId}.");
            throw new Exception("Permission denied.");
        }

        public async Task<Mail> SendSystemMailAsync(Mail mail, int? siteId = null)
        {
            var user = await _userRepository.GetByIdAsync(mail.ToUserId.Value);
            if (user != null)
            {
                mail.FromUserId = 0;
                mail.IsNew = true;
                mail.IsDeleted = false;
                mail.CreatedAt = _dateTimeProvider.Now;
                mail.SiteId = siteId ?? GetClaimId(ClaimType.SiteId);

                _memoryCache.Remove($"{CacheKey.UserUnreadMailCount}?u{mail.ToUserId}");

                return await _mailRepository.AddSaveNoAuditAsync(mail);
            }
            else
            {
                throw new GraException("User doesn't exist");
            }
        }

        public async Task<bool> UserHasUnreadAsync(int userId)
        {
            var authId = GetClaimId(ClaimType.UserId);
            if (userId == authId || HasPermission(Permission.ReadAllMail))
            {
                return await _mailRepository.UserHasUnreadAsync(userId);
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to view messages for {userId}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task<DataWithCount<ICollection<Broadcast>>> PageBroadcastsAsync(BroadcastFilter filter)
        {
            var authId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.SendBroadcastMail))
            {
                filter.SiteId = GetClaimId(ClaimType.SiteId);

                return new DataWithCount<ICollection<Broadcast>>
                {
                    Data = await _broadcastRepository.PageAsync(filter),
                    Count = await _broadcastRepository.CountAsync(filter)
                };
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to view broadcasts.");
                throw new Exception("Permission denied");
            }
        }

        public async Task<Broadcast> GetBroadcastByIdAsync(int id)
        {
            var authId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.SendBroadcastMail))
            {
                return await _broadcastRepository.GetByIdAsync(id);
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to view broadcasts.");
                throw new Exception("Permission denied");
            }
        }

        public async Task<Broadcast> AddBroadcastAsync(Broadcast broadcast)
        {
            var authId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.SendBroadcastMail))
            {
                broadcast.SiteId = GetClaimId(ClaimType.SiteId);

                return await _broadcastRepository.AddSaveAsync(authId, broadcast);
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to send broadcasts.");
                throw new Exception("Permission denied");
            }
        }

        public async Task<Broadcast> EditBroadcastAsync(Broadcast broadcast)
        {
            var authId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.SendBroadcastMail))
            {
                var currentBroadcast = await _broadcastRepository.GetByIdAsync(broadcast.Id);

                if (currentBroadcast.SendAt <= _dateTimeProvider.Now)
                {
                    throw new GraException($"This Broadcast has already been sent.");
                }

                broadcast.SiteId = currentBroadcast.SiteId;

                return await _broadcastRepository.UpdateSaveAsync(authId, broadcast);
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to edit broadcasts.");
                throw new Exception("Permission denied");
            }
        }

        public async Task RemoveBroadcastAsync(int id)
        {
            var authId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.SendBroadcastMail))
            {
                var broadcast = await _broadcastRepository.GetByIdAsync(id);

                if (broadcast.SendAt <= _dateTimeProvider.Now)
                {
                    throw new GraException($"This Broadcast has already been sent.");
                }

                await _broadcastRepository.RemoveSaveAsync(authId, id);
            }
            else
            {
                _logger.LogError($"User {authId} doesn't have permission to delete broadcasts.");
                throw new Exception("Permission denied");
            }
        }

        public async Task SendUserBroadcastsAsync(int userId, bool includeHousehold,
            bool newUser = false, bool userIdIsCurrentUser = false)
        {
            var authUserId = userIdIsCurrentUser ? userId : GetClaimId(ClaimType.UserId);

            var user = await _userRepository.GetByIdAsync(userId);
            var newBroadcasts = await _broadcastRepository.GetNewBroadcastsAsync(user.SiteId,
                user.LastBroadcast);
            if (newBroadcasts.Count() > 0)
            {
                var lastBroadcastDate = newBroadcasts.Last().SendAt;
                if (newUser == true)
                {
                    newBroadcasts = newBroadcasts.Where(_ => _.SendToNewUsers == true);
                }
                foreach (var broadcast in newBroadcasts)
                {
                    var mail = new Mail()
                    {
                        Subject = broadcast.Subject,
                        Body = broadcast.Body,
                        FromUserId = 0,
                        CanParticipantDelete = true,
                        IsNew = true,
                        IsDeleted = false,
                        SiteId = user.SiteId,
                        IsBroadcast = true,
                        ToUserId = userId
                    };

                    await _mailRepository.AddSaveAsync(authUserId, mail);
                }
                user.LastBroadcast = lastBroadcastDate.Value;
                await _userRepository.UpdateSaveAsync(authUserId, user);

                _memoryCache.Remove($"{CacheKey.UserUnreadMailCount}?u{userId}");
            }

            if (includeHousehold)
            {
                var householdMembers = await _userRepository.GetHouseholdAsync(userId);
                foreach (var member in householdMembers)
                {
                    await SendUserBroadcastsAsync(member.Id, false);
                }
            }
        }
    }
}
