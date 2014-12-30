using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.DAL;

namespace STG.SRP.Controls
{
    public partial class LeaderBoardControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
          
        }

        public void LoadData()
        {
            var ds = DAL.Programs.GetLeaderboard(((Patron)Session["Patron"]).ProgID);
            rptr.DataSource = ds;
            rptr.DataBind();
        }
    }
}