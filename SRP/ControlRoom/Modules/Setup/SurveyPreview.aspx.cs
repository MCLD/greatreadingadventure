using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRoom.Controls;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class SurveyPreview : BaseControlRoomPage
    {
        private bool JumpActivated = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
                SID.Text = Session["SID"] == null ? "" : Session["SID"].ToString();
                if (SID.Text == "") Response.Redirect("SurveyList.aspx");


                var s = DAL.Survey.FetchObject(int.Parse(SID.Text));
                lblSurvey.Text = s.LongName;
                lblPreamble.Text = s.Preamble;

                MasterPage.RequiredPermission = 5200;
                MasterPage.IsSecure = true;
                MasterPage.PageTitle = string.Format("{0}", "Survey/Test Question List");

                GetNextTestPage();
            }
        }


        protected void lb1_Click(object sender, EventArgs e)
        {
            Response.Redirect("SurveyAddEdit.aspx");
        }

        protected void lb2_Click(object sender, EventArgs e)
        {
            Response.Redirect("SurveyQuestionList.aspx");
        }



        public bool SaveTestPage()
        {
            // Save answers, in a loop;
            
            // Check in a loop if any of the answers generated a jump condition, and if so, set the start of the next page.
            foreach (RepeaterItem i in SurveyQLst.Items)
            {
                var qp = i.FindControl("qp") as QuestionPreview;
                if (qp != null)
                {
                    var jq = qp.JumpLogicActivated();
                    if (jq > 0) {
                        QNumber.Text = (jq-1).ToString();
                        JumpActivated = true;
                        break;
                    }
                }
            }

            return true;
        }

        public void GetNextTestPage()
        {
            if (Session["QNum"] == null) Session["QNum"] = 0;
            QNumber.Text = Session["QNum"].ToString();
            int qNum = 0;
            int.TryParse(QNumber.Text, out qNum);
            if (qNum != 0) lblPreamble.Visible = false;

            var ds = SurveyQuestion.GetSurveyPage(int.Parse(SID.Text), int.Parse(QNumber.Text));
            SurveyQLst.DataSource = ds;
            SurveyQLst.DataBind();

            if (ds.Tables[0].Rows.Count > 0)
                QNumber.Text = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["QNumber"].ToString();
        }

        public void EndTest()
        {
            // update SurveyResults and set is complete 
            // score if scorable;

            if (Session["TextReturnPage"] != null && Session["TextReturnPage"].ToString()!="")
            {
                Response.Redirect(Session["TextReturnPage"].ToString());
            }
            else
            {
                Response.Redirect("~");
            }
        }

        protected void SurveyQLst_ItemCommand1(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EOP")
            {

                SaveTestPage();
                Session["QNum"] = QNumber.Text;
                Response.Redirect("SurveyPreview.aspx");
            }

            if (e.CommandName == "EOT")
            {
                SaveTestPage();
                if (JumpActivated)
                {
                    Session["QNum"] = QNumber.Text;
                    Response.Redirect("SurveyPreview.aspx");
                }
                Session["QNum"] = null;
                EndTest();
            }
        }
    }
}