using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.ControlRoom.Modules.Settings
{
    public partial class Default : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MasterPage.IsSecure = true;
                MasterPage.PageTitle = string.Format("{0}", "Global Settings");
                SetPageRibbon(StandardModuleRibbons.SettingsRibbon());

            }
        }
    }
}
