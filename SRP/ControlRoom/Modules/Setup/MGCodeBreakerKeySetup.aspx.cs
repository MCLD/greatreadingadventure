using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class MGCodeBreakerKeySetup : BaseControlRoomPage
    {

        

        public IEnumerable<KeyItem> GetKeyCharacters()
        {
            var CBID = int.Parse(lblCBID.Text);
            return MGCodeBreaker.GetKeyCharacters(CBID);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var difficulty = 1;
                if (Request["d"] != null && Request["d"] == "2") difficulty = 2;
                if (Request["d"] != null && Request["d"] == "3") difficulty = 3;

                var sDifficulty = "Easy";
                if (difficulty == 2) sDifficulty = "Medium";
                if (difficulty == 3) sDifficulty = "Hard";

                lblSDifficulty.Text = sDifficulty;
                lblDifficulty.Text = difficulty.ToString();
                lblPrefix.Text= string.Empty;
                if (difficulty == 2) lblPrefix.Text = "m_";
                if (difficulty == 3) lblPrefix.Text = "h_";
            }

            MasterPage.RequiredPermission = 4300;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0} {1} {2}", "Code Breaker", lblSDifficulty.Text, "Key Setup");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            

                if (Session["MGID"] != null)
                {
                    lblMGID.Text = Session["MGID"].ToString();
                    lblCBID.Text = Request["CBID"];

                    var o = Minigame.FetchObject(int.Parse(lblMGID.Text));
                    AdminName.Text = o.AdminName;
                }
                else
                {
                    Response.Redirect("~/ControlRoom/");
                }
                rptr.DataSource = GetKeyCharacters();
                rptr.DataBind();
            }
        }

        protected void btnBack_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //string returnURL = "~/ControlRoom/Modules/Setup/MGCodeBreakerAddEdit.aspx?PK=" + lblMGID.Text;
            string returnURL = "~/ControlRoom/Modules/Setup/MGCodeBreakerAddEdit.aspx";
            Response.Redirect(returnURL);
        }

        protected void btnRefresh_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }
    }
}