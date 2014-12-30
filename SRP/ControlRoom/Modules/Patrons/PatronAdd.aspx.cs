using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;

namespace STG.SRP.ControlRoom.Modules.Patrons
{
    public partial class PatronAdd : BaseControlRoomPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            MasterPage.IsSecure = true;

            if (!IsPostBack)
            {
                string editpage = "~/ControlRoom/Modules/Patrons/PatronDetails.aspx";

                    Session["CURR_PATRON_ID"] = "";
                    Session["CURR_PATRON"] = null;
                    Session["CURR_PATRON_MODE"] = "ADDNEW";
                    Response.Redirect(editpage);

            }

        }
    }
}