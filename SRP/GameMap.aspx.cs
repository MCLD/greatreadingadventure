using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using STG.SRP.DAL;
using Image = System.Drawing.Image;

namespace STG.SRP
{
    public partial class GameMap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var pg = new Programs();
            var gm = new ProgramGame();
            var pp = 0;
            try {
                pg = Programs.FetchObject(int.Parse(Session["PatronProgramID"].ToString()));
                gm = ProgramGame.FetchObject(pg.ProgramGameID);
                pp = PatronPoints.GetTotalPatronPoints(((Patron)Session["Patron"]).PID);
                }
            catch
            {
                Response.Redirect("/images/game_map_icon.png");
            }

            var PID = pg.PID;
            var numSquares = gm.BoardWidth;
            var width = 800;
            var height = 800;

            //var AID = 5;

            var squareSize = width / numSquares;
            width = height = squareSize *numSquares;

            var backImageFile = Server.MapPath(GameBoardImage());
            var stampImageFile = Server.MapPath(GameBoardStamp);
            var avatarImageFile = Server.MapPath(GameBoardAvatarStamp);

            var newBmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Image avatarImage = null;
            System.Drawing.Image stampImage = null; ;
            try
            {
                newBmp.SetResolution(72, 72);
                newBmp.MakeTransparent();
            }catch{}

            var newGraphic = System.Drawing.Graphics.FromImage(newBmp);
            try
            {
            
            newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            var backImage = System.Drawing.Image.FromFile(backImageFile);
            newGraphic.DrawImage(backImage, 0, 0, width, height);
            }
            catch { }

            try
            {
            avatarImage = System.Drawing.Image.FromFile(avatarImageFile);
            }
            catch { }
            try
            {
            stampImage = System.Drawing.Image.FromFile(stampImageFile);
            }
            catch { }
            // ----------------------------------------------
            if (Request["p"] != null) int.TryParse(Request["p"].ToString(), out pp);
            // -------------------------------------------
            var ds = ProgramGameLevel.GetAll(GetGame().PGID);


            var normalLevelTotalPoints = GetGameCompletionPoints(ds);
            var bonusLevelTotalPoints = GetGameCompletionBonusPoints(ds);


            // in bonus levels if we have more points accumulated than the total normal level points
            var bonus = (pp > normalLevelTotalPoints);
            var bonusPostfix = (bonus ? "Bonus" : "");
            var rp = pp;   //remaining points
            if (bonus)
            {
                // if we are on the bonus, eliminate the "fully completed boards/levels) and then see what the remainder of the points is.
                rp = rp - normalLevelTotalPoints;
                rp = rp % bonusLevelTotalPoints;
            }
            var idx = 0;
            // ----------------------------------------------

            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                idx++;
                var multiplier = (bonus ? GetGame().BonusLevelPointMultiplier : 1.00m);
                var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);

                var locX = Convert.ToInt32(ds.Tables[0].Rows[i]["LocationX" + bonusPostfix]) - 1;
                var locY = Convert.ToInt32(ds.Tables[0].Rows[i]["LocationY" + bonusPostfix]) - 1;
                rp = rp - levelPoints;
                if (rp < 0)
                {
                    if (avatarImage!= null) newGraphic.DrawImage(avatarImage, locX * squareSize, locY * squareSize, squareSize, squareSize);
                    break;
                }
                else
                {
                    if (stampImage != null) newGraphic.DrawImage(stampImage, locX * squareSize, locY * squareSize, squareSize, squareSize);
                }

            }

          

            Response.ContentType = "image/png";
            EnableViewState = false;
            newBmp.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);

            if (stampImage != null) stampImage.Dispose();
            if (avatarImage != null) avatarImage.Dispose();
            newGraphic.Dispose();
            newBmp.Dispose();
            Response.End();
        }

        public string GameBoardImage()
        {
              var pp = PatronPoints.GetTotalPatronPoints(((Patron)Session["Patron"]).PID); ;  //pp = patron points
                // -------------------------------------------
                if (Request["p"] != null) int.TryParse(Request["p"].ToString(), out pp);
                // -------------------------------------------

                var ds = ProgramGameLevel.GetAll(GetGame().PGID);
                var normalLevelTotalPoints = GetGameCompletionPoints(ds);
                var bonus = (pp > normalLevelTotalPoints);

                if (bonus) return GameBoardBonusImage;
                return string.Format("/images/Games/Board/{0}.png", GetGame().PGID.ToString());
        }

        public string GameBoardBonusImage
        {
            get
            {
                return string.Format("/images/Games/Board/Bonus_{0}.png", GetGame().PGID.ToString());
            }
        }

        public string GameBoardStamp
        {
            get
            {
                return string.Format("/images/Games/Board/stamp_{0}.png", GetGame().PGID.ToString());
            }
        }

        public string GameBoardAvatarStamp
        {
            get
            {
                return string.Format("/images/Avatars/{0}.png", ((Patron)Session["Patron"]).AvatarID);

            }
        }


        public int GetGameCompletionPoints(DataSet ds)
        {
            var ret = 0;
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ret = ret + Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]));
            }
            return ret;
        }

        public int GetGameCompletionBonusPoints(DataSet ds)
        {
            var ret = 0;
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var multiplier = GetGame().BonusLevelPointMultiplier;
                var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
                ret = ret + levelPoints;
            }
            return ret;
        }

        public ProgramGame GetGame()
        {
            if (ViewState["gm"] != null) return ViewState["gm"] as ProgramGame;

            var pg = Programs.FetchObject(int.Parse(Session["PatronProgramID"].ToString()));
            var gm = ProgramGame.FetchObject(pg.ProgramGameID);
            ViewState["gm"] = gm;
            return gm;
        }
    }
}