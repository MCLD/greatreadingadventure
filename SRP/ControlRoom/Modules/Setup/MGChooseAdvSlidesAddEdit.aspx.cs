using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;
using STG.SRP.Utilities.CoreClasses;


// --> MODULENAME 
// --> XXXXXRibbon 
// --> PERMISSIONID 
namespace STG.SRP.ControlRoom.Modules.Setup
{
    public partial class MGChooseAdvSlidesAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            }

            //MasterPage.RequiredPermission = PERMISSIONID;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Choose Your Adventure Slides Edit");

            if (!IsPostBack)
            {
                if (Request["MGID"] != null)
                {
                    lblMGID.Text = Request["MGID"];
                    lblCAID.Text = Request["CAID"];
                    if (Request["L"] != null) lblDiff.Text = Request["L"];
                    var s = (lblDiff.Text == "1"
                                 ? " - EASY Difficulty"
                                 : (lblDiff.Text == "2" ? " - MEDIUM Difficulty" : " - HARD Difficulty"));

                    var o = Minigame.FetchObject(int.Parse(lblMGID.Text));
                    AdminName.Text = o.AdminName + s;

                    var obj = new MGChooseAdvSlides();
                    obj.CAID = int.Parse(lblCAID.Text);
                    obj.MGID = int.Parse(lblMGID.Text);
                    obj.SecondImageGoToStep = obj.FirstImageGoToStep = 0;
                    obj.SlideText = "";
                    obj.Difficulty = int.Parse(lblDiff.Text);
                    obj.StepNumber = -1;
                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;
                    obj.Insert();
                    lblPK.Text = obj.CASID.ToString();

                    dv.ChangeMode(DetailsViewMode.Edit);

                }
                else
                {
                    lblPK.Text = Request["PK"];

                    var o1 = MGChooseAdvSlides.FetchObject(int.Parse(lblPK.Text));
                    lblMGID.Text = o1.MGID.ToString();
                    lblCAID.Text = o1.CAID.ToString();

                    if (Request["L"] != null) lblDiff.Text = Request["L"];
                    var s = (lblDiff.Text == "1"
                                 ? " - EASY Difficulty"
                                 : (lblDiff.Text == "2" ? " - MEDIUM Difficulty" : " - HARD Difficulty")); 
                    
                    var o = Minigame.FetchObject(int.Parse(lblMGID.Text));
                    AdminName.Text = o.AdminName + s;

                    dv.ChangeMode(DetailsViewMode.Edit);
                }
                Page.DataBind();
            }

        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var control = (STG.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl_1");
                if (control != null) control.ProcessRender();

                control = (STG.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl_2");
                if (control != null) control.ProcessRender();
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/MGChooseAdvSlidesList.aspx?L=" + lblDiff.Text + "&MGID=" + lblMGID.Text; 
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
            
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback")
            {
                try
                {
                    var obj = new MGChooseAdvSlides();
                    var pk = int.Parse(((TextBox)((DetailsView)sender).FindControl("CASID")).Text);
                    obj.Fetch(pk);

                    obj.CAID = FormatHelper.SafeToInt(lblCAID.Text);
                    obj.MGID = FormatHelper.SafeToInt(lblMGID.Text);
                    //obj.Difficulty = FormatHelper.SafeToInt(lblDiff.Text);
                    //obj.StepNumber = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("StepNumber")).Text);
                    obj.SlideText = ((CKEditor.NET.CKEditorControl)((DetailsView)sender).FindControl("SlideText")).Text;
                    obj.FirstImageGoToStep = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("FirstImageGoToStep")).Text);
                    obj.SecondImageGoToStep = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("SecondImageGoToStep")).Text);

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

