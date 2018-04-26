using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IAuthorizationCodeRepository : IRepository<AuthorizationCode>
    {
        Task<DataWithCount<IEnumerable<AuthorizationCode>>> PageAsync(BaseFilter filter);
        Task<AuthorizationCode> GetByCodeAsync(int siteId, string authorizationCode);
    }
}
