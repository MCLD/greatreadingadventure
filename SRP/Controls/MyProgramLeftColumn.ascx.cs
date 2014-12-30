using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace STG.SRP.Controls
{
    public partial class MyProgramLeftColumn : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public MyGamemapNavControl GamemapNavControl
        {
            get { return MyGamemapNavControl1; }
        }
    }
}