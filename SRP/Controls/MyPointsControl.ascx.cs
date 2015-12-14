using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;
using GRA.SRP.Utilities.CoreClasses;

namespace GRA.SRP.Controls {
    public partial class MyPointsControl : System.Web.UI.UserControl {
        protected void Page_PreRender(object sender, EventArgs e) {
            if(Session["Patron"] == null) {
                Response.Redirect("/");
            }

            LoadData();
        }

        public void LoadData() {

            var patron = (Patron)Session["Patron"];

            int tp = PatronPoints.GetTotalPatronPoints(patron.PID);
            lblPoints.Text = tp.ToInt();
            var pgm = Programs.FetchObject(patron.ProgID);
            if(pgm.ProgramGameID > 0) {
                if(ProgramGame.FetchObject(pgm.ProgramGameID) != null) {
                    LoadNextLevelInfo(patron, pgm, tp);
                }
            }
        }

        public void LoadNextLevelInfo(Patron p, Programs pg, int tp) {
            int level, points;
            bool bonus;
            GetGameInfo(p, pg, tp, out level, out points, out bonus);

            lblNextLevel.Text =
                string.Format(
                    "I'm on <strong>{0}level: {1}</strong>.<br />I need <strong>{2} point{3}</strong> to level up.<br />",
                    bonus ? "bonus " : string.Empty,
                    level,
                    points,
                    points > 1 ? "s" : string.Empty);


        }

        public void GetGameInfo(Patron patron, Programs pgm, int StartingPoints, out int level, out int points, out bool bonus) {
            //Tally up the points  
            level = 0;
            points = 0;
            bonus = false;
            if(pgm.ProgramGameID > 0) {
                // only if we have a game we can earn badges by reading ....
                var gm = ProgramGame.FetchObject(pgm.ProgramGameID);
                var ds = ProgramGameLevel.GetAll(gm.PGID);

                var normalLevelTotalPoints = GetGameCompletionPoints(ds);
                var bonusLevelTotalPoints = GetGameCompletionBonusPoints(ds, gm.BonusLevelPointMultiplier);

                bonus = (StartingPoints > normalLevelTotalPoints);

                // loop thru the levels to see where we are at ... before awarding the new points
                var rp = StartingPoints;   //remaining points
                if(bonus) {
                    // if we are on the bonus, eliminate the "fully completed boards/levels) and then see what the remainder of the points is.
                    rp = rp - normalLevelTotalPoints;
                    level = ds.Tables[0].Rows.Count + 1;   // completed all the levels for the "normal"

                    level = level + (int)((int)rp / (int)bonusLevelTotalPoints) * (ds.Tables[0].Rows.Count + 1);

                    rp = rp % bonusLevelTotalPoints;

                }

                for(var i = 0; i < ds.Tables[0].Rows.Count; i++) {
                    var multiplier = (bonus ? gm.BonusLevelPointMultiplier : 1.00m);
                    var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
                    rp = rp - levelPoints;
                    if(rp < 0) {
                        points = -rp;
                        break;
                    }
                    level++;
                }
            }
        }

        public int GetGameCompletionPoints(DataSet ds) {
            var ret = 0;
            for(var i = 0; i < ds.Tables[0].Rows.Count; i++) {
                ret = ret + Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]));
            }
            return ret;
        }

        public int GetGameCompletionBonusPoints(DataSet ds, decimal bonusLevelPointMultiplier) {
            var ret = 0;
            for(var i = 0; i < ds.Tables[0].Rows.Count; i++) {
                var multiplier = bonusLevelPointMultiplier;
                var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
                ret = ret + levelPoints;
            }
            return ret;
        }

    }
}