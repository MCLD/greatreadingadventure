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
    public partial class PatronsMasterAccount : BaseControlRoomPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true;

            if (!IsPostBack)
            {
                if (Session["Curr_Patron"] == null)
                {
                    Response.Redirect("Default.aspx");
                }
                else
                {
                    var patron = (Patron)Session["Curr_Patron"];
                    string editpage = "~/ControlRoom/Modules/Patrons/PatronDetails.aspx";
                    Session["CURR_PATRON_ID"] = patron.MasterAcctPID;
                    Session["CURR_PATRON"] = Patron.FetchObject(patron.MasterAcctPID);
                    Response.Redirect(editpage);
                }
            }

        }
    }
}