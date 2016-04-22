using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom.Modules.Tenant
{
    public partial class TenantUserPasswordChange : System.Web.UI.Page
    {
        private SRPUser GetUser(string userIdString)
        {
            if (!string.IsNullOrWhiteSpace(userIdString))
            {
                int userId = 0;
                if (int.TryParse(userIdString, out userId))
                {
                    return SRPUser.Fetch(userId);
                }
            }
            return null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ErrorMessage.Visible = false;
            if (!IsPostBack)
            {
                var user = GetUser(Request.QueryString["Id"]);

                if (user == null)
                {
                    ErrorMessage.Visible = true;
                    return;
                }

                UserName.Text = string.Format("Change password for user: {0}", user.Username);
                SRPUserName.Value = user.Username;
                SRPUserId.Value = user.Uid.ToString();
                CameFrom.Value = Request.UrlReferrer.ToString();
            }
        }

        protected void ChangePassword(object sender, EventArgs e)
        {
            var user = GetUser(SRPUserId.Value);
            if (user == null)
            {
                ErrorMessage.Visible = true;
                return;
            }

            user.NewPassword = NewPassword.Text;
            user.Update();
            Session[CRSessionKey.CRMessage] = string.Format("Password successfully changed for {0}.",
                SRPUserName.Value);
            Response.Redirect(CameFrom.Value);
        }
    }
}