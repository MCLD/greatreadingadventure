using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls {
    public partial class MGChooseAdvPlay : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {

        }

        public void LoadGame(int mgid, int difficulty) {
            var mg = DAL.MGChooseAdv.FetchObjectByParent(mgid);
            CAID.Text = mg.CAID.ToString();
            Difficulty.Text = difficulty.ToString();
            LoadPage();
        }

        public void LoadPage() {
            int iCAID = int.Parse(CAID.Text);
            int iDiff = int.Parse(Difficulty.Text);
            int iStep = int.Parse(CurrStep.Text);

            var gm = DAL.MGChooseAdv.FetchObject(iCAID);

            var step = DAL.MGChooseAdvSlides.FetchPlaySlide(iCAID, iStep, iDiff);
            lblSlideText.Text = Server.HtmlDecode(step.SlideText);
            var audioFile = string.Format("~/images/Games/ChooseAdv/{0}_{1}.mp3", step.CASID, Difficulty.Text);
            lblSound.Text =
                string.Format(
                    "<audio controls><source src='{0}' type='audio/mpeg'>Your browser does not support this audio format.</audio>",
                    VirtualPathUtility.ToAbsolute(audioFile));
            lblSound.Visible = System.IO.File.Exists(Server.MapPath(audioFile));


            var ticks = new Random().Next(1, 100);
            if(ticks % 2 == 1) {
                btn1.ImageUrl = String.Format("~/Images/Games/ChooseAdv/i1_{0}.png", step.CASID.ToString());
                btn1.CommandArgument = step.FirstImageGoToStep.ToString();
                btn2.ImageUrl = String.Format("~/Images/Games/ChooseAdv/i2_{0}.png", step.CASID.ToString());
                btn2.CommandArgument = step.SecondImageGoToStep.ToString();
            } else {
                btn1.ImageUrl = String.Format("~/Images/Games/ChooseAdv/i2_{0}.png", step.CASID.ToString());
                btn1.CommandArgument = step.SecondImageGoToStep.ToString();
                btn2.ImageUrl = String.Format("~/Images/Games/ChooseAdv/i1_{0}.png", step.CASID.ToString());
                btn2.CommandArgument = step.FirstImageGoToStep.ToString();
            }

        }

        protected void btnImg_Command(object sender, CommandEventArgs e) {
            var nextStep = int.Parse(e.CommandArgument.ToString());
            if(nextStep == 0) {
                try {
                    ((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay();
                    return;
                } catch { }
                try {
                    ((GRA.SRP.ControlRoom.Controls.MinigamePreview)((Panel)Parent).Parent.Parent.Parent).CompleteGamePlay();
                    return;
                } catch { }
                return;
            }
            CurrStep.Text = nextStep.ToString();
            LoadPage();
        }
    }
}