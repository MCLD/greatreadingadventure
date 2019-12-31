using System.Threading.Tasks;

namespace GRA.Domain.Service.Abstract
{
    public interface IInitialSetupService
    {
        Task InsertAsync(int siteId, string initialAuthorizationCode);
    }
}
