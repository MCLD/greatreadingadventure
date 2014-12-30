using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using STG.SRP.DAL;

namespace STG.SRP.Controls
{
    public partial class GameboardControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {


            }
        }


        //public ProgramGame GetGame()
        //{
        //    if (ViewState["gm"] != null) return ViewState["gm"] as ProgramGame;

        //    var pg = Programs.FetchObject(int.Parse(Session["PatronProgramID"].ToString()));
        //    var gm = ProgramGame.FetchObject(pg.ProgramGameID);
        //    ViewState["gm"] = gm;
        //    return gm;
        //}

        //public string GameBoardImage
        //{
        //    get
        //    {
        //        //var pp = PatronPoints.GetTotalPatronPoints(((Patron)Session["Patron"]).PID); ;  //pp = patron points
        //        //// -------------------------------------------
        //        //if (Request["p"] != null) int.TryParse(Request["p"].ToString(), out pp);
        //        //// -------------------------------------------

        //        //var ds = ProgramGameLevel.GetAll(GetGame().PGID);
        //        //var normalLevelTotalPoints = GetGameCompletionPoints(ds);
        //        //var bonus = (pp > normalLevelTotalPoints);

        //        //if (bonus) return GameBoardBonusImage;
        //        ////return string.Format("/images/Games/Board/{0}.png", GetGame().PGID.ToString());
        //        return "/GameMap.aspx";// string.Format("/GameMap.aspx", GetGame().PGID.ToString());
        //    }
        //}

        //public string GameBoardBonusImage
        //{
        //    get
        //    {
        //        return string.Format("/images/Games/Board/Bonus_{0}.png", GetGame().PGID.ToString());
        //    }
        //}

        //public string GameBoardGrid
        //{
        //    get
        //    {
        //        var g = "";
        //        int idx = 0;
        //        int boardSize = GetGame().BoardWidth;

        //        for (int i = 1; i <= boardSize; i++)
        //        {

        //            g = g + string.Format("<tr>");

        //            for (int j = 1; j <= boardSize; j++)
        //            {
        //                idx++;
        //                g = g + string.Format("<td valign='middle' align='center' id='Td" + idx.ToString() + "' class='BoardSquare'></td>");
        //            }

        //            g = g + string.Format("</tr>");
        //        }
        //        return g;
        //    }
        //}

        //public string GameBoardSize { get { return GetGame().BoardWidth.ToString(); } }
        //public int GameBoardWidth { get { return GetGame().BoardWidth; } }

        //public string GameBoardStamp { 
        //    get {
        //        return string.Format("/images/Games/Board/md_stamp_{0}.png", GetGame().PGID.ToString());
        //    } 
        //}

        //public string GameBoardAvatarStamp
        //{
        //    get
        //    {
        //        return string.Format("/images/Avatars/{0}.png", ((Patron)Session["Patron"]).AvatarID);
                
        //    }
        //}


        //public int GetGameCompletionPoints(DataSet ds)
        //{
        //    var ret = 0;
        //    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
        //    {
        //        ret = ret + Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]));
        //    }
        //    return ret;
        //}

        //public int GetGameCompletionBonusPoints(DataSet ds)
        //{
        //    var ret = 0;
        //    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
        //    {
        //        var multiplier = GetGame().BonusLevelPointMultiplier;
        //        var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
        //        ret = ret + levelPoints;
        //    }
        //    return ret;
        //}

        //public string GameBoardCheckmarks
        //{
        //    get
        //    {
        //        var ret = "";
        //        // ----------------------------------------------
        //        var pp = PatronPoints.GetTotalPatronPoints(((Patron)Session["Patron"]).PID); ;  //pp = patron points
        //        // -------------------------------------------
        //        if (Request["p"] != null) int.TryParse(Request["p"].ToString(), out pp);
        //        // -------------------------------------------
        //        var ds = ProgramGameLevel.GetAll(GetGame().PGID);


        //        var normalLevelTotalPoints = GetGameCompletionPoints(ds);
        //        var bonusLevelTotalPoints = GetGameCompletionBonusPoints(ds);


        //        // in bonus levels if we have more points accumulated than the total normal level points
        //        var bonus = (pp > normalLevelTotalPoints);
        //        var rp = pp;   //remaining points
        //        if (bonus)
        //        {
        //            // if we are on the bonus, eliminate the "fully completed boards/levels) and then see what the remainder of the points is.
        //            rp = rp - normalLevelTotalPoints;
        //            rp = rp % bonusLevelTotalPoints;
        //        }
        //        var idx = 0;
        //        // ----------------------------------------------

        //        for (var i = 0; i < ds.Tables[0].Rows.Count; i++ )
        //        {
        //            idx++;
        //            var multiplier = (bonus ? GetGame().BonusLevelPointMultiplier : 1.00m);
        //            var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
        //            var levelTd = (Convert.ToInt32(ds.Tables[0].Rows[i]["LocationY"]) - 1) * GameBoardWidth +
        //                          Convert.ToInt32(ds.Tables[0].Rows[i]["LocationX"]);
        //            rp = rp - levelPoints;
        //            if (rp < 0)
        //            {
        //                ret = string.Format("{0}\r\n$(\"#Td{1}\").html(\"<img src='{2}' class='BoardSquareImg'/>\");", ret, levelTd, GameBoardAvatarStamp);
        //                break;
        //            }
        //            else
        //            {
        //                ret = string.Format("{0}\r\n$(\"#Td{1}\").html(\"<img src='{2}' class='BoardSquareImg'/>\");", ret, levelTd, GameBoardStamp);
        //            }

        //        }

        //        return ret;
        //    }
        //}
    }
}