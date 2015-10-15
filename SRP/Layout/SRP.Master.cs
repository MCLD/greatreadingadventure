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

namespace GRA.SRP {
    public partial class SRPMaster : BaseSRPMaster {
        public BaseSRPPage CurrentPage { get; set; }
        public string Unread { get; set; }

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

            object patronMessage = Session["PatronMessage"];

            if(patronMessage != null) {
                object patronMessageLevel = Session["PatronMessageLevel"];
                string alertLevel = "alert-success";
                if(patronMessageLevel != null) {
                    alertLevel = string.Format("alert-{0}", patronMessageLevel.ToString());
                    Session.Remove("PatronMessageLevel");
                }
                alertContainer.CssClass = string.Format("{0} {1}",
                                                        alertContainer.CssClass,
                                                        alertLevel);
                alertMessage.Text = patronMessage.ToString();
                alertContainer.Visible = true;
                Session.Remove("PatronMessage");
            } else {
                alertContainer.Visible = false;
            }

            if(!IsPostBack) {
                if(this.CurrentPage.IsLoggedIn) {
                    homeLink.HRef = "/MyProgram.aspx";
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

