using System.Threading.Tasks;

namespace GRA.Domain.Service.Abstract
{
    public interface IUserContextProvider
    {
        UserContext GetContext();
    }
}
