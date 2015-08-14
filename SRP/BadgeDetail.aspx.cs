using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP
{
    public partial class BadgeDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["BID"] == "")
            {
                pnlNotFound.Visible = true;
                pnlBadge.Visible = false;
            }
            else
            {
                int bid = 0;
                int.TryParse(Request["BID"], out bid);
                blbBID.Text = bid.ToString();
                if (bid > 0)
                {
                    pnlNotFound.Visible = false;
                    pnlBadge.Visible = true;
                    if (!IsPostBack) LoadBadgeInfo(bid);                    
                }
                else
                {
                    pnlNotFound.Visible = true;
                    pnlBadge.Visible = false;
                }
            }
        }

        private void LoadBadgeInfo(int bid)
        {
            var b = DAL.Badge.FetchObject(bid);
            if (b == null) return;

            lblName.Text = b.UserName;
            Image1.ImageUrl = string.Format("~/Images/Badges/{0}.png", bid);

            var p = DAL.Badge.GetEnrollmentPrograms(bid);
            if (p.Length > 0)
            {
                lblEarn.Text = string.Format("{1}<li>Enrolling in a one of these programs: {0}.</li>", p, lblEarn.Text.Length > 0 ? lblEarn.Text + "<br>" : "");
            }

            p = DAL.Badge.GetBadgeBookLists(bid);
            if (p.Length > 0)
            {
                lblEarn.Text = string.Format("{1}<li>Reading one of the many book lists compiled by our librarians.</li>", p, lblEarn.Text.Length > 0 ? lblEarn.Text + "<br>" : "");
            }

            p = DAL.Badge.GetBadgeReading(bid);
            if (p.Length > 0)
            {
                lblEarn.Text = string.Format("{1}<li>Logging your reading and earning points.</li>", p, lblEarn.Text.Length > 0 ? lblEarn.Text + "<br>" : "");
            }

            p = DAL.Badge.GetBadgeGames(bid);
            if (p.Length > 0)
            {
                lblEarn.Text = string.Format("{1}<li>Playing one of these minigames that become available as you earn program points: {0}.</li>", p, lblEarn.Text.Length > 0 ? lblEarn.Text + "<br>" : "");
            }

            p = DAL.Badge.GetBadgeEvents(bid);
            if (p.Length > 0)
            {
                lblEarn.Text = string.Format("{1}<li>Attending one of these events: <a id=\"LinkButton1\" href=\"javascript:__doPostBack(&#39;LinkButton1&#39;,&#39;&#39;)\">{0}</a>.</li>", p, lblEarn.Text.Length > 0 ? lblEarn.Text + "<br>" : "");
            }

        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {

            var ids = DAL.Badge.GetBadgeEventIDs(int.Parse(blbBID.Text));
            var ds = DAL.Event.GetEventList(ids);
            rptr.DataSource = ds;
            rptr.DataBind();
            pnlEvents.Visible = true;
            pnlEarn.Visible = false;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            pnlEvents.Visible = false;
            pnlEarn.Visible = true;
        }
    }
}