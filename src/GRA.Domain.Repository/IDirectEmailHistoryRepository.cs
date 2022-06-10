using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDirectEmailHistoryRepository : IRepository<Model.DirectEmailHistory>
    {
        public Task<HashSet<string>> GetSentEmailByTemplateIdAsync(int directEmailTemplateId);
    }
}
