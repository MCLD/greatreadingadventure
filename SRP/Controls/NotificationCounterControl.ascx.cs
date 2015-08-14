using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;

namespace GRA.SRP.Controls
{
    public partial class NotificationCounterControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Patron"] == null) Response.Redirect("/");
            // Get the DAL for notifications and get the count for this use
            Count1.Text = Count2.Text = Notifications.GetAllUnreadToPatron(((Patron)Session["Patron"]).PID).Tables[0].Rows.Count.ToString();
        }
    }
}