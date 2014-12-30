﻿using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;
using STG.SRP.Utilities.CoreClasses;

namespace STG.SRP.ControlRoom.Modules.Setup
{
    public partial class BookListBooksList : BaseControlRoomPage
    {
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

            }

            //MasterPage.RequiredPermission = PERMISSIONID;
            MasterPage.IsSecure = true;

             if (!IsPostBack)
             {
                 lblPK.Text = Request["PK"].ToString();
                 var bl = BookList.FetchObject(int.Parse(lblPK.Text));
                 MasterPage.PageTitle = string.Format("Books on \"{0}\" Book List", bl.AdminName);
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

        protected void LoadData()
        {
            odsData.Select();
            gv.DataBind();
        }


        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {
            string editpage = "~/ControlRoom/Modules/Setup/BookListBooksAddEdit.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                Response.Redirect(editpage);
            }
            
            if (e.CommandName.ToLower() == "deleterecord")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var obj = new BookListBooks();
                    if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                    {
                        BookListBooks.FetchObject(key).Delete();

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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var obj = new BookListBooks();
                
                //obj.BLID = FormatHelper.SafeToInt(((DropDownList) ((DetailsView) sender).FindControl("BLID")).SelectedValue);
                obj.BLID = FormatHelper.SafeToInt(lblPK.Text);
                obj.Author = Author.Text;
                obj.Title = Title.Text;
                obj.ISBN = ISBN.Text;
                obj.URL = URL.Text;

                obj.AddedDate = DateTime.Now;
                obj.AddedUser = ((SRPUser)Session[SessionData.UserProfile.ToString()]).Username;  //"N/A";  // Get from session
                obj.LastModDate = obj.AddedDate;
                obj.LastModUser = obj.AddedUser;

                if (obj.IsValid(BusinessRulesValidationMode.INSERT))
                {
                    obj.Insert();
                    Author.Text = Title.Text = ISBN.Text = URL.Text = "";

                    odsData.DataBind();
                    gv.DataBind();

                    var masterPage = (IControlRoomMaster)Master;
                    masterPage.PageMessage = SRPResources.AddedOK;
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
                    masterPage.PageError = message;
                }
            }
            catch (Exception ex)
            {
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("BookListList.aspx");
        }
    }
}

