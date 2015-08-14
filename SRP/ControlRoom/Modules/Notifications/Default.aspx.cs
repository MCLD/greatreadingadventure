using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom.Modules.Notifications
{
    public partial class Default : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5000;
            MasterPage.IsSecure = true;

            if (Session["Curr_Notification_ID"] != null && Session["Curr_Notification_ID"].ToString() != "")
            {
                string editpage = "~/ControlRoom/Modules/Notifications/QuestionView.aspx";

                int key = Convert.ToInt32(Session["Curr_Notification_ID"]);

                var n = DAL.Notifications.FetchObject(key);
                if (n!= null)
                {
                    var pid = n.PID_From;


                    Session["CURR_PATRON_ID"] = pid;
                    Session["CURR_PATRON"] = Patron.FetchObject(pid);
                    Session["CURR_PATRON_MODE"] = "EDIT";

                    Response.Redirect(String.Format("{0}?PK={1}", editpage, Session["Curr_Notification_ID"].ToString()));
                }
                

            }

            Response.Redirect("NotificationList.aspx");
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.NotificationsRibbon());

            }
        }
    }
}

