using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRoom.Controls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities;
using GRA.SRP.DAL;


namespace GRA.SRP.ControlRoom.Modules.Settings
{
    public partial class SRPSettings : BaseControlRoomPage
    {
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 3000;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", lblModName.Text);

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SettingsRibbon());

                if (!string.IsNullOrEmpty(Request["MID"]))
                {
                    lblMID.Text = (string)Request["MID"];
                }
                else
                {
                    lblMID.Text = "-1";
                }
                if (!string.IsNullOrEmpty(Request["MName"]))
                {
                    lblModName.Text = (string)Request["MName"];
                }
                else
                {
                    lblModName.Text = "Summer Reading Program Settings";
                }

            }

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

        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "save")
            {
                DoSave();
            }

        }

        protected void btnBack_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btnRefresh_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            odsSRPSettings.Select();
            gv.DataBind();

            MasterPage.PageMessage = SRPResources.RefreshAllOK;
        }

        protected void btnSave_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DoSave();
        }

        private void DoSave()
        {
            //bool hasErrors = false;
            string message= string.Empty;
                foreach (GridViewRow row in gv.Rows)
                {
                    SettingEditor e = (SettingEditor)row.FindControl("uxSettingEditor");

                    var s = new DAL.SRPSettings();
                    s = s.GetSRPSettings(int.Parse(e.SID));
                    s.Value = e.Value;
                    try
                    {
                        if (s.IsValid(BusinessRulesValidationMode.UPDATE))
                        {
                            s.Update();
                        }
                        else
                        {
                            message = message + String.Format("Setting \"{0}\" has errors: {1}", s.Name, "<ul>");
                            foreach (BusinessRulesValidationMessage m in s.ErrorCodes)
                            {
                                message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                            }
                            message = string.Format("{0}</ul>", message);
                        }
                    }
                    catch (Exception ex)
                    {
                        message = message + String.Format("Setting \"{0}\" has errors: <ul><li>{1}</li></ul>", s.Name, ex.Message);

                    }
            }
            if (message.Length > 0)
                MasterPage.PageError = message;
            else
                MasterPage.PageMessage = SRPResources.SaveAllOK;

        }
    }
}

