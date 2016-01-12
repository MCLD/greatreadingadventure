using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

// --> PERMISSIONID 
namespace GRA.SRP.ControlRoom.Modules.Programs
{
    public partial class ProgramsList : BaseControlRoomPage
    {
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 2200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Programs List");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.ProgramRibbon());

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
            string editpage = "~/ControlRoom/Modules/Programs/ProgramsAddEdit.aspx";
            string deletepage = "~/ControlRoom/Modules/Programs/ProgramsDelete.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                Session["PGM"]= string.Empty; Response.Redirect(editpage);
                Response.Redirect(editpage);
            }
            if (e.CommandName.ToLower() == "editrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);

                Session["Active_Program"] = key.ToString();
                Session["Active_Program_Filtered"] = "1";
                Session["PGM"] = key; Response.Redirect(editpage);
                //Response.Redirect(String.Format("{0}?PK={1}", editpage, key));
            }
            if (e.CommandName.ToLower() == "deleterecord")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                Session["Active_Program"] = key.ToString();
                Session["Active_Program_Filtered"] = "1";
                Session["PGM"] = key; Response.Redirect(deletepage);
                //Response.Redirect(String.Format("{0}?PK={1}", deletepage, key));

                //try
                //{
                //    var obj = new DAL.Programs();
                //    if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                //    {
                //        obj = DAL.Programs.FetchObject(key);
                //        obj.Delete();

                //        if (Session["Active_Program"] != null && Session["Active_Program"].ToString() == key.ToString())
                //        {
                //            Session["Active_Program_Filtered"] = "0";
                //            Session["Active_Program"] = null;
                //        }

                //        LoadData();
                //        var masterPage = (IControlRoomMaster)Master;
                //        if (masterPage != null) masterPage.PageMessage = SRPResources.DeleteOK;
                //    }
                //    else
                //    {
                //        var masterPage = (IControlRoomMaster)Master;
                //        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                //        foreach (BusinessRulesValidationMessage m in obj.ErrorCodes)
                //        {
                //            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                //        }
                //        message = string.Format("{0}</ul>", message);
                //        if (masterPage != null) masterPage.PageError = message;
                //    }
                //}
                //catch (Exception ex)
                //{
                //    var masterPage = (IControlRoomMaster)Master;
                //    if (masterPage != null)
                //        masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                //}
            }
        }
    }
}

