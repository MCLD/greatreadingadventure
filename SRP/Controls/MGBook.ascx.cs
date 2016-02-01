using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;
using System.Text;

namespace GRA.SRP.Controls {
    public partial class MGBook : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {

        }

        public void LoadGame(int mgid, int difficulty) {
            var mg = MGOnlineBook.FetchObjectByParent(mgid);
            OBID.Text = mg.OBID.ToString();
            CurrPage.Text = "1";
            Difficulty.Text = difficulty.ToString();
            LoadPage();
        }

        public void LoadPage() {
            var obpg = MGOnlineBookPages.FetchObjectByPage(int.Parse(CurrPage.Text), int.Parse(OBID.Text));

            if(obpg == null) {
                //((Minigame)Parent.Parent).CompleteGamePlay();
                try { ((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return; } catch { }
                try { ((GRA.SRP.ControlRoom.Controls.MinigamePreview)((Panel)Parent).Parent.Parent.Parent).CompleteGamePlay(); return; } catch { }
                //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return;
            }
            imgSlide.ImageUrl = string.Format("~/images/games/books/{0}.png", obpg.OBPGID);
            var difficulty = int.Parse(Difficulty.Text);
            StringBuilder audio = new StringBuilder("~/images/games/books/");
            switch(difficulty) {
                case 2:
                    //medium
                    lblText.Text = obpg.TextMedium;
                    audio.AppendFormat("m_{0}.mp3", obpg.OBPGID);
                    break;
                case 3:
                    //hard
                    lblText.Text = obpg.TextHard;
                    audio.AppendFormat("h_{0}.mp3", obpg.OBPGID);
                    break;
                default:
                    // 1 or anything else (shouldn't happen) is easy
                    lblText.Text = obpg.TextEasy;
                    audio.AppendFormat("e_{0}.mp3", obpg.OBPGID);
                    break;
            }

            if(System.IO.File.Exists(Server.MapPath(audio.ToString()))) {
                lblSound.Text = string.Format(
                    "<audio controls><source src='{0}' type='audio/mpeg'>Your browser does not support this audio format.</audio>",
                    VirtualPathUtility.ToAbsolute(audio.ToString()));
                pnlAudio.Visible = true;
            }
        }

        protected void btnPrevious_Click(object sender, EventArgs e) {
            var CPage = int.Parse(CurrPage.Text) - 1;
            CurrPage.Text = CPage.ToString();
            btnPrevious.Enabled = (CPage != 1);
            LoadPage();
        }

        protected void btnNext_Click(object sender, EventArgs e) {
            var CPage = int.Parse(CurrPage.Text) + 1;
            CurrPage.Text = CPage.ToString();
            btnPrevious.Enabled = (CPage != 1);
            LoadPage();
        }
    }
}