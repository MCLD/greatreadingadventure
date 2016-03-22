using System;
using SRPApp.Classes;

namespace GRA.SRP.ControlRoom
{
    public partial class Logoff : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CRLogout.Logout(this);
        }
    }
}