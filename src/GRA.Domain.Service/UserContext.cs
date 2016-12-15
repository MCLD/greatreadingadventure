using GRA.Domain.Model;
using System.Security.Claims;

namespace GRA.Domain.Service
{
    public class UserContext
    {
        public ClaimsPrincipal User { get; set; }
        public int SiteId { get; set; }
        public int? ActiveUserId { get; set; }
        public SiteStage SiteStage { get; set; }
    }
}
