using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom.Controls
{
    public partial class OpenBadgesBadgeMaker : System.Web.UI.UserControl
    {
        private const string BadgePath = "~/Images/Badges";
        private const string SmallThumbnailPrefix = "sm_";
        private const string MediumThumbnailPrefix = "md_";

        public string FileName {
            get {
                return this.BadgeFileName.Value;
            }
            set {
                this.BadgeFileName.Value = value;
            }
        }

        public string SmallThumbnailWidth {
            get {
                return this.SmallThumbnailSize.Value;
            }
            set {
                this.SmallThumbnailSize.Value = value;
            }
        }
        public string MediumThumbnailWidth {
            get {
                return this.MediumThumbnailSize.Value;
            }
            set {
                this.MediumThumbnailSize.Value = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            fblaunch.HRef = string.Format("https://www.openbadges.me/designer.html?origin={0}&email={1}",
                              GRA.Tools.WebTools.GetBaseUrl(this.Request),
                              GRA.SRP.DAL.SRPSettings.GetSettingValue("ContactEmail"));

            string parameter = Request["__EVENTARGUMENT"];
            if (!string.IsNullOrWhiteSpace(parameter))
            {
                if (parameter.StartsWith("data:image/png;base64,"))
                {
                    parameter = parameter.Substring(22);
                }
                string outputPath = Server.MapPath(string.Format("{0}/{1}.png",
                    BadgePath,
                    this.BadgeFileName.Value));
                try
                {
                    using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(parameter)))
                    {
                        using (var badgeImage = System.Drawing.Image.FromStream(ms))
                        {
                            badgeImage.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);

                            int thumbSize;
                            if (int.TryParse(this.SmallThumbnailSize.Value, out thumbSize)
                                && thumbSize > 0)
                            {
                                outputPath = Server.MapPath(string.Format("{0}/{1}{2}.png",
                                    BadgePath,
                                    SmallThumbnailPrefix,
                                    this.BadgeFileName.Value));
                                using (var thumbBitmap = new System.Drawing.Bitmap(thumbSize,
                                    thumbSize,
                                    System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                                {
                                    thumbBitmap.SetResolution(72, 72);
                                    thumbBitmap.MakeTransparent();
                                    using (var thumbnail = Graphics.FromImage(thumbBitmap))
                                    {

                                        thumbnail.SmoothingMode = SmoothingMode.AntiAlias;
                                        thumbnail.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                        thumbnail.DrawImage(badgeImage,
                                            0,
                                            0,
                                            thumbSize,
                                            thumbSize);
                                        thumbBitmap.Save(outputPath,
                                            System.Drawing.Imaging.ImageFormat.Png);
                                    }
                                }
                            }

                            thumbSize = 0;
                            if (int.TryParse(this.MediumThumbnailSize.Value, out thumbSize)
                                && thumbSize > 0)
                            {
                                outputPath = Server.MapPath(string.Format("{0}/{1}{2}.png",
                                    BadgePath,
                                    MediumThumbnailPrefix,
                                    this.BadgeFileName.Value));
                                using (var thumbBitmap = new System.Drawing.Bitmap(thumbSize,
                                    thumbSize,
                                    System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                                {
                                    thumbBitmap.SetResolution(72, 72);
                                    thumbBitmap.MakeTransparent();
                                    using (var thumbnail = Graphics.FromImage(thumbBitmap))
                                    {

                                        thumbnail.SmoothingMode = SmoothingMode.AntiAlias;
                                        thumbnail.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                        thumbnail.DrawImage(badgeImage,
                                            0,
                                            0,
                                            thumbSize,
                                            thumbSize);
                                        thumbBitmap.Save(outputPath,
                                            System.Drawing.Imaging.ImageFormat.Png);
                                    }
                                }
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    this.Log().Error("Error in image returned from badge maker: {0} - {1}",
                        ex.Message,
                        ex.StackTrace);
                    return;
                }
            }
        }
    }
}