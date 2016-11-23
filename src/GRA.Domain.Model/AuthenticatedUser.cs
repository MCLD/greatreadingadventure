using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Model
{
    public class AuthenticatedUser
    {
        public string AuthenticationMessage { get; set; }
        public bool Authenticated { get; set; }
        public ICollection<Permission> Permissions { get; set; }
        public User User { get; set; }
    }
}
