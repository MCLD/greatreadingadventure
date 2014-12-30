using System;
using System.Diagnostics;
using System.IO;
using System.Web.UI.WebControls;
using ExportToExcel;
using SRPApp.Classes;
using STG.SRP.ControlRooms;
using STG.SRP.Core.Utilities;
using STG.SRP.DAL;
using STG.SRP.Utilities.CoreClasses;


namespace STG.SRP.ControlRoom.Modules.Programs
{
    public partial class ProgramsDelete : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.ProgramRibbon());
            }

            MasterPage.RequiredPermission = 2000;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Delete Program");

            if (!IsPostBack)
            {
                lblPK.Text = Request["PK"];

                var p = DAL.Programs.FetchObject(int.Parse(lblPK.Text));
                lblProg.Text =
                    string.Format(
                        "You have are deleting a program: '{0}'.  Please select the options below to continue.",
                        p.AdminName);

            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            DAL.Programs.Delete( int.Parse(lblPK.Text),
                int.Parse(PatronProgram.SelectedValue), 
                int.Parse(PrizeProgram.SelectedValue), 
                int.Parse(OfferProgram.SelectedValue), 
                int.Parse(BookListProgram.SelectedValue) 
                );

            File.Delete(Server.MapPath("~/css/program/" + lblPK.Text + ".css"));
            File.Delete(Server.MapPath("~/images/banners/" + lblPK.Text  + ".png"));
            File.Delete(Server.MapPath("~/resources/program." + lblPK.Text + ".en-US.txt"));


            Session["Active_Program"] = "";
            Session["Active_Program_Filtered"] = "0";
            Response.Redirect("ProgramList.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ProgramList.aspx");
        }
    }
}