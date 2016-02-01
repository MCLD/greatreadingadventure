using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;

namespace GRA.SRP.Controls
{
    public partial class MGWordMatch : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

       


        public void LoadGame(int mgid, int difficulty)
        {
            var mg = DAL.MGWordMatch.FetchObjectByParent(mgid);
            WMID.Text = mg.WMID.ToString();
            Difficulty.Text = difficulty.ToString();
            
            LoadPage();        


        }

        public void LoadPage()
        {
            int iWMID = int.Parse(WMID.Text);
            int iDiff = int.Parse(Difficulty.Text);

            var gm = DAL.MGWordMatch.FetchObject(iWMID);
            DataSet ds = DAL.MGWordMatch.GetPlayItems(iWMID, gm.NumOptionsToChooseFrom);
            rptr.DataSource = ds;
            rptr.DataBind();


            var rnd = new Random(DateTime.Now.Millisecond);
            var ticks = rnd.Next(1, gm.NumOptionsToChooseFrom);
            var correctItem = MGWordMatchItems.FetchObject(Convert.ToInt32(ds.Tables[0].Rows[ticks - 1]["WMIID"]));

            if (correctItem == null) return;
            WMIID.Text = correctItem.WMIID.ToString();
            imgItem.ImageUrl = "/Images/Games/WordMatch/" + correctItem.WMIID.ToString() + ".png";

            //var difficulty = int.Parse(Difficulty.Text);
            //if (difficulty == 1) pnlAudioEasy.Visible = System.IO.File.Exists(Server.MapPath(AudioEasy));
            //if (difficulty == 2) pnlAudioMedium.Visible = System.IO.File.Exists(Server.MapPath(AudioMedium));
            //if (difficulty == 3) pnlAudioHard.Visible = System.IO.File.Exists(Server.MapPath(AudioHard));
        }


        //protected void btnImg_Command(object sender, CommandEventArgs e)
        //{
        //    var clicked = e.CommandArgument.ToString();

        //    if (clicked == Correct.Text)
        //    {

        //        lblMessage.Text = "<H1 style='color: Green!important;'>Correct!</H1>";
        //        pnlContinue.Visible = true;

        //        CurrWins.Text = (int.Parse(CurrWins.Text) + 1).ToString();

        //        btn1.Enabled = btn2.Enabled = btn3.Enabled = false;

        //        btn1.BorderStyle = BorderStyle.Solid;
        //        btn1.BorderWidth = Unit.Pixel(3);
        //        btn1.BorderColor = System.Drawing.Color.DarkRed;

        //        btn2.BorderStyle = BorderStyle.Solid;
        //        btn2.BorderWidth = Unit.Pixel(3);
        //        btn2.BorderColor = System.Drawing.Color.Green;

        //        btn3.BorderStyle = BorderStyle.Solid;
        //        btn3.BorderWidth = Unit.Pixel(3);
        //        btn3.BorderColor = System.Drawing.Color.DarkRed;


        //    }
        //    else
        //    {

        //        lblMessage.Text = "<H1 style='color: DarkRed!important;'>Incorrect ...</H1>";
        //        pnlContinue.Visible = true;

        //        btn1.Enabled = btn2.Enabled = btn3.Enabled = false;

        //        btn1.BorderStyle = BorderStyle.Solid;
        //        btn1.BorderWidth = Unit.Pixel(3);
        //        btn1.BorderColor = System.Drawing.Color.DarkRed;

        //        btn2.BorderStyle = BorderStyle.Solid;
        //        btn2.BorderWidth = Unit.Pixel(3);
        //        btn2.BorderColor = System.Drawing.Color.Green;

        //        btn3.BorderStyle = BorderStyle.Solid;
        //        btn3.BorderWidth = Unit.Pixel(3);
        //        btn3.BorderColor = System.Drawing.Color.DarkRed;

        //    }
        //}

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            lblMessage.Text= string.Empty;
            pnlContinue.Visible = false;
            pnlChoices.Visible = true;

            var mmg = DAL.MGWordMatch.FetchObject(int.Parse(WMID.Text));
            if (mmg.CorrectRoundsToWinCount.ToString() == CurrWins.Text)
            {

                try { ((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return; }
                catch { }
                try { ((GRA.SRP.ControlRoom.Controls.MinigamePreview)((Panel)Parent).Parent.Parent.Parent).CompleteGamePlay(); return; }
                catch { }
                //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay(); return;
                //((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay();
                
                return;
            }

            LoadPage();

        }

        protected void rptr_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void rptr_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandArgument.ToString() == WMIID.Text)
            {
                lblMessage.Text = "<H1 style='color: Green!important;'>Correct!</H1>";
                pnlContinue.Visible = true;
                pnlChoices.Visible = false;
                CurrWins.Text = (int.Parse(CurrWins.Text) + 1).ToString();
            }
            else
            {

                lblMessage.Text = "<H1 style='color: DarkRed!important;'>Incorrect ...</H1>";
                pnlContinue.Visible = true;
                pnlChoices.Visible = false;
            }
        }

        public bool SetAudioVisibility(string file)
        {
            bool result = System.IO.File.Exists(Server.MapPath(file));
            if (Difficulty.Text == "1")
            {
                return file.IndexOf("/mixmatch/e_") > 0 && result;
            }
            if (Difficulty.Text == "2")
            {
                return file.IndexOf("/mixmatch/m_") > 0 && result;
            }
            if (Difficulty.Text == "3")
            {
                return file.IndexOf("/mixmatch/h_") > 0 && result;
            }
            return result;
        }

        public string SetButonText(string s1, string s2, string s3)
        {
            if (Difficulty.Text == "1") return s1;
            if (Difficulty.Text == "2") return s2;
            if (Difficulty.Text == "3") return s3;
            return s1;
        }
    }
}