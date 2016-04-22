using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRP_DAL;
using SRPApp.Classes;
using GRA.SRP.ControlRoom.Controls;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities;
using GRA.SRP.DAL;
using GRA.Tools;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class BadgeList : BaseControlRoomPage
    {
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4700;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = "Badges List";

            _mStrSortExp = String.Empty;

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

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

        protected void LoadData()
        {
            odsData.Select();
            gv.DataBind();
        }

        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {
            string editpage = "~/ControlRoom/Modules/Setup/BadgeAddEdit.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                Session["BDD"] = string.Empty;
                Response.Redirect(editpage);
            }
            if (e.CommandName.ToLower() == "editrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                Session["BDD"] = key;
                Response.Redirect(editpage);
            }
            if (e.CommandName.ToLower() == "deleterecord")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                try
                {

                    var obj = new Badge();
                    if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                    {
                        Badge.FetchObject(key).Delete();
                        new SessionTools(Session).RemoveCache(Cache, CacheKey.BadgesActive);
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
        }

        #region search/filter fields and buttons
        protected void Search(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchText.Text))
            {
                SearchText.Text = string.Empty;
            }

            var wt = new WebTools();
            if (!string.IsNullOrEmpty(SearchText.Text) || BranchId.SelectedIndex > 0)
            {
                BranchId.CssClass = wt.CssEnsureClass(BranchId.CssClass, "gra-search-active");
                SearchText.CssClass = wt.CssEnsureClass(SearchText.CssClass, "gra-search-active");
            }
            else
            {
                BranchId.CssClass = wt.CssRemoveClass(BranchId.CssClass, "gra-search-active");
                SearchText.CssClass = wt.CssRemoveClass(SearchText.CssClass, "gra-search-active");
            }
            LoadData();
        }

        protected void ClearSearch(object sender, EventArgs e)
        {
            SearchText.Text = string.Empty;
            BranchId.SelectedIndex = 0;
            var wt = new WebTools();

            BranchId.CssClass = wt.CssRemoveClass(BranchId.CssClass, "gra-search-active");
            SearchText.CssClass = wt.CssRemoveClass(SearchText.CssClass, "gra-search-active");
            LoadData();
        }
        #endregion search/filter fields and buttons
    }
}