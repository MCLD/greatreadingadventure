using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.DAL;

namespace STG.SRP.Controls
{
    public partial class MGBook : System.Web.UI.UserControl
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
                    var obpg = MGOnlineBookPages.FetchObjectByPage(int.Parse(CurrPage.Text), int.Parse(OBID.Text));
                    return "/images/games/books/e_" + obpg.OBPGID.ToString() + ".mp3";
                }
                catch(Exception)
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
                    var obpg = MGOnlineBookPages.FetchObjectByPage(int.Parse(CurrPage.Text), int.Parse(OBID.Text));
                    return "/images/games/books/m_" + obpg.OBPGID.ToString() + ".mp3";
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
                    var obpg = MGOnlineBookPages.FetchObjectByPage(int.Parse(CurrPage.Text), int.Parse(OBID.Text));
                    return "/images/games/books/h_" + obpg.OBPGID.ToString() + ".mp3";
                }
                catch (Exception)
                {
                    return "";
                }

            }
        }


        public void LoadGame(int mgid, int difficulty)
        {
            var mg = MGOnlineBook.FetchObjectByParent(mgid);
            OBID.Text = mg.OBID.ToString();
            CurrPage.Text = "1";
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

            LoadPage();


        }

        public void LoadPage()
        {
            var obpg = MGOnlineBookPages.FetchObjectByPage(int.Parse(CurrPage.Text), int.Parse(OBID.Text));

            if (obpg == null)
            {
                //((Minigame)Parent.Parent).CompleteGamePlay();
                try { ((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return; } catch {}
                try { ((STG.SRP.ControlRoom.Controls.MinigamePreview)((Panel)Parent).Parent.Parent.Parent).CompleteGamePlay(); return; } catch { }
                //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return;
            }
             //       return "/images/games/books/e_" + obpg.OBPGID.ToString();
            imgSlide.ImageUrl = "/images/games/books/" + obpg.OBPGID.ToString() + ".png";
            lblEasy.Text = obpg.TextEasy;
            lblMedium.Text = obpg.TextMedium;
            lblHard.Text = obpg.TextHard;
            var difficulty = int.Parse(Difficulty.Text);
            if (difficulty == 1) pnlAudioEasy.Visible = System.IO.File.Exists(Server.MapPath(AudioEasy));
            if (difficulty == 2) pnlAudioMedium.Visible = System.IO.File.Exists(Server.MapPath(AudioMedium));
            if (difficulty == 3) pnlAudioHard.Visible = System.IO.File.Exists(Server.MapPath(AudioHard));
        }


        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            var CPage = int.Parse(CurrPage.Text) - 1;
            CurrPage.Text = CPage.ToString();
            btnPrevious.Enabled = (CPage != 1);
            LoadPage();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            var CPage = int.Parse(CurrPage.Text) + 1;
            CurrPage.Text = CPage.ToString();
            btnPrevious.Enabled = (CPage != 1);
            LoadPage();
        }
    }
}