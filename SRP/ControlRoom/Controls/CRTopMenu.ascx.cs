using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using System.Web.UI.HtmlControls;

namespace GRA.SRP.ControlRoom
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

            if (permList.Contains("5100"))
            {
                MenuItems.Add(new SRPTopMenuItem
                {
                    Url = "/ControlRoom/Modules/Patrons/Default.aspx",
                    Name = "Patrons",
                    IsSelected = Request.Url.AbsoluteUri.Contains("Modules/Patrons")
                });
            }

            if (permList.Contains("5000"))
            {
                MenuItems.Add(new SRPTopMenuItem
                {
                    Url = "/ControlRoom/Modules/Notifications/Default.aspx",
                    Name = "Mail",
                    IsSelected = Request.Url.AbsoluteUri.Contains("Modules/Notifications")
                });
            }
            //if (permList.Contains("5300"))
            //    MenuItems.Add(new SRPTopMenuItem { Url = "/ControlRoom/Modules/Notifications/Default.aspx", Name = "Reviews", IsSelected = false });

            if (permList.Contains("2200"))
            {
                MenuItems.Add(new SRPTopMenuItem
                {
                    Url = "/ControlRoom/Modules/Programs/ProgramList.aspx",
                    Name = "Programs",
                    IsSelected = Request.Url.AbsoluteUri.Contains("Modules/Programs")
                });
            }
            if (permList.Contains("4300") || permList.Contains("4400") || permList.Contains("4500") || permList.Contains("4600") ||
                permList.Contains("4700") || permList.Contains("4800") || permList.Contains("4900"))
            {
                MenuItems.Add(new SRPTopMenuItem
                {
                    Url = "/ControlRoom/Modules/Setup/Default.aspx",
                    Name = "Management",
                    IsSelected = Request.Url.AbsoluteUri.Contains("Modules/Setup")
                });
            }
            if (permList.Contains("4000"))
            {
                MenuItems.Add(new SRPTopMenuItem
                {
                    Url = "/ControlRoom/Modules/Drawings/Default.aspx",
                    Name = "Drawings",
                    IsSelected = Request.Url.AbsoluteUri.Contains("Modules/Drawings")
                });
            }
            if (permList.Contains("4200"))
            {
                MenuItems.Add(new SRPTopMenuItem
                {
                    Url = "/ControlRoom/Modules/Reports/Default.aspx",
                    Name = "Reports",
                    IsSelected = Request.Url.AbsoluteUri.Contains("Modules/Reports")
                });
            }
            if (permList.Contains("3000") || permList.Contains("4100"))
            {
                MenuItems.Add(new SRPTopMenuItem
                {
                    Url = "/ControlRoom/Modules/Settings/Default.aspx",
                    Name = "Settings",
                    IsSelected = Request.Url.AbsoluteUri.Contains("Modules/Settings")
                });
            }
            if (permList.Contains("8000"))
            {
                MenuItems.Add(new SRPTopMenuItem
                {
                    Url = "/ControlRoom/Modules/Tenant/Default.aspx",
                    Name = "Organization",
                    IsSelected = Request.Url.AbsoluteUri.Contains("Modules/Tenant")
                });
            }
            if (permList.Contains("1000"))
            {
                MenuItems.Add(new SRPTopMenuItem
                {
                    Url = "/ControlRoom/Modules/Security/Default.aspx",
                    Name = "Security",
                    IsSelected = Request.Url.AbsoluteUri.Contains("Modules/Security")
                });
            }
            MenuItems.Add(new SRPTopMenuItem
            {
                Url = "/ControlRoom/Modules/PortalUser/MyAccount.aspx",
                Name = "My Account",
                IsSelected = Request.Url.AbsoluteUri.Contains("Modules/PortalUser")
            });
            MenuItems.Add(new SRPTopMenuItem
            {
                Url = "/ControlRoom/Modules/About/Default.aspx",
                Name = "About",
                IsSelected = Request.Url.AbsoluteUri.Contains("Modules/About")
            });
            MenuItems.Add(new SRPTopMenuItem
            {
                Url = "/ControlRoom/Logoff.aspx",
                Name = "Logoff",
                IsSelected = false
            });
        }

        protected object NoSpaces(object menuText)
        {
            var menuTitle = menuText as string;
            if (string.IsNullOrEmpty(menuTitle))
            {
                return menuText;
            }
            else
            {
                return menuTitle.Replace(" ", string.Empty);
            }
        }
    }


}