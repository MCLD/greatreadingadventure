using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities;

namespace GRA.SRP.ControlRoom.Modules.Security
{
    public partial class LoginHistory : BaseControlRoomPage
    {
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = "User Login History";

            lblUID.Text = Session["UID"] == null ? "" : Session["UID"].ToString(); //Session["UID"]= string.Empty;
            if (lblUID.Text == "") Response.Redirect("~/ControlRoom/");
            if (!IsPostBack)
            {
                //lblUID.Text = Request["UID"].ToString();
                var user = new SRPUser(int.Parse(lblUID.Text));
                lblUsername.Text = user.Username;
                lblName.Text = user.FirstName + " " + user.LastName;
                lblUsername.Visible = lblName.Visible = true;
            }
            ControlRoomAccessPermission.CheckControlRoomAccessPermission(1000); // User Security;

            if (!IsPostBack)
            {
                List<RibbonPanel> moduleRibbonPanels = StandardModuleRibbons.SecurityRibbon();
                foreach (var moduleRibbonPanel in moduleRibbonPanels)
                {
                    MasterPage.PageRibbon.Add(moduleRibbonPanel);
                }
                MasterPage.PageRibbon.DataBind();
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

            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            var ds = new DataSet();
            var dt = SRPUser.GetLoginHistory(int.Parse(lblUID.Text));
            ds.Tables.Add(dt);
            if (!string.IsNullOrEmpty(_mStrSortExp))
            {
                dt.DefaultView.Sort = _mStrSortExp + (_mSortDirection == SortDirection.Descending ? " DESC" : "");
                var ds2 = new DataSet();
                ds2.Tables.Add(dt.DefaultView.ToTable());
                ds = ds2;
            }
            gv.DataSource = ds;
            gv.DataBind();
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
            LoadData();
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

        }

        protected void gv_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            LoadData();
            gv.PageIndex = e.NewPageIndex;
            gv.DataBind(); 
        }

    }
}