using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class SurveyQuestionAddWizard : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
                SID.Text = Session["SID"] == null ? "" : Session["SID"].ToString();
                if (SID.Text == "") Response.Redirect("SurveyList.aspx");
                var s = Survey.FetchObject(int.Parse(SID.Text));
                lblSurvey.Text = string.Format("{0} - {1}", s.Name, s.LongName);
                var j = Survey.GetNumQuestions(s.SID);
                for (var i = 1; i <= j; i++)
                {
                    JumpToQuestion2.Items.Add(i.ToString());
                    JumpToQuestion4.Items.Add(i.ToString());
                }
                Session["tmpQ2"] = null;
                Session["tmpC2"] = 0;

                Session["tmpQ4"] = null;
                Session["tmpC4"] = 0;

                Session["tmpL4"] = null;
                Session["tmpLC4"] = 0;
            }
            MasterPage.RequiredPermission = 5200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Survey/Test Question Add");
        }

        protected void QType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (QType.SelectedValue)
            {
                case "1":
                    pnlType1.Visible = true;
                    pnlEnd.Visible = false;
                    pnlType3.Visible = false;
                    pnlType2.Visible = false;
                    pnlType4.Visible = false;
                    pnlType2Answers.Visible = false;
                    pnlType4Answers.Visible = false;
                    break;
                case "2":
                    pnlType1.Visible = false;
                    pnlEnd.Visible = false;
                    pnlType3.Visible = false;
                    pnlType2.Visible = true;
                    pnlType4.Visible = false;
                    pnlType2Answers.Visible = false;
                    pnlType4Answers.Visible = false;
                    break;
                case "3":
                    pnlType1.Visible = false;
                    pnlEnd.Visible = false;
                    pnlType3.Visible = true;
                    pnlType2.Visible = false;
                    pnlType4.Visible = false;
                    pnlType2Answers.Visible = false;
                    pnlType4Answers.Visible = false;
                    break;
                case "4":
                    pnlType1.Visible = false;
                    pnlEnd.Visible = false;
                    pnlType3.Visible = false;
                    pnlType2.Visible = false;
                    pnlType4.Visible = true;
                    pnlType2Answers.Visible = false;
                    pnlType4Answers.Visible = false;
                    break;
                case "5":
                case "6":
                    pnlType1.Visible = false;
                    pnlEnd.Visible = true;
                    pnlType3.Visible = false;
                    pnlType2.Visible = false;
                    pnlType4.Visible = false;
                    pnlType2Answers.Visible = false;
                    pnlType4Answers.Visible = false;
                    break;
                default: 
                    pnlType1.Visible = false;
                    pnlEnd.Visible = false;
                    pnlType3.Visible = false;
                    pnlType2.Visible = false;
                    pnlType4.Visible = false;
                    pnlType2Answers.Visible = false;
                    pnlType4Answers.Visible = false;
                    break;
            } 
        }

        protected bool Save()
        {
                try
                {
                    var obj = new SurveyQuestion();
                    obj.QType = QType.SelectedValue.SafeToInt();
                    obj.QNumber = -1;
                    obj.SID = SID.Text.SafeToInt();
                    switch (obj.QType)
                    {
                        case 1:
                            obj.QText = QText.InnerHtml;
                            obj.QName = "Instructions/Text/Description";
                            break;
                        case 5:
                        case 6:
                            obj.QText= string.Empty;
                            obj.QName= string.Empty;
                            break;
                        case 3:
                            obj.QText = QText3.InnerHtml;
                            obj.QName = QName3.Text;
                            obj.IsRequired = IsRequired3.Checked;
                            break;
                        case 2:
                            obj.QText = QText2.InnerHtml;
                            obj.QName = QName2.Text;
                            obj.IsRequired = IsRequired2.Checked;
                            obj.DisplayControl = DisplayControl2.SelectedValue.SafeToInt();
                            obj.DisplayDirection = DisplayDirection2.SelectedValue.SafeToInt();
                            break;
                        case 4:
                            obj.QText = QText4.InnerHtml;
                            obj.QName = QName4.Text;
                            obj.IsRequired = IsRequired4.Checked;
                            obj.DisplayControl = DisplayControl4.SelectedValue.SafeToInt();
                            break;
                    }
                  

                    if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                    {
                        obj.Insert();

                        switch (obj.QType)
                        {
                            case 2:
                                SaveType2Answers(obj.QID);
                                break;
                            case 4:
                                SaveType4Answers(obj.QID);
                                SaveType4Lines(obj.QID);
                                break;
                        }

                        Session["QID"]  = obj.QID;

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
        
        protected void SaveType2Answers(int QID)
        {
            if (Session["tmpQ2"] == null) return;
            var ds = (DataSet)Session["tmpQ2"];
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                var obj = new SQChoices();
                obj.QID = QID;
                obj.ChoiceText = r["ChoiceText"].ToString();
                obj.Score = (int) r["Score"];
                obj.JumpToQuestion = (int)r["JumpToQuestion"];
                obj.AskClarification = (bool)r["AskClarification"];
                obj.ClarificationRequired = (bool)r["ClarificationRequired"];

                obj.Insert();
            }            
        }

        protected void SaveType4Answers(int QID)
        {
            if (Session["tmpQ4"] == null) return;
            var ds = (DataSet)Session["tmpQ4"];
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                var obj = new SQChoices();
                obj.QID = QID;
                obj.ChoiceText = r["ChoiceText"].ToString();
                obj.Score = (int)r["Score"];
                obj.JumpToQuestion = (int)r["JumpToQuestion"];

                obj.Insert();
            }
        }

        protected void SaveType4Lines(int QID)
        {
            if (Session["tmpL4"] == null) return;
            var ds = (DataSet)Session["tmpL4"];
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                var obj = new SQMatrixLines();
                obj.QID = QID;
                obj.LineText = r["LineText"].ToString();

                obj.Insert();
            }
        }

        protected void Cancel()
        {
            Response.Redirect("SurveyQuestionList.aspx");
        }

        //----------------------------------------------------------------

        protected void btnCancel0_Click(object sender, ImageClickEventArgs e)
        {
            Cancel();
        }

        protected void btnSave0_Click(object sender, ImageClickEventArgs e)
        {
            Save();
            Response.Redirect("SurveyQuestionList.aspx");
        }

        protected void btnSave1_Click(object sender, ImageClickEventArgs e)
        {
            Save();
            Response.Redirect("SurveyQuestionAddEdit.aspx?M=K");
        }

        protected void btnSave3_Click(object sender, ImageClickEventArgs e)
        {
            if (Save()) Response.Redirect("SurveyQuestionAddEdit.aspx?M=K");
        }

        protected void btnContinue2_Click(object sender, ImageClickEventArgs e)
        {
            pnlType2.Visible = false;
            pnlType2Answers.Visible = true;
        }

        protected void btnContinue4_Click(object sender, ImageClickEventArgs e)
        {
            pnlType4.Visible = false;
            pnlType4Answers.Visible = true;
        }

        protected void btnPrevious22_Click(object sender, ImageClickEventArgs e)
        {
            pnlType2.Visible = true;
            pnlType2Answers.Visible = false;
        }

        protected void btnSave22_Click(object sender, ImageClickEventArgs e)
        {
            if (!Save()) return;
            Session["tmpQ2"] = null;
            Session["tmpC2"] = 0;
            Response.Redirect("SurveyQuestionAddEdit.aspx?M=K");
        }

        protected void btnPrevious44_Click(object sender, ImageClickEventArgs e)
        {
            pnlType4.Visible = true;
            pnlType4Answers.Visible = false;
        }

        protected void btnSave44_Click(object sender, ImageClickEventArgs e)
        {
            if (!Save()) return;
            Session["tmpQ4"] = null;
            Session["tmpC4"] = 0;
            Session["tmpL4"] = null;
            Session["tmpLC4"] = 0;
            Response.Redirect("SurveyQuestionAddEdit.aspx?M=K");
        }

        protected void btnAddAnswer2_Click(object sender, EventArgs e)
        {


            if (Session["tmpQ2"] == null)
            {
                Session["tmpQ2"] = SQChoices.GetAll(-1);
            }
            var tmpC2 = (int) Session["tmpC2"];
            tmpC2++;
            var ds = (DataSet) Session["tmpQ2"];
            DataRow newRow = ds.Tables[0].NewRow();
                newRow["ChoiceOrder"] = tmpC2;
                newRow["ChoiceText"] = ChoiceText2.Text;
                newRow["Score"] = Score2.Text.SafeToInt();
                newRow["JumpToQuestion"] = JumpToQuestion2.Text.SafeToInt();
                newRow["AskClarification"] = AskClarification2.Checked;
                newRow["ClarificationRequired"] = ClarificationRequired2.Checked;
            ds.Tables[0].Rows.Add(newRow);

            Session["tmpQ2"] = ds;
            Session["tmpC2"] = tmpC2;
            
            rptrAnswers2.DataSource = ds;
            rptrAnswers2.DataBind();

            ChoiceText2.Text= string.Empty;
            Score2.Text= string.Empty;
            JumpToQuestion2.SelectedValue = "0";
            AskClarification2.Checked = false;
            ClarificationRequired2.Checked = false;
        }

        protected void rptrAnswers2_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "delete")
            {
                int k = int.Parse(e.CommandArgument.ToString());
                var tmpC2 = (int)Session["tmpC2"];
                tmpC2--;
                int i = -1;
                int j = -1;
                var ds = (DataSet)Session["tmpQ2"];
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    i++;
                    if ((int) r["ChoiceOrder"] == k)
                    {
                        j = i;
                    }
                    if ((int) r["ChoiceOrder"] > k)
                    {
                        r["ChoiceOrder"] = (int) r["ChoiceOrder"] - 1;
                    }
                }
                ds.Tables[0].Rows[j].Delete();

                Session["tmpQ2"] = ds;
                Session["tmpC2"] = tmpC2;

                rptrAnswers2.DataSource = ds;
                rptrAnswers2.DataBind();
            }
        }

        protected void btnAddAnswer4_Click(object sender, EventArgs e)
        {


            if (Session["tmpQ4"] == null)
            {
                Session["tmpQ4"] = SQChoices.GetAll(-1);
            }
            var tmpC4 = (int)Session["tmpC4"];
            tmpC4++;
            var ds = (DataSet)Session["tmpQ4"];
            DataRow newRow = ds.Tables[0].NewRow();
            newRow["ChoiceOrder"] = tmpC4;
            newRow["ChoiceText"] = ChoiceText4.Text;
            newRow["Score"] = Score4.Text.SafeToInt();
            newRow["JumpToQuestion"] = JumpToQuestion4.Text.SafeToInt();
            ds.Tables[0].Rows.Add(newRow);

            Session["tmpQ4"] = ds;
            Session["tmpC4"] = tmpC4;

            rptrAnswers4.DataSource = ds;
            rptrAnswers4.DataBind();

            ChoiceText4.Text= string.Empty;
            Score4.Text= string.Empty;
            JumpToQuestion4.SelectedValue = "0";
        }

        protected void rptrAnswers4_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "delete")
            {
                int k = int.Parse(e.CommandArgument.ToString());
                var tmpC4 = (int)Session["tmpC4"];
                tmpC4--;
                int i = -1;
                int j = -1;
                var ds = (DataSet)Session["tmpQ4"];
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    i++;
                    if ((int)r["ChoiceOrder"] == k)
                    {
                        j = i;
                    }
                    if ((int)r["ChoiceOrder"] > k)
                    {
                        r["ChoiceOrder"] = (int)r["ChoiceOrder"] - 1;
                    }
                }
                ds.Tables[0].Rows[j].Delete();

                Session["tmpQ4"] = ds;
                Session["tmpC4"] = tmpC4;

                rptrAnswers4.DataSource = ds;
                rptrAnswers4.DataBind();
            }
        }

        protected void btnAddLines4_Click(object sender, EventArgs e)
        {


            if (Session["tmpL4"] == null)
            {
                Session["tmpL4"] = SQMatrixLines.GetAll(-1);
            }
            var tmpLC4 = (int)Session["tmpLC4"];
            tmpLC4++;
            var ds = (DataSet)Session["tmpL4"];
            DataRow newRow = ds.Tables[0].NewRow();
            newRow["LineOrder"] = tmpLC4;
            newRow["LineText"] = LineText4.Text;
            ds.Tables[0].Rows.Add(newRow);

            Session["tmpL4"] = ds;
            Session["tmpLC4"] = tmpLC4;

            rptrLines4.DataSource = ds;
            rptrLines4.DataBind();

            LineText4.Text= string.Empty;
        }

        protected void rptrLines4_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "delete")
            {
                int k = int.Parse(e.CommandArgument.ToString());
                var tmpLC4 = (int)Session["tmpLC4"];
                tmpLC4--;
                int i = -1;
                int j = -1;
                var ds = (DataSet)Session["tmpL4"];
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    i++;
                    if ((int)r["LineOrder"] == k)
                    {
                        j = i;
                    }
                    if ((int)r["LineOrder"] > k)
                    {
                        r["LineOrder"] = (int)r["LineOrder"] - 1;
                    }
                }
                ds.Tables[0].Rows[j].Delete();

                Session["tmpL4"] = ds;
                Session["tmpLC4"] = tmpLC4;

                rptrLines4.DataSource = ds;
                rptrLines4.DataBind();
            }
        }
        //----------------------------------------------------------------

    }
}