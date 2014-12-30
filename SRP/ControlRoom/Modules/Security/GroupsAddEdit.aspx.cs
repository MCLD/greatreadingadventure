using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using SRP_DAL;
using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;
using STG.SRP.Utilities;

namespace STG.SRP.ControlRoom.Modules.Security
{
    public partial class GroupsAddEdit : BaseControlRoomPage
	{

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = "SRP - Add / Edit User Groups";


            ControlRoomAccessPermission.CheckControlRoomAccessPermission(1000); // User Security;

            if (!IsPostBack)
            {
                List<RibbonPanel> moduleRibbonPanels = StandardModuleRibbons.SecurityRibbon();
                foreach (var moduleRibbonPanel in moduleRibbonPanels)
                {
                    MasterPage.PageRibbon.Add(moduleRibbonPanel);
                }
                MasterPage.PageRibbon.DataBind();
            }

            if (!IsPostBack )
            {
                lblGID.Text = Request["PK"];
                if (lblGID.Text.Length == 0)
                    dv.ChangeMode(DetailsViewMode.Insert);
                else
                {
                    dv.ChangeMode(DetailsViewMode.Edit);
                }
            }
        }

        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Security/GroupsList.aspx";
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(returnURL);
            }
            if (e.CommandName.ToLower() == "refresh")
            {
                try
                {
                        odsSRPGroups.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        MasterPage.PageMessage = SRPResources.RefreshOK;

                }
                catch (Exception ex)
                {
                    MasterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }

            if (e.CommandName.ToLower() == "add" || e.CommandName.ToLower() == "addandback")
            {
                try
                {


                        SRPGroup obj = new SRPGroup();

						//obj.GID = int.Parse(          ((Label)((DetailsView)sender).FindControl(".GID")).Text    );
						obj.GroupName = ((TextBox)((DetailsView)sender).FindControl("GroupName")).Text;
						obj.GroupDescription = ((TextBox)((DetailsView)sender).FindControl("GroupDescription")).Text;

                        obj.AddedDate = DateTime.Now;
                        obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                        obj.LastModDate = obj.AddedDate;
                        obj.LastModUser = obj.AddedUser;

                        if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                        {
	                        obj.Insert();
        	                if (e.CommandName.ToLower() == "addandback")
                	        {
                        	    Response.Redirect(returnURL);
	                        }
	
        	                lblGID.Text = obj.GID.ToString();

	                        odsSRPGroups.DataBind();
        	                dv.DataBind();
                	        dv.ChangeMode(DetailsViewMode.Edit);
	
                            MasterPage.PageMessage = SRPResources.AddedOK;
                        }
                        else
                        {
                            string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                            foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                            {
                                message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                            }
                            message = string.Format("{0}</ul>", message);
                            MasterPage.PageError = message;                                              
                        }
                }
                catch(Exception ex)
                {
                    MasterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);                  

                }
            }
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {

                        SRPGroup obj = new SRPGroup();
                        int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
                        obj = SRPGroup.Fetch(pk);


						obj.GroupName = ((TextBox)((DetailsView)sender).FindControl("GroupName")).Text;
						obj.GroupDescription = ((TextBox)((DetailsView)sender).FindControl("GroupDescription")).Text;

                        obj.LastModDate = DateTime.Now;
                        obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                        if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                        {
	                        obj.Update();

                            SaveUsers((DetailsView)sender, obj);
                            SavePermissions((DetailsView)sender, obj);

	                        if (e.CommandName.ToLower() == "saveandback")
        	                {
                	            Response.Redirect(returnURL);
                        	}
	                        odsSRPGroups.DataBind();
        	                dv.DataBind();
                	        dv.ChangeMode(DetailsViewMode.Edit);

                            MasterPage.PageMessage = SRPResources.SaveOK;
                            MasterPage.PageMessage = SRPResources.AddedOK;
                        }
                        else
                        {

                            string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                            foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                            {
                                message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                            }
                            message = string.Format("{0}</ul>", message);
                            MasterPage.PageError = message;                                              
                        }

                }
                catch(Exception ex)
                {

                    MasterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);                  

                }


            }
        }

        protected void SaveUsers(DetailsView dv, SRPGroup obj)
        {
            GridView gv = (GridView)dv.FindControl("gvGroupUsers");
            string memberUsers = "";
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isMember")).Checked)
                {
                    memberUsers = string.Format("{0},{1}", memberUsers, ((Label)row.FindControl("UID")).Text);
                }
            }
            if (memberUsers.Length > 0) memberUsers = memberUsers.Substring(1, memberUsers.Length - 1);

            SRPGroup.UpdateMemberUsers(obj.GID, memberUsers, ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username);
        }

        protected void SavePermissions(DetailsView dv, SRPGroup obj)
        {
            GridView gv = (GridView)dv.FindControl("gvGroupPermissions");
            string groupPermissions = "";
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isChecked")).Checked)
                {
                    groupPermissions = string.Format("{0},{1}", groupPermissions, ((Label)row.FindControl("PermissionID")).Text);
                }
            }
            if (groupPermissions.Length > 0) groupPermissions = groupPermissions.Substring(1, groupPermissions.Length - 1);

            SRPGroup.UpdatePermissions(obj.GID, groupPermissions, ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username);
        }









    }
}		

