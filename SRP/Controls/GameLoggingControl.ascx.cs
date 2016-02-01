using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.DAL;

namespace GRA.SRP.Controls {
    public partial class GameLoggingControl : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                if(Session["Patron"] == null)
                    Response.Redirect("/");

                var patron = (Patron)Session["Patron"];
                var pgm = Programs.FetchObject(patron.ProgID);
                int tp = PatronPoints.GetTotalPatronPoints(patron.PID);
                var gm = ProgramGame.FetchObject(pgm.ProgramGameID);
                var defMGID1 = 0;
                var defMGID2 = 0;
                if(gm != null) {
                    defMGID1 = gm.Minigame1ID;
                    defMGID2 = gm.Minigame2ID;
                }

                /*
                string LevelIDs = GetGameInfo(patron, pgm, gm, tp);

                var getMinigames0 = DAL.Programs.GetProgramMinigames(LevelIDs, 0, defMGID1);
                rptrx1.DataSource = getMinigames0;
                rptrx1.DataBind();
                var getMinigames1 = DAL.Programs.GetProgramMinigames(LevelIDs, 1, defMGID2);
                rptrx2.DataSource = getMinigames1;
                rptrx2.DataBind();
                */

                var getMinigames0 = DAL.Minigame.GetMinigamesList(GetMGIDs(patron, pgm, gm, tp, defMGID1, 1));
                rptrx1.DataSource = getMinigames0;
                rptrx1.DataBind();
                var getMinigames1 = DAL.Minigame.GetMinigamesList(GetMGIDs(patron, pgm, gm, tp, defMGID2, 2));
                rptrx2.DataSource = getMinigames1;
                rptrx2.DataBind();

                NoAdventures.Visible = getMinigames0.Tables[0].Rows.Count == 0 
                    && getMinigames1.Tables[0].Rows.Count == 0;
            }

        }


        public string GetMGIDs(Patron patron, Programs pgm, ProgramGame gm, int StartingPoints, int defMGID = 0, int whichMinigames = 1) {
            //Tally up the points  
            //var level = 0;
            //var points = 0;
            var bonus = false;
            string ret = defMGID == 0 ? "" : defMGID.ToString();
            var prefix1 = whichMinigames == 1 ? "1" : "2";
            var prefixBonus = bonus ? "Bonus" : "";

            if(pgm.ProgramGameID > 0) {
                // only if we have a game we can earn badges by reading ....
                var ds = ProgramGameLevel.GetAll(gm.PGID);

                var normalLevelTotalPoints = GetGameCompletionPoints(ds);
                //var bonusLevelTotalPoints = GetGameCompletionBonusPoints(ds, gm.BonusLevelPointMultiplier);

                bonus = (StartingPoints > normalLevelTotalPoints);

                // loop thru the levels to see where we are at ... before awarding the new points
                var rp = StartingPoints;   //remaining points
                if(bonus) {
                    prefixBonus = string.Empty;   // first do all non bonus levels
                    // if we are on the bonus, we have access to all of them
                    for(var i = 0; i < ds.Tables[0].Rows.Count; i++) {
                        var MGIDfield = string.Format("Minigame{0}ID{1}", prefix1, prefixBonus);
                        if(Convert.ToInt32(ds.Tables[0].Rows[i][MGIDfield]) != 0) {
                            ret = string.Format("{0}{1}{2}", ret, (ret.Length > 0 ? "," : ""), Convert.ToInt32(ds.Tables[0].Rows[i][MGIDfield]));
                        }
                    }
                    rp = StartingPoints - normalLevelTotalPoints;
                }

                prefixBonus = bonus ? "Bonus" : "";
                // we have not tallied the bonus levels yet, or if not on bonus mode we have not tallied the regular levels yet ....
                for(var i = 0; i < ds.Tables[0].Rows.Count; i++) {
                    var multiplier = (bonus ? gm.BonusLevelPointMultiplier : 1.00m);
                    var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
                    rp = rp - levelPoints;
                    if(rp < 0) {
                        return ret;
                        //break;
                    }
                    var MGIDfield = string.Format("Minigame{0}ID{1}", prefix1, prefixBonus);
                    if(Convert.ToInt32(ds.Tables[0].Rows[i][MGIDfield]) != 0) {
                        ret = string.Format("{0}{1}{2}", ret, (ret.Length > 0 ? "," : ""), Convert.ToInt32(ds.Tables[0].Rows[i][MGIDfield]));
                    }
                }
            }
            return ret;
        }






        public string GetGameInfo(Patron patron, Programs pgm, ProgramGame gm, int StartingPoints) {
            //Tally up the points  
            //var level = 0;
            //var points = 0;
            var bonus = false;
            string ret = string.Empty;

            if(pgm.ProgramGameID > 0) {
                // only if we have a game we can earn badges by reading ....
                var ds = ProgramGameLevel.GetAll(gm.PGID);

                var normalLevelTotalPoints = GetGameCompletionPoints(ds);
                var bonusLevelTotalPoints = GetGameCompletionBonusPoints(ds, gm.BonusLevelPointMultiplier);

                bonus = (StartingPoints > normalLevelTotalPoints);

                // loop thru the levels to see where we are at ... before awarding the new points
                var rp = StartingPoints;   //remaining points
                if(bonus) {
                    // if we are on the bonus, we have access to all of them
                    for(var i = 0; i < ds.Tables[0].Rows.Count; i++) {
                        ret = string.Format("{0}{1}{2}", ret, (ret.Length > 0 ? "," : ""),
                                            Convert.ToInt32(ds.Tables[0].Rows[i]["PGLID"]));

                    }
                    return ret;
                }

                // we have not completed the bonus yet ....
                for(var i = 0; i < ds.Tables[0].Rows.Count; i++) {
                    var multiplier = (bonus ? gm.BonusLevelPointMultiplier : 1.00m);
                    var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
                    rp = rp - levelPoints;
                    if(rp < 0) {
                        return ret;
                        //break;
                    }
                    ret = string.Format("{0}{1}{2}", ret, (ret.Length > 0 ? "," : ""),
                                            Convert.ToInt32(ds.Tables[0].Rows[i]["PGLID"]));
                }
            }
            return ret;
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

        protected void rptrx1_ItemCommand(object source, RepeaterCommandEventArgs e) {
            var MGID = e.CommandArgument.ToString();
            Session["MGID"] = MGID;
            Session["GoToUrl"] = "~/Adventures/";
            Response.Redirect("~/Adventures/Adventure.aspx");
        }

        protected void rptrx1_ItemDataBound(object sender, RepeaterItemEventArgs e) {
            var rpt = sender as Repeater;
            if(rpt != null && rpt.Items.Count < 1) {
                if(e.Item.ItemType == ListItemType.Footer) {
                    // Show the Error Label (if no data is present).
                    Label lblEmptyMsg = e.Item.FindControl("lblEmptyMsg") as Label;
                    if(lblEmptyMsg != null) {
                        lblEmptyMsg.Visible = true;
                    }
                }
            }
        }

        protected void rptrx2_ItemDataBound(object sender, RepeaterItemEventArgs e) {
            var rpt = sender as Repeater;
            if(rpt != null && rpt.Items.Count < 1) {
                if(e.Item.ItemType == ListItemType.Footer) {
                    // Show the Error Label (if no data is present).
                    Label lblEmptyMsg = e.Item.FindControl("lblEmptyMsg") as Label;
                    if(lblEmptyMsg != null) {
                        lblEmptyMsg.Visible = true;
                    }
                }
            }
        }



    }
}