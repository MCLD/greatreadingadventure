using System;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.ControlRooms;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;

// --> PERMISSIONID 
namespace GRA.SRP.ControlRoom.Modules.Programs
{
    public partial class ProgramOrder : BaseControlRoomPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.RequiredPermission = 2200;
            MasterPage.IsSecure = true;
            MasterPage.PageTitle = string.Format("{0}", "Programs List");
            if (!IsPostBack)
            {
                SetPageRibbon(StandardModuleRibbons.ProgramRibbon());

            }
          
        }



        protected void LoadData()
        {
            odsData.Select();
            gv.DataBind();
        }


        protected void GvRowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName.ToLower() == "moveup")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                DAL.Programs.MoveUp(key);
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = "Program Moved Up!";

                LoadData();
            }

            if (e.CommandName.ToLower() == "movedn")
            {
                var key = Convert.ToInt32(e.CommandArgument);
                DAL.Programs.MoveDn(key);
                var masterPage = (IControlRoomMaster)Master;
                masterPage.PageMessage = "Program Moved Down";
                LoadData();
            }
        }
    }
}

