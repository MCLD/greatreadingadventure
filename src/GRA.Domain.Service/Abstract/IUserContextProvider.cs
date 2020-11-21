using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Service.Abstract
{
    public interface IUserContextProvider
    {
        UserContext GetContext();
        Task<Site> GetCurrentSiteAsync();
        bool UserHasPermission(ClaimsPrincipal user, string permissionName);
        string UserClaim(ClaimsPrincipal user, string claimType);
        int GetId(ClaimsPrincipal user, string claimType);
        CultureInfo GetCurrentCulture();
    }
}
