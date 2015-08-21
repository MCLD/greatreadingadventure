using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRA.SRP.Core.Utilities
{
    public class ControlRoomAccessPermission
    {
        public static void CheckControlRoomAccessPermission(int permission)
        {
            CheckControlRoomAccessPermission(permission.ToString());
        }

        public static void CheckControlRoomAccessPermission(string permission)
        {
            if (HttpContext.Current.Session[SessionData.IsLoggedIn.ToString()] == null || !(bool)HttpContext.Current.Session[SessionData.IsLoggedIn.ToString()])
                return;
            string permList = HttpContext.Current.Session[SessionData.StringPermissionList.ToString()].ToString();

            if (!permList.Contains(permission)) HttpContext.Current.Response.Redirect("~/ControlRoom/NoAccess.aspx");
        }

        public static bool HavePermission(int permission)
        {
            return HavePermission(permission.ToString());
        }

        public static bool HavePermission(string permission)
        {
            if (HttpContext.Current.Session[SessionData.IsLoggedIn.ToString()] == null || !(bool)HttpContext.Current.Session[SessionData.IsLoggedIn.ToString()])
                return false;
            string permList = HttpContext.Current.Session[SessionData.StringPermissionList.ToString()].ToString();

            if (!permList.Contains(permission)) return false;

            return true;
        }

        public static void CheckControlRoomAccessPermission(int permission, string redirectUrl)
        {
            CheckControlRoomAccessPermission(permission.ToString(), redirectUrl);
        }

        public static void CheckControlRoomAccessPermission(string permission, string redirectUrl)
        {
            if (HttpContext.Current.Session[SessionData.IsLoggedIn.ToString()] == null || !(bool)HttpContext.Current.Session[SessionData.IsLoggedIn.ToString()])
                return;
            string permList = HttpContext.Current.Session[SessionData.StringPermissionList.ToString()].ToString();

            if (!permList.Contains(permission)) HttpContext.Current.Response.Redirect(redirectUrl);
        }

    }
}
