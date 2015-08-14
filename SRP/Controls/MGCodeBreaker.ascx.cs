using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;

namespace GRA.SRP.Controls
{
    public partial class MGCodeBreaker : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void LoadGame(int mgid, int difficulty)
        {
            var mg = DAL.MGCodeBreaker.FetchObjectByParent(mgid);
            CBID.Text = mg.CBID.ToString();
            Difficulty.Text = difficulty.ToString();
            LoadPage();

            

        }

        public void LoadPage()
        {
            int difficulty = int.Parse(Difficulty.Text);
            var mg = DAL.MGCodeBreaker.FetchObject(int.Parse(CBID.Text));
            if (difficulty == 1)
            {
                Correct.Text = mg.EasyString;
            }
            if (difficulty == 2)
            {
                Correct.Text = mg.MediumString;
            }
            if (difficulty == 3)
            {
                Correct.Text = mg.HardString;
            }

            lblEncoded.Text = mg.GetEncoded(Correct.Text, difficulty);
            lblKey.Text = mg.GetKey(Correct.Text, difficulty);
        }

        protected void btnScore_Click(object sender, EventArgs e)
        {
            if (txtAnswer.Text.Replace(" ","") == Correct.Text.Replace(" ",""))
            {
                lblMessage.Text = "<H1 style='color: Green!important;'>Correct! ... </H1>";
                lblMessage2.Text = lblMessage.Text + "<br/><br/>";
                btnContinue.Visible = true;
                btnScore.Visible = false;
            }
            else
            {
                lblMessage.Text = "<H1 style='color: DarkRed!important;'>Incorrect ... Try Again!</H1>";
                lblMessage2.Text = lblMessage.Text + "<br/><br/>";
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            lblMessage2.Text = lblMessage.Text = "";

            try { ((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return; }
            catch { }
            try { ((GRA.SRP.ControlRoom.Controls.MinigamePreview)((Panel)Parent).Parent.Parent.Parent).CompleteGamePlay(); return; }
            catch { }
            //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return;
            //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay();

            return;
            
        }

    }
}