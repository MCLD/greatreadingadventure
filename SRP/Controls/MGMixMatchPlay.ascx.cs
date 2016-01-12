using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;

namespace GRA.SRP.Controls
{
    public partial class MGMixMatchPlay : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string AudioEasy
        {
            get
            {
                try
                {
                    return "/images/games/mixmatch/e_" + MMIID.Text + ".mp3";
                }
                catch (Exception)
                {
                    return "";
                }
                
            }
        }
        public string AudioMedium
        {
            get
            {
                try
                {
                    return "/images/games/mixmatch/m_" + MMIID.Text + ".mp3";
                }
                catch (Exception)
                {
                    return "";
                }

            }
        }
        public string AudioHard
        {
            get
            {
                try
                {
                    return "/images/games/mixmatch/h_" + MMIID.Text + ".mp3";
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }


        public void LoadGame(int mgid, int difficulty)
        {
            var mg = MGMixAndMatch.FetchObjectByParent(mgid);
            MMID.Text = mg.MMID.ToString();

            LoadPage();

            Difficulty.Text = difficulty.ToString();
            if (difficulty == 2)
            {
                lblMedium.Visible = pnlAudioMedium.Visible = true;
                lblEasy.Visible = lblHard.Visible = pnlAudioEasy.Visible = pnlAudioHard.Visible = false;
            }
            if (difficulty == 3)
            {
                pnlAudioHard.Visible = lblHard.Visible = true;
                lblEasy.Visible = lblMedium.Visible = pnlAudioEasy.Visible = pnlAudioMedium.Visible = false;
            }

            


        }

        public void LoadPage()
        {
            var i1 = new MGMixAndMatchItems();
            var i2 = new MGMixAndMatchItems();
            var i3 = new MGMixAndMatchItems();
            MGMixAndMatch.Get3RandomItems(int.Parse(MMID.Text), out i1, out i2, out i3);

            MGMixAndMatchItems correctItem = null;
            var rnd = new Random(DateTime.Now.Millisecond);
            var ticks = rnd.Next(1, 3);
            if (ticks == 1) correctItem = i1;
            if (ticks == 2) correctItem = i2;
            if (ticks == 3) correctItem = i3;

            if (correctItem != null)
            {
                MMIID.Text = correctItem.MMIID.ToString();
                lblEasy.Text = correctItem.EasyLabel;
                lblMedium.Text = correctItem.MediumLabel;
                lblHard.Text = correctItem.HardLabel;
            }

            Correct.Text = ticks.ToString();

            btn1.ImageUrl = "/Images/Games/MixMatch/" + i1.MMIID.ToString() + ".png";
            btn2.ImageUrl = "/Images/Games/MixMatch/" + i2.MMIID.ToString() + ".png";
            btn3.ImageUrl = "/Images/Games/MixMatch/" + i3.MMIID.ToString() + ".png";


            var difficulty = int.Parse(Difficulty.Text);
            if (difficulty == 1) pnlAudioEasy.Visible = System.IO.File.Exists(Server.MapPath(AudioEasy));
            if (difficulty == 2) pnlAudioMedium.Visible = System.IO.File.Exists(Server.MapPath(AudioMedium));
            if (difficulty == 3) pnlAudioHard.Visible = System.IO.File.Exists(Server.MapPath(AudioHard));
        }


        protected void btnImg_Command(object sender, CommandEventArgs e)
        {
            var clicked = e.CommandArgument.ToString();

            ImageButton btn = btn1;

            if (Correct.Text == "1") btn = btn1;
            if (Correct.Text == "2") btn = btn2;
            if (Correct.Text == "3") btn = btn3;
            if (clicked == Correct.Text)
            {

                lblMessage.Text = "<H1 style='color: Green!important;'>Correct!</H1>";
                pnlContinue.Visible = true;

                CurrWins.Text = (int.Parse(CurrWins.Text) + 1).ToString();

                btn1.Enabled = btn2.Enabled = btn3.Enabled = false;

                btn1.BorderStyle = BorderStyle.Solid;
                btn1.BorderWidth = Unit.Pixel(3);
                btn1.BorderColor = System.Drawing.Color.DarkRed;

                btn2.BorderStyle = BorderStyle.Solid;
                btn2.BorderWidth = Unit.Pixel(3);
                btn2.BorderColor = System.Drawing.Color.DarkRed; //System.Drawing.Color.Green;

                btn3.BorderStyle = BorderStyle.Solid;
                btn3.BorderWidth = Unit.Pixel(3);
                btn3.BorderColor = System.Drawing.Color.DarkRed;

                btn.BorderStyle = BorderStyle.Solid;
                btn.BorderWidth = Unit.Pixel(3);
                btn.BorderColor = System.Drawing.Color.Green;
            }
            else
            {

                lblMessage.Text = "<H1 style='color: DarkRed!important;'>Incorrect ...</H1>";
                pnlContinue.Visible = true;

                btn1.Enabled = btn2.Enabled = btn3.Enabled = false;

                btn1.BorderStyle = BorderStyle.Solid;
                btn1.BorderWidth = Unit.Pixel(3);
                btn1.BorderColor = System.Drawing.Color.DarkRed;

                btn2.BorderStyle = BorderStyle.Solid;
                btn2.BorderWidth = Unit.Pixel(3);
                btn2.BorderColor = System.Drawing.Color.DarkRed; //System.Drawing.Color.Green;

                btn3.BorderStyle = BorderStyle.Solid;
                btn3.BorderWidth = Unit.Pixel(3);
                btn3.BorderColor = System.Drawing.Color.DarkRed;

                btn.BorderStyle = BorderStyle.Solid;
                btn.BorderWidth = Unit.Pixel(3);
                btn.BorderColor = System.Drawing.Color.Green;
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            lblMessage.Text= string.Empty;
            pnlContinue.Visible = false;

            var mmg = DAL.MGMixAndMatch.FetchObject(int.Parse(MMID.Text));
            if (mmg.CorrectRoundsToWinCount.ToString() == CurrWins.Text)
            {
                //((Minigame)Parent.Parent).CompleteGamePlay();
                try { ((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return; }
                catch { }
                try { ((GRA.SRP.ControlRoom.Controls.MinigamePreview)((Panel)Parent).Parent.Parent.Parent).CompleteGamePlay(); return; }
                catch { }
                //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return;
                
                //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay();
                
                return;
            }

            LoadPage();

            btn1.Enabled = btn2.Enabled = btn3.Enabled = true;

            btn1.BorderStyle = BorderStyle.None;
            btn1.BorderWidth = Unit.Pixel(5);
            btn1.BorderColor = System.Drawing.Color.Green;

            btn2.BorderStyle = BorderStyle.None;
            btn2.BorderWidth = Unit.Pixel(5);
            btn2.BorderColor = System.Drawing.Color.Green;

            btn3.BorderStyle = BorderStyle.None;
            btn3.BorderWidth = Unit.Pixel(5);
            btn3.BorderColor = System.Drawing.Color.Green;
        }



    }
}