using System;
using System.Diagnostics;
using System.IO;
using System.Web.UI.WebControls;
using ExportToExcel;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;
using System.Web.UI.HtmlControls;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class SurveyQuestionAddEdit : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Survey/Test Question Edit");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                SID.Text = Session["SID"] == null ? "" : Session["SID"].ToString();
                if (SID.Text == "") Response.Redirect("SurveyList.aspx");
                var s = Survey.FetchObject(int.Parse(SID.Text));
                lblSurvey.Text = string.Format("{0} - {1}", s.Name, s.LongName);
                if (s.Status > 1) ReadOnly.Text = "true";

                //var j = Survey.GetNumQuestions(s.SID);
                //for (var i = 1; i <= j; i++)
                //{
                //    JumpToQuestion2.Items.Add(i.ToString());
                //    JumpToQuestion4.Items.Add(i.ToString());
                //}

                lblPK.Text = Session["QID"] == null ? "" : Session["QID"].ToString();
                dv.ChangeMode(lblPK.Text.Length == 0 ? DetailsViewMode.Insert : DetailsViewMode.Edit);
                Page.DataBind();

                if (Request["M"] == "K")
                {
                    MasterPage.DisplayMessageOnLoad = true;
                    MasterPage.PageMessage = "Item was saved successfully! ";
                }
            }
        }

        protected void Cancel()
        {
            Response.Redirect("SurveyQuestionList.aspx");
        }

        protected void dv_DataBound(object sender, EventArgs e)
        {
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                var tab1 = dv.FindControl("TabContainer1").FindControl("TabPanel1");
                var tab2 = dv.FindControl("TabContainer1").FindControl("TabPanel2");

                var QType = ((Label)dv.FindControl("QType")).Text.SafeToInt();

                switch (QType)
                {
                    case 1:
                        var p1 = (Panel)tab1.FindControl("pnlType1");
                        p1.Visible = true;
                        break;
                    case 2:
                        var p2 = (Panel)tab1.FindControl("pnlType2");
                        p2.Visible = true;

                        var ctl2 = (DropDownList)p2.FindControl("DisplayControl2"); 
                        var lbl2 = (Label)p2.FindControl("DisplayControl2Lbl");
                        var i = ctl2.Items.FindByValue(lbl2.Text);
                        if (i != null) ctl2.SelectedValue = lbl2.Text;

                        ctl2 = (DropDownList)p2.FindControl("DisplayDirection2");
                        lbl2 = (Label)p2.FindControl("DisplayDirection2Lbl");
                        i = ctl2.Items.FindByValue(lbl2.Text);
                        if (i != null) ctl2.SelectedValue = lbl2.Text;

                        tab2.Visible = true;
                        p2 = (Panel)tab2.FindControl("pnlType2Answers");
                        p2.Visible = true;

                        p2 = (Panel)tab2.FindControl("Panel1");
                        var dd2 = (DropDownList)p2.FindControl("JumpToQuestion2");
                        var j2 = Survey.GetNumQuestions(int.Parse(SID.Text));
                        for (var i2 = 1; i2 <= j2; i2++)
                        {
                            dd2.Items.Add(i2.ToString());
                        }

                        break;
                    case 3:
                        var p3 = (Panel)tab1.FindControl("pnlType3");
                        p3.Visible = true;
                        break;
                    case 4:
                        var p4 = (Panel)tab1.FindControl("pnlType4");
                        p4.Visible = true;

                        var ctl4 = (DropDownList)p4.FindControl("DisplayControl4");
                        var lbl4 = (Label)p4.FindControl("DisplayControl4Lbl");
                        var i4 = ctl4.Items.FindByValue(lbl4.Text);
                        if (i4 != null) ctl4.SelectedValue = lbl4.Text;

                        tab2.Visible = true;
                        p4 = (Panel)tab2.FindControl("pnlType4Answers");
                        p4.Visible = true;

                        p4 = (Panel)tab2.FindControl("Panel2");
                        var dd = (DropDownList)p4.FindControl("JumpToQuestion4");
                        var jj = Survey.GetNumQuestions(int.Parse(SID.Text));
                            for (var ii = 1; ii <= jj; ii++)
                            {
                                dd.Items.Add(ii.ToString());
                            }

                        break;
                }


            }
        }

        protected void dv_DataBinding(object sender, EventArgs e)
        {
        }

        protected void DvItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string returnURL = "~/ControlRoom/Modules/Setup/SurveyQuestionList.aspx";
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
                    if (Save((DetailsView) sender))
                    {
                        if (e.CommandName.ToLower() == "saveandback")
                        {
                            Response.Redirect(returnURL);
                        }

                        odsData.DataBind();
                        dv.DataBind();
                        dv.ChangeMode(DetailsViewMode.Edit);

                        MasterPage.PageMessage = SRPResources.SaveOK;
                    }

                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }

            if (e.CommandName.ToLower() == "moveup21")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                SQChoices.MoveUp(key);
                //MasterPage.PageMessage = "Survey/Test Question Moved Up!";

                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");
                odsData41.DataBind();
                var p4 = (Panel)tab2.FindControl("pnlType2Answers");
                p4 = (Panel)p4.FindControl("Panel1");
                var gv = (GridView)p4.FindControl("gv21");
                gv.DataBind();
            }

            if (e.CommandName.ToLower() == "movedn21")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                SQChoices.MoveDn(key);
                //MasterPage.PageMessage = "Survey/Test Question Moved Down";
                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");
                odsData41.DataBind();
                var p4 = (Panel)tab2.FindControl("pnlType2Answers");
                p4 = (Panel)p4.FindControl("Panel1");
                var gv = (GridView)p4.FindControl("gv21");
                gv.DataBind();
            }

            if (e.CommandName.ToLower() == "deleterecord21")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                var obj = SQChoices.FetchObject(key);
                obj.Delete();
                //MasterPage.PageMessage = "Survey/Test Question Moved Down";
                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");
                odsData41.DataBind();
                var p4 = (Panel)tab2.FindControl("pnlType2Answers");
                p4 = (Panel)p4.FindControl("Panel1");
                var gv = (GridView)p4.FindControl("gv21");
                gv.DataBind();
            }

            if (e.CommandName.ToLower() == "addrecord21")
            {
                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");
                var p4 = (Panel)tab2.FindControl("pnlType2Answers");

                var obj = new SQChoices();
                obj.QID = int.Parse(lblPK.Text);
                obj.ChoiceText = ((TextBox)p4.FindControl("ChoiceText2")).Text;
                obj.Score = ((TextBox)p4.FindControl("Score2")).Text.SafeToInt();
                obj.JumpToQuestion = ((DropDownList)p4.FindControl("JumpToQuestion2")).Text.SafeToInt();
                obj.AskClarification = ((CheckBox)p4.FindControl("AskClarification2")).Checked;
                obj.ClarificationRequired = ((CheckBox)p4.FindControl("ClarificationRequired2")).Checked;
                obj.Insert();
                //MasterPage.PageMessage = "Survey/Test Question Moved Down";
                odsData41.DataBind();
                p4 = (Panel)p4.FindControl("Panel1");
                var gv = (GridView)p4.FindControl("gv21");
                gv.DataBind();
            }


            if (e.CommandName.ToLower() == "moveup41")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                SQChoices.MoveUp(key);
                //MasterPage.PageMessage = "Survey/Test Question Moved Up!";

                var tab2 = ((DetailsView) sender).FindControl("TabContainer1").FindControl("TabPanel2");
                odsData41.DataBind();
                var p4 = (Panel)tab2.FindControl("pnlType4Answers");
                p4 = (Panel)p4.FindControl("Panel2");
                var gv = (GridView) p4.FindControl("gv41");
                gv.DataBind();
            }

            if (e.CommandName.ToLower() == "movedn41")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                SQChoices.MoveDn(key);
                //MasterPage.PageMessage = "Survey/Test Question Moved Down";
                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");
                odsData41.DataBind();
                var p4 = (Panel)tab2.FindControl("pnlType4Answers");
                p4 = (Panel)p4.FindControl("Panel2");
                var gv = (GridView)p4.FindControl("gv41");
                gv.DataBind();
            }

            if (e.CommandName.ToLower() == "deleterecord41")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                var obj = SQChoices.FetchObject(key);
                obj.Delete();
                //MasterPage.PageMessage = "Survey/Test Question Moved Down";
                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");
                odsData41.DataBind();
                var p4 = (Panel)tab2.FindControl("pnlType4Answers");
                p4 = (Panel)p4.FindControl("Panel2");
                var gv = (GridView)p4.FindControl("gv41");
                gv.DataBind();
            }

            if (e.CommandName.ToLower() == "addrecord41")
            {
                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");
                var p4 = (Panel)tab2.FindControl("pnlType4Answers");

                var obj = new SQChoices();
                obj.QID = int.Parse(lblPK.Text);
                obj.ChoiceText = ((TextBox)p4.FindControl("ChoiceText4")).Text;
                obj.Score = ((TextBox)p4.FindControl("Score4")).Text.SafeToInt();
                obj.JumpToQuestion = ((DropDownList)p4.FindControl("JumpToQuestion4")).Text.SafeToInt();

                obj.Insert();
                //MasterPage.PageMessage = "Survey/Test Question Moved Down";
                odsData41.DataBind();
                p4 = (Panel)p4.FindControl("Panel2");
                var gv = (GridView)p4.FindControl("gv41");
                gv.DataBind();
            }







            if (e.CommandName.ToLower() == "moveupl")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                SQMatrixLines.MoveUp(key);
                //MasterPage.PageMessage = "Survey/Test Question Moved Up!";
                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");
                odsData42.DataBind();
                var p4 = (Panel)tab2.FindControl("pnlType4Answers");
                p4 = (Panel)p4.FindControl("Panel2");
                var gv = (GridView)p4.FindControl("gv42");
                gv.DataBind();
            }

            if (e.CommandName.ToLower() == "movednl")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                SQMatrixLines.MoveDn(key);
                //MasterPage.PageMessage = "Survey/Test Question Moved Down";
                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");
                odsData42.DataBind();
                var p4 = (Panel)tab2.FindControl("pnlType4Answers");
                p4 = (Panel)p4.FindControl("Panel2");
                var gv = (GridView)p4.FindControl("gv42");
                gv.DataBind();
            }



            if (e.CommandName.ToLower() == "deleterecordl")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                var obj = SQMatrixLines.FetchObject(key);
                obj.Delete();
                //MasterPage.PageMessage = "Survey/Test Question Moved Down";
                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");
                odsData42.DataBind();
                var p4 = (Panel)tab2.FindControl("pnlType4Answers");
                p4 = (Panel)p4.FindControl("Panel2");
                var gv = (GridView)p4.FindControl("gv42");
                gv.DataBind();
            }

            if (e.CommandName.ToLower() == "addrecordl")
            {
                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");

                var p4 = (Panel)tab2.FindControl("pnlType4Answers");

                var obj = new SQMatrixLines();
                obj.QID = int.Parse(lblPK.Text);
                obj.LineText = ((TextBox)p4.FindControl("LineText4")).Text;

                obj.Insert();
                //MasterPage.PageMessage = "Survey/Test Question Moved Down";

                odsData42.DataBind();
                p4 = (Panel)p4.FindControl("Panel2");
                var gv = (GridView)p4.FindControl("gv42");
                gv.DataBind();
            }

        }


        protected bool Save(DetailsView sender)
        {
            try
            {
                var tab1 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel1");
                var tab2 = ((DetailsView)sender).FindControl("TabContainer1").FindControl("TabPanel2");

                var obj = new DAL.SurveyQuestion();
                int pk = int.Parse(lblPK.Text);
                obj.Fetch(pk);

                switch (obj.QType)
                {
                    case 1:
                        obj.QText = ((HtmlTextArea)((Panel)tab1.FindControl("pnlType1")).FindControl("QText")).InnerHtml;
                        break;
                    case 2:
                        var p2 = (Panel)tab1.FindControl("pnlType2");
                        obj.QText = ((HtmlTextArea)p2.FindControl("QText2")).InnerHtml;
                        obj.QName = ((TextBox)p2.FindControl("QName2")).Text;
                        obj.IsRequired = ((CheckBox)p2.FindControl("IsRequired2")).Checked;
                        obj.DisplayControl = ((DropDownList)p2.FindControl("DisplayControl2")).SelectedValue.SafeToInt();
                        obj.DisplayDirection = ((DropDownList)p2.FindControl("DisplayDirection2")).SelectedValue.SafeToInt();
                        break;
                    case 3:
                        var p3 = (Panel) tab1.FindControl("pnlType3");
                        obj.QText = ((HtmlTextArea)p3.FindControl("QText3")).InnerHtml;
                        obj.QName = ((TextBox)p3.FindControl("QName3")).Text; 
                        obj.IsRequired = ((CheckBox)p3.FindControl("IsRequired3")).Checked;
                        break;
                    case 4:
                        var p4 = (Panel) tab1.FindControl("pnlType4");
                        obj.QText = ((HtmlTextArea)p4.FindControl("QText4")).InnerHtml;
                        obj.QName = ((TextBox)p4.FindControl("QName4")).Text; 
                        obj.IsRequired = ((CheckBox)p4.FindControl("IsRequired4")).Checked;
                        obj.DisplayControl = ((DropDownList)p4.FindControl("DisplayControl4")).SelectedValue.SafeToInt();
                        break;
                }


                if (obj.IsValid(BusinessRulesValidationMode.UPDATE))
                {
                    obj.Update();

                    //switch (obj.QType)
                    //{
                    //    case 2:
                    //        SaveType2Answers(obj.QID, tab2);
                    //        break;
                    //    case 4:
                    //        SaveType4Answers(obj.QID, tab2);
                    //        SaveType4Lines(obj.QID, tab2);
                    //        break;
                    //}

                    return true;
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
                    if (masterPage != null) masterPage.PageError = message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                var masterPage = (IControlRoomMaster)Master;
                if (masterPage != null)
                    masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                return false;
            }
        }            
    }
}