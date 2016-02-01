using System;
using System.Diagnostics;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;


namespace GRA.SRP.ControlRoom.Modules.Programs
{
    public partial class ProgramText : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 2200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Program Text Resources  Management");

            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.ProgramRibbon());
                if (WasFiltered())
                {
                    GetFilterSessionValues();
                    LoadScreen();
                }
            }

        }

        public void SetFilterSessionValues()
        {
            Session["Active_Program"] = ProgramId.SelectedValue;
            Session["Active_Program_Filtered"] = "1";
        }

        public void GetFilterSessionValues()
        {
            if (Session["Active_Program"] != null) try { ProgramId.SelectedValue = Session["Active_Program"].ToString(); }
                catch (Exception) { }
        }

        public bool WasFiltered()
        {
            return (Session["Active_Program_Filtered"] != null);
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            SetFilterSessionValues();
            LoadScreen();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ProgramId.SelectedValue = "0";
            SetFilterSessionValues();
            LoadScreen();
        }

        public void LoadScreen()
        {
            if (Session["Active_Program"] == null || Session["Active_Program"].ToString() == "0")
            {
                pnlEdit.Visible = false;
                txtCSS.Text= string.Empty;
            }
            else
            {
                try
                {
                    var resourceFile = "program." + Session["Active_Program"].ToString() + ".en-US.txt";
                    txtCSS.Text = System.IO.File.ReadAllText(Server.MapPath("~/Resources/" + resourceFile));
                }
                catch (Exception)
                {
                    LoadDefault();
                }
                pnlEdit.Visible = true;
            }
        }

        public void LoadDefault()
        {
            var resourceFile = "program.default.en-US.txt";
            txtCSS.Text = System.IO.File.ReadAllText(Server.MapPath("~/Resources/" + resourceFile));
        }

        public void SaveScreen()
        {
            var resourceFile = "program." + ProgramId.SelectedValue + ".en-US.txt";
            System.IO.File.WriteAllText(Server.MapPath("~/Resources/" + resourceFile), txtCSS.Text);
            //CompileResourceFile(resourceFile);
            StringResources.LoadProgramResourceFile(ProgramId.SelectedValue);
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            LoadScreen();
            MasterPage.PageMessage = "Program text resources re-loaded successfully!";
        }

        protected void btnDefaultCSS_Click(object sender, EventArgs e)
        {
            LoadDefault();
            MasterPage.PageMessage = "Default program text resources  loaded successfully!";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveScreen();
            MasterPage.PageMessage = "Program text resources  saved successfully!";
        }

        //protected void CompileResourceFile(string resourceFile)
        //{
        //    Process objProcess = new Process();
        //    //objProcess.StartInfo.FileName = @"C:\Program Files\Microsoft SDKs\Windows\v6.0A\Bin\x64\resgen.exe"; 
        //    objProcess.StartInfo.FileName = @"C:\temp\resgen.exe";
        //    objProcess.StartInfo.FileName = (Server.MapPath("/Resources/") + "\\resgen.exe").Replace("\\\\", "\\");
        //    objProcess.StartInfo.UseShellExecute = false;
        //    objProcess.StartInfo.CreateNoWindow = false;
        //    objProcess.StartInfo.WorkingDirectory = Server.MapPath("~/Resources/"); ;
        //    objProcess.StartInfo.Arguments = resourceFile;// @"\\remote_server_name -w c:\physical_path perl c:\physical_path\test.pl ""parameter1""";
        //    objProcess.Start();
        //}

    }
}