using GRA.Domain.Model;
using System.Threading.Tasks;

namespace GRA.Domain.Service.Abstract
{
    public interface IUserContextProvider
    {
        UserContext GetContext();
        Task<Site> GetCurrentSiteAsync();
    }
}
