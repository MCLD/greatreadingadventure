using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;

namespace STG.SRP.Controls
{
    public partial class Offers : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                var ds = DAL.Offer.GetForDisplay(((Patron) Session["Patron"]).PID);
                rptr.DataSource = ds;
                rptr.DataBind();
            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            pnlList.Visible = false;


            var o = new Offer();
            o.Fetch(int.Parse(e.CommandArgument.ToString()));

            lblTitle.Text = o.Title;
            o.TotalImpressions = o.TotalImpressions + 1;
            lblSerial.Text = o.SerialPrefix + o.TotalImpressions.ToString();

            Coupon.ImageUrl = WebHelper.GetAppURL("/images/Offers/" + o.OID + ".png").Replace("///", "/");// +DateTime.Now.ToString();
            o.Update();
            pnlDetail.Visible = true;
        }

        protected void btnList_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MyOffers.aspx");
        }
    }
}