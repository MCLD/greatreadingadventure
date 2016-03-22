using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using System.Web.UI.HtmlControls;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class SurveyAddEdit : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
            }

            MasterPage.RequiredPermission = 5200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Test/Survey Add / Edit");

            if (!IsPostBack)
            {
                lblPK.Text = Session["SID"] == null ? "" : Session["SID"].ToString();
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();
            }
        }


        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var ctl = (DropDownList)dv.FindControl("Status");
                var lbl = (Label)dv.FindControl("StatusLbl");
                if (ctl != null)
                {
                    var i = ctl.Items.FindByValue(lbl.Text);
                    if (i != null)
                    {
                        ctl.SelectedValue = lbl.Text;
                    }
                }
                ctl = (DropDownList)dv.FindControl("BadgeID");
                lbl = (Label)dv.FindControl("BadgeIDLbl");
                if(ctl != null)
                {
                    var i = ctl.Items.FindByValue(lbl.Text);
                    if(i != null)
                    {
                        ctl.SelectedValue = lbl.Text;
                    }
                }
            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }


        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/SurveyList.aspx";
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
                    if (masterPage != null)
                        masterPage.PageMessage = SRPResources.RefreshOK;
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
                    var survey = new Survey();
                    //obj.GenNotificationFlag = ((CheckBox)((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2").FindControl("GenNotificationFlag")).Checked;

                    survey.Name = ((TextBox)((DetailsView)sender).FindControl("Name")).Text;
                    survey.LongName = ((TextBox)((DetailsView)sender).FindControl("LongName")).Text;
                    survey.Description = ((TextBox)((DetailsView)sender).FindControl("Description")).Text;
                    survey.Preamble = ((HtmlTextArea)((DetailsView)sender).FindControl("Preamble")).InnerHtml;
                    survey.Status = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("Status")).SelectedValue);
                    survey.BadgeId = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BadgeID")).SelectedValue);
                    //obj.TakenCount = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("TakenCount")).Text);
                    //obj.PatronCount = FormatHelper.SafeToInt(((TextBox)((DetailsView)sender).FindControl("PatronCount")).Text);
                    //obj.CanBeScored = ((CheckBox)((DetailsView)sender).FindControl("CanBeScored")).Checked;

                    if (survey.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        survey.Insert();
                        if (e.CommandName.ToLower() == "addandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        lblPK.Text = survey.SID.ToString();
                        Session["SID"] = survey.SID;

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.AddedOK;
                    }
                    else {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in survey.ErrorCodes)
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
            if (e.CommandName.ToLower() == "save" || e.CommandName.ToLower() == "saveandback" || e.CommandName.ToLower() == "questions")
            {
                try
                {
                    var survey = new Survey();
                    int pk = int.Parse(lblPK.Text);
                    survey.Fetch(pk);

                    survey.Name = ((TextBox)((DetailsView)sender).FindControl("Name")).Text;
                    survey.LongName = ((TextBox)((DetailsView)sender).FindControl("LongName")).Text;
                    survey.Description = ((TextBox)((DetailsView)sender).FindControl("Description")).Text;
                    survey.Preamble = ((HtmlTextArea)((DetailsView)sender).FindControl("Preamble")).InnerHtml;
                    survey.Status = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("Status")).SelectedValue);
                    survey.BadgeId = FormatHelper.SafeToInt(((DropDownList)((DetailsView)sender).FindControl("BadgeID")).SelectedValue);

                    if (survey.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        survey.Update();
                        if (e.CommandName.ToLower() == "saveandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        if (e.CommandName.ToLower() == "questions")
                        {
                            Response.Redirect("~/ControlRoom/Modules/Setup/SurveyQuestionList.aspx");
                        }

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        var masterPage = (IControlRoomMaster)Master;
                        masterPage.PageMessage = SRPResources.SaveOK;
                    }
                    else {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in survey.ErrorCodes)
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

            if (e.CommandName.ToLower() == "results")
            {
                Response.Redirect("SurveyResults.aspx");
            }

            if (e.CommandName.ToLower() == "embed")
            {
                Response.Redirect("SurveyEmbedCode.aspx");
            }
        }

        protected void LoadBadgeSearch()
        {
            var odsBadge = dv.FindControl("odsBadge") as ObjectDataSource;
            odsBadge.Select();
            var BadgeID = dv.FindControl("BadgeID") as DropDownList;
            BadgeID.Items.Clear();
            BadgeID.Items.Add(new ListItem("[Select a Badge]", "0"));
            BadgeID.DataBind();
        }

        protected void Search(object sender, EventArgs e)
        {
            LoadBadgeSearch();
        }
    }
}

