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

            var thisPage = (BaseSRPPage)Page;
            if(thisPage.IsSecure && !thisPage.IsLoggedIn)
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
                lnkRegister.Visible = true;
                lnkLogin.Visible = true;
                lnkLogout.Visible = false;
                n.Visible = b.Visible = v.Visible = o.Visible = a.Visible = p.Visible = f.Visible = false;
                slogan.Visible = true;

                if(thisPage.IsLoggedIn) {
                    lnkRegister.Visible = false;
                    lnkLogin.Visible = false;
                    lnkLogout.Visible = true;
                    slogan.Visible = false;
                    n.Visible = b.Visible = v.Visible = o.Visible = r.Visible = a.Visible = p.Visible = true;
                    homeLink.HRef = "/MyProgram.aspx";
                    //f.Visible = ((Patron) Session["Patron"]).IsMasterAccount;
                    if(Session["IsMasterAcct"] as bool? == true) {
                        a.Title = "My Family";
                    }
                    var unread = Notifications.GetAllUnreadToPatron(((Patron)Session["Patron"]).PID).Tables[0].Rows.Count.ToString();
                    if(!string.IsNullOrEmpty(unread) && unread != "0") {
                        unreadBadge.InnerText = unread;
                        unreadBadge.Visible = true;
                        nIcon.Attributes["class"] = nIcon.Attributes["class"] + " text-primary";
                    }
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

