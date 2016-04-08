using GRA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls {
    public partial class Feed : System.Web.UI.UserControl {
        public int CurrentPatronProgram { get; set; }
        protected void Page_Load(object sender, EventArgs e) {
            var patron = Session[SessionKey.Patron] as DAL.Patron;
            if(patron == null)
            {
                CurrentPatronProgram = -1;
            }
            else
            {
                CurrentPatronProgram = patron.ProgID;
            }
        }
    }
}