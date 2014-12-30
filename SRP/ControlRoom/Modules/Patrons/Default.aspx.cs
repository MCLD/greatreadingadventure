﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;

namespace STG.SRP.ControlRoom.Modules.Patrons
{
    public partial class Default : BaseControlRoomPage
    {

        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;

        protected void Page_Load(object sender, EventArgs e)
        {
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
                if (Filter.WasFiltered())
                {
                    Filter.LoadDropdowns();
                    Filter.GetFilterSessionValues();
                    DoFilter();
                    
                }
                else
                {
                    gv1.DataSourceID = "";
                    gv1.DataBind();
                }
            }

        }



        private void DoFilter()
        {
            Filter.SetFilterSessionValues();
            Filter.GetFilterSessionValues();
            if (HaveFilter()) {gv1.DataSourceID = "odsSearch";}
            else
            {
                var masterPage = (IControlRoomMaster)Master;
                if (masterPage != null) masterPage.PageWarning = "You must enter a filter!";
            }
        }

        public bool HaveFilter()
        {
            if (string.IsNullOrEmpty(Session["PS_First"].ToString()) &&
                string.IsNullOrEmpty(Session["PS_Last"].ToString()) &&
                string.IsNullOrEmpty(Session["PS_User"].ToString()) &&
                string.IsNullOrEmpty(Session["PS_Email"].ToString()) &&
                string.IsNullOrEmpty(Session["PS_DOB"].ToString()) &&
                string.IsNullOrEmpty(Session["PS_Gender"].ToString()) &&
                string.IsNullOrEmpty(Session["PS_Prog"].ToString())  
                )
            {
               Session["PS_Filtered"] = null;
               return false;
            }

            return true;
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            DoFilter();
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

        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {
            string editpage = "~/ControlRoom/Modules/Patrons/PatronDetails.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                Session["CURR_PATRON_ID"] = "";
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
            Session["PS_First"] = Session["PS_Last"] = Session["PS_User"] = Session["PS_Email"] = Session["PS_DOB"] = Session["PS_Gender"] = "";
            Session["PS_Prog"] = "";
            Session["PS_Filtered"] = null;

            Filter.GetFilterSessionValues();
            gv1.DataSourceID = "";
        }


    }
}
