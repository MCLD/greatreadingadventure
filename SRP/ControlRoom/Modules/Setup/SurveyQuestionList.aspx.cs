using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class SurveyQuestionList : BaseControlRoomPage
    {
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());
                SID.Text = Session["SID"] == null ? "" : Session["SID"].ToString();
                if (SID.Text == "") Response.Redirect("SurveyList.aspx");
                var s = Survey.FetchObject(int.Parse(SID.Text));
                lblSurvey.Text = string.Format("{0} - {1}", s.Name, s.LongName);
                if (s.Status > 1) ReadOnly.Text = "true";
            }

            MasterPage.RequiredPermission = 5200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Survey/Test Question List");

            _mStrSortExp = String.Empty;
            if (!IsPostBack)
            {
                _mStrSortExp = String.Empty;
            }
            else
            {
                if (null != ViewState["_SortExp_"])
                {
                    _mStrSortExp = ViewState["_SortExp_"] as String;
                }

                if (null != ViewState["_Direction_"])
                {
                    _mSortDirection = (SortDirection)ViewState["_Direction_"];
                }
            }
        }

        public string RenderHTML(string s)
        {
            s = s.HtmlStrip();
            if (s.Length > 200) s = s.Substring(0, 196) + "...";
            return s;
        }
        protected void GvSorting(object sender, GridViewSortEventArgs e)
        {
            if (String.Empty != _mStrSortExp)
            {
                if (String.Compare(e.SortExpression, _mStrSortExp, true) == 0)
                {
                    _mSortDirection =
                        (_mSortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
                }
            }
            ViewState["_Direction_"] = _mSortDirection;
            ViewState["_SortExp_"] = _mStrSortExp = e.SortExpression;
        }

        protected void GvRowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (String.Empty != _mStrSortExp)
                {
                    GlobalUtilities.AddSortImage(e.Row, (GridView)sender, _mStrSortExp, _mSortDirection);
                }
            }
        }

        protected void GvSelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void LoadData()
        {
            odsData.Select();
            gv.DataBind();
        }


        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {
            string editpage = "~/ControlRoom/Modules/Setup/SurveyQuestionAddEdit.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                var s = Survey.FetchObject(int.Parse(SID.Text));
                if (s.Status == 2)
                {
                    MasterPage.PageError = String.Format("<font color=red>{0}", "This Survey/Test is 'Locked / Active' and cannot be modified.");
                    return;
                }

                Response.Redirect("~/ControlRoom/Modules/Setup/SurveyQuestionAddWizard.aspx");
            }
            if (e.CommandName.ToLower() == "editrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                Session["QID"] = key; Response.Redirect(editpage);                
            }
            if (e.CommandName.ToLower() == "deleterecord")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var obj = SurveyQuestion.FetchObject(key);
                    if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                    {
                        obj.Delete();

                        LoadData();
                        var masterPage = (IControlRoomMaster)Master;
                        if (masterPage != null) masterPage.PageMessage = SRPResources.DeleteOK;
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
                    }
                }
                catch (Exception ex)
                {
                    var masterPage = (IControlRoomMaster)Master;
                    if (masterPage != null)
                        masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }

            if (e.CommandName.ToLower() == "moveup")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                SurveyQuestion.MoveUp(key);
                MasterPage.PageMessage = "Survey/Test Question Moved Up!";

                LoadData();
            }

            if (e.CommandName.ToLower() == "movedn")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                SurveyQuestion.MoveDn(key);
                MasterPage.PageMessage = "Survey/Test Question Moved Down";
                LoadData();
            }


            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect("~/ControlRoom/Modules/Setup/SurveyList.aspx");
            }

            if (e.CommandName.ToLower() == "preview")
            {
                Session["QNum"] = null;
                Session["TextReturnPage"] = "~/ControlRoom/Modules/Setup/SurveyQuestionList.aspx";
                Response.Redirect("~/ControlRoom/Modules/Setup/SurveyPreview.aspx");
            }

            if (e.CommandName.ToLower() == "results")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                Response.Redirect("~/ControlRoom/Modules/Setup/SurveyResults.aspx");
            }

        }
    }
}

