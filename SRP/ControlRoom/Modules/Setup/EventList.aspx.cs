using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;


namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class EventList : BaseControlRoomPage
    {
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4500;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Events List");

            _mStrSortExp = String.Empty;

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                if (WasFiltered())
                {
                    GetFilterSessionValues();
                }
            
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
            Session["EL_Start"] = StartDate.Text;
            Session["EL_End"] = EndDate.Text;
            Session["EL_Branch"] = BranchId.SelectedValue;
            Session["EL_Filtered"] = "1";
        }

        public void GetFilterSessionValues()
        {
            if (Session["EL_Start"] != null) StartDate.Text = Session["EL_Start"].ToString();
            if (Session["EL_End"] != null) EndDate.Text = Session["EL_End"].ToString();
            if (Session["EL_Branch"] != null) try { BranchId.SelectedValue = Session["EL_Branch"].ToString(); }
                catch (Exception) { }
        }

        public bool WasFiltered()
        {
            return (Session["EL_Filtered"] != null);
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            SetFilterSessionValues();
            odsData.DataBind();
            gv.DataBind();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            StartDate.Text = EndDate.Text= string.Empty;
            BranchId.SelectedValue = "0";
            SetFilterSessionValues();
            odsData.DataBind();
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
            string editpage = "~/ControlRoom/Modules/Setup/EventAddEdit.aspx";
            string addpage = "~/ControlRoom/Modules/Setup/EventAddWizard.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                Session["EID"]= string.Empty; Response.Redirect(addpage);
            }
            if (e.CommandName.ToLower() == "editrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                Session["EID"] = key; Response.Redirect(editpage);
            }
            if (e.CommandName.ToLower() == "deleterecord")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var obj = new Event();
                    if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                    {
                        obj.FetchObject(key).Delete();

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


        public static string CodeToDescByID(int code, int codeid)
        {
            return "";
        }

        public static string CodeToDescByName(int code, string codename)
        {
            return "";
        }



    }
}

