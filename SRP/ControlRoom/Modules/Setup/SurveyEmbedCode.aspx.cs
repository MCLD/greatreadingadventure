using System;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using ExportToExcel;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;


namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class SurveyEmbedCode : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

            }

            MasterPage.RequiredPermission = 5200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Test/Survey Results");
        }

        protected void DDSourceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDSourceID.Items.Clear();
            var ds = new DataSet();
            switch (DDSourceType.SelectedValue)
            {
                case "3":   // game
                    ds = Minigame.GetAll();
                    DDSourceID.DataTextField = "AdminName";
                    DDSourceID.DataValueField = "MGID";
                    DDSourceID.DataSource = ds;
                    DDSourceID.DataBind();
                    DDSourceID.SelectedValue = DDSourceID.Items[0].Value;
                    break;
                case "4":   // booklist
                    ds = BookList.GetAll();
                    DDSourceID.DataTextField = "AdminName";
                    DDSourceID.DataValueField = "BLID";
                    DDSourceID.DataSource = ds;
                    DDSourceID.DataBind();
                    DDSourceID.SelectedValue = DDSourceID.Items[0].Value;
                    break;
                case "5":   // event
                    ds = Event.GetAll();
                    DDSourceID.DataTextField = "EventTitle";
                    DDSourceID.DataValueField = "EID";
                    DDSourceID.DataSource = ds;
                    DDSourceID.DataBind();
                    DDSourceID.SelectedValue = DDSourceID.Items[0].Value;
                    break;
                case "0":
                default:
                    break;
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (DDSourceType.SelectedValue == "0" || SID.SelectedValue == "0")
            {
                MasterPage.PageError = "You need to select a survey and a source.";
                return;
            }


            string iFrame =
                "<iframe allowtransparency=\"true\" frameborder=\"0\" id=\"srvy\" style=\"border:0px;padding-bottom:4px;\" \n\tsrc=\"FramedSurvey.aspx?SID={0}&SSrc={1}&SSrcID={2}\" \n\theight=\"{3}\" width=\"{4}\">\n\t</iframe>";

            lblInfo.Text = "Below you will find the IFRAME tag you will need to insert into your badge's message or notification";

            txtResults.Text = string.Format(iFrame, SID.SelectedValue, DDSourceType.SelectedValue, DDSourceID.SelectedValue, IWidth.Text, IHeight.Text);

            pnlResults.Visible = true;
        }

        protected void btnFilter0_Click(object sender, EventArgs e)
        {
            pnlResults.Visible = false;
            lblInfo.Text = txtResults.Text= string.Empty;
            IWidth.Text = "700";
            IHeight.Text = "800";
            DDSourceID.Items.Clear();
            DDSourceType.SelectedValue = "0";
            SID.SelectedValue = "0";
        }


    }
}