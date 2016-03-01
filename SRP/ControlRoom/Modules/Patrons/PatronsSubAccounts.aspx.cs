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
    public partial class PatronsSubAccounts : BaseControlRoomPage
    {

        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true;
            
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

            if (Session["Curr_Patron"] == null) Response.Redirect("Default.aspx");

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);
            } 
                       

            if (!IsPostBack)
            {
                GetData();
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

        protected void GetData()
        {
            var patron = (Patron)Session["Curr_Patron"];
            MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Patron Sub Accounts", patron.FirstName, patron.LastName, patron.Username);
            gv1.DataSourceID = null;
            gv1.DataSource = Patron.GetSubAccountList(patron.PID);
            gv1.DataBind();
        }

        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {
            string editpage = "~/ControlRoom/Modules/Patrons/PatronDetails.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                //Session["CURR_PATRON_ID"]= string.Empty;
                //Session["CURR_PATRON"] = null;
                //Response.Redirect(editpage);
                Response.Redirect("~/ControlRoom/Modules/Patrons/PatronsAddSubAccount.aspx");
            }
            if (e.CommandName.ToLower() == "editrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                Session["CURR_PATRON_ID"] = key;
                Session["CURR_PATRON"] = Patron.FetchObject(key);
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

                        GetData();

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

    }
}
