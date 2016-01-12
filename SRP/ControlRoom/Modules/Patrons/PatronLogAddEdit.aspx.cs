using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;


namespace GRA.SRP.ControlRoom.Modules.Patrons
{
    public partial class PatronLogAddEdit : BaseControlRoomPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true;
            if (Session["Curr_Patron"] == null) Response.Redirect("Default.aspx");

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);

                var patron = (Patron)Session["Curr_Patron"];
                MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Patron Log Entry", patron.FirstName, patron.LastName, patron.Username);
            }


            if (!IsPostBack)
            {
                
                if (Session["PLID"] == null || Session["PLID"].ToString() == "" )
                {
                    PatronLogEntryCtl1.PatronID = Session["CURR_PATRON_ID"].ToString();
                    PatronLogEntryCtl1.PatronPointsID= string.Empty;
                    PatronLogEntryCtl1.LoadControl();
                }
                else
                {
                    PatronLogEntryCtl1.PatronID = Session["CURR_PATRON_ID"].ToString();
                    PatronLogEntryCtl1.PatronPointsID = Session["PLID"].ToString();
                    PatronLogEntryCtl1.LoadControl();
                }
            }

        }
    }
}