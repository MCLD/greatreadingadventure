using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

namespace GRA.SRP.Controls
{
    public partial class Badges : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                var ds = DAL.PatronBadges.GetAll(((Patron) Session["Patron"]).PID);
                rptr.DataSource = ds;
                rptr.DataBind();
                NoBadges.Visible = (ds.Tables[0].Rows.Count == 0);
            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            pnlList.Visible = false;


            var o = new PatronBadges();
            o.Fetch(int.Parse(e.CommandArgument.ToString()));

            var b = new DAL.Badge();
            b.Fetch(o.BadgeID);

            lblTitle.Text = b.UserName;



            Badge.ImageUrl = WebHelper.GetAppURL("/images/Badges/" + o.BadgeID + ".png").Replace("///", "/");// +DateTime.Now.ToString();
            o.Update();
            pnlDetail.Visible = true;
        }

        protected void btnList_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MyBadges.aspx");
        }
    }
}