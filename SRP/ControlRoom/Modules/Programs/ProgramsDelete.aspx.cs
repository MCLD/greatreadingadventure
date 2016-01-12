using System;
using System.Diagnostics;
using System.IO;
using System.Web.UI.WebControls;
using ExportToExcel;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;


namespace GRA.SRP.ControlRoom.Modules.Programs
{
    public partial class ProgramsDelete : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 2000;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Delete Program");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.ProgramRibbon());

                lblPK.Text = Session["PGM"] == null ? "" : Session["PGM"].ToString(); 

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
            File.Delete(Server.MapPath("~/resources/program." + lblPK.Text + ".en-US.txt"));
            string filename = Server.MapPath("~/images/banners/" + lblPK.Text + ".png");
            if(File.Exists(filename)) {
                File.Delete(filename);
            }
            filename = Server.MapPath("~/images/banners/" + lblPK.Text + "@2x.png");
            if(File.Exists(filename)) {
                File.Delete(filename);
            }
            filename = Server.MapPath("~/images/banners/" + lblPK.Text + ".jpg");
            if(File.Exists(filename)) {
                File.Delete(filename);
            }
            filename = Server.MapPath("~/images/banners/" + lblPK.Text + "@2x.jpg");
            if(File.Exists(filename)) {
                File.Delete(filename);
            }

            Session["Active_Program"]= string.Empty;
            Session["Active_Program_Filtered"] = "0";
            Response.Redirect("ProgramList.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ProgramList.aspx");
        }
    }
}