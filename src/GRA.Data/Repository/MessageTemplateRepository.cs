using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class MessageTemplateRepository
        : AuditingRepository<Model.MessageTemplate, MessageTemplate>,
        IMessageTemplateRepository
    {
        public MessageTemplateRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<MessageTemplateRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<MessageTemplateText> AddSaveTextAsync(int messageTemplateId,
            int languageId,
            string subject,
            string body)
        {
            var text = new Model.MessageTemplateText
            {
                Body = body,
                LanguageId = languageId,
                MessageTemplateId = messageTemplateId,
                Subject = subject
            };
            await _context.MessageTemplateTexts.AddAsync(text);
            await _context.SaveChangesAsync();
            return _mapper.Map<MessageTemplateText>(text);
        }

        public async Task<MessageTemplate> GetByNameAsync(string templateName)
        {
            return await DbSet
                .AsNoTracking()
                .ProjectToType<Domain.Model.MessageTemplate>()
                .SingleOrDefaultAsync(_ => _.Name == templateName);
        }

        public async Task<int[]> GetLanguagesAsync(int messageTemplateId)
        {
            return await _context.MessageTemplateTexts
                .AsNoTracking()
                .Where(_ => _.MessageTemplateId == messageTemplateId)
                .Select(_ => _.LanguageId)
                .ToArrayAsync();
        }

        public async Task<MessageTemplateText> GetTextAsync(int messageTemplateId, int languageId)
        {
            return await _context.MessageTemplateTexts
                .AsNoTracking()
                .ProjectToType<MessageTemplateText>()
                .SingleOrDefaultAsync(_ => _.MessageTemplateId == messageTemplateId
                    && _.LanguageId == languageId);
        }

        public async Task RemoveIfUnusedAsync(int messageTemplateId)
        {
            var texts = _context.MessageTemplateTexts
                .Any(_ => _.MessageTemplateId == messageTemplateId);

            if (!texts)
            {
                var messageTemplate = await DbSet.SingleAsync(_ => _.Id == messageTemplateId);
                DbSet.Remove(messageTemplate);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveTextAsync(int messageTemplateId, int languageId)
        {
            var messageTemplateText = _context.MessageTemplateTexts
                .SingleOrDefault(_ => _.MessageTemplateId == messageTemplateId
                    && _.LanguageId == languageId)
                ?? throw new GraDbUpdateException($"Unable to find message template text for id {messageTemplateId} in language {languageId}");
            _context.MessageTemplateTexts.Remove(messageTemplateText);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TextExistsAsync(int messageTemplateId, int languageId)
        {
            return await _context.MessageTemplateTexts
                .AsNoTracking()
                .AnyAsync(_ => _.MessageTemplateId == messageTemplateId
                    && _.LanguageId == languageId);
        }

        public async Task UpdateTextSaveAsync(int messageTemplateId,
            int languageId,
            string subject,
            string body)
        {
            var messageTemplateText = await _context.MessageTemplateTexts
                .SingleOrDefaultAsync(_ => _.MessageTemplateId == messageTemplateId
                    && _.LanguageId == languageId)
                        ?? throw new GraDbUpdateException($"Unable to find MessageTemplateText id {messageTemplateId}, language id {languageId}");

            messageTemplateText.Subject = subject;
            messageTemplateText.Body = body;

            _context.Update(messageTemplateText);

            await _context.SaveChangesAsync();
        }
    }
}
