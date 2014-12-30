using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.Core.Utilities;

namespace STG.SRP.ControlRoom
{
    public partial class CRTopMenu : System.Web.UI.UserControl
    {
        private List<SRPTopMenuItem> MenuItems = new List<SRPTopMenuItem>();

        protected void Page_Load(object sender, EventArgs e)
        {
            BuildList();
            rptTabs.DataSource = MenuItems;
            rptTabs.DataBind();
        }

        private void BuildList()
        {
            if (Session[SessionData.IsLoggedIn.ToString()] == null || !(bool)Session[SessionData.IsLoggedIn.ToString()])
                return;
            string permList = Session[SessionData.StringPermissionList.ToString()].ToString();

            MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Modules/Patrons/Default.aspx", Name = "Patrons", IsSelected = false });
            MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Modules/Notifications/Default.aspx", Name = "Notifications", IsSelected = false });

            if (permList.Contains("2200"))
                MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Modules/Programs/Default.aspx", Name = "Programs", IsSelected = false });

            //MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Modules/Setup/Default.aspx", Name = "System Setup", IsSelected = false });

            MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Modules/Setup/Default.aspx", Name = "Management", IsSelected = false });

            MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Modules/Reports/Default.aspx", Name = "Reports", IsSelected = false });
            
            if (permList.Contains("1000"))
                MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Modules/Security/Default.aspx", Name = "Security", IsSelected = false });

            if (permList.Contains("3000"))
                MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Modules/Settings/Default.aspx", Name = "Settings", IsSelected = false });

            MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Modules/Drawings/Default.aspx", Name = "Drawings", IsSelected = false });

            MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Modules/PortalUser/MyAccount.aspx", Name = "My Account", IsSelected = false });
            MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Logoff.aspx", Name = "Logoff", IsSelected = false });
        }
    }


}