﻿using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;


namespace STG.SRP.ControlRoom.Modules.Patrons
{
    public partial class PatronLogAddEdit : BaseControlRoomPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.IsSecure = true;
            if (Session["Curr_Patron"] == null) Response.Redirect("Default.aspx");

            if (!IsPostBack)
            {
                PatronsRibbon.GetByAppContext(this);

                var patron = (Patron)Session["Curr_Patron"];
                MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Patron Log Entry", patron.FirstName, patron.LastName, patron.Username);
            }


            if (!IsPostBack)
            {
                if (Request["PK"] == "" || Request["PK"] == null)
                {
                    PatronLogEntryCtl1.PatronID = Session["CURR_PATRON_ID"].ToString();
                    PatronLogEntryCtl1.PatronPointsID = "";
                    PatronLogEntryCtl1.LoadControl();
                }
                else
                {
                    PatronLogEntryCtl1.PatronID = Session["CURR_PATRON_ID"].ToString();
                    PatronLogEntryCtl1.PatronPointsID = Request["PK"].ToString();
                    PatronLogEntryCtl1.LoadControl();
                }
            }

        }
    }
}