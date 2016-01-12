using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;


namespace GRA.SRP.ControlRoom.Modules.Programs
{
    public partial class ProgramCSS : BaseControlRoomPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 2200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Program Text Management"); 
            
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
                try {
                    txtCSS.Text = System.IO.File.ReadAllText(Server.MapPath("~/CSS/Program/" + Session["Active_Program"].ToString() + ".css"));
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
            txtCSS.Text = System.IO.File.ReadAllText(Server.MapPath("~/CSS/Program/Default.css"));
        }

        public void SaveScreen()
        {
            System.IO.File.WriteAllText(Server.MapPath("~/CSS/Program/" + ProgramId.SelectedValue + ".css"), txtCSS.Text);
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            LoadScreen();
            MasterPage.PageMessage = "Program CSS re-loaded successfully!";
        }

        protected void btnDefaultCSS_Click(object sender, EventArgs e)
        {
            LoadDefault();
            MasterPage.PageMessage = "Default program CSS loaded successfully!";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveScreen();
            MasterPage.PageMessage = "Program CSS saved successfully!";
        }

    }
}