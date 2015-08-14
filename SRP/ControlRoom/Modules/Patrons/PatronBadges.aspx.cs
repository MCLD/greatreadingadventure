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
    public partial class PatronBadges : BaseControlRoomPage
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
                MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Patron Badges", patron.FirstName, patron.LastName, patron.Username);

                LoadData();

            }

        }

        protected void LoadData()
        {
            var patron = (Patron)Session["Curr_Patron"];

            var ds = DAL.PatronBadges.GetAll(patron.PID);

            gv1.DataSource = ds;
            gv1.DataBind();            
        }

        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {
            string editpage = "~/ControlRoom/Modules/Patrons/PatronBadgesAddEdit.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                Response.Redirect(editpage);
            }

            if (e.CommandName.ToLower() == "deleterecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var pp = DAL.PatronBadges.FetchObject(key);
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
        }
    }
}