using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDirectEmailHistoryRepository : IRepository<Model.DirectEmailHistory>
    {
        public Task<ISet<string>> GetSentEmailByTemplateIdAsync(int directEmailTemplateId);
    }
}
