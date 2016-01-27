using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using System.Data;
using System.Web;
using GRA.Tools;

namespace GRA.SRP.ControlRoom.Modules.Setup {
    public partial class MGChooseAdvSlidesList : BaseControlRoomPage {
        private String _mStrSortExp;
        private SortDirection _mSortDirection = SortDirection.Ascending;


        protected void Page_Load(object sender, EventArgs e) {
            MasterPage.RequiredPermission = 4300;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Choose Your Adventure Slides List");

            _mStrSortExp = String.Empty;

            if(!IsPostBack) {
                SetPageRibbon(StandardModuleRibbons.SetupRibbon());

                if(Session["MGID"] != null) {
                    lblMGID.Text = Session["MGID"].ToString();
                    if(Request["L"] != null)
                        lblDiff.Text = Request["L"];
                    var s = (lblDiff.Text == "1"
                                 ? " - EASY Difficulty"
                                 : (lblDiff.Text == "2" ? " - MEDIUM Difficulty" : " - HARD Difficulty"));


                    var o = Minigame.FetchObject(int.Parse(lblMGID.Text));
                    AdminName.Text = o.AdminName + s;

                    var o2 = MGChooseAdv.FetchObjectByParent(int.Parse(lblMGID.Text));
                    lblCAID.Text = o2.CAID.ToString();


                } else {
                    Response.Redirect("MiniGameList.aspx");
                }

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
            string editpage = "~/ControlRoom/Modules/Setup/MGChooseAdvSlidesAddEdit.aspx";
            if(e.CommandName.ToLower() == "addrecord") {
                //Response.Redirect(String.Format("{0}?MGID={1}&CAID={2}&L={3}", editpage, lblMGID.Text, lblCAID.Text, lblDiff.Text));
                Response.Redirect(String.Format("{0}?CAID={1}&L={2}", editpage, lblCAID.Text, lblDiff.Text));
            }

            if(e.CommandName.ToLower() == "editrecord") {
                int key = Convert.ToInt32(e.CommandArgument);
                Response.Redirect(String.Format("{0}?PK={1}&L={2}", editpage, key, lblDiff.Text));
            }

            if(e.CommandName.ToLower() == "back") {
                Response.Redirect(String.Format("~/ControlRoom/Modules/Setup/MGChooseAdvAddEdit.aspx?PK={0}", lblMGID.Text));
            }


            if(e.CommandName.ToLower() == "moveup") {
                var key = Convert.ToInt32(e.CommandArgument);
                DAL.MGChooseAdvSlides.MoveUp(key);
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = "Step Moved Up!";
                LoadData();
            }

            if(e.CommandName.ToLower() == "movedn") {
                var key = Convert.ToInt32(e.CommandArgument);
                DAL.MGChooseAdvSlides.MoveDn(key);
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = "Step Moved Down";
                LoadData();
            }




            if(e.CommandName.ToLower() == "deleterecord") {
                var key = Convert.ToInt32(e.CommandArgument);
                try {
                    var obj = new MGChooseAdvSlides();
                    if(obj.IsValid(BusinessRulesValidationMode.DELETE)) {
                        MGChooseAdvSlides.FetchObject(key).Delete();

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

        protected void GvRowDataBound(object sender, GridViewRowEventArgs e) {
            if(e.Row.RowType == DataControlRowType.DataRow) {
                var slideText = e.Row.FindControl("SlideTextDisplay") as Literal;
                var dataRow = e.Row.DataItem as DataRowView;
                if(slideText != null && dataRow != null) {
                    slideText.Text = DisplayHelper.RemoveHtml(dataRow["SlideText"].ToString(),
                                                              200);
                }

            }
        }
    }
}

