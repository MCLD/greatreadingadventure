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
    public partial class PatronLog : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.IsSecure = true;
            if (Session["Curr_Patron"] == null) Response.Redirect("Default.aspx");

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);
            }

            if (!IsPostBack)
            {
                var patron = (Patron)Session["Curr_Patron"];
                MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Patron Points Log", patron.FirstName, patron.LastName, patron.Username);

                LoadData();

            }

        }

        protected void LoadData()
        {
            var patron = (Patron)Session["Curr_Patron"];

            var ds = PatronPoints.GetAll(patron.PID);

            gv1.DataSource = ds;
            gv1.DataBind();            
        }

        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {
            string editpage = "~/ControlRoom/Modules/Patrons/PatronLogAddEdit.aspx";
            if (e.CommandName.ToLower() == "addrecord")
            {
                Response.Redirect(editpage);
            }
            if (e.CommandName.ToLower() == "editrecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);

                Response.Redirect(String.Format("{0}?PK={1}", editpage, key));
            }


            if (e.CommandName.ToLower() == "deleterecord")
            {
                int key = Convert.ToInt32(e.CommandArgument);
                try
                {
                    var pp = PatronPoints.FetchObject(key);
                    if (pp.IsValid(BusinessRulesValidationMode.DELETE))
                    {

                        if (pp.isReading)
                        {
                            var prl = PatronReadingLog.FetchObject(pp.LogID);
                            if (prl != null && prl.HasReview)
                            {
                                PatronReview.Delete(PatronReview.FetchObjectByLogId(pp.LogID));
                                prl.Delete();
                            }
                            
                        }

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