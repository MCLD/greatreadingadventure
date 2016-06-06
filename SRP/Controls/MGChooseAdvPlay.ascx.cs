using GRA.Tools;
using System;
using System.Web;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls
{
    public partial class MGChooseAdvPlay : System.Web.UI.UserControl
    {
        private const string CyoImagePath = "~/Images/Games/ChooseAdv/i{0}_{1}.png";
        private const string CyoAudioPath = "~/images/Games/ChooseAdv/{0}_{1}.mp3";

        public void LoadGame(int mgid, int difficulty)
        {
            var mg = DAL.MGChooseAdv.FetchObjectByParent(mgid);
            CAID.Text = mg.CAID.ToString();
            Difficulty.Text = difficulty.ToString();
            LoadPage(mg.CAID, difficulty);
        }

        public void LoadPage(int caid = -1, int difficulty = -1)
        {
            if (caid == -1)
            {
                caid = int.Parse(CAID.Text);
            }
            if (difficulty == -1)
            {
                difficulty = int.Parse(Difficulty.Text);
            }
            int stepId = int.Parse(CurrStep.Text);

            var step = DAL.MGChooseAdvSlides.FetchPlaySlide(caid, stepId, difficulty);
            lblSlideText.Text = Server.HtmlDecode(step.SlideText);
            var audioFile = string.Format(CyoAudioPath, step.CASID, Difficulty.Text);
            lblSound.Text =
                string.Format(
                    "<audio controls><source src='{0}' type='audio/mpeg'>Your browser does not support this audio format.</audio>",
                    VirtualPathUtility.ToAbsolute(audioFile));
            lblSound.Visible = System.IO.File.Exists(Server.MapPath(audioFile));

            string firstPath = String.Format(CyoImagePath, 1, step.CASID.ToString());
            string secondPath = String.Format(CyoImagePath, 2, step.CASID.ToString());

            var firstFileExists = System.IO.File.Exists(Server.MapPath(firstPath));
            var secondFileExists = System.IO.File.Exists(Server.MapPath(secondPath));

            bool selector;

            if (firstFileExists && secondFileExists)
            {
                selector = new Random().NextDouble() >= 0.5;
            }
            else
            {
                selector = firstFileExists;
                cyoContainer1.Attributes["class"]
                    = new WebTools().CssEnsureClass(cyoContainer1.Attributes["class"], "col-sm-offset-3");
                cyoContainer2.Visible = false;
            }

            btn1.ImageUrl = selector ? firstPath : secondPath;
            btn1.CommandArgument = selector
                ? step.FirstImageGoToStep.ToString()
                : step.SecondImageGoToStep.ToString();

            btn2.ImageUrl = selector ? secondPath : firstPath;
            btn2.CommandArgument = selector
                ? step.SecondImageGoToStep.ToString()
                : step.FirstImageGoToStep.ToString();
        }

        protected void btnImg_Command(object sender, CommandEventArgs e)
        {
            var nextStep = int.Parse(e.CommandArgument.ToString());
            if (nextStep == 0)
            {
                var game = Parent.Parent.Parent.Parent as Minigame;
                if (game != null)
                {
                    game.CompleteGamePlay();
                    return;
                }
                try
                {
                    ((GRA.SRP.ControlRoom.Controls.MinigamePreview)((Panel)Parent).Parent.Parent.Parent).CompleteGamePlay();
                    return;
                }
                catch (Exception ex)
                {
                    this.Log().Error("Error in completing MGChooseAdvPlay step {0}: {1} - {2}",
                        nextStep,
                        ex.Message,
                        ex.StackTrace);
                }
                return;
            }
            CurrStep.Text = nextStep.ToString();
            LoadPage();
        }
    }
}