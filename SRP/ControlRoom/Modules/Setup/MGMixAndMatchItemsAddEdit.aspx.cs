using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;


namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class MGMixAndMatchItemsAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4300;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Mix-And-Match Item Add / Edit");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            
                if (Request["MMID"] != null)
                {
                    lblMGID.Text = Session["MGID"].ToString();
                    lblMMID.Text = Request["MMID"];

                    var o = Minigame.FetchObject(int.Parse(lblMGID.Text));
                    AdminName.Text = o.AdminName;

                    lblPK.Text= string.Empty;
                    dv.ChangeMode(DetailsViewMode.Insert);

                }
                else
                {
                    lblPK.Text = Request["PK"];

                    var o1 = MGMixAndMatchItems.FetchObject(int.Parse(lblPK.Text));
                    lblMGID.Text = o1.MGID.ToString();
                    lblMMID.Text = o1.MMID.ToString();

                    var o = Minigame.FetchObject(int.Parse(lblMGID.Text));
                    AdminName.Text = o.AdminName;

                    dv.ChangeMode(DetailsViewMode.Edit);
                }
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl");
                if (control != null) control.ProcessRender();

                var auc = (GRA.SRP.Controls.AudioUploadCtl)dv.FindControl("AudioUploadCtlE");
                if (auc != null) auc.ProcessRender();


                auc = (GRA.SRP.Controls.AudioUploadCtl)dv.FindControl("AudioUploadCtlM");
                if (auc != null) auc.ProcessRender();

                auc = (GRA.SRP.Controls.AudioUploadCtl)dv.FindControl("AudioUploadCtlH");
                if (auc != null) auc.ProcessRender();
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            //string returnURL = "~/ControlRoom/Modules/Setup/MGMixAndMatchItemsList.aspx?MGID=" + lblMGID.Text;
            string returnURL = "~/ControlRoom/Modules/Setup/MGMixAndMatchItemsList.aspx";
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
                    var obj = new MGMixAndMatchItems();

                    obj.MMID = FormatHelper.SafeToInt(lblMMID.Text);
                    obj.MGID = FormatHelper.SafeToInt(lblMGID.Text);
                    
                    //obj.ItemImage = ((TextBox)((DetailsView)sender).FindControl("ItemImage")).Text;
                    obj.EasyLabel = ((TextBox)((DetailsView)sender).FindControl("EasyLabel")).Text;
                    obj.MediumLabel = ((TextBox)((DetailsView)sender).FindControl("MediumLabel")).Text;
                    obj.HardLabel = ((TextBox)((DetailsView)sender).FindControl("HardLabel")).Text;
                    //obj.AudioEasy = ((TextBox)((DetailsView)sender).FindControl("AudioEasy")).Text;
                    //obj.AudioMedium = ((TextBox)((DetailsView)sender).FindControl("AudioMedium")).Text;
                    //obj.AudioHard = ((TextBox)((DetailsView)sender).FindControl("AudioHard")).Text;

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

                        lblPK.Text = obj.MMIID.ToString();

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
                    var obj = new MGMixAndMatchItems();
                    int pk = int.Parse(((TextBox)((DetailsView)sender).FindControl("MMIID")).Text);
                    obj.Fetch(pk);

                    obj.MMID = FormatHelper.SafeToInt(lblMMID.Text);
                    obj.MGID = FormatHelper.SafeToInt(lblMGID.Text);

                    //obj.ItemImage = ((TextBox)((DetailsView)sender).FindControl("ItemImage")).Text;
                    obj.EasyLabel = ((TextBox)((DetailsView)sender).FindControl("EasyLabel")).Text;
                    obj.MediumLabel = ((TextBox)((DetailsView)sender).FindControl("MediumLabel")).Text;
                    obj.HardLabel = ((TextBox)((DetailsView)sender).FindControl("HardLabel")).Text;
                    //obj.AudioEasy = ((TextBox)((DetailsView)sender).FindControl("AudioEasy")).Text;
                    //obj.AudioMedium = ((TextBox)((DetailsView)sender).FindControl("AudioMedium")).Text;
                    //obj.AudioHard = ((TextBox)((DetailsView)sender).FindControl("AudioHard")).Text;

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
    }
}

