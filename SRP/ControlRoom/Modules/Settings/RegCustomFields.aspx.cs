using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;



namespace GRA.SRP.ControlRoom.Modules.Settings
{
    public partial class RegCustomFields : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4100;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Custom Registration Fields");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SettingsRibbon());

                dv.ChangeMode(DetailsViewMode.Edit);
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var ctl = (DropDownList)dv.FindControl("DDValues1"); //this.FindControlRecursive(this, "BranchId");
                var lbl = (Label)dv.FindControl("DDValues1Lbl");
                var i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("DDValues2");
                lbl = (Label)dv.FindControl("DDValues2Lbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("DDValues3");
                lbl = (Label)dv.FindControl("DDValues3Lbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("DDValues4");
                lbl = (Label)dv.FindControl("DDValues4Lbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;

                ctl = (DropDownList)dv.FindControl("DDValues5");
                lbl = (Label)dv.FindControl("DDValues5Lbl");
                i = ctl.Items.FindByValue(lbl.Text);
                if (i != null) ctl.SelectedValue = lbl.Text;
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "Default.aspx";
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(returnURL);
            }
            if (e.CommandName.ToLower() == "codes")
            {
                Response.Redirect("Codes.aspx");
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
                    var obj = DAL.CustomRegistrationFields.FetchObject();
                    //int pk = 1;

                    obj.Label1 = ((TextBox)((DetailsView)sender).FindControl("Label1")).Text;
                    obj.Label2 = ((TextBox)((DetailsView)sender).FindControl("Label2")).Text;
                    obj.Label3 = ((TextBox)((DetailsView)sender).FindControl("Label3")).Text;
                    obj.Label4 = ((TextBox)((DetailsView)sender).FindControl("Label4")).Text;
                    obj.Label5 = ((TextBox)((DetailsView)sender).FindControl("Label5")).Text;

                    obj.Use1 = ((CheckBox)((DetailsView)sender).FindControl("Use1")).Checked;
                    obj.Use2 = ((CheckBox)((DetailsView)sender).FindControl("Use2")).Checked;
                    obj.Use3 = ((CheckBox)((DetailsView)sender).FindControl("Use3")).Checked;
                    obj.Use4 = ((CheckBox)((DetailsView)sender).FindControl("Use4")).Checked;
                    obj.Use5 = ((CheckBox)((DetailsView)sender).FindControl("Use5")).Checked;

                    obj.DDValues1 = ((DropDownList)((DetailsView)sender).FindControl("DDValues1")).SelectedValue;
                    obj.DDValues2 = ((DropDownList)((DetailsView)sender).FindControl("DDValues2")).SelectedValue;
                    obj.DDValues3 = ((DropDownList)((DetailsView)sender).FindControl("DDValues3")).SelectedValue;
                    obj.DDValues4 = ((DropDownList)((DetailsView)sender).FindControl("DDValues4")).SelectedValue;
                    obj.DDValues5 = ((DropDownList)((DetailsView)sender).FindControl("DDValues5")).SelectedValue;

                    obj.LastModDate = DateTime.Now;
                    obj.LastModUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session

                    if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        obj.Update();
                        if (!obj.Use1)
                        {
                            var o2 = new RegistrationSettings();
                            o2.Fetch(0);
                            o2.Custom1_Req = o2.Custom1_Edit = o2.Custom1_Prompt = o2.Custom1_Show = false;
                            o2.Update();
                        }
                        if (!obj.Use2)
                        {
                            var o2 = new RegistrationSettings();
                            o2.Fetch(0);
                            o2.Custom2_Req = o2.Custom2_Edit = o2.Custom2_Prompt = o2.Custom2_Show = false;
                            o2.Update();
                        }
                        if (!obj.Use3)
                        {
                            var o2 = new RegistrationSettings();
                            o2.Fetch(0);
                            o2.Custom3_Req = o2.Custom3_Edit = o2.Custom3_Prompt = o2.Custom3_Show = false;
                            o2.Update();
                        }
                        if (!obj.Use4)
                        {
                            var o2 = new RegistrationSettings();
                            o2.Fetch(0);
                            o2.Custom4_Req = o2.Custom4_Edit = o2.Custom4_Prompt = o2.Custom4_Show = false;
                            o2.Update();
                        }
                        if (!obj.Use5)
                        {
                            var o2 = new RegistrationSettings();
                            o2.Fetch(0);
                            o2.Custom5_Req = o2.Custom5_Edit = o2.Custom5_Prompt = o2.Custom5_Show = false;
                            o2.Update();
                        }


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

