using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRoom;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.ControlRoom
{
    public partial class Reports : BaseControlRoomMaster, IControlRoomMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_isSecurePage)
            {
                if (Session[SessionData.IsLoggedIn.ToString()] == null || !(bool)Session[SessionData.IsLoggedIn.ToString()])
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
            }
            CheckPermissions(this.RequiredPermission);
            lblPageTitle.Visible = (lblPageTitle.Text.Length > 0);
            pnlMessage.Visible = DisplayMessageOnLoad;
        }
        public CRRibbon PageRibbon {
            get {
                return null;
            }
        }

        public bool DisplayMessageOnLoad {
            get;
            set;
        }

        public string PageTitle {
            get {
                return lblPageTitle.Text;
            }
            set {
                lblPageTitle.Text = value;
                lblPageTitle.Visible = (lblPageTitle.Text.Length > 0);
            }
        }

        public string PageError {
            set {
                lblMessage.Text = value.Replace("\n", "<BR/>");
                lblMessage.ForeColor = Color.FromKnownColor(KnownColor.Red); ;
                pnlMessage.Visible = (lblMessage.Text.Length > 0);
                imgMessage.ImageUrl = "~/ControlRoom/Images/Error.png";
            }
        }
        public string PageWarning {
            set {
                lblMessage.Text = value.Replace("\n", "<BR/>");
                lblMessage.ForeColor = Color.FromKnownColor(KnownColor.DarkGray); ;
                pnlMessage.Visible = (lblMessage.Text.Length > 0);
                imgMessage.ImageUrl = "~/ControlRoom/Images/Warning.png";
            }

        }
        public string PageMessage {
            set {
                lblMessage.Text = value.Replace("\n", "<BR/>");
                lblMessage.ForeColor = Color.FromKnownColor(KnownColor.Black); ;
                pnlMessage.Visible = (lblMessage.Text.Length > 0);
                imgMessage.ImageUrl = "~/ControlRoom/Images/Info.png";
            }

        }

        private bool _isSecurePage = true;
        public bool IsSecure {
            get {
                return _isSecurePage;
            }
            set {
                _isSecurePage = value;
                if (_isSecurePage)
                {
                    if (Session[SessionData.IsLoggedIn.ToString()] == null || !(bool)Session[SessionData.IsLoggedIn.ToString()])
                    {
                        FormsAuthentication.RedirectToLoginPage();
                    }
                }
            }
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
                    string permList = Session[SessionData.StringPermissionList.ToString()].ToString();
                    if (!permList.Contains(permissionValue.ToString()))
                    {
                        Response.Redirect("~/ControlRoom/NoAccess.aspx", false);
                    }
                }
                catch (Exception ex)
                {
                    this.Log().Error("Error checking permissions: {0}", ex.Message);
                    Response.Redirect("~/ControlRoom/Login.aspx");
                }
            }
        }
    }
}