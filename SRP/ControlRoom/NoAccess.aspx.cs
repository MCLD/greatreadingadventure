using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;

namespace STG.SRP.ControlRoom
{
    public partial class NoAccess : BaseControlRoomPage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            IControlRoomMaster masterPage = (IControlRoomMaster)Master;
            masterPage.IsSecure = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[SessionData.IsLoggedIn.ToString()] == null || (bool)Session[SessionData.IsLoggedIn.ToString()] == false)
            {
                Response.Redirect("Login.aspx");
            }
            IControlRoomMaster masterPage = (IControlRoomMaster)Master;
            //masterPage.PageRibbon.DataBind();

            masterPage.PageTitle = "STG Summer Reading Program - Control Room<BR> &nbsp;Access Permission Error";
            masterPage.PageError = "You do not have permission for the screen, feature or function you were trying to access.";
            masterPage.DisplayMessageOnLoad = true;
        }
    }
}