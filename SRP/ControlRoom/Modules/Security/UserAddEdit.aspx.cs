﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.Core.Utilities;
using STG.SRP.Utilities;

namespace STG.SRP.ControlRoom.Modules.Security
{
    public partial class UserAddEdit : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            MasterPage.IsSecure = false;
            MasterPage.PageTitle = "Summer Reading Program - Add / Edit User";

            ControlRoomAccessPermission.CheckControlRoomAccessPermission(1000); // Change Appropriately;

            if (!IsPostBack)
            {
                List<RibbonPanel> moduleRibbonPanels = StandardModuleRibbons.SecurityRibbon();
                foreach (var moduleRibbonPanel in moduleRibbonPanels)
                {
                    MasterPage.PageRibbon.Add(moduleRibbonPanel);
                }
                MasterPage.PageRibbon.DataBind();
            }


            if (!IsPostBack)
            {
                lblUID.Text = Request["PK"];
                if (lblUID.Text.Length == 0)
                    dv.ChangeMode(DetailsViewMode.Insert);
                else
                {
                    dv.ChangeMode(DetailsViewMode.Edit);
                }
            }
        }

        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Security/Default.aspx";
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(returnURL);
            }
            if (e.CommandName.ToLower() == "refresh")
            {
                try
                {
                    odsSRPUser.DataBind();
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

                        SRPUser obj = new SRPUser();


                        obj.Username = ((TextBox)((DetailsView)sender).FindControl("Username")).Text;
                        obj.Password = ((TextBox)((DetailsView)sender).FindControl("Password")).Text;
                        obj.FirstName = ((TextBox)((DetailsView)sender).FindControl("FirstName")).Text;
                        obj.LastName = ((TextBox)((DetailsView)sender).FindControl("LastName")).Text;
                        obj.EmailAddress = ((TextBox)((DetailsView)sender).FindControl("EmailAddress")).Text;
                        obj.Division = ((TextBox)((DetailsView)sender).FindControl("Division")).Text;
                        obj.Department = ((TextBox)((DetailsView)sender).FindControl("Department")).Text;
                        obj.Title = ((TextBox)((DetailsView)sender).FindControl("Title")).Text;
                        //((TextBox)((DetailsView)sender).FindControl("Password")).Attributes["value"] = obj.Password;

                        obj.IsActive = true;
                        obj.MustResetPassword = true;
                        obj.IsDeleted = false;

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

                            lblUID.Text = obj.Uid.ToString();

                            odsSRPUser.DataBind();
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
                catch (Exception ex)
                {
                    MasterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);

                }
            }
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {

                        int pk = int.Parse(((DetailsView)sender).Rows[0].Cells[1].Text);
                        SRPUser obj = new SRPUser(pk);


                        obj.Username = ((TextBox)((DetailsView)sender).FindControl("Username")).Text;
                        obj.Password = ((TextBox)((DetailsView)sender).FindControl("Password")).Text;
                        obj.FirstName = ((TextBox)((DetailsView)sender).FindControl("FirstName")).Text;
                        obj.LastName = ((TextBox)((DetailsView)sender).FindControl("LastName")).Text;
                        obj.EmailAddress = ((TextBox)((DetailsView)sender).FindControl("EmailAddress")).Text;
                        obj.Division = ((TextBox)((DetailsView)sender).FindControl("Division")).Text;
                        obj.Department = ((TextBox)((DetailsView)sender).FindControl("Department")).Text;
                        obj.Title = ((TextBox)((DetailsView)sender).FindControl("Title")).Text;
                        obj.IsActive = ((CheckBox)((DetailsView)sender).FindControl("IsActive")).Checked;
                        obj.MustResetPassword = ((CheckBox)((DetailsView)sender).FindControl("MustResetPassword")).Checked;
                        //((TextBox)((DetailsView)sender).FindControl("Password")).Attributes.Add("value", obj.Password);


                        //obj.IsDeleted = ((TextBox)((DetailsView)sender).FindControl("IsDeleted")).Text;
                        //obj.LastPasswordReset = ((TextBox)((DetailsView)sender).FindControl("LastPasswordReset")).Text;
                        //obj.DeletedDate = ((TextBox)((DetailsView)sender).FindControl("DeletedDate")).Text;
                        //obj.LastModDate = ((TextBox)((DetailsView)sender).FindControl("LastModDate")).Text;
                        //obj.LastModUser = ((TextBox)((DetailsView)sender).FindControl("LastModUser")).Text;
                        //obj.AddedDate = ((TextBox)((DetailsView)sender).FindControl("AddedDate")).Text;
                        //obj.AddedUser = ((TextBox)((DetailsView)sender).FindControl("AddedUser")).Text;

                        obj.LastModDate = DateTime.Now;
                        obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                        if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                        {
                            obj.Update();

                            SaveGroups((DetailsView)sender, obj);
                            //SavePermissions((DetailsView)sender, obj);
                            //SaveFolders((DetailsView)sender, obj);

                            if (e.CommandName.ToLower() == "saveandback")
                            {
                                Response.Redirect(returnURL);
                            }
                            odsSRPUser.DataBind();
                            dv.DataBind();
                            dv.ChangeMode(DetailsViewMode.Edit);

                            MasterPage.PageMessage = SRPResources.SaveOK;
                            
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
                catch (Exception ex)
                {
                    MasterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);

                }
            }
            if (e.CommandName.ToLower() == "loginhistory")
            {
                int key = Convert.ToInt32(lblUID.Text);
                Response.Redirect(String.Format("{0}?UID={1}", "~/ControlRoom/Modules/Security/LoginHistory.aspx", key));
            }
            //if (e.CommandName.ToLower() == "audituser")
            //{
            //    int key = Convert.ToInt32(lblUID.Text);
            //    Response.Redirect(String.Format("{0}?UID={1}", "~/ControlRoom/Modules/Security/UserAudit.aspx", key));
            //}


        }

        protected void SaveGroups(DetailsView dv, SRPUser obj)
        {
            GridView gv = (GridView)dv.FindControl("gvUserGroups");
            string memberGroups = "";
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isMember")).Checked)
                {
                    memberGroups = string.Format("{0},{1}", memberGroups, ((Label)row.FindControl("GID")).Text);
                }
            }
            if (memberGroups.Length > 0) memberGroups = memberGroups.Substring(1, memberGroups.Length - 1);

            SRPUser.UpdateMemberGroups((int)obj.Uid, memberGroups, ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username);
        }

        protected void SavePermissions(DetailsView dv, SRPUser obj)
        {
            GridView gv = (GridView)dv.FindControl("gvUserPermissions");
            string groupPermissions = "";
            foreach (GridViewRow row in gv.Rows)
            {
                if (((CheckBox)row.FindControl("isChecked")).Checked)
                {
                    groupPermissions = string.Format("{0},{1}", groupPermissions, ((Label)row.FindControl("PermissionID")).Text);
                }
            }
            if (groupPermissions.Length > 0) groupPermissions = groupPermissions.Substring(1, groupPermissions.Length - 1);

            SRPUser.UpdatePermissions((int)obj.Uid, groupPermissions, ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username);
        }

        protected void dv_PreRender(object sender, EventArgs e)
        {
            var pwd = (TextBox)((DetailsView) sender).FindControl("Password");
            pwd.Attributes.Add("Value", pwd.Text);
        }

        //protected void SaveFolders(DetailsView dv, SRPUser obj)
        //{
        //    GridView gv = (GridView)dv.FindControl("gvUserFolders");
        //    string groupFolders = "";
        //    foreach (GridViewRow row in gv.Rows)
        //    {
        //        if (((CheckBox)row.FindControl("isChecked")).Checked)
        //        {
        //            groupFolders = string.Format("{0}#{1}", groupFolders, ((Label)row.FindControl("Folder")).Text);
        //        }
        //    }
        //    if (groupFolders.Length > 0) groupFolders = groupFolders.Substring(1, groupFolders.Length - 1);

        //    SRPUser.UpdateFolders((int)obj.Uid, groupFolders, ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username);
        //}



    }
}

