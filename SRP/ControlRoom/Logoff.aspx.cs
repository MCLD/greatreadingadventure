using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using SRPApp.Classes;

namespace GRA.SRP.ControlRoom {
    public partial class Logoff : BaseControlRoomPage {
        protected void Page_Load(object sender, EventArgs e) {
            CRLogout.Logout(this);
        }
    }
}