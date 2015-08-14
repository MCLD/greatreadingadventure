using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.ControlRoom.Modules.Reports
{
    public partial class Default : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4200;
            MasterPage.IsSecure = true; 
            
            if (!IsPostBack)
            {
               MasterPage.PageTitle = string.Format("{0}", "Reports Home");

               SetPageRibbon(StandardModuleRibbons.ReportsRibbon());

            }
        }
    }
}
