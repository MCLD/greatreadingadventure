using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class MGHiddenPicBkAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4300;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Background/Coloring Page Edit");
            
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            
                if (Request["HPID"] != null)
                {
                    lblMGID.Text = Session["MGID"].ToString();
                    lblHPID.Text = Request["HPID"];

                    var o = Minigame.FetchObject(int.Parse(lblMGID.Text));
                    AdminName.Text = o.AdminName;

                    var obj = new MGHiddenPicBk();
                    obj.HPID = int.Parse(lblHPID.Text);
                    obj.MGID = int.Parse(lblMGID.Text);
                    obj.AddedDate = DateTime.Now;
                    obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                    obj.LastModDate = obj.AddedDate;
                    obj.LastModUser = obj.AddedUser;
                    obj.Insert();
                    lblPK.Text = obj.HPBID.ToString();

                    dv.ChangeMode(DetailsViewMode.Edit);

                }
                else
                {
                    lblPK.Text = Request["PK"];

                    var o1 = MGHiddenPicBk.FetchObject(int.Parse(lblPK.Text));
                    lblMGID.Text = o1.MGID.ToString();
                    lblHPID.Text = o1.HPID.ToString();

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
                var control = (GRA.SRP.Classes.FileDownloadCtl)dv.FindControl("FileUploadCtl_1");
                if (control != null) control.ProcessRender();
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            //var returnURL = "~/ControlRoom/Modules/Setup/MGHiddenPicBkList.aspx?MGID=" + lblMGID.Text;
            var returnURL = "~/ControlRoom/Modules/Setup/MGHiddenPicBkList.aspx";
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
                    var obj = new MGHiddenPicBk();
                    var pk = int.Parse(((TextBox)((DetailsView)sender).FindControl("HPBID")).Text);
                    obj.Fetch(pk);

                    obj.HPID = FormatHelper.SafeToInt(lblHPID.Text);
                    obj.MGID = FormatHelper.SafeToInt(lblMGID.Text);

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

