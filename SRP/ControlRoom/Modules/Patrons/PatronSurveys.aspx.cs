using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom.Modules.Patrons
{
    public partial class PatronSurveys : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true;
            if (Session["Curr_Patron"] == null) Response.Redirect("Default.aspx");

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);
            }

            if (!IsPostBack)
            {
                var patron = (Patron)Session["Curr_Patron"];
                MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Patron Tests/Surveys", patron.FirstName, patron.LastName, patron.Username);

                LoadData();

            }

        }

        protected void LoadData()
        {
            var patron = (Patron)Session["Curr_Patron"];

            var ds = DAL.SurveyResults.GetAllByPatron(patron.PID);

            gv.DataSource = ds;
            gv.DataBind();
        }

        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {
            string viewpage = "~/ControlRoom/Modules/Patrons/PatronSurveyDetails.aspx";
            
            if (e.CommandName.ToLower() == "viewrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                Session["PSRID"] = key;
                Response.Redirect(viewpage);

            }

        }




    }
}