using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;
using System.Text;
using GRA.Tools;
using SRPApp.Classes;

namespace GRA.SRP.Controls {
    public partial class MGCodeBreaker : System.Web.UI.UserControl {
        private const string CBImagePath = "~/images/games/codebreaker/{0}";
        protected void Page_Load(object sender, EventArgs e) {

        }

        public void LoadGame(int mgid, int difficulty) {
            var mg = DAL.MGCodeBreaker.FetchObjectByParent(mgid);
            CBID.Text = mg.CBID.ToString();
            Difficulty.Text = difficulty.ToString();
            LoadPage();
        }

        public void LoadPage() {
            int difficulty = int.Parse(Difficulty.Text);
            var mg = DAL.MGCodeBreaker.FetchObject(int.Parse(CBID.Text));
            string correctText = string.Empty;
            if(difficulty == 1) {
                correctText = mg.EasyString;
            }
            if(difficulty == 2) {
                correctText = mg.MediumString;
            }
            if(difficulty == 3) {
                correctText = mg.HardString;
            }

            var encodedImageList = mg.GetEncoded(correctText, difficulty);
            StringBuilder text = new StringBuilder();
            foreach(var imageFile in encodedImageList) {
                if(!string.IsNullOrEmpty(imageFile)) {
                    text.AppendFormat("<img src=\"{0}\" class=\"codebreaker-glyph\" />",
                                      VirtualPathUtility.ToAbsolute(string.Format(CBImagePath,
                                                                                  imageFile)));
                } else {
                    text.Append("<div class=\"codebreaker-space\">&nbsp;</div>");
                }
            }
            lblEncoded.Text = text.ToString();

            var keyImageList = mg.GetKey(correctText, difficulty);
            text = new StringBuilder();
            foreach(var letter in keyImageList.Keys) {
                text.Append("<div class=\"col-xs-3 text-center codebreaker-key-container\">");
                text.AppendFormat("<img src=\"{0}\" class=\"codebreaker-key-glyph\" /><br>{1}",
                                  VirtualPathUtility.ToAbsolute(string.Format(CBImagePath,
                                                                              keyImageList[letter])),
                                  char.ToUpper(letter));
                text.Append("</div>");
            }
            lblKey.Text = text.ToString();
            Correct.Text = correctText.Replace(" ", string.Empty);
        }

        protected void btnScore_Click(object sender, EventArgs e) {
            var answerNoSpaces = txtAnswer.Text.Replace(" ", string.Empty);

            if(answerNoSpaces.Equals(Correct.Text, StringComparison.OrdinalIgnoreCase)) {
                try {
                    ((Minigame)Parent.Parent.Parent.Parent).CompleteGamePlay();
                    return;
                } catch { }
                try {
                    ((GRA.SRP.ControlRoom.Controls.MinigamePreview)((Panel)Parent).Parent.Parent.Parent).CompleteGamePlay();
                    return;
                } catch { }
            } else {
                new SessionTools(Session).AlertPatron(
                    StringResources.getString("adventures-codebreaker-failure"),
                    PatronMessageLevels.Warning,
                    "question-sign");
                if(!formGroup.CssClass.Contains("has-error")){
                    formGroup.CssClass += " has-error";
                }
            }
        }
    }
}