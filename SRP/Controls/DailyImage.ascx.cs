using GRA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls
{
    public partial class DailyImage : System.Web.UI.UserControl
    {
        private const string DailyImagePath = "~/Images/DailyImage/{0}/{1}.jpg";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var patron = Session[SessionKey.Patron] as DAL.Patron;
                if (patron == null)
                {
                    return;
                }

                var program = DAL.Programs.FetchObject(patron.ProgID);
                if (program == null
                    || !program.IsOpen
                    || !program.DisplayDailyImage
                    || DateTime.Now < program.LoggingStart
                    || DateTime.Now > program.LoggingEnd)
                {
                    return;
                }

                string imagePath = string.Format(DailyImagePath,
                    program.PID,
                    (DateTime.Now - program.LoggingStart).Days + 1);

                if (System.IO.File.Exists(Server.MapPath(imagePath)))
                {
                    DailyImageButton.HRef = Page.ResolveUrl(imagePath);
                    DailyImageButton.Visible = true;
                }
                else
                {
                    var sessionTools = new SessionTools(Session);
                    var cached = sessionTools.GetCache(Cache, "LoggedAboutDailyImage");
                    if (cached == null)
                    {
                        this.Log().Warn("Unable to show daily image - file doesn't exist: {0}",
                            Server.MapPath(imagePath));
                        sessionTools.SetCache(Cache, "LoggedAboutDailyImage", true);
                    }
                }
            }
        }
    }
}