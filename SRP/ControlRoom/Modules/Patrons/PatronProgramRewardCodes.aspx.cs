using GRA.Tools;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom.Modules.Patrons
{
    public partial class PatronProgramRewardCodes : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5100;
            MasterPage.AdditionalRequiredPermission = 2200;
            MasterPage.IsSecure = true;
            if (Session["Curr_Patron"] == null)
            {
                Response.Redirect("Default.aspx");
            }

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);

                var patron = Session["Curr_Patron"] as DAL.Patron;
                if (patron == null)
                {
                    Response.Redirect("Default.aspx");
                }

                MasterPage.PageTitle = string.Format("Patron Program Reward Codes - {0}",
                    DisplayHelper.FormatName(patron.FirstName, patron.LastName, patron.Username));

                var programCodes = DAL.ProgramCodes.GetAllForPatron(patron.PID);
                if (programCodes != null
                    && programCodes.Tables.Count > 0
                    && programCodes.Tables[0].Rows.Count > 0)
                {
                    EarnedCodeRepeater.DataSource = programCodes;
                    EarnedCodeRepeater.DataBind();
                    NoCode.Visible = false;
                }
                else
                {
                    NoCode.Visible = true;
                }

            }
        }
    }
}