using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;


namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class MGOnlineBookPagesAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4300;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Online Book Page Add / Edit");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            
                if (Request["OBID"] != null)
                {
                    lblMGID.Text = Session["MGID"].ToString();
                    lblOBID.Text = Request["OBID"];

                    var o = Minigame.FetchObject(int.Parse(lblMGID.Text));
                    AdminName.Text = o.AdminName;

                    lblPK.Text= string.Empty;
                    dv.ChangeMode(DetailsViewMode.Insert);

                }
                else {
                    lblPK.Text = Request["PK"];

                    var o1 = MGOnlineBookPages.FetchObject(int.Parse(lblPK.Text));
                    lblMGID.Text = o1.MGID.ToString();
                    lblOBID.Text = o1.OBID.ToString();

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
            string returnURL = "~/ControlRoom/Modules/Setup/MGOnlineBookPagesList.aspx";
            //string returnURL = "~/ControlRoom/Modules/Setup/MGOnlineBookPagesList.aspx?MGID=" + lblMGID.Text;
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
                    var obj = new MGOnlineBookPages();
                    //obj.GenNotificationFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("GenNotificationFlag")).Checked;

                    obj.OBID = FormatHelper.SafeToInt(lblOBID.Text);
                    obj.MGID = FormatHelper.SafeToInt(lblMGID.Text);
                    //obj.PageNumber = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PageNumber")).Text);
                    obj.TextEasy = ((TextBox)((DetailsView)sender).FindControl("TextEasy")).Text;
                    obj.TextMedium = ((TextBox)((DetailsView)sender).FindControl("TextMedium")).Text;
                    obj.TextHard = ((TextBox)((DetailsView)sender).FindControl("TextHard")).Text;
                    obj.AudioEasy= string.Empty;// ((TextBox)((DetailsView)sender).FindControl("AudioEasy")).Text;
                    obj.AudioMedium= string.Empty;//"((TextBox)((DetailsView)sender).FindControl("AudioMedium")).Text;
                    obj.AudioHard= string.Empty;//"((TextBox)((DetailsView)sender).FindControl("AudioHard")).Text;

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

                        lblPK.Text = obj.OBPGID.ToString();

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
                    var obj = new MGOnlineBookPages();
                    int pk = int.Parse(((TextBox)((DetailsView)sender).FindControl("OBPGID")).Text);
                    obj.Fetch(pk);
                    obj.OBID = FormatHelper.SafeToInt(lblOBID.Text);
                    obj.MGID = FormatHelper.SafeToInt(lblMGID.Text);
                    //obj.PageNumber = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PageNumber")).Text);
                    obj.TextEasy = ((TextBox)((DetailsView)sender).FindControl("TextEasy")).Text;
                    obj.TextMedium = ((TextBox)((DetailsView)sender).FindControl("TextMedium")).Text;
                    obj.TextHard = ((TextBox)((DetailsView)sender).FindControl("TextHard")).Text;
                    obj.AudioEasy= string.Empty;// ((TextBox)((DetailsView)sender).FindControl("AudioEasy")).Text;
                    obj.AudioMedium= string.Empty;//"((TextBox)((DetailsView)sender).FindControl("AudioMedium")).Text;
                    obj.AudioHard= string.Empty;//"((TextBox)((DetailsView)sender).FindControl("AudioHard")).Text;

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

