using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using System.Data;

namespace GRA.SRP.Controls {
    public partial class Offers : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {

            if(!IsPostBack) {
                var ds = DAL.Offer.GetForDisplay(((Patron)Session["Patron"]).PID);
                rptr.DataSource = ds;
                rptr.DataBind();
            }
        }

        protected void rptr_ItemBound(object source, RepeaterItemEventArgs e) {
            if(e.Item.ItemType == ListItemType.Item
               || e.Item.ItemType == ListItemType.AlternatingItem) {
                var dataRow = e.Item.DataItem as DataRowView;
                if(dataRow != null) {
                    var eventDetailsLink = e.Item.FindControl("eventDetailsLink") as LinkButton;
                    if(eventDetailsLink == null) {
                        return;
                    }
                    if((bool)dataRow["ExternalRedirectFlag"]) {
                        // redirection
                        eventDetailsLink.CommandName = "Redirect";
                        eventDetailsLink.Attributes.Add("target", "_blank");
                        eventDetailsLink.Attributes.Add("href", dataRow["RedirectURL"].ToString());
                    } else {
                        // details display
                        eventDetailsLink.CommandName = "Details";
                        eventDetailsLink.CommandArgument = dataRow["OID"].ToString();
                    }
                } else {
                    return;
                }
            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e) {

            if(e.CommandName.Equals("Details", StringComparison.OrdinalIgnoreCase)) {
                pnlList.Visible = false;
                var o = new Offer();
                o.Fetch(int.Parse(e.CommandArgument.ToString()));
                lblTitle.Text = o.Title;
                o.TotalImpressions = o.TotalImpressions + 1;
                lblSerial.Text = o.SerialPrefix + o.TotalImpressions.ToString();
                Coupon.ImageUrl = string.Format("~/images/Offers/{0}.png", o.OID);
                o.Update();
                pnlDetail.Visible = true;
            }
        }

        protected void btnList_Click(object sender, EventArgs e) {
            Response.Redirect("~/Offers/");
        }
    }
}
 