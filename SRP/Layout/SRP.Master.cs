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
        public BaseSRPPage CurrentPage { get; set; }
        public string Unread { get; set; }
        public string SystemNameText {
            get {
                if(SRPPage != null) {
                    return SRPPage.GetResourceString("system-name");
                } else {
                    return string.Empty;
                }
            }
        }

        public string SloganText {
            get {
                if(SRPPage != null) {
                    return SRPPage.GetResourceString("slogan");
                } else {
                    return string.Empty;
                }
            }
        }

        public string UpsellText {
            get {
                if(SRPPage != null) {
                    return SRPPage.GetResourceString("upsell");
                } else {
                    return string.Empty;
                }
            }
        }

        public string CopyrightStatementText {
            get {
                if(SRPPage != null) {
                    return SRPPage.GetResourceString("footer-copyright");
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

        public string OffersPageActive {
            get {
                if(Request.Path.Contains("/Offers/")) {
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

        public string BadgesSectionActive {
            get {
                if(Request.Path.Contains("/Badges/")) {
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

        public string EarnedBadges {
            get; set;
        }

        public bool ShowLoginPopup { get; set; }
        public string LoginPopupErrorMessage { get; set; }

        public string BasePath { get; set; }

        protected void Page_PreRender(object sender, EventArgs e) {
            object patronMessage = Session[SessionKey.PatronMessage];

            if(patronMessage != null) {
                object patronMessageLevel = Session[SessionKey.PatronMessageLevel];
                string alertLevel = "alert-success";
                if(patronMessageLevel != null) {
                    alertLevel = string.Format("alert-{0}", patronMessageLevel.ToString());
                    Session.Remove(SessionKey.PatronMessageLevel);
                }
                alertContainer.CssClass = string.Format("alert {0}",
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

            var earnedBadges = Session[SessionKey.EarnedBadges];
            if(earnedBadges != null) {
                this.EarnedBadges = earnedBadges.ToString().Replace('|', ',');
                Session.Remove(SessionKey.EarnedBadges);
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            base.PageLoad(sender, e);

            if(string.IsNullOrEmpty(Page.Title) && !string.IsNullOrEmpty(this.SystemNameText)) {
                Page.Title = this.SystemNameText;
            }

            Control ctl = LoadControl("~/Controls/ProgramCSS.ascx");
            var plc = FindControl("ProgramCSS");
            plc.Controls.Add(ctl);

            this.CurrentPage = (BaseSRPPage)Page;
            if(this.CurrentPage.IsSecure && !this.CurrentPage.IsLoggedIn) {
                Response.Redirect("~/Logout.aspx");
            }

            if(Session[SessionKey.AdventuresActive] == null) {
                var programGames = DAL.ProgramGame.GetAll();
                if(programGames.Tables.Count > 0 && programGames.Tables[0].Rows.Count > 0) {
                    Session[SessionKey.AdventuresActive] = true;
                } else {
                    Session[SessionKey.AdventuresActive] = false;
                }
            }
            adventuresNav.Visible = Session[SessionKey.AdventuresActive] as bool? == true;
            adventuresNav.Attributes.Add("class", this.AdventuresSectionActive);

            if(Session[SessionKey.ChallengesActive] == null) {
                var challenges = DAL.BookList.GetAll();
                if(challenges.Tables.Count > 0 && challenges.Tables[0].Rows.Count > 0) {
                    Session[SessionKey.ChallengesActive] = true;
                } else {
                    Session[SessionKey.ChallengesActive] = false;
                }
            }
            challengesNav.Visible = Session[SessionKey.ChallengesActive] as bool? == true;
            challengesNav.Attributes.Add("class", this.ChallengesSectionActive);

            if(Session[SessionKey.OffersActive] == null) {
                var offers = DAL.Offer.GetAll();
                if(offers.Tables.Count > 0 && offers.Tables[0].Rows.Count > 0) {
                    Session[SessionKey.OffersActive] = true;
                } else {
                    Session[SessionKey.OffersActive] = false;
                }
            }
            offersNav.Visible = Session[SessionKey.OffersActive] as bool? == true;
            offersNav.Attributes.Add("class", this.OffersPageActive);

            if(Session[SessionKey.BadgesActive] == null) {
                var badges = DAL.Badge.GetAll();
                if(badges.Tables.Count > 0 && badges.Tables[0].Rows.Count > 0) {
                    Session[SessionKey.BadgesActive] = true;
                } else {
                    Session[SessionKey.BadgesActive] = false;
                }
            }
            badgesNav.Visible = Session[SessionKey.BadgesActive] as bool? == true;
            badgesAnonNav.Visible = Session[SessionKey.BadgesActive] as bool? == true;
            badgesNav.Attributes.Add("class", this.BadgesSectionActive);
            badgesAnonNav.Attributes.Add("class", this.BadgesSectionActive);

            if(Session[SessionKey.EventsActive] == null) {
                var events = DAL.Event.GetAll();
                if(events.Tables.Count > 0 && events.Tables[0].Rows.Count > 0) {
                    Session[SessionKey.EventsActive] = true;
                } else {
                    Session[SessionKey.EventsActive] = false;
                }
            }
            eventsNav.Visible = Session[SessionKey.EventsActive] as bool? == true;
            eventsAnonNav.Visible = Session[SessionKey.EventsActive] as bool? == true;
            eventsNav.Attributes.Add("class", this.EventsSectionActive);
            eventsAnonNav.Attributes.Add("class", this.EventsSectionActive);


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
                        ViewState[SessionKey.RequestedPath] = Session[SessionKey.RequestedPath];
                        Session.Remove(SessionKey.RequestedPath);
                    }
                    if(Request.Cookies[CookieKey.Username] != null) {
                        loginPopupUsername.Text = Request.Cookies[CookieKey.Username].Value;
                        loginPopupRememberMe.Checked = true;
                    }
                }
            }
        }

        protected void loginPopupClick(object sender, EventArgs e) {
            if(!(string.IsNullOrEmpty(loginPopupUsername.Text.Trim()) 
               || string.IsNullOrEmpty(loginPopupPassword.Text.Trim()))) {
                var patron = new Patron();
                if(Patron.Login(loginPopupUsername.Text.Trim(), loginPopupPassword.Text)) {
                    var bp = Patron.GetObjectByUsername(loginPopupUsername.Text.Trim());

                    var pgm = DAL.Programs.FetchObject(bp.ProgID);
                    if(pgm == null) {
                        int schoolGrade;
                        int.TryParse(bp.SchoolGrade, out schoolGrade);
                        var progID = Programs.GetDefaultProgramForAgeAndGrade(bp.Age, schoolGrade);
                        bp.ProgID = progID;
                        bp.Update();
                    }
                    new PatronSession(Session).Establish(bp);

                    TestingBL.CheckPatronNeedsPreTest();
                    TestingBL.CheckPatronNeedsPostTest();

                    if(loginPopupRememberMe.Checked) {
                        var loginUsernameCookie = new HttpCookie(CookieKey.Username);
                        loginUsernameCookie.Expires = DateTime.Now.AddDays(14);
                        loginUsernameCookie.Value = loginPopupUsername.Text.Trim();
                        Response.SetCookie(loginUsernameCookie);
                    } else {
                        if(Request.Cookies[CookieKey.Username] != null) {
                            Response.Cookies[CookieKey.Username].Expires = DateTime.Now.AddDays(-1);
                        }
                    }

                    if(Session[SessionKey.RequestedPath] != null) {
                        string requestedPath = Session[SessionKey.RequestedPath].ToString();
                        Session.Remove(SessionKey.RequestedPath);
                        Response.Redirect(requestedPath);
                    } else if(ViewState[SessionKey.RequestedPath] != null) {
                        string requestedPath = ViewState[SessionKey.RequestedPath].ToString();
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

