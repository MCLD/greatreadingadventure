using GRA.Domain.Model;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IAuthorizationCodeRepository : IRepository<AuthorizationCode>
    {
        Task<AuthorizationCode> GetByCodeAsync(int siteId, string authorizationCode);
    }
}
