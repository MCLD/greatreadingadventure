using System.Security.Claims;

namespace GRA.Domain.Service
{
    public class UserContext
    {
        public ClaimsPrincipal User { get; set; }
        public int SiteId { get; set; }
    }
}
