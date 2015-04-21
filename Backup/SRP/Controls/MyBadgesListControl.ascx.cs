using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.DAL;

namespace STG.SRP.Controls
{
    public partial class MyBadgesListControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                var ds = DAL.PatronBadges.GetAll(((Patron)Session["Patron"]).PID);
                rptr.DataSource = ds;
                rptr.DataBind();

                NoBadges.Visible = (ds.Tables[0].Rows.Count == 0);
            }

        }
    }
}