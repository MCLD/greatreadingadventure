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
    public partial class PatronPrizesAddEdit : BaseControlRoomPage
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
                MasterPage.PageTitle = string.Format("{0} - {1} {2} ({3})", "Add Patron Prizes", patron.FirstName, patron.LastName, patron.Username);
            }

        }

        protected void btn_Command(object sender, CommandEventArgs e)
        {
            string listpage = "~/ControlRoom/Modules/Patrons/PatronPrizes.aspx";
            if (e.CommandName.ToLower() == "back")
            {
                Response.Redirect(listpage);
            }

            if (e.CommandName.ToLower() == "add")
            {

                var pp = new DAL.PatronPrizes();
                pp.PID = ((Patron) Session["Curr_Patron"]).PID;
                pp.PrizeSource = 2;
                pp.PrizeName = PrizeName.Text;
                pp.RedeemedFlag = true;
                pp.LastModUser = pp.AddedUser = ((SRPUser) Session[SessionData.UserProfile.ToString()]).Username;
                pp.AddedDate = pp.LastModDate = DateTime.Now;

                pp.Insert();

                Response.Redirect(listpage);
            }


        }


    }
}