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

namespace GRA.SRP {
    public partial class SRPMaster : BaseSRPMaster {
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
                object patronMessageGlyph = Session[SessionKey.PatronMessageGlyphicon];
                if(patronMessageGlyph != null) {
                    alertGlyphicon.CssClass = string.Format("glyphicon glyphicon-{0} margin-1em-right",
                                                            patronMessageGlyph);
                    Session.Remove(SessionKey.PatronMessageGlyphicon);
                }
                alertMessage.Text = patronMessage.ToString();
                alertContainer.Visible = true;
                Session.Remove(SessionKey.PatronMessage);
            } else {
                alertContainer.Visible = false;
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
            if(this.CurrentPage.IsSecure && !this.CurrentPage.IsLoggedIn)
                Response.Redirect("~/Logout.aspx");

            if(!IsPostBack) {
                if(this.CurrentPage.IsLoggedIn) {
                    homeLink.HRef = "~/Dashboard.aspx";
                    //f.Visible = ((Patron) Session["Patron"]).IsMasterAccount;
                    if(Session["IsMasterAcct"] as bool? == true) {
                        a.Title = "My Family";
                    }
                    this.Unread = Notifications.GetAllUnreadToPatron(((Patron)Session["Patron"]).PID).Tables[0].Rows.Count.ToString();
                    if(!(Page is AddlSurvey || Page is Register || Page is Login || Page is Logout || Page is Recover)) {
                        if(Session["PreTestMandatory"] != null && (bool)Session["PreTestMandatory"]) {
                            TestingBL.PatronNeedsPreTest();
                        }
                    }
                }

            }

        }
    }
}

