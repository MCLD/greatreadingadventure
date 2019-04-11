using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface ILanguageRepository : IRepository<Model.Language>
    {
        Task<ICollection<Language>> GetActiveAsync();
        Task<ICollection<Language>> GetAllAsync();
        Task<Language> GetActiveByIdAsync(int id);
        Task<int> GetDefaultLanguageId();
        Task<int> GetLanguageId(string culture);
    }
}
