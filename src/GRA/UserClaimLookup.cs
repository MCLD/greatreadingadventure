using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GRA
{
    public class UserClaimLookup
    {
        private readonly ClaimsPrincipal _user;
        public UserClaimLookup(ClaimsPrincipal user)
        {
            _user = Require.IsNotNull(user, nameof(user));
        }
        public bool UserHasPermission(string permissionName)
        {
            return _user.HasClaim(ClaimType.Permission, permissionName);
        }

        public string UserClaim(string claimType)
        {
            var claim = _user
                .Claims
                .Where(_ => _.Type == claimType)
                .SingleOrDefault();

            if (claim != null)
            {
                return claim.Value;
            }
            else
            {
                return null;
            }
        }

        public int GetId(string claimType)
        {
            string result = UserClaim(claimType);
            if (string.IsNullOrEmpty(result))
            {
                throw new Exception($"Could not find user claim '{claimType}'");
            }
            int id;
            if (int.TryParse(result, out id))
            {
                return id;
            }
            else
            {
                throw new Exception($"Could not convert '{claimType}' to a number.");
            }
        }
    }
}
