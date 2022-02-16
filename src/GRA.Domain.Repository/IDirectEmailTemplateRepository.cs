using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IDirectEmailTemplateRepository : IRepository<Model.DirectEmailTemplate>
    {
        public Task<DirectEmailTemplate> GetWithTextByIdAsync(int directEmailTemplateId,
            int languageId);

        public Task<DirectEmailTemplate> GetWithTextBySystemId(string systemEmailId,
            int languageId);

        public Task UpdateSentBulkAsync(int directEmailTemplateId);
    }
}
