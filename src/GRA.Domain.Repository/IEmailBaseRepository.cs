using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IEmailBaseRepository : IRepository<Model.EmailBase>
    {
        public Task<EmailBase> GetWithTextByIdAsync(int emailBaseId, int languageId);
        public Task<IEnumerable<EmailBase>> GetAllAsync();
    }
}
