using System;
using System.ComponentModel;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.Classes
{
    public partial class FileDownloadCtl : System.Web.UI.UserControl
    {
        public int ImgWidth {
            get {
                if (null != ViewState["_ImgWidth_" + this.ID + "_"])
                {
                    return ViewState["_ImgWidth_" + this.ID + "_"] as dynamic;
                }
                return 0;
            }
            set {
                ViewState["_ImgWidth_" + this.ID + "_"] = value;
            }
        }

        public int ImgHeight {
            get {
                if (null != ViewState["_ImgHeight_" + this.ID + "_"])
                {
                    return ViewState["_ImgWidth_" + this.ID + "_"] as dynamic;
                }
                return 0;
            }
            set {
                ViewState["_ImgHeight_" + this.ID + "_"] = value;
            }
        }

        public int SmallThumbnailWidth {
            get {
                if (null != ViewState["_SmallThumbnailWidth_" + this.ID + "_"])
                {
                    return ViewState["_SmallThumbnailWidth_" + this.ID + "_"] as dynamic;
                }
                return 0;
            }
            set {
                ViewState["_SmallThumbnailWidth_" + this.ID + "_"] = value;
            }
        }
        public int MediumThumbnailWidth {
            get {
                if (null != ViewState["_MediumThumbnailWidth_" + this.ID + "_"])
                {
                    return ViewState["_MediumThumbnailWidth_" + this.ID + "_"] as dynamic;
                }
                return 0;
            }
            set {
                ViewState["_MediumThumbnailWidth_" + this.ID + "_"] = value;
            }
        }
        public bool CreateSmallThumbnail {
            get {
                if (null != ViewState["_CreateSmallThumbnail_" + this.ID + "_"])
                {
                    return ViewState["_CreateSmallThumbnail_" + this.ID + "_"] as dynamic;
                }
                return false;
            }
            set {
                ViewState["_CreateSmallThumbnail_" + this.ID + "_"] = value;
            }
        }
        public bool CreateMediumThumbnail {
            get {
                if (null != ViewState["_CreateMediumThumbnail_" + this.ID + "_"])
                {
                    return ViewState["_CreateMediumThumbnail_" + this.ID + "_"] as dynamic;
                }
                return false;
            }
            set {
                ViewState["_CreateMediumThumbnail_" + this.ID + "_"] = value;
            }
        }
        public string SmallThumbnailPrefix {
            get {
                if (null != ViewState["_SmallThumbnailPrefix_" + this.ID + "_"])
                {
                    return ViewState["_SmallThumbnailPrefix_" + this.ID + "_"] as string;
                }
                return "sm_";
            }
            set {
                ViewState["_SmallThumbnailPrefix_" + this.ID + "_"] = value;
            }
        }
        public string MediumThumbnailPrefix {
            get {
                if (null != ViewState["_MediumThumbnailPrefix_" + this.ID + "_"])
                {
                    return ViewState["_MediumThumbnailPrefix_" + this.ID + "_"] as string;
                }
                return "md_";
            }
            set {
                ViewState["_MediumThumbnailPrefix_" + this.ID + "_"] = value;
            }
        }
        public string Folder {
            get {
                if (null != ViewState["_Folder_" + this.ID + "_"])
                {
                    return ViewState["_Folder_" + this.ID + "_"] as string;
                }
                return "";
            }
            set {
                ViewState["_Folder_" + this.ID + "_"] = value;
            }
        }

        public string BlankSmallImage {
            get {
                if (null != ViewState["_BlankSmallImage_" + this.ID + "_"])
                {
                    return ViewState["_BlankSmallImage_" + this.ID + "_"] as string;
                }
                return "";
            }
            set {
                ViewState["_BlankSmallImage_" + this.ID + "_"] = value;
            }
        }

        public string BlankMediumImage {
            get {
                if (null != ViewState["_BlankMediumImage_" + this.ID + "_"])
                {
                    return ViewState["_BlankMediumImage_" + this.ID + "_"] as string;
                }
                return "";
            }
            set {
                ViewState["_BlankMediumImage_" + this.ID + "_"] = value;
            }
        }

        public string BlankImage {
            get {
                if (null != ViewState["_BlankImage_" + this.ID + "_"])
                {
                    return ViewState["_BlankImage_" + this.ID + "_"] as string;
                }
                return "";
            }
            set {
                ViewState["_BlankImage_" + this.ID + "_"] = value;
            }
        }



        [Browsable(true)]
        [Bindable(true, BindingDirection.TwoWay)]
        [DefaultValue("")]
        [PersistenceMode(PersistenceMode.Attribute)]
        public string FileName {
            get {
                if (null != ViewState["_FileName_" + this.ID + "_"])
                {
                    return ViewState["_FileName_" + this.ID + "_"] as string;
                }
                return "";
            }
            set {
                ViewState["_FileName_" + this.ID + "_"] = value;
            }
        }
        public string Extension {
            get {
                if (null != ViewState["_Extension_" + this.ID + "_"])
                {
                    return ViewState["_Extension_" + this.ID + "_"] as string;
                }
                return "png";
            }
            set {
                ViewState["_Extension_" + this.ID + "_"] = value;
            }
        }
        public bool FileExists()
        {
            try
            {
                return File.Exists(Server.MapPath(Folder) + "\\" + FileName + "." + Extension);
            }
            catch
            {
                return false;
            }
        }
        public bool SmallThumbnailExists()
        {
            try
            {
                return File.Exists(Server.MapPath(Folder) + "\\" + SmallThumbnailPrefix + FileName + "." + Extension);
            }
            catch
            {
                return false;
            }
        }
        public bool MediumThumbnailExists()
        {
            try
            {
                return File.Exists(Server.MapPath(Folder) + "\\" + MediumThumbnailPrefix + FileName + "." + Extension);
            }
            catch
            {
                return false;
            }
        }

        public void ProcessRender()
        {
            lblUplderr.Text = lblUplderr1.Text = string.Empty;
            if (FileExists())
            {
                pnlExists.Visible = true;
                pnlNew.Visible = false;
                pnlReplace.Visible = false;

                if (CreateSmallThumbnail && SmallThumbnailExists())
                {
                    PreviewImage1.NavigateUrl = Folder + "\\" + SmallThumbnailPrefix + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                    if (File.Exists(Server.MapPath(Folder + "\\" + SmallThumbnailPrefix + FileName + "." + Extension)))
                    {
                        PreviewImage1.ImageUrl = Folder + "\\" + SmallThumbnailPrefix + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                    }
                    lblSm.Text = "<b>Thumbnail</b><br />Width=" + SmallThumbnailWidth.ToString() + "px";
                    lblSm.Visible = true;
                }
                else {
                    PreviewImage1.Visible = false;
                    lblSm.Visible = false;
                }

                if (CreateMediumThumbnail && MediumThumbnailExists())
                {
                    PreviewImage2.NavigateUrl = Folder + "\\" + MediumThumbnailPrefix + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                    if (File.Exists(Server.MapPath(Folder + "\\" + MediumThumbnailPrefix + FileName + "." + Extension)))
                    {
                        PreviewImage2.ImageUrl = Folder + "\\" + MediumThumbnailPrefix + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                    }
                    lblMd.Text = "<b>Medium Size</b><br />Width=" + MediumThumbnailWidth.ToString() + "px";
                    lblMd.Visible = true;
                }
                else {
                    PreviewImage2.Visible = false;
                    lblMd.Visible = false;
                }

                PreviewImage3.NavigateUrl = Folder + "\\" + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                if (File.Exists(Server.MapPath(Folder + "\\" + FileName + "." + Extension)))
                {
                    PreviewImage3.ImageUrl = Folder + "\\" + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                }
                lblLg.Text = "<b>Image</b><br />Width=" + ImgWidth.ToString() + "px";
            }
            else {
                pnlNew.Visible = true;
                pnlExists.Visible = false;
                pnlReplace.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    ProcessRender();
            //}

            ProcessRender();
        }

        protected void btnReplace_Click(object sender, EventArgs e)
        {
            pnlExists.Visible = false;
            pnlReplace.Visible = true;
            pnlNew.Visible = false;
        }

        protected void btnCancel1_Click(object sender, EventArgs e)
        {
            pnlExists.Visible = true;
            pnlReplace.Visible = false;
            pnlNew.Visible = false;
        }

        private void CreateSquareImage(System.Drawing.Image upImage, string thumbName, int maxDimension)
        {
            var ratioX = (double)maxDimension / upImage.Width;
            var ratioY = (double)maxDimension / upImage.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(upImage.Width * ratio);
            var newHeight = (int)(upImage.Height * ratio);

            int x = 0;
            int y = 0;

            if (upImage.Height > upImage.Width)
            {
                x = (int)(((float)maxDimension / 2) - ((float)newWidth / 2));
            }
            else
            {
                y = (int)(((float)maxDimension / 2) - ((float)newHeight / 2));
            }

            using (var newImage = new System.Drawing.Bitmap(maxDimension, maxDimension))
            {
                using (var newGraphic = System.Drawing.Graphics.FromImage(newImage))
                {
                    newGraphic.DrawImage(upImage, x, y, newWidth, newHeight);
                    newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    newImage.Save(thumbName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        private void HandleUpload(FileUpload fileControl)
        {
            if (fileControl.HasFile)
            {
                try
                {
                    Extension = fileControl.FileName.Substring(fileControl.FileName.LastIndexOf(".") + 1);
                    Extension = "png";
                    var fileName = (Server.MapPath(Folder) + FileName + "." + Extension);

                    lblUplderr.Text = "<br>File size: " +
                            fileControl.PostedFile.ContentLength + " kb<br>" +
                            "Content type: " +
                            fileControl.PostedFile.ContentType;

                    using (var upImage = System.Drawing.Image.FromStream(fileControl.PostedFile.InputStream))
                    {
                        // Get height and width of current image
                        int currWidth = (int)upImage.Width;
                        int currHeight = (int)upImage.Height;

                        Int32 newWidth = ImgWidth;
                        float ratio = 0;
                        Int32 newHeight = 0;
                        ratio = (float)currWidth / (float)newWidth;
                        newHeight = (int)(currHeight / ratio);


                        using (var newBmp = new System.Drawing.Bitmap(newWidth,
                            newHeight,
                            System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                        {
                            newBmp.SetResolution(72, 72);
                            newBmp.MakeTransparent();
                            using (var newGraphic = System.Drawing.Graphics.FromImage(newBmp))
                            {
                                newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                                newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                newGraphic.DrawImage(upImage, 0, 0, newWidth, newHeight);
                                newBmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }

                        if (CreateSmallThumbnail)
                        {
                            string thumbName = string.Format("{0}\\{1}{2}.{3}",
                                Server.MapPath(Folder),
                                SmallThumbnailPrefix,
                                FileName,
                                Extension);
                            CreateSquareImage(upImage, thumbName, SmallThumbnailWidth);
                        }

                        if (CreateMediumThumbnail)
                        {
                            string thumbName = string.Format("{0}\\{1}{2}.{3}",
                                Server.MapPath(Folder),
                                MediumThumbnailPrefix,
                                FileName,
                                Extension);
                            CreateSquareImage(upImage, thumbName, MediumThumbnailWidth);
                        }
                    }
                    ProcessRender();
                }
                catch (Exception ex)
                {
                    lblUplderr.Text = "<br><font color=red>ERROR: " + ex.Message.ToString() + "</font>";
                }
            }
            else {
                lblUplderr.Text = "<br><font color=red>ERROR: You have not specified a file.</font>";
            }

        }

        protected void btnUpload0_Click(object sender, EventArgs e)
        {
            HandleUpload(flUpload);
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            HandleUpload(flUploadReplace);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string path = string.Format("{0}{1}",
                Server.MapPath(Folder),
                Path.DirectorySeparatorChar);

            var fileName = string.Format("{0}{1}.{2}",
                path,
                FileName,
                Extension);
            File.Delete(fileName);
            if(!string.IsNullOrEmpty(this.BlankImage))
            {
                System.IO.File.Copy(string.Format("{0}{1}.{2}", path, BlankImage, Extension),
                                    fileName);
            }
            if (CreateSmallThumbnail)
            {
                fileName = string.Format("{0}{1}{2}.{3}",
                    path,
                    SmallThumbnailPrefix,
                    FileName,
                    Extension);
                File.Delete(fileName);
                if (!string.IsNullOrEmpty(this.BlankSmallImage))
                {
                    System.IO.File.Copy(string.Format("{0}{1}.{2}", path, BlankSmallImage, Extension),
                                        fileName);
                }
            }
            if (CreateMediumThumbnail)
            {
                fileName = string.Format("{0}{1}{2}.{3}",
                    path,
                    MediumThumbnailPrefix,
                    FileName,
                    Extension);
                File.Delete(fileName);
                if (!string.IsNullOrEmpty(this.BlankMediumImage))
                {
                    System.IO.File.Copy(string.Format("{0}{1}.{2}", path, BlankMediumImage, Extension),
                                        fileName);
                }
            }
            ProcessRender();
        }

    }
}
