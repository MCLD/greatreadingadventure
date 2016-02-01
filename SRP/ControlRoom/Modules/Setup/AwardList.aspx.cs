using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Controls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom.Modules.Setup
{
    public partial class AwardList : BaseControlRoomPage
    {
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;
        private static string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 4900;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Badge Awards List");

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
            string editpage = "~/ControlRoom/Modules/Setup/AwardAddEdit.aspx";
            string addtpage = "~/ControlRoom/Modules/Setup/AwardAddWizard.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                Session["AWD"]= string.Empty;
                Response.Redirect(addtpage);
            }
            if (e.CommandName.ToLower() == "editrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                Session["AWD"] = key;
                Response.Redirect(editpage);
                //Response.Redirect(String.Format("{0}?PK={1}", editpage, key));
            }
            if (e.CommandName.ToLower() == "deleterecord")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var obj = Award.FetchObject(key);
                    if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                    {
                        obj.Delete();

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

            if (e.CommandName.ToLower() == "applyaward")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                var awd = Award.FetchObject(key);
                var pl = GetMatchingData(awd);
                var pMax = pl.Tables[0].Rows.Count;
                var pCount = 0;
                Response.Buffer = false;
                Response.Write("<!-- ");
                foreach (DataRow row in pl.Tables[0].Rows)
                {
                    var pid = Convert.ToInt32(row["PID"]);
                    var list = new List<Badge>();
                    var p = Patron.FetchObject(pid);
                    if (AwardPoints.AwardBadgeToPatron(awd.BadgeID, p, ref list))
                    {
                        pCount++;
                        // if they got a badge, then maybe they match the criteria to get another as well ...
                        AwardPoints.AwardBadgeToPatronViaMatchingAwards(p, ref list);
                        Response.Write(" :-> "); Response.Flush();
                    }
                }
                Response.Write("-->");

                var masterPage = (IControlRoomMaster)Master;
                if (masterPage != null)
                    masterPage.PageMessage = String.Format("{0} patrons matched the award criteria. Award has been applied to {1} patrons who had not previously received the award.", pMax, pCount);
            }

        }

        private DataSet GetMatchingData(Award awd)
        {
            var arrParams = new SqlParameter[5];

            if (awd.ProgramID == 0)
            {
                arrParams[0] = new SqlParameter("@ProgId", DBNull.Value);
            }
            else
            {
                arrParams[0] = new SqlParameter("@ProgId", awd.ProgramID);
            }
            if (awd.BranchID == 0)
            {
                arrParams[1] = new SqlParameter("@BranchID", DBNull.Value);
            }
            else
            {
                arrParams[1] = new SqlParameter("@BranchID", awd.BranchID);
            }
            if (awd.District == "" || awd.District == "0")
            {
                arrParams[2] = new SqlParameter("@LibSys", DBNull.Value);
            }
            else
            {
                arrParams[2] = new SqlParameter("@LibSys", awd.District);
            }
            if (awd.SchoolName == "" || awd.SchoolName == "0")
            {
                arrParams[3] = new SqlParameter("@School", DBNull.Value);
            }
            else
            {
                arrParams[3] = new SqlParameter("@School", awd.SchoolName);
            }

            arrParams[4] = new SqlParameter("@TenID", CRTenantID == null ? -1 : CRTenantID);
            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "rpt_PatronFilter", arrParams);

            return ds;
        }
    }
}

