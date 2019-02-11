using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ILanguageRepository : IRepository<Model.Language>
    {
        Task<ICollection<Model.Language>> GetActiveAsync();
        Task<ICollection<Model.Language>> GetAllAsync();
    }
}
