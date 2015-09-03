using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities;
using GRA.Tools;

namespace SRPApp.Classes {
    public class BaseControlRoomPage : System.Web.UI.Page {
        #region Properties
        protected static string DbConn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        public IControlRoomMaster MasterPage;
        protected SRPUser SRPUser;
        protected List<SRPPermission> UserPermissions;
        protected string UserPermissionList = string.Empty;


        #endregion

        protected string PageTitle {
            //get { return masterPage.PageMessage; }
            set { if(MasterPage != null) MasterPage.PageTitle = value; }
        }

        protected string PageMessage {
            //get { return masterPage.PageMessage; }
            set { if(MasterPage != null) MasterPage.PageMessage = value; }
        }

        protected string PageError {
            //get { return masterPage.PageMessage; }
            set { if(MasterPage != null) MasterPage.PageError = value; }
        }

        protected string PageWarning {
            //get { return masterPage.PageMessage; }
            set { if(MasterPage != null) MasterPage.PageWarning = value; }
        }

        protected string BaseUrl {
            get {
                return WebTools.GetBaseUrl(Request);
            }
        }

        //public static bool UserHasRight(string requiredPermission)
        //{
        //    bool returnValue = false;
        //    var rights = (List<SRPPermission>)HttpContext.Current.Session[SessionData.PermissionList.ToString()];
        //    if (rights != null)
        //    {
        //        var r = from right in rights
        //                where right.Name == requiredPermission
        //                select right;
        //        returnValue = (r.Count() > 0);
        //    }
        //    return returnValue;
        //}

        public void SetPageRibbon(List<RibbonPanel> moduleRibbonPanels) {
            foreach(var moduleRibbonPanel in moduleRibbonPanels) {
                MasterPage.PageRibbon.Add(moduleRibbonPanel);
            }
            MasterPage.PageRibbon.DataBind();
        }

        protected override void OnPreLoad(EventArgs e) {
            MasterPage = (IControlRoomMaster)Master;
            if(MasterPage != null)
                MasterPage.IsSecure = true;
            SRPUser = (SRPUser)Session[SessionData.UserProfile.ToString()];
            //UserPermissions = (List<SRPPermission>)Session[SessionData.PermissionList.ToString()];
            UserPermissionList = (string)Session[SessionData.StringPermissionList.ToString()];

            base.OnPreLoad(e);
        }

        protected override void OnLoadComplete(EventArgs e) {
            base.OnInit(e);
        }


        protected void PageLoad(object sender, EventArgs e) {


        }

        protected Control FindControlRecursive(Control rootControl, string controlID) {
            if(rootControl.ID == controlID)
                return rootControl;

            foreach(Control controlToSearch in rootControl.Controls) {
                Control controlToReturn =
                    FindControlRecursive(controlToSearch, controlID);
                if(controlToReturn != null)
                    return controlToReturn;
            }
            return null;
        }
    }
}