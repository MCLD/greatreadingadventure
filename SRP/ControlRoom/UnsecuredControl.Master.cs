using System;
using System.Drawing;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.ControlRoom
{
    public partial class UnsecuredControl : BaseControlRoomMaster, IControlRoomMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckPermissions(this.RequiredPermission);
            lblPageTitle.Visible = (lblPageTitle.Text.Length > 0);
            pnlMessage.Visible = DisplayMessageOnLoad;
        }

        public CRRibbon PageRibbon
        {
            get
            {
                return CMSRibbonCtl;
            }
        }

        public bool DisplayMessageOnLoad
        {
            get;
            set;
        }

        public string PageTitle
        {
            get
            {
                return lblPageTitle.Text;
            }
            set
            {
                lblPageTitle.Text = value;
                lblPageTitle.Visible = (lblPageTitle.Text.Length > 0);
            }
        }

        public  string PageError
        {
            set
            {
                lblMessage.Text = value.Replace("\n", "<BR/>");
                lblMessage.ForeColor = Color.FromKnownColor(KnownColor.Red); ;
                pnlMessage.Visible = (lblMessage.Text.Length > 0);
                imgMessage.ImageUrl = "~/ControlRoom/Images/Error.png";
            }
        }
        public  string PageWarning
        {
            set
            {
                lblMessage.Text = value.Replace("\n", "<BR/>");
                lblMessage.ForeColor = Color.FromKnownColor(KnownColor.DarkGray); ;
                pnlMessage.Visible = (lblMessage.Text.Length > 0);
                imgMessage.ImageUrl = "~/ControlRoom/Images/Warning.png";
            }

        }
        public string PageMessage
        {
            set
            {
                lblMessage.Text = value.Replace("\n", "<BR/>");
                lblMessage.ForeColor = Color.FromKnownColor(KnownColor.Black); ;
                pnlMessage.Visible = (lblMessage.Text.Length > 0);
                imgMessage.ImageUrl = "~/ControlRoom/Images/Info.png";
            }

        }

        private bool _isSecurePage;// = false;
        public bool IsSecure
        {
            get
            {
                return false;
            }
            set
            {
                if (value)
                {
                    _isSecurePage = false;// value;
                }
                else
                {
                    _isSecurePage = value;
                }

            }
        }
    }
}