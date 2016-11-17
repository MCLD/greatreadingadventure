using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Model.MissionControl
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool CookieUsername { get; set; }
    }
}
