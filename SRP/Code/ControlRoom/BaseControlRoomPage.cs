using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities;
using GRA.Tools;
using GRA;

namespace SRPApp.Classes {
    public class BaseControlRoomPage : System.Web.UI.Page {
        #region Properties
        protected static string DbConn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        public IControlRoomMaster MasterPage;
        protected SRPUser SRPUser;
        protected List<SRPPermission> UserPermissions;
        protected string UserPermissionList = string.Empty;
        #endregion

        public BaseControlRoomPage() {
            this.Load += (s, e) => {
                var tenantIdSession = Session["TenantID"];
                int? tenantId = tenantIdSession as int?;
                int? crTenantId = CRTenantID;

                if(tenantId != crTenantId) {
                    // tenant mismatch between user's TenantID and CR login tenant id
                    // log out user
                    try {
                        SRPUser user = Session[SessionData.UserProfile.ToString()] as SRPUser;
                        string who = "Unknown user";
                        if(user != null) {
                            who = user.Username;
                        }

                        this.Log().Debug("{0} has mismatched tenants ({1} public and {2} CR) - performing logout",
                                         who,
                                         tenantId,
                                         crTenantId);
                    } catch(Exception ex) {
                        this.Log()
                            .Debug("Unknown user has mismatched tenants ({0} public and {1} CR) - error occurred: {2} - performing logout",
                                   tenantId,
                                   crTenantId,
                                   ex.Message);
                    }
                    GRA.SRP.ControlRoom.CRLogout.Logout(this);
                    return;
                }

                this.ViewState["TenantID"] = crTenantId;
            };
        }

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

        protected bool? SafeSessionReturnBool(string sessionKey) {
            var sessionValue = Session[sessionKey];
            try {
                return (bool)sessionValue;
            } catch(Exception) {
                return null;
            }
        }

        protected int? SafeSessionReturnInt(string sessionKey) {
            try {
                return (int)Session[sessionKey];
            } catch(Exception) {
                return null;
            }
        }

        protected int? CRTenantID {
            set {
                Session[GRA.SRP.ControlRoom.CRSessionKey.TenantID] = value;
            }
            get {
                return SafeSessionReturnInt(GRA.SRP.ControlRoom.CRSessionKey.TenantID);
            }
        }

        protected bool? CRIsMasterTenant {
            set {
                Session[GRA.SRP.ControlRoom.CRSessionKey.IsMaster] = value;
            }
            get {
                return SafeSessionReturnBool(GRA.SRP.ControlRoom.CRSessionKey.IsMaster);
            }
        }

    }
}