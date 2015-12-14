using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.Controls
{
    public partial class MyGamemapNavControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Patron"] == null) Response.Redirect("/");
            var patron = (Patron)Session["Patron"];

            var gameLogic = new Logic.Game();
            this.gamemapNav.Visible = !string.IsNullOrEmpty(gameLogic.GetGameboardPath(patron));
        }

        public  string Message
        { 
            get { return lbl.Text; }
            set { lbl.Text = value; }
        }
    }
}