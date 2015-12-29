using GRA.SRP.DAL;
using GRA.Tools;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace GRA.SRP.Badges {
    public partial class Details : BaseSRPPage {
        private const string NoBadgePath = "~/images/Badges/no_badge.png";

        protected void Page_Load(object sender, EventArgs e) {
            if(!String.IsNullOrEmpty(Request["PID"])) {
                Session["ProgramID"] = Request["PID"].ToString();
            }
            if(!IsPostBack) {
                if(Session["ProgramID"] == null) {
                    try {
                        int PID = Programs.GetDefaultProgramID();
                        Session["ProgramID"] = PID.ToString();
                    } catch {
                        Response.Redirect("~/Badges/");
                    }
                }
            }
            TranslateStrings(this);

            if(Request.UrlReferrer == null) {
                badgeBackLink.NavigateUrl = "~/Badges/";
            } else {
                badgeBackLink.NavigateUrl = Request.UrlReferrer.AbsolutePath;
            }

            Badge badge = null;
            int badgeId = 0;
            string displayBadge = Request.QueryString["BadgeId"];
            if(!string.IsNullOrEmpty(displayBadge)
                && int.TryParse(displayBadge.ToString(), out badgeId)) {
                badge = DAL.Badge.FetchObject(badgeId);
                if(badge != null) {
                    badgeTitle.Text = badge.UserName;
                    this.Title = string.Format("'{0}' Badge Details", badgeTitle.Text);
                    this.MetaDescription = string.Format("All about the {0} badge - {1}",
                                                         badgeTitle.Text,
                                                         GetResourceString("system-name"));
                    string badgePath = NoBadgePath;
                    string potentialBadgePath = string.Format("~/Images/Badges/{0}.png",
                                                              badgeId);
                    if(System.IO.File.Exists(Server.MapPath(potentialBadgePath))) {
                        badgePath = potentialBadgePath;
                    }

                    badgeImage.ImageUrl = badgePath;
                    badgeImage.AlternateText = string.Format("{0} Badge", badge.UserName);

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
                        badgeEarnPanel.Visible = true;
                        badgeEarnLabel.Text = earn.ToString();
                    } else {
                        badgeEarnPanel.Visible = false;
                    }
                    badgeDetails.Visible = true;
                }
                badgeDetails.Visible = true;
            }
            if(badge == null) {
                badgeDetails.Visible = false;
                var cph = Page.Master.FindControl("HeaderContent") as ContentPlaceHolder;
                if(cph != null) {
                    cph.Controls.Add(new HtmlMeta {
                        Name = "robots",
                        Content = "noindex"
                    });
                }
                new SessionTools(Session).AlertPatron("Could not find details on that badge.",
                    PatronMessageLevels.Warning,
                    "exclamation-sign");
            }
        }
    }
}