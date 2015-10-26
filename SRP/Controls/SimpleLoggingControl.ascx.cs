using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.Controls
{
    public partial class SimpleLoggingControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (Session["Patron"] == null) Response.Redirect("/");
                var patron = (Patron)Session["Patron"];
                lblPID.Text = patron.PID.ToString();
                var prog = Programs.FetchObject(patron.ProgID);
                lblPGID.Text = prog.PID.ToString();
                pnlReview.Visible = prog.PatronReviewFlag;
                
                // Load the Activity Types to log

                foreach (ActivityType val in Enum.GetValues(typeof(ActivityType)))
                {
                    var pgc = ProgramGamePointConversion.FetchObjectByActivityId(prog.PID, (int)val);
                    if (pgc != null && pgc.PointCount > 0)
                    {
                        rbActivityType.Items.Add(new ListItem(val.ToString(), ((int) val).ToString()));
                    }
                }
                rbActivityType.SelectedIndex = 0;



            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var txtCount = txtCountSubmitted.Text.Trim();
            var txtCode = txtProgramCode.Text.Trim();
            // ---------------------------------------------------------------------------------------------------
            if (txtCount.Length > 0 && txtCode.Length > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please enter either how much you have read OR a code, but not both.<br><br>";
                return;
            }

            if (txtCount.Length == 0 && txtCode.Length == 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please enter either how much you have read OR a code.<br><br>";
                return;
            }
            // ---------------------------------------------------------------------------------------------------

            int PID = int.Parse(lblPID.Text);
            int PGID = int.Parse(lblPGID.Text);

            var pa = new AwardPoints(PID);
            var sBadges = "";
            var points = 0;
            #region Reading
            // ---------------------------------------------------------------------------------------------------
            // Logging reading ...
            //Badge EarnedBadge;
            if (txtCount.Length > 0)
            {
                var intCount = 0;
                if (!int.TryParse(txtCount, out intCount))
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "How much you have read must be a number.<br><br>";
                    return;
                }

                if (intCount < 0)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "Hmmm, you must enter a positive number...<br><br>";
                    return;
                }

                int maxAmountForLogging = 0;
                int maxPointsPerDayForLogging = SRPSettings.GetSettingValue("MaxPtsDay").SafeToInt();
                switch (int.Parse(rbActivityType.SelectedValue))
                {
                    case 0: maxAmountForLogging = SRPSettings.GetSettingValue("MaxBook").SafeToInt();
                        break;
                    case 1: maxAmountForLogging = SRPSettings.GetSettingValue("MaxPage").SafeToInt();
                        break;
                    //case 2: maxAmountForLogging = SRPSettings.GetSettingValue("MaxPar").SafeToInt();
                    //    break;
                    case 3: maxAmountForLogging = SRPSettings.GetSettingValue("MaxMin").SafeToInt();
                        break;
                    default: maxAmountForLogging = SRPSettings.GetSettingValue("MaxMin").SafeToInt();
                        break;
                }

                if (intCount > maxAmountForLogging)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = string.Format("That is an awful lot of reading... unfortunately the maximum you can submit at one time is {0} {1}.<br><br>",
                        maxAmountForLogging, ((ActivityType)int.Parse(rbActivityType.SelectedValue)).ToString());
                    return;
                }

                // convert pages/minutes/etc. to points
                var pc = new ProgramGamePointConversion();
                pc.FetchByActivityId(PGID, int.Parse(rbActivityType.SelectedValue));
                points = Convert.ToInt32(intCount * pc.PointCount / pc.ActivityCount);

                var allPointsToday = PatronPoints.GetTotalPatronPoints(PID, DateTime.Now);
                if (intCount + allPointsToday > maxPointsPerDayForLogging)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = string.Format("We are sorry, you have reached the maximum amount of points you are allowed to log in a single day, regardless of how the points were earned. Please come back and and log them tomorrow.<br><br>");
                    return;
                }

                

                sBadges = pa.AwardPointsToPatron(points, PointAwardReason.Reading,
                     0, 
                    (ActivityType)pc.ActivityTypeId,intCount,txtAuthor.Text.Trim(), txtTitle.Text.Trim(), Review.Text.Trim());
            }
            #endregion

            #region Event Attendance
            // Logging event attendance
            if (txtCode.Length > 0)
            {
                // verify event code was not previously redeemed
                if (PatronPoints.HasRedeemedKeywordPoints(PID, txtCode))
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "This code has already been redeemend for your account.<br><br>";
                    return;
                }

                // get event for that code, get the # points
                var ds = Event.GetEventByEventCode(pa.pgm.StartDate.ToShortDateString(),
                                                   DateTime.Now.ToShortDateString(), txtCode);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "This code is not valid.<br><br>";
                    return;
                }
                var EID = (int) ds.Tables[0].Rows[0]["EID"];
                var evt = Event.GetEvent(EID);
                points = evt.NumberPoints;
                //var newPBID = 0;

                sBadges = pa.AwardPointsToPatron(points, PointAwardReason.EventAttendance, eventCode: txtCode, eventID: EID);
                //if (evt.BadgeID != 0)
                //{
                //    sBadges = pa.AwardPointsToPatron(points, PointAwardReason.EventAttendance,
                //                                     eventCode: txtCode, eventID: EID);
                //}
            }
            #endregion

            if (sBadges != "")
            {
                Session["GoToUrl"] = GoToUrl;
                Response.Redirect("~/BadgeAward.aspx?b=" + sBadges);
            }
        
            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = string.Format("Your reading activity has been logged.<br>You have earned {0} points.<br><br>", points);

            if (Review.Text != "" && Session["LastPRID"] != null && SRPSettings.GetSettingValue("FBReviewOn").SafeToBool())
            {
                var FBID = ConfigurationManager.AppSettings["FBAPPID"] ?? "121002584737306";
                var strBuilderJS =
                    "<script>    (function (d, s, id) { var js, fjs = d.getElementsByTagName(s)[0]; if (d.getElementById(id)) return; js = d.createElement(s); js.id = id; " +
                    "js.src = \"//connect.facebook.net/en_US/all.js#xfbml=1&appId=" + FBID + "\"; fjs.parentNode.insertBefore(js, fjs); } (document, 'script', 'facebook-jssdk'));</script>";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "fb", strBuilderJS.ToString(), false);

                var fbButton =
                    string.Format("<div class=\"fb-share-button\" data-href='{0}://{1}{2}/ShareReview.aspx?ID={3}' data-type=\"button\"></div>",
                                  Request.Url.Scheme, Request.Url.Authority, Request.ApplicationPath.TrimEnd('/'), (int) Session["LastPRID"]);
                lblMessage.Text = string.Format("{0}<br>You may also want to share your book review on facebook. <br>{1}<br><br>", lblMessage.Text, fbButton);
                lblFB.Text = @"Y";
            }
            else
            {
                lblFB.Text = "";
            }


            txtAuthor.Text = txtTitle.Text = txtCountSubmitted.Text = Review.Text = txtProgramCode.Text = "";
            btnSubmit.Visible = false;
            btnReSubmit.Visible = true;
            EntryTable.Visible = false;
            //return;))

            var c = ((BaseSRPPage)Page).FindControlRecursive(Page, "MyPointsControl1");
            if (c!=null)
            {
                ((MyPointsControl)c).LoadData();
            }

            c = ((BaseSRPPage)Page).FindControlRecursive(Page, "LeaderBoardControl1");
            if (c != null)
            {
                ((LeaderBoardControl)c).LoadData();
            }
            
            if (!StayOnPage) Response.Redirect(GoToUrl);


            

        }

        protected void btnReSubmit_Click(object sender, EventArgs e)
        {
            // need to reload page  on AJAX refresh because Facebook posting wont work ...
            if (lblFB.Text != "") Response.Redirect(HttpContext.Current.Request.Url.AbsolutePath);
            
            // but if no FB button, then reuse page thru AJAX 
            btnSubmit.Visible = true;
            btnReSubmit.Visible = false;
            EntryTable.Visible = true;
            lblMessage.Text = "";
            
            
        }

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            Session["ActHistPID"] = lblPID.Text;
            Response.Redirect("~/ActivityHistory.aspx");
        }

        public string GoToUrl
        {
            get
            {
                if (ViewState["gotourl"]==null || ViewState["gotourl"].ToString().Length == 0 )
                {
                    ViewState["gotourl"] = "~/Dashboard.aspx";
                }
                return ViewState["gotourl"].ToString();
            }
            set { ViewState["gotourl"] = value; }
        }

        public bool StayOnPage
        {
            get
            {
                if (ViewState["StayOnPage"] == null)
                {
                    ViewState["StayOnPage"] = true;// false;
                }
                return (bool)ViewState["StayOnPage"];
            }
            set { ViewState["StayOnPage"] = value; }
        }
    }
}