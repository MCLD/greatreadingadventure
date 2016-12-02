using System.Threading.Tasks;

namespace GRA.Domain.Service.Abstract
{
    public interface IUserContextProvider
    {
        Task<UserContext> GetContext();
    }
}
