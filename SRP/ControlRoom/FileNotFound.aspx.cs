using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;

namespace GRA.SRP.ControlRoom
{
    public partial class FileNotFound : BaseControlRoomPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.PageTitle = "Application Error Page";
            MasterPage.IsSecure = false;
        }
    }
}