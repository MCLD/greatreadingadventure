using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRoom.Modules.Patrons;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;


namespace GRA.SRP.ControlRoom.Modules.Notifications
{
    public partial class NotificationList : BaseControlRoomPage
    {
        private const int MaxLengthInList = 100;
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5000;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Patron Questions");


            _mStrSortExp = String.Empty;
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.NotificationsRibbon());
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
            string editpage = "~/ControlRoom/Modules/Notifications/QuestionView.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                Response.Redirect(editpage);
            }
            if (e.CommandName.ToLower() == "editrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);

                var n = DAL.Notifications.FetchObject(key);
                var pid = n.PID_From;


                Session["CURR_PATRON_ID"] = pid;
                Session["CURR_PATRON"] = Patron.FetchObject(pid);
                Session["CURR_PATRON_MODE"] = "EDIT";

                Session["Curr_Notification_ID"] = key;
                Response.Redirect(String.Format("{0}?PK={1}", editpage, key));
            }
            if (e.CommandName.ToLower() == "deleterecord")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var obj = new DAL.Notifications();
                    if (obj.IsValid(BusinessRulesValidationMode.DELETE))
                    {
                        DAL.Notifications.FetchObject(key).Delete();

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
                new Code.ControlRoom.Mail().ClearUnreadCrMailCache(this.Context);
            }
        }

        protected string DisplaySubjectMessage(object repeaterObject)
        {
            var dataView = repeaterObject as System.Data.DataRowView;
            string subject = dataView["Subject"] as string;
            string body = dataView["Body"] as string;
            if (!string.IsNullOrEmpty(subject))
            {
                if (subject.Length <= MaxLengthInList - 10)
                {
                    if (!string.IsNullOrEmpty(body))
                    {
                        string shortBody = body.Substring(0,
                            Math.Min(body.Length, MaxLengthInList - subject.Length));
                        return string.Format("<strong>{0}</strong> / {1}{2}",
                            subject,
                            shortBody,
                            body.Length > shortBody.Length ? "..." : string.Empty);
                    }
                }
                return string.Format("<strong>{0}</strong>{1}",
                    subject.Substring(0, Math.Min(subject.Length, MaxLengthInList)),
                    subject.Length > MaxLengthInList ? "..." : string.Empty);
            }
            if (!string.IsNullOrEmpty(body))
            {
                return string.Format("{0}{1}",
                    body.Substring(0, Math.Min(body.Length, MaxLengthInList)),
                    body.Length > MaxLengthInList ? "..." : string.Empty);
            }
            return "<strong>** Unknown Message Content **</strong>";
        }
    }
}

