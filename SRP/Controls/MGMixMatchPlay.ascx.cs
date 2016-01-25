using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;
using System.Text;
using SRPApp.Classes;
using GRA.Tools;

namespace GRA.SRP.Controls {
    public partial class MGMixMatchPlay : System.Web.UI.UserControl {
        private const string MixMatchBasePath = "~/images/games/mixmatch/";
        private const int BorderPixels = 5;
        protected void Page_Load(object sender, EventArgs e) {

        }

        public void LoadGame(int mgid, int difficulty) {
            var mg = MGMixAndMatch.FetchObjectByParent(mgid);
            MMID.Text = mg.MMID.ToString();
            Difficulty.Text = difficulty.ToString();

            LoadPage();
        }

        public void LoadPage() {
            var i1 = new MGMixAndMatchItems();
            var i2 = new MGMixAndMatchItems();
            var i3 = new MGMixAndMatchItems();
            MGMixAndMatch.Get3RandomItems(int.Parse(MMID.Text), out i1, out i2, out i3);

            MGMixAndMatchItems correctItem = null;
            var correctItemNumber = new Random(DateTime.Now.Millisecond).Next(1, 3);
            switch(correctItemNumber) {
                case 1:
                    correctItem = i1;
                    break;
                case 2:
                    correctItem = i2;
                    break;
                case 3:
                    correctItem = i3;
                    break;
            }

            var difficulty = int.Parse(Difficulty.Text);

            if(correctItem != null) {
                MMIID.Text = correctItem.MMIID.ToString();
                StringBuilder audio = new StringBuilder(MixMatchBasePath);

                switch(difficulty) {
                    case 2:
                        //medium
                        lblMixMatch.Text = correctItem.MediumLabel;
                        audio.AppendFormat("m_{0}.mp3", MMIID.Text);
                        break;
                    case 3:
                        //hard
                        lblMixMatch.Text = correctItem.HardLabel;
                        audio.AppendFormat("h_{0}.mp3", MMIID.Text);
                        break;
                    default:
                        lblMixMatch.Text = correctItem.EasyLabel;
                        audio.AppendFormat("e_{0}.mp3", MMIID.Text);
                        break;
                }

                if(System.IO.File.Exists(Server.MapPath(audio.ToString()))) {
                    lblSound.Text = string.Format(
                        "<audio controls><source src='{0}' type='audio/mpeg'>Your browser does not support this audio format.</audio>",
                        VirtualPathUtility.ToAbsolute(audio.ToString()));
                    pnlAudio.Visible = true;
                }
            }

            Correct.Text = correctItemNumber.ToString();

            btn1.ImageUrl = string.Format("{0}{1}.png", MixMatchBasePath, i1.MMIID);
            btn2.ImageUrl = string.Format("{0}{1}.png", MixMatchBasePath, i2.MMIID);
            btn3.ImageUrl = string.Format("{0}{1}.png", MixMatchBasePath, i3.MMIID);
        }


        protected void btnImg_Command(object sender, CommandEventArgs e) {
            var clicked = e.CommandArgument.ToString();

            // default to first item
            ImageButton btn = btn1;

            // change if not first item
            switch(Correct.Text) {
                case "2":
                    btn = btn2;
                    break;
                case "3":
                    btn = btn3;
                    break;
            }

            if(clicked == Correct.Text) {
                new SessionTools(Session).AlertPatron(
                    StringResources.getString("adventures-mixmatch-success"),
                    PatronMessageLevels.Success,
                    "thumbs-up");
                btnContinue.Text = StringResources.getString("adventures-mixmatch-button-success");
                btnContinue.CssClass = "btn btn-success btn-lg btn-block";
                CurrWins.Text = (int.Parse(CurrWins.Text) + 1).ToString();
            } else {
                new SessionTools(Session).AlertPatron(
                    StringResources.getString("adventures-mixmatch-failure"),
                    PatronMessageLevels.Warning,
                    "question-sign");
                btnContinue.Text = StringResources.getString("adventures-mixmatch-button-failure");
                btnContinue.CssClass = "btn btn-danger btn-lg btn-block";
            }
            pnlContinue.Visible = true;

            btn1.Enabled = btn2.Enabled = btn3.Enabled = false;

            btn1.BorderStyle = btn2.BorderStyle = btn3.BorderStyle = BorderStyle.Solid;
            btn1.BorderWidth = btn2.BorderWidth = btn3.BorderWidth = Unit.Pixel(BorderPixels);
            btn1.BorderColor = btn2.BorderColor = btn3.BorderColor = System.Drawing.Color.DarkRed;

            btn.BorderStyle = BorderStyle.Solid;
            btn.BorderWidth = Unit.Pixel(BorderPixels);
            btn.BorderColor = System.Drawing.Color.Green;

        }

        protected void btnContinue_Click(object sender, EventArgs e) {
            pnlContinue.Visible = false;

            var mmg = DAL.MGMixAndMatch.FetchObject(int.Parse(MMID.Text));
            if(mmg.CorrectRoundsToWinCount.ToString() == CurrWins.Text) {
                //((Minigame)Parent.Parent).CompleteGamePlay();
                try { ((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return; } catch { }
                try { ((GRA.SRP.ControlRoom.Controls.MinigamePreview)((Panel)Parent).Parent.Parent.Parent).CompleteGamePlay(); return; } catch { }
                //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return;

                //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay();

                return;
            }

            LoadPage();

            btn1.Enabled = btn2.Enabled = btn3.Enabled = true;

            btn1.BorderStyle = btn2.BorderStyle = btn3.BorderStyle = BorderStyle.None;
            btn1.BorderWidth = btn2.BorderWidth = btn3.BorderWidth = Unit.Pixel(BorderPixels);
            btn1.BorderColor = btn2.BorderColor = btn3.BorderColor = System.Drawing.Color.Green;
        }
    }
}