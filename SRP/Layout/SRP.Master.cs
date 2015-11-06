using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Controls;
using GRA.SRP.DAL;
using GRA.Tools;
using System.Text;

namespace GRA.SRP {
    public partial class SRPMaster : BaseSRPMaster {
        private const string NoBadgePath = "~/images/Badges/no_badge.png";

        public BaseSRPPage CurrentPage { get; set; }
        public string Unread { get; set; }
        public string SystemNameText {
            get {
                if(SRPPage != null) {
                    return SRPPage.GetResourceString("System_Name");
                } else {
                    return string.Empty;
                }
            }
        }

        public string SloganText {
            get {
                if(SRPPage != null) {
                    return SRPPage.GetResourceString("Slogan");
                } else {
                    return string.Empty;
                }
            }
        }

        public string UpsellText {
            get {
                if(SRPPage != null) {
                    return SRPPage.GetResourceString("Upsell");
                } else {
                    return string.Empty;
                }
            }
        }

        public string CopyrightStatementText {
            get {
                if(SRPPage != null) {
                    return SRPPage.GetResourceString("Copyright_Statement");
                } else {
                    return string.Empty;
                }
            }
        }

        public string RegisterPageActive {
            get {
                if(Request.Path.EndsWith("Register.aspx")) {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string LoginPageActive {
            get {
                if(Request.Path.EndsWith("Login.aspx")) {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string DashboardPageActive {
            get {
                if(Request.Path.EndsWith("Dashboard.aspx")) {
                    return "active";
                }
                return string.Empty;
            }
        }


        public string MailSectionActive {
            get {
                if(Request.Path.Contains("/Mail/")) {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string AdventuresSectionActive {
            get {
                if(Request.Path.Contains("/Adventures/")) {
                    return "active";
                }
                return string.Empty;
            }
        }
        public string ChallengesSectionActive {
            get {
                if(Request.Path.Contains("/Challenges/")) {
                    return "active";
                }
                return string.Empty;
            }
        }
        public string EventsSectionActive {
            get {
                if(Request.Path.Contains("/Events/")) {
                    return "active";
                }
                return string.Empty;
            }
        }

        public string AccountSectionActive {
            get {
                if(Request.Path.Contains("/Account/")) {
                    return "active";
                }
                return string.Empty;
            }
        }

        public bool ShowBadgePopup { get; set; }
        public bool ShowLoginPopup { get; set; }
        public string LoginPopupErrorMessage { get; set; }
        protected void Page_PreRender(object sender, EventArgs e) {
            object patronMessage = Session[SessionKey.PatronMessage];

            if(patronMessage != null) {
                object patronMessageLevel = Session[SessionKey.PatronMessageLevel];
                string alertLevel = "alert-success";
                if(patronMessageLevel != null) {
                    alertLevel = string.Format("alert-{0}", patronMessageLevel.ToString());
                    Session.Remove(SessionKey.PatronMessageLevel);
                }
                alertContainer.CssClass = string.Format("{0} {1}",
                                                        alertContainer.CssClass,
                                                        alertLevel);
                alertGlyphicon.Visible = false;
                object patronMessageGlyph = Session[SessionKey.PatronMessageGlyphicon];
                if(patronMessageGlyph != null) {
                    alertGlyphicon.Visible = true;
                    alertGlyphicon.CssClass = string.Format("glyphicon glyphicon-{0} margin-halfem-right",
                                                            patronMessageGlyph);
                    Session.Remove(SessionKey.PatronMessageGlyphicon);
                }
                alertMessage.Text = patronMessage.ToString();
                alertContainer.Visible = true;
                Session.Remove(SessionKey.PatronMessage);
            } else {
                alertContainer.Visible = false;
            }

            var displayBadge = Session[SessionKey.DisplayBadge];
            if(displayBadge != null) {
                int badgeId = 0;
                if(int.TryParse(displayBadge.ToString(), out badgeId)) {
                    var badge = DAL.Badge.FetchObject(badgeId);
                    if(badge != null) {
                        this.ShowBadgePopup = true;
                        badgePopupPanel.Visible = true;
                        Session.Remove(SessionKey.DisplayBadge);
                        badgePopupTitle.Text = badge.UserName;

                        string badgePath = NoBadgePath;
                        string potentialBadgePath = string.Format("~/Images/Badges/{0}.png",
                                                                  badgeId);
                        if(System.IO.File.Exists(Server.MapPath(potentialBadgePath))) {
                            badgePath = potentialBadgePath;
                        }

                        badgePopupImage.ImageUrl = badgePath;
                        badgePopupImage.AlternateText = string.Format("{0} Badge", badge.UserName);

                        StringBuilder earn = new StringBuilder();

                        string earnText = DAL.Badge.GetBadgeReading(badgeId);
                        if(earnText.Length > 0) {
                            earn.AppendFormat("<li>Earn points by reading: {0}.</li>", earnText);
                        }

                        earnText = DAL.Badge.GetEnrollmentPrograms(badgeId);
                        if(earnText.Length > 0) {
                            earn.AppendFormat("<li>Enroll in a reading program: {0}</li>", earnText);
                        }

                        earnText = DAL.Badge.GetBadgeBookLists(badgeId);
                        if(earnText.Length > 0) {
                            earn.AppendFormat("<li>Complete a Challenge: {0}</li>", earnText);
                        }

                        earnText = DAL.Badge.GetBadgeGames(badgeId);
                        if(earnText.Length > 0) {
                            earn.AppendFormat("<li>Unlock and complete an Adventure: {0}</li>", earnText);
                        }

                        earnText = DAL.Badge.GetBadgeEvents(badgeId);
                        if(earnText.Length > 0) {
                            earn.AppendFormat("<li>Attend an Event: {0}</li>", earnText);
                        }

                        if(earn.Length > 0) {
                            badgePopupEarn.Visible = true;
                            badgePopupEarnText.Text = earn.ToString();
                        } else {
                            badgePopupEarn.Visible = false;
                        }
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            base.PageLoad(sender, e);

            //var systemName = SRPSettings.GetSettingValue("SystemName");
            //PageTitle = (string.IsNullOrEmpty(systemName) ? "Summer Reading Program" : systemName);

            if(string.IsNullOrEmpty(Page.Title) && !string.IsNullOrEmpty(PageTitle)) {
                Page.Title = PageTitle;
            }

            Control ctl = LoadControl("~/Controls/ProgramCSS.ascx");
            var plc = FindControl("ProgramCSS");
            plc.Controls.Add(ctl);

            this.CurrentPage = (BaseSRPPage)Page;
            if(this.CurrentPage.IsSecure && !this.CurrentPage.IsLoggedIn) {
                Response.Redirect("~/Logout.aspx");
            }

            if(!IsPostBack) {
                if(this.CurrentPage.IsLoggedIn) {
                    homeLink.HRef = "~/Dashboard.aspx";
                    //f.Visible = ((Patron) Session["Patron"]).IsMasterAccount;
                    if(Session[SessionKey.IsMasterAccount] as bool? == true) {
                        a.Title = "My Family";
                    }
                    this.Unread = Notifications.GetAllUnreadToPatron(((Patron)Session["Patron"]).PID).Tables[0].Rows.Count.ToString();
                    if(!(Page is AddlSurvey || Page is Register || Page is Login || Page is Logout || Page is Recover)) {
                        if(Session["PreTestMandatory"] != null && (bool)Session["PreTestMandatory"]) {
                            TestingBL.PatronNeedsPreTest();
                        }
                    }
                } else {
                    this.loginPopupPanel.Visible = true;
                    if(Session[SessionKey.RequestedPath] != null) {
                        this.ShowLoginPopup = true;
                    }
                }
            }
        }

        protected void loginPopupClick(object sender, EventArgs e) {
            if(!(string.IsNullOrEmpty(loginPopupUsername.Text.Trim()) || string.IsNullOrEmpty(loginPopupPassword.Text.Trim()))) {
                var patron = new Patron();
                if(Patron.Login(loginPopupUsername.Text.Trim(), loginPopupPassword.Text.Trim())) {
                    var bp = Patron.GetObjectByUsername(loginPopupUsername.Text.Trim());

                    var pgm = DAL.Programs.FetchObject(bp.ProgID);
                    if(pgm == null) {
                        int schoolGrade;
                        int.TryParse(bp.SchoolGrade, out schoolGrade);
                        var progID = Programs.GetDefaultProgramForAgeAndGrade(bp.Age, schoolGrade); //Programs.FetchObject(Programs.GetDefaultProgramID());
                        bp.ProgID = progID;
                        bp.Update();
                    }
                    new PatronSession(Session).Establish(bp);

                    TestingBL.CheckPatronNeedsPreTest();
                    TestingBL.CheckPatronNeedsPostTest();

                    if(Session[SessionKey.RequestedPath] != null) {
                        string requestedPath = Session[SessionKey.RequestedPath].ToString();
                        Session.Remove(SessionKey.RequestedPath);
                        Response.Redirect(requestedPath);
                    } else {
                        Response.Redirect("~/Dashboard.aspx");
                    }
                } else {
                    this.LoginPopupErrorMessage = "Invalid username or password.";
                    Session["PatronLoggedIn"] = false;
                    Session["Patron"] = null;
                }
            }

        }

    }
}

