using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls
{
    public partial class MGMatchingGamePlay : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private int boardSize = -1;
        public int BoardSize
        {
            get
            {
                //if (boardSize < 0) return boardSize;
                
                // Get from DB
                int iMAGID = int.Parse(MAGID.Text);
                int iDiff = int.Parse(Difficulty.Text);

                var gm = DAL.MGMatchingGame.FetchObject(iMAGID);
                switch (iDiff)
                {
                    case 1:
                        boardSize = gm.EasyGameSize;
                        break;
                    case 2:
                        boardSize = gm.MediumGameSize;
                        break;
                    case 3:
                        boardSize = gm.HardGameSize;
                        break;
                    default:
                        boardSize = gm.EasyGameSize;
                        break;
                }

                return boardSize;
            }
        }

        public string Grid
        {
            get
            {
                if (ViewState["imageList"] == null || ViewState["imageList"].ToString() == "")
                {
                    LoadPage();
                }

                var imageList = ViewState["imageList"].ToString().Split('|');
                
                var g= string.Empty;
                int idx = 0;
                for (int i = 1; i <= BoardSize; i++)
                {

                    g = g + string.Format("<tr>");

                    for (int j = 1; j <= BoardSize; j++)
                    {
                        idx++;
                        // IGTd - Image Table TD
                        try
                        {
                            g = g +
                                string.Format("<td valign='middle' align='center' id='IGTd" + idx.ToString() +
                                              "' class=''><img id='img" + idx.ToString() +
                                              "' src='/Images/Games/MatchingGame/" + imageList[idx - 1] + "' style='max-width: 100%;max-height: 100%'></td>");
                        }
                        catch
                        {

                            g = g +
                                string.Format("<td valign='middle' align='center' id='IGTd" + idx.ToString() +
                                              "' class=''><font color=red><b>ERROR NOT<br>ENOUGH TILES<br>DEFINED<b></font></td>");

                        }
                    }

                    g = g + string.Format("</tr>");
                }
                return g;
            }
        }


        public string PlainGrid
        {
            get
            {
                var g= string.Empty;
                int idx = 0;
                for (int i = 1; i <= BoardSize; i++)
                {

                    g = g + string.Format("<tr>");

                    for (int j = 1; j <= BoardSize; j++)
                    {
                        idx++;
                        g = g + string.Format("<td valign='middle' align='center' id='Td" + idx.ToString() + "' class='BoardSquare color'></td>");
                    }

                    g = g + string.Format("</tr>");
                }
                return g;
            }
        }

        public string GameMatches
        {
            get
            {

                if (ViewState["matchList"] == null || ViewState["matchList"].ToString() == "")
                {
                    LoadPage();
                }
                return ViewState["matchList"].ToString();
            }
        }

        public string MatchTracking
        {
            get
            {

                string s = "[";
                int maxIdx = BoardSize*BoardSize;
                for (int i = 1; i <= maxIdx; i++)
                {
                    s = string.Format("{0}false{1}", s, (i == maxIdx ? "" : ","));
                }
                return s + "]";
            }
        }

        public void LoadGame(int mgid, int difficulty)
        {
            var mg = DAL.MGMatchingGame.FetchObjectByParent(mgid);
            MAGID.Text = mg.MAGID.ToString();
            Difficulty.Text = difficulty.ToString();

            LoadPage();


        }

        public void LoadPage()
        {
            int iMAGID = int.Parse(MAGID.Text);
            int iDiff = int.Parse(Difficulty.Text);

            DataSet ds = DAL.MGMatchingGame.GetRandomPlayItems(iMAGID, (int)BoardSize * BoardSize / 2, iDiff);

            var imageList= string.Empty;
            var matchList = "[";

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                imageList = string.Format("{0}{1}{2}", imageList, ds.Tables[0].Rows[i]["TileImage"].ToString(), (i == ds.Tables[0].Rows.Count - 1 ? "" : "|"));
                matchList = string.Format("{0}\"{1}\"{2}", matchList, ds.Tables[0].Rows[i]["MAGTID"].ToString(), (i == ds.Tables[0].Rows.Count - 1 ? "" : ", "));
            }

            imageList = imageList + "";
            matchList = matchList + "]";

            ViewState["imageList"] = imageList;
            ViewState["matchList"] = matchList;

        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            try { ((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return; }
            catch { }
            try { ((GRA.SRP.ControlRoom.Controls.MinigamePreview)((Panel)Parent).Parent.Parent.Parent).CompleteGamePlay(); return; }
            catch { }
            //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return;
            //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay();
        }

    }
}