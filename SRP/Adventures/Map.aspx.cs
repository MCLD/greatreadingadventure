using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using GRA.SRP.DAL;
using Image = System.Drawing.Image;
using GRA.Tools;

namespace GRA.SRP {
    public partial class GameMap : System.Web.UI.Page {
        private const string StampBasePath = "~/images/Games/Board/stamp_{0}.png";
        private const string AvatarBasePath = "~/images/Avatars/{0}.png";
        protected void Page_Load(object sender, EventArgs e) {
            var patron = Session[SessionKey.Patron] as Patron;
            if(patron == null) {
                Response.Redirect("~");
            }

            Programs pg = null;
            ProgramGame gm = null;
            var pp = 0;
            try {
                pg = Programs.FetchObject(patron.ProgID);
                gm = GetGame(patron);
                pp = PatronPoints.GetTotalPatronPoints(patron.PID);
            } catch {
                pg = null;
                gm = null;
            }
            if(pg == null || gm == null) {
                Response.Redirect(Server.MapPath("~/images/game_map_icon.png"));
            }

            var PID = pg.PID;
            var numSquares = gm.BoardWidth;
            var width = 800;
            var height = 800;

            var squareSize = width / numSquares;
            width = height = squareSize * numSquares;

            var backImageFile = Server.MapPath(new Logic.Game().GetGameboardPath(patron, gm.PGID));
            var stampImageFile = Server.MapPath(string.Format(StampBasePath, gm.PGID));
            var avatarImageFile = Server.MapPath(string.Format(AvatarBasePath, patron.AvatarID));

            var newBmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Image avatarImage = null;
            System.Drawing.Image stampImage = null;
            ;
            try {
                newBmp.SetResolution(72, 72);
                newBmp.MakeTransparent();
            } catch { }

            var newGraphic = System.Drawing.Graphics.FromImage(newBmp);
            try {

                newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                var backImage = System.Drawing.Image.FromFile(backImageFile);
                newGraphic.DrawImage(backImage, 0, 0, width, height);
            } catch { }

            try {
                avatarImage = System.Drawing.Image.FromFile(avatarImageFile);
            } catch { }
            try {
                stampImage = System.Drawing.Image.FromFile(stampImageFile);
            } catch { }
            // ----------------------------------------------
            if(Request["p"] != null)
                int.TryParse(Request["p"].ToString(), out pp);
            // -------------------------------------------
            var ds = ProgramGameLevel.GetAll(GetGame(patron).PGID);


            var normalLevelTotalPoints = GetGameCompletionPoints(ds);
            var bonusLevelTotalPoints = GetGameCompletionBonusPoints(ds, patron);


            // in bonus levels if we have more points accumulated than the total normal level points
            var bonus = (pp > normalLevelTotalPoints);
            var bonusPostfix = (bonus ? "Bonus" : "");
            var rp = pp;   //remaining points
            if(bonus) {
                // if we are on the bonus, eliminate the "fully completed boards/levels) and then see what the remainder of the points is.
                rp = rp - normalLevelTotalPoints;
                rp = rp % bonusLevelTotalPoints;
            }
            var idx = 0;
            // ----------------------------------------------

            for(var i = 0; i < ds.Tables[0].Rows.Count; i++) {
                idx++;
                var multiplier = (bonus ? GetGame(patron).BonusLevelPointMultiplier : 1.00m);
                var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);

                var locX = Convert.ToInt32(ds.Tables[0].Rows[i]["LocationX" + bonusPostfix]) - 1;
                var locY = Convert.ToInt32(ds.Tables[0].Rows[i]["LocationY" + bonusPostfix]) - 1;
                rp = rp - levelPoints;
                if(rp < 0) {
                    if(avatarImage != null)
                        newGraphic.DrawImage(avatarImage, locX * squareSize, locY * squareSize, squareSize, squareSize);
                    break;
                } else {
                    if(stampImage != null)
                        newGraphic.DrawImage(stampImage, locX * squareSize, locY * squareSize, squareSize, squareSize);
                }

            }



            Response.ContentType = "image/png";
            EnableViewState = false;
            newBmp.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);

            if(stampImage != null)
                stampImage.Dispose();
            if(avatarImage != null)
                avatarImage.Dispose();
            newGraphic.Dispose();
            newBmp.Dispose();
            Response.End();
        }

        public int GetGameCompletionPoints(DataSet ds) {
            var ret = 0;
            for(var i = 0; i < ds.Tables[0].Rows.Count; i++) {
                ret = ret + Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]));
            }
            return ret;
        }

        public int GetGameCompletionBonusPoints(DataSet ds, Patron patron) {
            var ret = 0;
            for(var i = 0; i < ds.Tables[0].Rows.Count; i++) {
                var multiplier = GetGame(patron).BonusLevelPointMultiplier;
                var levelPoints = Convert.ToInt32(Convert.ToInt32(ds.Tables[0].Rows[i]["PointNumber"]) * multiplier);
                ret = ret + levelPoints;
            }
            return ret;
        }

        public ProgramGame GetGame(Patron patron) {
            if(ViewState["gm"] != null) {
                return ViewState["gm"] as ProgramGame;
            }

            var pg = Programs.FetchObject(patron.ProgID);
            var gm = ProgramGame.FetchObject(pg.ProgramGameID);
            ViewState["gm"] = gm;
            return gm;
        }
    }
}