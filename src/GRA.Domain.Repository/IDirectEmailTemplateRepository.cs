using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;

namespace GRA.Domain.Repository
{
    public interface IDirectEmailTemplateRepository : IRepository<Model.DirectEmailTemplate>
    {
        public Task<int> AddSaveWithTextAsync(int userId, DirectEmailTemplate directEmailTemplate);

        public Task<(int, List<int>)> GetIdAndLanguagesBySystemIdAsync(string systemEmailId);

        public Task<IDictionary<int, bool>> GetLanguageUnsubAsync(int directEmailTemplateId);

        public Task<DirectEmailTemplate> GetWithTextByIdAsync(int directEmailTemplateId,
                                            int languageId);

        public Task<DirectEmailTemplate> GetWithTextBySystemId(string systemEmailId,
            int languageId);

        public Task ImportSaveTextAsync(int userId,
            DirectEmailTemplateText directEmailTemplateText);

        public Task IncrementSentCountAsync(int directEmailTemplateId, int incrementBy);

        public Task<ICollectionWithCount<EmailTemplateListItem>> PageAsync(BaseFilter filter);

        public Task<bool> SystemEmailIdExistsAsync(string systemEmailId);

        public Task<bool> SystemEmailTextExists(int directEmailTemplateId, int languageId);

        public Task UpdateSaveWithText(int userId, DirectEmailTemplate directEmailTemplate);

        public Task UpdateSentBulkAsync(int directEmailTemplateId);
    }
}
