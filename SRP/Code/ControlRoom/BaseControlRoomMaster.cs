using GRA;
using GRA.SRP.ControlRoom;
using GRA.SRP.Core.Utilities;
using System;
using System.Web.UI;

namespace SRPApp.Classes
{
    public class BaseControlRoomMaster : MasterPage
    {
        #region Properties
        protected BaseControlRoomPage CRPage;
        #endregion

        protected void PageLoad(object sender, EventArgs e)
        {
            try
            {
                CRPage = (BaseControlRoomPage)this.Page;
            }
            catch { }
        }

        private long _requiredPermission = 0;
        public long RequiredPermission {
            get { return _requiredPermission; }
            set {
                _requiredPermission = value;
                CheckPermissions(_requiredPermission);
            }
        }

        private long _additionalRequiredPermission = 0;
        public long AdditionalRequiredPermission {
            get {
                return _additionalRequiredPermission;
            }
            set {
                _additionalRequiredPermission = value;
                CheckPermissions(_additionalRequiredPermission);
            }
        }

        protected void CheckPermissions(long permissionValue)
        {
            if (permissionValue != 0)
            {
                try
                {
                    string permList = Session[SessionData.StringPermissionList.ToString()] as string;
                    if (string.IsNullOrEmpty(permList))
                    {
                        Response.Redirect("~/ControlRoom/Login.aspx", false);
                    }
                    if (!permList.Contains(permissionValue.ToString()))
                    {
                        Response.Redirect("~/ControlRoom/NoAccess.aspx", false);
                    }
                }
                catch (Exception ex)
                {
                    this.Log().Error("Error checking permissions: {0}", ex.Message);
                    Response.Redirect("~/ControlRoom/Login.aspx", false);
                }
            }
        }
    }
}
