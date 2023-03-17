using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IMessageTemplateRepository : IRepository<MessageTemplate>
    {
        public Task<MessageTemplateText> AddSaveTextAsync(int messageTemplateId,
            int languageId,
            string subject,
            string body);

        public Task<MessageTemplate> GetByNameAsync(string templateName);

        public Task<int[]> GetLanguagesAsync(int messageTemplateId);

        public Task<MessageTemplateText> GetTextAsync(int messageTemplateId, int languageId);

        public Task RemoveIfUnusedAsync(int messageTemplateId);

        public Task RemoveTextAsync(int messageTemplateId, int languageId);

        public Task<bool> TextExistsAsync(int messageTemplateId, int languageId);

        public Task UpdateTextSaveAsync(int messageTemplateId,
            int languageId,
            string subject,
            string body);
    }
}
