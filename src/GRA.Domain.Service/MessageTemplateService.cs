using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class MessageTemplateService : BaseUserService<MessageTemplateService>
    {
        private readonly IMessageTemplateRepository _messageTemplateRepository;

        public MessageTemplateService(IDateTimeProvider dateTimeProvider,
            ILogger<MessageTemplateService> logger,
            IMessageTemplateRepository messageTemplateRepository,
            IUserContextProvider userContextProvider)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            ArgumentNullException.ThrowIfNull(messageTemplateRepository);

            _messageTemplateRepository = messageTemplateRepository;
        }

        public async Task<MessageTemplateText> AddTextAsync(int languageId,
            string subject,
            string body,
            string item)
        {
            var template = await _messageTemplateRepository
                    .AddSaveAsync(GetActiveUserId(), new MessageTemplate
                    {
                        CreatedAt = _dateTimeProvider.Now,
                        CreatedBy = GetActiveUserId(),
                        Name = GetMessageTemplateName(item)
                    });

            return await _messageTemplateRepository.AddSaveTextAsync(template.Id,
                languageId,
                subject,
                body);
        }

        public async Task<IDictionary<int, int[]>> GetLanguageStatusAsync(int?[] messageTemplateIds)
        {
            var languageStatus = new Dictionary<int, int[]>();

            foreach (var messageTemplateId in messageTemplateIds.Where(_ => _.HasValue))
            {
                languageStatus.Add(messageTemplateId.Value,
                    await _messageTemplateRepository.GetLanguagesAsync(messageTemplateId.Value));
            }

            return languageStatus;
        }

        public async Task<int> GetMessageIdAsync(string name)
        {
            var template = await _messageTemplateRepository.GetByNameAsync(name);
            return template.Id;
        }

        public async Task<MessageTemplateText> GetMessageTextAsync(int messageTemplateId,
                    int languageId)
        {
            return await _messageTemplateRepository.GetTextAsync(messageTemplateId, languageId);
        }

        public async Task UpdateTextAsync(int messageTemplateId,
            int languageId,
            string subject,
            string body)
        {
            if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(body))
            {
                await _messageTemplateRepository.RemoveTextAsync(messageTemplateId, languageId);
                await _messageTemplateRepository.RemoveIfUnusedAsync(messageTemplateId);
            }
            else
            {
                if (await _messageTemplateRepository.TextExistsAsync(messageTemplateId, languageId))
                {
                    await _messageTemplateRepository.UpdateTextSaveAsync(messageTemplateId,
                        languageId,
                        subject,
                        body);
                }
                else
                {
                    await _messageTemplateRepository.AddSaveTextAsync(messageTemplateId,
                        languageId,
                        subject,
                        body);
                }
            }
        }

        private static string GetMessageTemplateName(string item) => item switch
        {
            nameof(VendorCodeType.DonationMessageTemplateId) => SegmentNames.VendorCodeDonation,
            nameof(VendorCodeType.EmailAwardMessageTemplateId) => SegmentNames.VendorCodeEmailAward,
            nameof(VendorCodeType.MessageTemplateId) => "Vendor Code Award",
            nameof(VendorCodeType.OptionMessageTemplateId) => "Vendor Code Option",
            _ => throw new GraException("Unknown message template type")
        };
    }
}
