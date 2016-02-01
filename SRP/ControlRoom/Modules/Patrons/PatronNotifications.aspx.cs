using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.Tools;

namespace GRA.SRP.ControlRoom.Modules.Patrons {
    public partial class PatronNotifications : BaseControlRoomPage {
        public string patronDisplayName { get; set; }

        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;

        public string DisplayFrom(object notificationRowObject) {
            var notificationRow = notificationRowObject as System.Data.DataRowView;
            if(notificationRow != null) {
                if((int)notificationRow["PID_From"] == 0) {
                    if(notificationRow["AddedUser"].ToString().Equals(notificationRow["ToUsername"].ToString(), StringComparison.OrdinalIgnoreCase)) {
                        // from system
                        return "System";
                    } else {
                        // from admin user
                        return string.Format("Administrator (<span class=\"cr-administrator\">{0}</span>)", notificationRow["AddedUser"]);
                    }
                } else {
                    return DisplayHelper.FormatName(notificationRow["FromFirstName"].ToString(),
                                                    notificationRow["FromLastName"].ToString(),
                                                    notificationRow["FromUsername"].ToString());
                }
            } else {
                return string.Empty;
            }
        }

        public string DisplayTo(object notificationRowObject) {
            var notificationRow = notificationRowObject as System.Data.DataRowView;
            if(notificationRow != null) {
                return DisplayHelper.FormatName(notificationRow["ToFirstName"].ToString(),
                                                notificationRow["ToLastName"].ToString(),
                                                notificationRow["ToUsername"].ToString());

            } else {
                return string.Empty;
            }
        }
        protected void Page_Load(object sender, EventArgs e) {
            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true;
            if(Session["Curr_Patron"] == null)
                Response.Redirect("Default.aspx");

            if(!IsPostBack) {
                PatronsRibbon.GetByAppContext(this);

                var patron = (Patron)Session["Curr_Patron"];
                this.patronDisplayName = string.Format("{0} {1} ({2})",
                                                       patron.FirstName,
                                                       patron.LastName,
                                                       patron.Username);
                MasterPage.PageTitle = string.Format("Patron Notifications - {0}",
                                                     this.patronDisplayName);
            }

            _mStrSortExp = String.Empty;
            if(!IsPostBack) {
                _mStrSortExp = String.Empty;
            } else {
                if(null != ViewState["_SortExp_"]) {
                    _mStrSortExp = ViewState["_SortExp_"] as String;
                }

                if(null != ViewState["_Direction_"]) {
                    _mSortDirection = (SortDirection)ViewState["_Direction_"];
                }
            }
        }


        protected void GvSorting(object sender, GridViewSortEventArgs e) {
            if(String.Empty != _mStrSortExp) {
                if(String.Compare(e.SortExpression, _mStrSortExp, true) == 0) {
                    _mSortDirection =
                        (_mSortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
                }
            }
            ViewState["_Direction_"] = _mSortDirection;
            ViewState["_SortExp_"] = _mStrSortExp = e.SortExpression;
        }

        protected void GvRowCreated(object sender, GridViewRowEventArgs e) {
            if(e.Row.RowType == DataControlRowType.Header) {
                if(String.Empty != _mStrSortExp) {
                    GlobalUtilities.AddSortImage(e.Row, (GridView)sender, _mStrSortExp, _mSortDirection);
                }
            }
        }

        protected void GvSelectedIndexChanged(object sender, EventArgs e) {
        }

        protected void LoadData() {
            odsData.Select();
            gv.DataBind();
        }


        protected void GvRowCommand(object sender, GridViewCommandEventArgs e) {
            string editpage = "~/ControlRoom/Modules/Patrons/NotificationsAddEdit.aspx";
            if(e.CommandName.ToLower() == "addrecord") {
                Session["NID"]= string.Empty;
                Response.Redirect(editpage);
            }
            if(e.CommandName.ToLower() == "editrecord") {
                int key = Convert.ToInt32(e.CommandArgument);
                Session["NID"] = key;
                Response.Redirect(editpage);
            }
            if(e.CommandName.ToLower() == "deleterecord") {
                var key = Convert.ToInt32(e.CommandArgument);
                try {
                    var obj = new DAL.Notifications();
                    if(obj.IsValid(BusinessRulesValidationMode.DELETE)) {
                        DAL.Notifications.FetchObject(key).Delete();

                        LoadData();
                        var masterPage = (IControlRoomMaster)Master;
                        if(masterPage != null)
                            masterPage.PageMessage = SRPResources.DeleteOK;
                    } else {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach(BusinessRulesValidationMessage m in obj.ErrorCodes) {
                            message = string.Format(String.Format("{0}<li>{{0}}</li>", message), m.ErrorMessage);
                        }
                        message = string.Format("{0}</ul>", message);
                        if(masterPage != null)
                            masterPage.PageError = message;
                    }
                } catch(Exception ex) {
                    var masterPage = (IControlRoomMaster)Master;
                    if(masterPage != null)
                        masterPage.PageError = String.Format(SRPResources.ApplicationError1, ex.Message);
                }
            }
        }
    }
}

