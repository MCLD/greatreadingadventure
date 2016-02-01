using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;
using GRA.Tools;
using System.Data;
using System.Web.UI.HtmlControls;

namespace GRA.SRP.Controls {
    public partial class MyBadgesListControl : System.Web.UI.UserControl {

        protected string BadgeClass { get; set; }

        protected void Page_PreRender(object sender, EventArgs e) {
            if(Session[SessionKey.RefreshBadgeList] != null) {
                RenderBadges();
                new SessionTools(Session).ClearRefreshBadgeList();
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            int badgeCount = 0;

            if(!IsPostBack) {
                RenderBadges();
            } else {
                var badgeCountObj = ViewState["BadgeCount"];
                if(!(badgeCountObj != null
                     && int.TryParse(badgeCountObj.ToString(), out badgeCount))) {
                    // if we don't have a count, format for 3
                    badgeCount = 3;
                }
                FixBootstrapClasses(badgeCount);
            }
        }

        protected void RenderBadges() {
            var ds = DAL.PatronBadges.GetTop(((Patron)Session["Patron"]).PID, 6);
            rptr.DataSource = ds;
            rptr.DataBind();
            var badgeCount = ds.Tables[0].Rows.Count;
            FixBootstrapClasses(badgeCount);
            ViewState["BadgeCount"] = badgeCount;
        }

        protected void FixBootstrapClasses(int badgeCount) {
            NoBadges.Visible = (badgeCount == 0);
            if(badgeCount == 1) {
                this.BadgeClass = "col-xs-6 col-xs-offset-3 col-md-4 col-md-offset-4";
            } else if(badgeCount == 2) {
                this.BadgeClass = "col-xs-6 col-md-6";
            } else {
                this.BadgeClass = "col-xs-6 col-md-4";
            }
        }
    }
}