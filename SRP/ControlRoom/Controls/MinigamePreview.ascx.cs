using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SRPApp.Classes;
using GRA.SRP.DAL;

namespace GRA.SRP.ControlRoom.Controls
{
    public partial class MinigamePreview : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["MGID"]) && (Session["MGID"] == null || Session["MGID"].ToString() == ""))
                {
                    Response.Redirect("~/ControlRoom/Modules/Setup/MiniGameList.aspx");
                }
                if (!string.IsNullOrEmpty(Request["MGID"]))
                {
                    MGID.Text = Request["MGID"];
                    Session["MGID"] = MGID.Text;
                }
                else
                {
                    MGID.Text = Session["MGID"].ToString();
                }

                if (Session["CRGoToUrl"] != null && Session["CRGoToUrl"].ToString() != "")
                {
                    GoToUrl = Session["CRGoToUrl"].ToString();
                    Session["CRGoToUrl"]= string.Empty;
                }

                var mg = DAL.Minigame.FetchObject(int.Parse(MGID.Text));
                MGName.Text = mg.GameName;
                Acknowledgements.Text = Server.HtmlDecode(mg.Acknowledgements);
                if (Acknowledgements.Text.Length == 0)
                {
                    Acknowledgements.Visible = false;
                }
                else
                {
                    Acknowledgements.Text = "<br><br>" + Acknowledgements.Text;
                }
                SetIntructions(mg.MiniGameType);
                LoadMinigame(mg.MGID, mg.MiniGameType);
            }
        }

        private void SetIntructions(int minigame)
        {
            var lbl = FindControl("lbl_mg" + minigame.ToString()) as Label;
            if (lbl != null) lbl.Visible = true;
        }

        protected override void OnInit(EventArgs e)
        {
            // Turn Off AJAX so we can use the JQuery for game boad manipulation on Hidden Picture AND Matching Game
            if (string.IsNullOrEmpty(Request["MGID"]) && (Session["MGID"] == null || Session["MGID"].ToString() == ""))
            {
                Response.Redirect("~/ControlRoom/Modules/Setup/MiniGameList.aspx");
            }
            if (!string.IsNullOrEmpty(Request["MGID"]))
            {
                MGID.Text = Request["MGID"];
                Session["MGID"] = MGID.Text;
            }
            else
            {
                MGID.Text = Session["MGID"].ToString();
            }
            var mg = DAL.Minigame.FetchObject(int.Parse(MGID.Text));
            if (mg.MiniGameType == 5 || mg.MiniGameType == 6)
            {
                try
                {
                    ((AJAX) ((BaseControlRoomPage) Page).Master).ToolkitScriptManager1.EnablePartialRendering = false;
                }catch {}
            }
            base.OnInit(e);
        }

        private void LoadMinigame(int mgid, int minigame)
        {
            if (minigame == 1)
            {
                var mg = DAL.MGOnlineBook.FetchObjectByParent(mgid);
                MGBook1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGBook1.Visible = true;

            }

            if (minigame == 2)
            {
                var mg = DAL.MGMixAndMatch.FetchObjectByParent(mgid);
                MGMixMatchPlay1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGMixMatchPlay1.Visible = true;
            }

            if (minigame == 3)
            {
                //var mg = DAL.MGCodeBreaker.FetchObjectByParent(mgid);
                var mg = DAL.MGCodeBreaker.FetchObjectByParent(mgid);
                MGCodeBreaker1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGCodeBreaker1.Visible = true;
            }

            if (minigame == 4)
            {
                var mg = DAL.MGWordMatch.FetchObjectByParent(mgid);
                MGWordMatch1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGWordMatch1.Visible = true;
            }

            if (minigame == 5)
            {
                var mg = DAL.MGMatchingGame.FetchObjectByParent(mgid);
                MGMatchingGamePlay1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGMatchingGamePlay1.Visible = true;
            }

            if (minigame == 6)
            {

                var mg = DAL.MGHiddenPic.FetchObjectByParent(mgid);
                MGHiddenPicPlay1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGHiddenPicPlay1.Visible = true;
            }

            if (minigame == 7)
            {
                var mg = DAL.MGChooseAdv.FetchObjectByParent(mgid);
                MGChooseAdvPlay1.LoadGame(mgid, 1);
                if (mg.EnableMediumDifficulty || mg.EnableHardDifficulty)
                {
                    pnlDifficulty.Visible = true;
                    BtnMedium.Visible = mg.EnableMediumDifficulty;
                    BtnHard.Visible = mg.EnableHardDifficulty;

                    pnlGame.Visible = false;
                }
                else
                {
                    pnlDifficulty.Visible = false;
                    pnlGame.Visible = true;
                }
                MGChooseAdvPlay1.Visible = true;
            }

            
        }

        protected void BtnEasy_Click(object sender, EventArgs e)
        {
            pnlDifficulty.Visible = false;
            pnlGame.Visible = true;

        }

        protected void BtnMedium_Click(object sender, EventArgs e)
        {
            pnlDifficulty.Visible = false;
            pnlGame.Visible = true;
        

            //ReloadGame(Medium)
            var mg = DAL.Minigame.FetchObject(int.Parse(MGID.Text));
            if (mg.MiniGameType == 1)
            {
                MGBook1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 2)
            {
                MGMixMatchPlay1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 3)
            {
                MGCodeBreaker1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 4)
            {
                MGWordMatch1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 5)
            {
                MGMatchingGamePlay1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 6)
            {
                MGHiddenPicPlay1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 7)
            {
                MGChooseAdvPlay1.LoadGame(mg.MGID, 2);
                pnlGame.Visible = true;
            }
        }

        protected void BtnHard_Click(object sender, EventArgs e)
        {
            pnlDifficulty.Visible = false;
            pnlGame.Visible = true;

            //ReloadGame(Hard)
            var mg = DAL.Minigame.FetchObject(int.Parse(MGID.Text));
            if (mg.MiniGameType == 1)
            {
                MGBook1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 2)
            {
                MGMixMatchPlay1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 3)
            {
                MGCodeBreaker1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 4)
            {
                MGWordMatch1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 5)
            {
                MGMatchingGamePlay1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 6)
            {
                MGHiddenPicPlay1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
            if (mg.MiniGameType == 7)
            {
                MGChooseAdvPlay1.LoadGame(mg.MGID, 3);
                pnlGame.Visible = true;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect(GoToUrl); 
            //CompleteGamePlay();
        }

        public void CompleteGamePlay()
        {
            pnlGame.Visible = false;
            pnlWinner.Visible = true;

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect(GoToUrl);
        }

        public string GoToUrl
        {
            get
            {
                if (ViewState["gotourl"] == null || ViewState["gotourl"].ToString().Length == 0)
                {
                    ViewState["gotourl"] = "~/ControlRoom/Modules/Setup/MiniGameList.aspx";
                }
                return ViewState["gotourl"].ToString();
            }
            set { ViewState["gotourl"] = value; }
        }

    }
}