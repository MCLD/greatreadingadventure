using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

// --> PERMISSIONID 
namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class ProgramGameLevelList : BaseControlRoomPage
    {
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4300;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Board Game Levels List");

            _mStrSortExp = String.Empty;
            
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                if (Request["PGID"] != null)
                {
                    odsDDPG.DataBind();
                    PGID.DataBind();
                    var i = PGID.Items.FindByValue(Request["PGID"].ToString());
                    if (i != null)
                    {
                        PGID.SelectedValue = Request["PGID"].ToString();
                        Session["PGL_PGID"] = Request["PGID"].ToString();
                    }
                    SetFilterSessionValues();
                }
                pnlList.Visible = btnBoard.Visible = (PGID.SelectedValue != "0");
                GameName.Text = PGID.SelectedItem.Text;
                GetFilterSessionValues();               
            
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

        public void SetFilterSessionValues()
        {
            Session["PGL_PGID"] = PGID.SelectedValue;
            Session["PGL_Filtered"] = "1";
        }

        public void GetFilterSessionValues()
        {
            if (Session["PGL_PGID"] != null) try { PGID.SelectedValue = Session["PGL_PGID"].ToString(); GameName.Text = PGID.SelectedItem.Text; }
                catch (Exception) { }
        }

        public bool WasFiltered()
        {
            return (Session["PGL_Filtered"] != null);
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            SetFilterSessionValues();
            odsData.DataBind();
            gv.DataBind();
            pnlList.Visible = btnBoard.Visible = (PGID.SelectedValue != "0");
            
            GameName.Text = PGID.SelectedItem.Text;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            PGID.SelectedValue = "0";
            SetFilterSessionValues();
            odsData.DataBind();
            gv.DataBind();
            pnlList.Visible = btnBoard.Visible = (PGID.SelectedValue != "0");
            GameName.Text = PGID.SelectedItem.Text;
        }

        protected void LoadData()
        {
            odsData.Select();
            gv.DataBind();
            GameName.Text = PGID.SelectedItem.Text;
            pnlList.Visible = btnBoard.Visible = (PGID.SelectedValue != "0");
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
            string editpage = "~/ControlRoom/Modules/Setup/ProgramGameLevelAddEdit.aspx";
            if (e.CommandName.ToLower() == "moveup")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                DAL.ProgramGameLevel.MoveUp(key);
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = "Level Moved Up!";
                LoadData();
            }

            if (e.CommandName.ToLower() == "movedn")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                DAL.ProgramGameLevel.MoveDn(key);
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = "Level Moved Down";
                LoadData();
            }

            if (e.CommandName.ToLower() == "addrecord")
            {
                Response.Redirect(editpage + "?PK2=" + PGID.SelectedValue);
            }
            if (e.CommandName.ToLower() == "editrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                Response.Redirect(String.Format("{0}?PK={1}", editpage, key));
            }
            if (e.CommandName.ToLower() == "deleterecord")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var obj = new ProgramGameLevel();
                    if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                    {
                        ProgramGameLevel.FetchObject(key).Delete();

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

        protected void btnBoard_Click(object sender, EventArgs e)
        {
            Response.Redirect("BoardGameAddEdit.aspx?PK=" + PGID.SelectedValue);
        }
    }
}

