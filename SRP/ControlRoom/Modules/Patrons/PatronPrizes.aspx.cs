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
    public partial class PatronPrizes : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 5100;
            MasterPage.IsSecure = true;
            if (Session["Curr_Patron"] == null) Response.Redirect("Default.aspx");

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);
            }

            if (!IsPostBack)
            {
                var patron = (Patron)Session["Curr_Patron"];
                MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Patron Prizes", patron.FirstName, patron.LastName, patron.Username);

                LoadData();

            }

        }

        protected void LoadData()
        {
            var patron = (Patron)Session["Curr_Patron"];

            var ds = DAL.PatronPrizes.GetAll(patron.PID);

            gv.DataSource = ds;
            gv.DataBind();
        }

        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {
            string editpage = "~/ControlRoom/Modules/Patrons/PatronPrizesAddEdit.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                Response.Redirect(editpage);
            }

            if (e.CommandName.ToLower() == "deleterecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var pp = DAL.PatronPrizes.FetchObject(key);
                    if (pp.IsValid(BusinessRulesValidationMode.DELETE))
                    {





                        pp.Delete();
                        LoadData();
                        var masterPage = (IControlRoomMaster)Master;
                        if (masterPage != null) masterPage.PageMessage = SRPResources.DeleteOK;
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in pp.ErrorCodes)
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



            if (e.CommandName.ToLower() == "pickup")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var pp = DAL.PatronPrizes.FetchObject(key);
                    if (pp.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        var now = DateTime.Now;
                        string currentUser = "N/A";
                        try
                        {
                            currentUser = HttpContext.Current.User.Identity.Name;
                        }
                        catch (Exception) { }

                        pp.RedeemedFlag = true;
                        pp.LastModDate = now;
                        pp.LastModUser = currentUser;
                        pp.Update();

                        if (pp.DrawingID != 0)
                        {
                            var pw = PrizeDrawingWinners.FetchObject(pp.DrawingID);
                            if (pw != null)
                            {
                                pw.PrizePickedUpFlag = true;
                                pw.LastModDate = now;
                                pw.LastModUser = currentUser;
                                pw.Update();

                                if (pw.NotificationID != 0)
                                {
                                    var n = DAL.Notifications.FetchObject(pw.NotificationID);
                                    if (n != null) n.Delete();
                                }
                            }
                        }

                        LoadData();
                        var masterPage = (IControlRoomMaster)Master;
                        if (masterPage != null) masterPage.PageMessage = "Prize has been marked a picked up!";
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in pp.ErrorCodes)
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

            if (e.CommandName.ToLower() == "undopickup")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var pp = DAL.PatronPrizes.FetchObject(key);
                    if (pp.IsValid(BusinessRulesValidationMode.UPDATE))
                    {
                        var now = DateTime.Now;
                        string currentUser = "N/A";
                        try
                        {
                            currentUser = HttpContext.Current.User.Identity.Name;
                        }
                        catch (Exception) { }

                        this.Log().Warn("Reward for patron id {0} marked as NOT picked up by {1}",
                            pp.PID,
                            currentUser);

                        pp.RedeemedFlag = false;
                        pp.LastModDate = now;
                        pp.LastModUser = currentUser;
                        pp.Update();

                        if (pp.DrawingID != 0)
                        {
                            var pw = PrizeDrawingWinners.FetchObject(pp.DrawingID);
                            if (pw != null)
                            {
                                pw.PrizePickedUpFlag = false;
                                pw.LastModDate = now;
                                pw.LastModUser = currentUser;
                                pw.Update();
                            }
                        }

                        LoadData();
                        var masterPage = (IControlRoomMaster)Master;
                        if (masterPage != null) masterPage.PageMessage = "Prize has been marked as available. This change was logged.";
                    }
                    else
                    {
                        var masterPage = (IControlRoomMaster)Master;
                        string message = String.Format(SRPResources.ApplicationError1, "<ul>");
                        foreach (BusinessRulesValidationMessage m in pp.ErrorCodes)
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