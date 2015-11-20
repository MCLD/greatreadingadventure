using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.Tools;

namespace GRA.SRP.Controls {
    public partial class Badges : System.Web.UI.UserControl {

        protected void Page_Load(object sender, EventArgs e) {
            if(!Page.IsPostBack) {
                var ds = DAL.PatronBadges.GetAll(((Patron)Session["Patron"]).PID);
                rptr.DataSource = ds;
                rptr.DataBind();
                NoBadges.Visible = (ds.Tables[0].Rows.Count == 0);
            }
        }

        protected void btnList_Click(object sender, EventArgs e) {
            Response.Redirect("~/Badges/MyBadges.aspx");
        }
    }
}