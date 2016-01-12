using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRP_DAL;
using SRPApp.Classes;
using GRA.SRP.ControlRoom.Controls;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities;
using GRA.SRP.DAL;
namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class AvatarAddEdit : BaseControlRoomPage
    {


            
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4800;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Avatar Add/Edit");
            
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                //lblPK.Text = Request["PK"]; 
                lblPK.Text = Session["AID"] == null ? "" : Session["AID"].ToString(); //Session["AID"]= string.Empty;
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();
            }
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/AvatarList.aspx";
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(returnURL);
            }
            if (e.CommandName.ToLower() == "refresh")
            {
                
                try
                {
                    
                    odsData.DataBind();
                    dv.DataBind();
                    dv.ChangeMode(DetailsViewMode.Edit);
                    

                    var masterPage = (IControlRoomMaster)Master;
                    if (masterPage != null) masterPage.PageMessage = SRPResources.RefreshOK;

                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }

            if (e.CommandName.ToLower() == "add" || e.CommandName.ToLower() == "addandback")
            {
                try
                {
                    var obj = new Avatar();
                    obj.Name = ((TextBox)((DetailsView)sender).FindControl("Name")).Text;
                    obj.Gender = "O";//"((DropDownList) ((DetailsView) sender).FindControl("Gender")).SelectedValue;
                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;

                    if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        obj.Insert();

                        try {
                            var badgePath = string.Format(Server.MapPath("~/images/Avatars/"));
                            System.IO.File.Copy(string.Format("{0}no_avatar.png", badgePath),
                                                string.Format("{0}{1}.png", badgePath, obj.AID));
                            System.IO.File.Copy(string.Format("{0}no_avatar_sm.png", badgePath),
                                                string.Format("{0}sm_{1}.png", badgePath, obj.AID));
                        } catch(Exception ex) {
                            this.Log().Error("Couldn't copy no_avatar images into new avatar: {0}",
                                             ex.Message);
                        }


                        if(e.CommandName.ToLower() == "addandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        lblPK.Text = obj.AID.ToString();

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.AddedOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                        {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                    }
                      
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);

                }
            }
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {
                    var obj = new Avatar();
                    int pk = int.Parse(lblPK.Text);
                    obj = obj.GetAvatar(pk);
                    obj.Name = ((TextBox)((DetailsView)sender).FindControl("Name")).Text;
                    obj.Gender = "O";//"((DropDownList)((DetailsView)sender).FindControl("Gender")).SelectedValue;
                    obj.LastModDate = DateTime.Now;
                    obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        obj.Update();

                        if (e.CommandName.ToLower() == "saveandback")
                        {
                            Response.Redirect(returnURL);
                        }
                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.SaveOK;
                        masterPage.PageMessage = SRPResources.AddedOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                        {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        masterPage.PageError = message;
                    }
                   
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);

                }

            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
            //if (dv.CurrentMode == DetailsViewMode.Edit)
            //{
            //    var view = (DetailsView)sender;
            //    DetailsViewRowCollection rows = view.Rows;
            //    if (rows.Count > 0) 
            //    {
            //        DetailsViewRow row = rows[3];
            //        ((GRA.SRP.Classes.FileDownloadCtl)row.Cells[1].Controls[0]).FileName = DateTime.Now.ToShortDateString();
            //    }
            //}
        }

        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {

                var control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl");
                if (control!=null) control.ProcessRender();

            }
        }

    }
}

