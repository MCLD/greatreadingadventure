using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;

namespace GRA.Domain.Repository
{
    public interface IDirectEmailTemplateRepository : IRepository<Model.DirectEmailTemplate>
    {
        public Task<int> AddSaveWithTextAsync(int userId, DirectEmailTemplate directEmailTemplate);

        public Task<DirectEmailTemplate> GetWithTextByIdAsync(int directEmailTemplateId,
                                    int languageId);

        public Task<DirectEmailTemplate> GetWithTextBySystemId(string systemEmailId,
            int languageId);

        public Task<ICollectionWithCount<EmailTemplateListItem>> PageAsync(BaseFilter filter);

        public Task UpdateSaveWithText(int userId, DirectEmailTemplate directEmailTemplate);

        public Task UpdateSentBulkAsync(int directEmailTemplateId);
    }
}
