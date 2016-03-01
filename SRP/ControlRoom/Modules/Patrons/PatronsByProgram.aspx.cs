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

namespace GRA.SRP.ControlRoom.Modules.Patrons
{
    public partial class PatronsByProgram : BaseControlRoomPage
    {

        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true; 
            MasterPage.PageTitle = string.Format("{0}", "Patron Search");

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

            gv1.PageSize = int.Parse(SRPSettings.GetSettingValue("PageSize"));

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);
            }

            if (!IsPostBack)
            {
                if (WasFiltered())
                {
                    GetFilterSessionValues();
                    DoFilter();
                    
                }
                else
                {
                    gv1.DataSourceID= null;
                    gv1.DataBind();
                }
            }

        }

        private void DoFilter()
        {

            SetFilterSessionValues();
            if (ProgID.SelectedValue == "") return; 
            gv1.DataSourceID = "odsSearch";
        }

        public void SetFilterSessionValues()
        {
            Session["PS_Prog"] = ProgID.SelectedValue;
            Session["PS_Filtered"] = "1";
        }

        public void GetFilterSessionValues()
        {
            if (Session["PS_Prog"] != null) ProgID.SelectedValue = Session["PS_Prog"].ToString();
        }

        public bool WasFiltered()
        {
            return (Session["PS_Filtered"] != null);
        }

        public DataSet GetFilteredList()
        {
            SetFilterSessionValues();
            return Patron.CRSearch();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (ProgID.SelectedValue == "")
            {
                var masterPage = (IControlRoomMaster)Master;
                if (masterPage != null) masterPage.PageError = "You must select a program!";
                return;
            }
            DoFilter();
        }

        // ----  Generic code ---- //

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

        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {
            string editpage = "~/ControlRoom/Modules/Patrons/PatronDetails.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                Session["CURR_PATRON_ID"]= string.Empty;
                Session["CURR_PATRON"] = null;
                Session["CURR_PATRON_MODE"] = "ADDNEW";
                Response.Redirect(editpage);
            }
            if (e.CommandName.ToLower() == "editrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                Session["CURR_PATRON_ID"] = key;
                Session["CURR_PATRON"] = Patron.FetchObject(key);
                Session["CURR_PATRON_MODE"] = "EDIT";
                Response.Redirect(editpage);
                //Response.Redirect(String.Format("{0}?PK={1}", editpage, key));
            }
            if (e.CommandName.ToLower() == "deleterecord")
            {
                var key = Convert.ToInt32(e.CommandArgument);

                //var masterPage = (IControlRoomMaster)Master;
                //if (masterPage != null)
                //    masterPage.PageError = "Functionality Not Implemented (yet).";

                try
                {
                    var obj = Patron.FetchObject(key);
                    if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                    {
                        obj.Delete();

                        DoFilter();

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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Session["PS_First"] = Session["PS_Last"] = Session["PS_User"] = Session["PS_Email"] = Session["PS_DOB"] = Session["PS_Gender"]= string.Empty;
            Session["PS_Prog"]= string.Empty;
            Session["PS_Filtered"] = null;

            GetFilterSessionValues();
            gv1.DataSourceID= null;
        }


    }
}
