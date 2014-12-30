using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using STG.CMS.Portal.ControlRoom;
using STG.CMS.Portal.ControlRoom.Controls;
using STG.CMS.Portal.Core;



namespace STG.CMS.Portal.ControlRoom.Module.Security
{
    public partial class UserAudit : CMSBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ICMSMasterPage masterPage = (ICMSMasterPage)Master;
            masterPage.IsSecure = false;
            masterPage.PageTitle = "User Security/Permission/Access Audit";

            ControlRoomAccessPermission.CheckControlRoomAccessPermission(10000000); // Change Appropriately;

            if (!IsPostBack)
            {
                List<RibbonPanel> moduleRibbonPanels = StandardModuleRibbons.SecurityRibbon();
                foreach (var moduleRibbonPanel in moduleRibbonPanels)
                {
                    masterPage.PageRibbon.Add(moduleRibbonPanel);
                }
                masterPage.PageRibbon.DataBind();
            }

            if (!IsPostBack)
            {
                lblUID.Text = Request["UID"];
                dv.ChangeMode(DetailsViewMode.ReadOnly);
            }

        }

        protected string GetType(string type, string name)
        {
            string retVal = "";
            switch (type)
            {
                case "G":
                    retVal = string.Format("Granted by membership in group \"{0}\"", name);
                    break;
                case "U":
                    retVal = "Granted to user as a special setting";
                    break;
                default:
                    retVal = "Unknown";
                    break;
            }
            return retVal;
        }

        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string editpage = "~/ControlRoom/Module/Security/CMSUserAddEdit.aspx";
            if (e.CommandName.ToLower() == "editrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                Response.Redirect(String.Format("{0}?PK={1}", editpage, key));
            }
            if (e.CommandName.ToLower() == "loginhistory")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                Response.Redirect(String.Format("{0}?UID={1}", "~/ControlRoom/Module/Security/LoginHistory.aspx", key));
            }
            if (e.CommandName.ToLower() == "userlist")
            {
                Response.Redirect("~/ControlRoom/Module/Security/CMSUserList.aspx");
            }
            if (e.CommandName.ToLower() == "refresh")
            {
                try
                {
                    odsCMSUser.DataBind();
                    dv.DataBind();
                    dv.ChangeMode(DetailsViewMode.Edit);

                    ICMSMasterPage masterPage = (ICMSMasterPage)Master;
                    masterPage.PageMessage = CMSResource.RefreshOK;

                }
                catch (Exception ex)
                {
                    ICMSMasterPage masterPage = (ICMSMasterPage)Master;
                    masterPage.PageError = String.Format(CMSResource.ApplicationError1, ex.Message);
                }
            }
        }
    }
}
