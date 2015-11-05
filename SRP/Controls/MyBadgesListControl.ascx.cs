using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;

namespace GRA.SRP.Controls
{
    public partial class MyBadgesListControl : System.Web.UI.UserControl
    {

        protected string BadgeClass { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                var ds = DAL.PatronBadges.GetAll(((Patron)Session["Patron"]).PID);
                rptr.DataSource = ds;
                rptr.DataBind();

                NoBadges.Visible = (ds.Tables[0].Rows.Count == 0);
                if(ds.Tables[0].Rows.Count == 1) {
                    this.BadgeClass = "col-xs-6 col-xs-offset-3 col-md-4 col-md-offset-4";
                } else if(ds.Tables[0].Rows.Count == 2) {
                    this.BadgeClass = "col-xs-6 col-md-6";
                } else {
                    this.BadgeClass = "col-xs-6 col-md-4";
                }
            }

        }
    }
}