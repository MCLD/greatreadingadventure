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
    public partial class PatronNotificationsCtl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                var ds = DAL.Notifications.GetAllToPatron(((Patron) Session["Patron"]).PID);
                rptr.DataSource = ds;
                rptr.DataBind();
            }
        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            pnlList.Visible = false;


            var o = new Notifications();
            o.Fetch(int.Parse(e.CommandArgument.ToString()));

            lblTitle.Text = o.Subject;
            lblBody.Text = o.Body;
            NID.Text = o.NID.ToString();
            lblReceived.Text = o.AddedDate.ToString();

            o.isUnread = false;
            o.Update();

            pnlDetail.Visible = true;
        }

        protected void btnList_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MyNotifications.aspx");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var o = Notifications.FetchObject(int.Parse(NID.Text));

            o.Delete();
            Response.Redirect("~/MyNotifications.aspx");

        }

        protected void btnAsk_Click(object sender, EventArgs e)
        {
            pnlList.Visible = pnlDetail.Visible = pnlDone.Visible = false;
            pnlAsk.Visible = true;

        }

        protected void btnUnread_Click(object sender, EventArgs e)
        {
            var o = Notifications.FetchObject(int.Parse(NID.Text));
            o.isUnread = true;
            o.Update();
            Response.Redirect("~/MyNotifications.aspx");

        }

        protected void btnAskSubmit_Click(object sender, EventArgs e)
        {
            var o = new Notifications();
            o.PID_From = ((Patron)Session["Patron"]).PID;
            o.PID_To = 0;
            o.isQuestion = true;
            o.Subject = txtSubject.Text;
            o.Body = txtBody.Text;
            o.AddedDate = o.LastModDate = DateTime.Now;
            o.LastModUser = o.AddedUser = ((Patron)Session["Patron"]).Username;
            o.Insert();
            txtSubject.Text = txtBody.Text = "";

            pnlAsk.Visible = false;
            pnlDone.Visible = true;
        }
    }
}