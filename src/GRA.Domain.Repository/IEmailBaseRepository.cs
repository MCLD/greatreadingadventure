using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IEmailBaseRepository : IRepository<Model.EmailBase>
    {
        public Task<int> AddSaveWithTextAsync(int userId, EmailBase emailBase);

        public Task<IEnumerable<EmailBase>> GetAllAsync();

        public Task<EmailBase> GetDefaultAsync();

        public Task<IEnumerable<int>> GetTextLanguagesAsync(int emailBaseId);

        public Task<EmailBase> GetWithTextAsync(int emailBaseId, int languageId);

        public Task<EmailBase> GetWithTextByIdAsync(int emailBaseId, int languageId);

        public Task ImportSaveTextAsync(int userId, EmailBaseText emailBaseText);

        public Task<ICollectionWithCount<EmailBase>> PageAsync(BaseFilter filter);

        public Task UpdateSaveWithText(int userId, EmailBase emailBase);
    }
}
