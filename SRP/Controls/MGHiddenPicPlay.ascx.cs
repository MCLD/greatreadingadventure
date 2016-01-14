using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls
{
    public partial class MGHiddenPicPlay : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private int boardSize = -1;
        public int BoardSize
        {
            get
            {
                if (boardSize > 0) return boardSize;
                //Always size 3x3
                boardSize = 3;
                return boardSize;
            }
        }

        public string Grid
        {
            get 
            { 
                var g= string.Empty;
                int idx = 0;
                for (int i = 1 ; i <= BoardSize ; i++ )
                {
                    
                    g = g + string.Format("<tr>");

                    for (int j = 1; j <= BoardSize; j++)
                    {
                        idx++;
                        g = g + string.Format("<td valign='middle' align='center' id='Td" + idx.ToString() +"' class='BoardSquare color'></td>");
                    }

                    g = g + string.Format("</tr>");
                }
                return g;
            }
        }


        public List<string> SplitWords(string input)
        {
            // this splits on spaces and honors quote delineation
            // e.g. 1: test1 test2 "lions and tigers", "oh my"
            // splits to: test1, test2, lions and tigers, oh my
            var csvSplit = new Regex("(?:^| *)(\"(?:[^\"]+|\"\")*\"|[^ *]*)", RegexOptions.Compiled);

            var ret = new List<string>();
            foreach (Match match in csvSplit.Matches(input))
            {

                if (!ret.Contains(match.Value.Trim(' ')))
                    ret.Add(match.Value.Trim(' '));
            }
            //return (from Match match in csvSplit.Matches(input.Trim()) select match.Value.Trim(' ')).ToList();
            return ret;
        }

        public string GameDictionary
        {
            get
            {
                int iHPID = int.Parse(HPID.Text);
                int iDiff = int.Parse(Difficulty.Text);

                var gm = DAL.MGHiddenPic.FetchObject(iHPID);
                var dict= string.Empty;
                if (iDiff == 1) dict = gm.EasyDictionary;
                if (iDiff == 2) dict = gm.MediumDictionary;
                if (iDiff == 3) dict = gm.HardDictionary;

                List<string> terms = SplitWords(dict);
                var h = new HashSet<string>();

                string s = "[";
                var rnd = new Random(DateTime.Now.Millisecond);

                //["a", "b", "c", "d", "e", "f", "g", "h", "i"]
                if (terms.Count < 10) return "[\"a\", \"b\", \"c\", \"d\", \"e\", \"f\", \"g\", \"h\", \"i\"]";

                for (var i = 0; i < 9; i++ )
                {
                    var ticks = rnd.Next(1, terms.Count);
                    var term = terms[ticks - 1].Replace("\"", "");
                    if (term.Length > 0 && !h.Contains(term))
                    {
                        s = string.Format("{0}\"{1}\"{2}", s, terms[ticks - 1].Replace("\"", ""), (i == 8 ? "" : ", "));
                        h.Add(term);
                    }
                    else
                    {
                        i--;
                    }
                }
                s = s + "]";
                    
                return s;
            }
        }


        public void LoadGame(int mgid, int difficulty)
        {
            var mg = DAL.MGHiddenPic.FetchObjectByParent(mgid);
            HPID.Text = mg.HPID.ToString();
            Difficulty.Text = difficulty.ToString();

            LoadPage();


        }

        public void LoadPage()
        {
            int iHPID = int.Parse(HPID.Text);
            int iDiff = int.Parse(Difficulty.Text);

            var gm = DAL.MGHiddenPic.FetchObject(iHPID);
            var HPBID = DAL.MGHiddenPic.GetRandomBK(iHPID);
            BoardImg.ImageUrl = "/images/games/HiddenPic/" + HPBID + ".png";
            printLink.NavigateUrl = BoardImg.ImageUrl;
            Label1.Text = BoardImg.ImageUrl;
            Label2.Text = HPBID.ToString();
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