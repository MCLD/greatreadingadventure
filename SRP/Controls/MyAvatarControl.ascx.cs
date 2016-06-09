using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web;
using System.Web.UI;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using SRPApp.Classes;
using GRA.Tools;
using System.IO;
using System.Data;
using System.Web.UI.WebControls;

namespace GRA.SRP.Controls
{
    public partial class MyAvatarControl : System.Web.UI.UserControl
    {
        protected System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

        public Dictionary<string, List<int>> jsAvatarComponents = new Dictionary<string, List<int>>();


        protected void Page_Load(object sender, EventArgs e)
        {
            var patron = (Patron)Session["Patron"];

            var avatarPartData = AvatarPart.GetQualifiedByPatron(patron.PID);

            jsAvatarComponents.Clear();

            foreach (DataRow data in avatarPartData.Tables[0].Rows)
            {
                int avatarPartID = int.Parse(data["APID"].ToString());
                string componentKey = data["ComponentID"].ToString();

                if (!jsAvatarComponents.ContainsKey(componentKey))
                {
                    jsAvatarComponents.Add(componentKey, new List<int>());
                }

                jsAvatarComponents[componentKey].Add(avatarPartID);
            }

            if (!IsPostBack)
            {
                List<int> state = Patron.ReadAvatarStateString(patron.AvatarState);

                if (state.Count == 3)
                {
                    componentState0.Value = state[0].ToString();
                    componentState1.Value = state[1].ToString();
                    componentState2.Value = state[2].ToString();
                }
            }
        }

        protected void SaveShareButton_Command(Object sender, CommandEventArgs e)
        {
            SaveButton_Command(sender, e);
            var patron = (Patron)Session["Patron"];
            Response.Redirect(string.Format("~/Avatar/View.aspx?AvatarId={0}",
                patron.AvatarState));
        }

        protected void SaveButton_Command(Object sender, CommandEventArgs e)
        {
            if (e.CommandName == "save")
            {
                var patron = (Patron)Session["Patron"];

                var avatarState = new List<int>();

                try
                {
                    avatarState.Add(int.Parse(componentState0.Value));
                    avatarState.Add(int.Parse(componentState1.Value));
                    avatarState.Add(int.Parse(componentState2.Value));
                }
                catch (Exception ex)
                {
                    this.Log().Error("Error parsing avatar component states: componentState0 = {0}, componentState1 = {1}, componentState2 = {2}: {3} - {4}",
                        componentState0.Value,
                        componentState1.Value,
                        componentState2.Value,
                        ex.Message,
                        ex.StackTrace);
                    throw ex;
                }
                string state = Patron.WriteAvatarStateString(avatarState);
                patron.AvatarState = state;
                patron.Update();

                var outputPath = Server.MapPath($"/images/AvatarCache/{patron.AvatarState}.png");
                var mdOutputPath = Server.MapPath($"/images/AvatarCache/md_{patron.AvatarState}.png");
                var smOutputPath = Server.MapPath($"/images/AvatarCache/sm_{patron.AvatarState}.png");

                if (!File.Exists(outputPath))
                {
                    var image = GenerateAvatar(avatarState);
                    var mdImage = GenerateSmallImage(image, 0.6);
                    var smImage = GenerateSmallImage(image, 0.3);

                    try
                    {
                        image.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
                        mdImage.Save(mdOutputPath, System.Drawing.Imaging.ImageFormat.Png);
                        smImage.Save(smOutputPath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch (Exception ex)
                    {
                        this.Log().Error(string.Format("A problem occurred: {0}",
                                              ex.Message));
                        Session[SessionKey.PatronMessage] = string.Format("<strong>{0}</strong>",
                                                                          ex.Message);
                        Session[SessionKey.PatronMessageLevel] = PatronMessageLevels.Warning;
                        Session[SessionKey.PatronMessageGlyphicon] = "exclamation-sign";

                        return;
                    }
                }

                Session[SessionKey.PatronMessage] = "Your avatar has been updated!";
                Session[SessionKey.PatronMessageGlyphicon] = "check";
            }
        }

        protected static System.Drawing.Image GenerateSmallImage(System.Drawing.Image image, double ratio)
        {
            /* scale and crop larger image into small square */

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            /* generate square image constrained to width */
            var newImage = new Bitmap(newWidth, newWidth);
            using (var g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        protected Bitmap GenerateAvatar(List<int> avatarState)
        {
            int width = 280;
            int height = 400;

            var bitmap = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                foreach (var partID in avatarState)
                {
                    var imageFile = $"/images/AvatarParts/{partID}.png";
                    var imageUrl = Server.MapPath(imageFile);

                    var image = System.Drawing.Image.FromFile(imageUrl);

                    g.DrawImage(image, 0, 0);
                }

                g.Save();
            }

            return bitmap;
        }
    }
}