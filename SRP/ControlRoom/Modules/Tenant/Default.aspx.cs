
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.ControlRoom.Modules.Tenant
{
    public partial class Default : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 8000;
            MasterPage.IsSecure = true; 
            
            if ((bool)CRIsMasterTenant)
            {
                Response.Redirect("TenantList.aspx");
            }
            else
            {
                Response.Redirect("MyTenantAccount.aspx");
            }
            
        }
    }
}
