using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.ControlRoom
{
    public partial class GenericErrorPage : BaseControlRoomPage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            IControlRoomMaster masterPage = (IControlRoomMaster)Master;
            masterPage.IsSecure = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            IControlRoomMaster masterPage = (IControlRoomMaster)Master;
            //masterPage.PageRibbon.DataBind();
            masterPage.PageTitle = "Application Error";
            masterPage.PageError = "There has been an application error.";
            masterPage.DisplayMessageOnLoad = true;

            Exception ex = (Exception)Session[SessionData.LastException.ToString()];
            if (ex != null)
            {
                uxExceptionMessage.Text = ex.Message;
                if (null != ex.InnerException)
                {
                    uxExceptionMessage.Text += string.Format(" - {0}", ex.InnerException.Message);
                }

                uxStackTrace.Text = ex.StackTrace.Replace("\n", "<br/>");
            }
        }
    }
}