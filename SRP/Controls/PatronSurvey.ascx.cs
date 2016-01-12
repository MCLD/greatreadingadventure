using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.ControlRoom.Controls;
using GRA.SRP.DAL;

namespace GRA.SRP.Controls {
    public partial class PatronSurvey : System.Web.UI.UserControl {
        private bool JumpActivated = false;
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                SRID.Text = Session["SRID"] == null ? "0" : Session["SRID"].ToString();
                SID.Text = Session["SID"] == null ? "" : Session["SID"].ToString();
                if(string.IsNullOrEmpty(SID.Text))
                    Response.Redirect("~");

                var p = (Patron)Session["Patron"];

                var s = DAL.Survey.FetchObject(int.Parse(SID.Text));
                lblSurvey.Text = s.LongName;
                lblPreamble.Text = Server.HtmlDecode(s.Preamble);

                GetNextTestPage();
            }
        }

        public void GetNextTestPage() {
            if(Session["QNum"] == null)
                Session["QNum"] = 0;
            QNumber.Text = Session["QNum"].ToString();
            int qNum = 0;
            int.TryParse(QNumber.Text, out qNum);
            if(qNum != 0)
                lblPreamble.Visible = false;

            var ds = SurveyQuestion.GetSurveyPage(int.Parse(SID.Text), int.Parse(QNumber.Text));
            SurveyQLst.DataSource = ds;
            SurveyQLst.DataBind();

            if(ds.Tables[0].Rows.Count > 0)
                QNumber.Text = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["QNumber"].ToString();
        }


        public bool SaveTestPage() {
            var sr = new SurveyResults();
            var p = (Patron)Session["Patron"];
            if(SRID.Text != "0") {
                sr = SurveyResults.FetchObject(int.Parse(SRID.Text));
            } else {

                sr.TenID = p.TenID;
                sr.PID = p.PID;
                sr.SID = int.Parse(SID.Text);
                sr.StartDate = DateTime.Now;
                sr.IsComplete = false;
                sr.LastAnswered = 0;
                sr.Source = Session["SSrc"] == null ? "" : Session["SSrc"].ToString();
                sr.SourceID = Session["SSrcID"] == null ? 0 : int.Parse(Session["SSrcID"].ToString());

                sr.Insert();
                Session["SRID"] = sr.SRID;
                SRID.Text = sr.SRID.ToString();
            }
            // Save answers, in a loop;
            foreach(RepeaterItem i in SurveyQLst.Items) {
                var qp = i.FindControl("qp") as QuestionPreview;
                if(qp != null) {
                    qp.Save((int)Session["SRID"]);
                    sr.LastAnswered = int.Parse(qp.QNumber);
                    sr.Update();
                }
            }

            // Check in a loop if any of the answers generated a jump condition, and if so, set the start of the next page.))
            foreach(RepeaterItem i in SurveyQLst.Items) {
                var qp = i.FindControl("qp") as QuestionPreview;
                if(qp != null) {
                    var jq = qp.JumpLogicActivated();
                    if(jq > 0) {
                        QNumber.Text = (jq - 1).ToString();
                        sr.LastAnswered = int.Parse(QNumber.Text);   //If they quit, then they come back and start at the jump
                        sr.Update();
                        JumpActivated = true;
                        break;
                    }
                }
            }

            return true;
        }

        public void EndTest() {
            // update SurveyResults and set is complete 
            // score if scorable;
            var sr = SurveyResults.FetchObject(int.Parse(SRID.Text));
            sr.EndDate = DateTime.Now;
            sr.IsComplete = true;
            sr.IsScorable = Survey.IsScorable(sr.SID);
            if(sr.IsScorable) {
                sr.Score = SurveyResults.PerformScoring(sr.SRID);
                if((decimal)Survey.MaxScore(sr.SID) != 0)
                    sr.ScorePct = (decimal)sr.Score * 100 / (decimal)Survey.MaxScore(sr.SID);
                else
                    sr.ScorePct = 0;

                var p = (Patron)Session["Patron"];
                if(sr.Source == Survey.Source(1)) {
                    p.Score1 = sr.Score;
                    p.Score1Pct = sr.ScorePct;
                    p.Score1Date = sr.EndDate;
                    p.Update();
                    Session["Patron"] = p;
                }
                if(sr.Source == Survey.Source(2)) {
                    p.Score2 = sr.Score;
                    p.Score2Pct = sr.ScorePct;
                    p.Score2Date = sr.EndDate;
                    p.Update();
                    Session["Patron"] = p;
                }
            }
            sr.Update();

            var SurveyEndPage = "~";
            if(Session["Page"] != null && Session["Page"].ToString() == "1") {
                SurveyEndPage = "Thanks.aspx";
            }
            Session.Remove("SRID");
            Session.Remove("SID");
            Session.Remove("QNum");
            Session.Remove("SSrc");
            Session.Remove("SSrcID");
            Session.Remove("PreTestMandatory");
            Session.Remove("Page");

            if(Session["TestReturnPage"] != null &&
                !string.IsNullOrEmpty(Session["TestReturnPage"].ToString())) {
                var gotoStr = Session["TestReturnPage"].ToString();
                Session.Remove("TestReturnPage");
                if(string.IsNullOrEmpty(gotoStr)) {
                    Response.Redirect("~");
                } else {
                    Response.Redirect(string.Format("~/{0}", gotoStr));
                }
            } else {
                Response.Redirect(SurveyEndPage);
            }
        }

        protected void SurveyQLst_ItemCommand1(object source, RepeaterCommandEventArgs e) {
            var SurveyTakingPage = "AddlSurvey.aspx";
            if(Session["Page"] != null && Session["Page"].ToString() == "1") {
                SurveyTakingPage = "FramedSurvey.aspx?C=Y";
            }
            if(e.CommandName == "EOP") {

                SaveTestPage();
                Session["QNum"] = QNumber.Text;
                Response.Redirect(SurveyTakingPage);
            }

            if(e.CommandName == "EOT") {
                SaveTestPage();
                if(JumpActivated) {
                    Session["QNum"] = QNumber.Text;
                    Response.Redirect(SurveyTakingPage);
                }

                Session["QNum"] = null;
                EndTest();
            }
        }
    }
}