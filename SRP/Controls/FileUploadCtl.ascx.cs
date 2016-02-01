using System;
using System.ComponentModel;
using System.IO;
using System.Web.UI;

namespace GRA.SRP.Classes {
    public partial class FileDownloadCtl : System.Web.UI.UserControl {
        public int ImgWidth
        {
            get
            {
                if(null != ViewState["_ImgWidth_" + this.ID + "_"]) {
                    return ViewState["_ImgWidth_" + this.ID + "_"] as dynamic;
                }
                return 0;
            }
            set
            {
                ViewState["_ImgWidth_" + this.ID + "_"] = value;
            }
        }

        public int ImgHeight
        {
            get
            {
                if(null != ViewState["_ImgHeight_" + this.ID + "_"]) {
                    return ViewState["_ImgWidth_" + this.ID + "_"] as dynamic;
                }
                return 0;
            }
            set
            {
                ViewState["_ImgHeight_" + this.ID + "_"] = value;
            }
        }

        public int SmallThumbnailWidth
        {
            get
            {
                if(null != ViewState["_SmallThumbnailWidth_" + this.ID + "_"]) {
                    return ViewState["_SmallThumbnailWidth_" + this.ID + "_"] as dynamic;
                }
                return 0;
            }
            set
            {
                ViewState["_SmallThumbnailWidth_" + this.ID + "_"] = value;
            }
        }
        public int MediumThumbnailWidth
        {
            get
            {
                if(null != ViewState["_MediumThumbnailWidth_" + this.ID + "_"]) {
                    return ViewState["_MediumThumbnailWidth_" + this.ID + "_"] as dynamic;
                }
                return 0;
            }
            set
            {
                ViewState["_MediumThumbnailWidth_" + this.ID + "_"] = value;
            }
        }
        public bool CreateSmallThumbnail
        {
            get
            {
                if(null != ViewState["_CreateSmallThumbnail_" + this.ID + "_"]) {
                    return ViewState["_CreateSmallThumbnail_" + this.ID + "_"] as dynamic;
                }
                return false;
            }
            set
            {
                ViewState["_CreateSmallThumbnail_" + this.ID + "_"] = value;
            }
        }
        public bool CreateMediumThumbnail
        {
            get
            {
                if(null != ViewState["_CreateMediumThumbnail_" + this.ID + "_"]) {
                    return ViewState["_CreateMediumThumbnail_" + this.ID + "_"] as dynamic;
                }
                return false;
            }
            set
            {
                ViewState["_CreateMediumThumbnail_" + this.ID + "_"] = value;
            }
        }
        public string SmallThumbnailPrefix
        {
            get
            {
                if(null != ViewState["_SmallThumbnailPrefix_" + this.ID + "_"]) {
                    return ViewState["_SmallThumbnailPrefix_" + this.ID + "_"] as string;
                }
                return "sm_";
            }
            set
            {
                ViewState["_SmallThumbnailPrefix_" + this.ID + "_"] = value;
            }
        }
        public string MediumThumbnailPrefix
        {
            get
            {
                if(null != ViewState["_MediumThumbnailPrefix_" + this.ID + "_"]) {
                    return ViewState["_MediumThumbnailPrefix_" + this.ID + "_"] as string;
                }
                return "md_";
            }
            set
            {
                ViewState["_MediumThumbnailPrefix_" + this.ID + "_"] = value;
            }
        }
        public string Folder
        {
            get
            {
                if(null != ViewState["_Folder_" + this.ID + "_"]) {
                    return ViewState["_Folder_" + this.ID + "_"] as string;
                }
                return "";
            }
            set
            {
                ViewState["_Folder_" + this.ID + "_"] = value;
            }
        }

        [Browsable(true)]
        [Bindable(true, BindingDirection.TwoWay)]
        [DefaultValue("")]
        [PersistenceMode(PersistenceMode.Attribute)]
        public string FileName
        {
            get
            {
                if(null != ViewState["_FileName_" + this.ID + "_"]) {
                    return ViewState["_FileName_" + this.ID + "_"] as string;
                }
                return "";
            }
            set
            {
                ViewState["_FileName_" + this.ID + "_"] = value;
            }
        }
        public string Extension
        {
            get
            {
                if(null != ViewState["_Extension_" + this.ID + "_"]) {
                    return ViewState["_Extension_" + this.ID + "_"] as string;
                }
                return "png";
            }
            set
            {
                ViewState["_Extension_" + this.ID + "_"] = value;
            }
        }
        public bool FileExists() {
            try {
                return File.Exists(Server.MapPath(Folder) + "\\" + FileName + "." + Extension);
            } catch {
                return false;
            }
        }
        public bool SmallThumbnailExists() {
            try {
                return File.Exists(Server.MapPath(Folder) + "\\" + SmallThumbnailPrefix + FileName + "." + Extension);
            } catch {
                return false;
            }
        }
        public bool MediumThumbnailExists() {
            try {
                return File.Exists(Server.MapPath(Folder) + "\\" + MediumThumbnailPrefix + FileName + "." + Extension);
            } catch {
                return false;
            }
        }

        public void ProcessRender() {
            lblUplderr.Text = lblUplderr1.Text = string.Empty;
            if(FileExists()) {
                pnlExists.Visible = true;
                pnlNew.Visible = false;
                pnlReplace.Visible = false;

                if(CreateSmallThumbnail && SmallThumbnailExists()) {
                    PreviewImage1.NavigateUrl = Folder + "\\" + SmallThumbnailPrefix + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                    if(File.Exists(Server.MapPath(Folder + "\\" + SmallThumbnailPrefix + FileName + "." + Extension))) {
                        PreviewImage1.ImageUrl = Folder + "\\" + SmallThumbnailPrefix + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                    }
                    lblSm.Text = "<b>Thumbnail</b><br />Width=" + SmallThumbnailWidth.ToString() + "px";
                    lblSm.Visible = true;
                    Image1.ImageUrl = Folder + "\\" + SmallThumbnailPrefix + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                } else {
                    PreviewImage1.Visible = false;
                    lblSm.Visible = false;
                }

                if(CreateMediumThumbnail && MediumThumbnailExists()) {
                    PreviewImage2.NavigateUrl = Folder + "\\" + MediumThumbnailPrefix + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                    if(File.Exists(Server.MapPath(Folder + "\\" + MediumThumbnailPrefix + FileName + "." + Extension))) {
                        PreviewImage2.ImageUrl = Folder + "\\" + MediumThumbnailPrefix + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                    }
                    lblMd.Text = "<b>Medium Size</b><br />Width=" + MediumThumbnailWidth.ToString() + "px";
                    lblMd.Visible = true;
                    Image2.ImageUrl = Folder + "\\" + MediumThumbnailPrefix + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                } else {
                    PreviewImage2.Visible = false;
                    lblMd.Visible = false;
                }

                PreviewImage3.NavigateUrl = Folder + "\\" + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                if(File.Exists(Server.MapPath(Folder + "\\" + FileName + "." + Extension))) {
                    PreviewImage3.ImageUrl = Folder + "\\" + FileName + "." + Extension + "?" + DateTime.Now.Ticks;
                }
                lblLg.Text = "<b>Image</b><br />Width=" + ImgWidth.ToString() + "px";
                Image3.ImageUrl = Folder + "\\" + FileName + "." + Extension + "?" + DateTime.Now.Ticks;

            } else {
                pnlNew.Visible = true;
                pnlExists.Visible = false;
                pnlReplace.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            //if (!IsPostBack)
            //{
            //    ProcessRender();
            //}

            ProcessRender();
        }

        protected void btnReplace_Click(object sender, EventArgs e) {
            pnlExists.Visible = false;
            pnlReplace.Visible = true;
            pnlNew.Visible = false;
        }

        protected void btnCancel1_Click(object sender, EventArgs e) {
            pnlExists.Visible = true;
            pnlReplace.Visible = false;
            pnlNew.Visible = false;
        }

        protected void btnUpload0_Click(object sender, EventArgs e) {
            if(flUpload.HasFile) {
                try {
                    Extension = flUpload.FileName.Substring(flUpload.FileName.LastIndexOf(".") + 1);
                    Extension = "png";
                    var fileName = (Server.MapPath(Folder) + FileName + "." + Extension);

                    //flUpload.SaveAs(fileName);
                    lblUplderr.Text = "<br>File size: " +
                            flUpload.PostedFile.ContentLength + " kb<br>" +
                            "Content type: " +
                            flUpload.PostedFile.ContentType;

                    var upImage = System.Drawing.Image.FromStream(flUpload.PostedFile.InputStream);
                    // Get height and width of current image
                    int currWidth = (int)upImage.Width;
                    int currHeight = (int)upImage.Height;

                    Int32 newWidth = ImgWidth;
                    float ratio = 0;
                    Int32 newHeight = 0;
                    ratio = (float)currWidth / (float)newWidth;
                    newHeight = (int)(currHeight / ratio);


                    var newBmp = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    newBmp.SetResolution(72, 72);
                    newBmp.MakeTransparent();
                    var newGraphic = System.Drawing.Graphics.FromImage(newBmp);

                    //newGraphic.Clear(Color.DarkRed);
                    newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    newGraphic.DrawImage(upImage, 0, 0, newWidth, newHeight);
                    newBmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

                    newBmp.Dispose();
                    newGraphic.Dispose();




                    if(CreateSmallThumbnail) {
                        fileName = Server.MapPath(Folder) + "\\" + SmallThumbnailPrefix + FileName + "." + Extension;
                        newWidth = SmallThumbnailWidth;
                        ratio = (float)currWidth / (float)newWidth;
                        newHeight = (int)(currHeight / ratio);

                        newBmp = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        newBmp.SetResolution(72, 72);
                        newBmp.MakeTransparent();
                        newGraphic = System.Drawing.Graphics.FromImage(newBmp);

                        //newGraphic.Clear(Color.DarkRed);
                        newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        newGraphic.DrawImage(upImage, 0, 0, newWidth, newHeight);
                        newBmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

                        newBmp.Dispose();
                        newGraphic.Dispose();

                    }

                    if(CreateMediumThumbnail) {
                        fileName = Server.MapPath(Folder) + "\\" + MediumThumbnailPrefix + FileName + "." + Extension;
                        newWidth = MediumThumbnailWidth;
                        ratio = (float)currWidth / (float)newWidth;
                        newHeight = (int)(currHeight / ratio);

                        newBmp = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        newBmp.SetResolution(72, 72);
                        newBmp.MakeTransparent();
                        newGraphic = System.Drawing.Graphics.FromImage(newBmp);

                        //newGraphic.Clear(Color.DarkRed);
                        newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        newGraphic.DrawImage(upImage, 0, 0, newWidth, newHeight);
                        newBmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

                        newBmp.Dispose();
                        newGraphic.Dispose();

                    }

                    upImage.Dispose();


                    // resize
                    // thumb
                    // medium


                    ProcessRender();

                } catch(Exception ex) {
                    lblUplderr.Text = "<br><font color=red>ERROR: " + ex.Message.ToString() + "</font>";
                }
            } else {
                lblUplderr.Text = "<br><font color=red>ERROR: You have not specified a file.</font>";
            }

        }

        protected void btnUpload_Click(object sender, EventArgs e) {
            if(flUploadReplace.HasFile) {
                try {
                    Extension = flUploadReplace.FileName.Substring(flUpload.FileName.LastIndexOf(".") + 1);
                    Extension = "png";
                    var fileName = (Server.MapPath(Folder) + FileName + "." + Extension);

                    //flUpload.SaveAs(fileName);
                    lblUplderr.Text = "<br>File size: " +
                            flUploadReplace.PostedFile.ContentLength + " kb<br>" +
                            "Content type: " +
                            flUploadReplace.PostedFile.ContentType;

                    var upImage = System.Drawing.Image.FromStream(flUploadReplace.PostedFile.InputStream);
                    // Get height and width of current image
                    int currWidth = (int)upImage.Width;
                    int currHeight = (int)upImage.Height;

                    Int32 newWidth = ImgWidth;
                    float ratio = 0;
                    Int32 newHeight = 0;
                    ratio = (float)currWidth / (float)newWidth;
                    newHeight = (int)(currHeight / ratio);


                    var newBmp = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    newBmp.SetResolution(72, 72);
                    newBmp.MakeTransparent();
                    var newGraphic = System.Drawing.Graphics.FromImage(newBmp);

                    //newGraphic.Clear(Color.DarkRed);\

                    newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    newGraphic.DrawImage(upImage, 0, 0, newWidth, newHeight);
                    newBmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

                    newBmp.Dispose();
                    newGraphic.Dispose();




                    if(CreateSmallThumbnail) {
                        fileName = Server.MapPath(Folder) + "\\" + SmallThumbnailPrefix + FileName + "." + Extension;
                        newWidth = SmallThumbnailWidth;
                        ratio = (float)currWidth / (float)newWidth;
                        newHeight = (int)(currHeight / ratio);

                        newBmp = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        newBmp.SetResolution(72, 72);
                        newBmp.MakeTransparent();
                        newGraphic = System.Drawing.Graphics.FromImage(newBmp);

                        //newGraphic.Clear(Color.DarkRed);
                        newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        newGraphic.DrawImage(upImage, 0, 0, newWidth, newHeight);
                        newBmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

                        newBmp.Dispose();
                        newGraphic.Dispose();

                    }

                    if(CreateMediumThumbnail) {
                        fileName = Server.MapPath(Folder) + "\\" + MediumThumbnailPrefix + FileName + "." + Extension;
                        newWidth = MediumThumbnailWidth;
                        ratio = (float)currWidth / (float)newWidth;
                        newHeight = (int)(currHeight / ratio);

                        newBmp = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        newBmp.SetResolution(72, 72);
                        newBmp.MakeTransparent();
                        newGraphic = System.Drawing.Graphics.FromImage(newBmp);

                        //newGraphic.Clear(Color.DarkRed);
                        newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        newGraphic.DrawImage(upImage, 0, 0, newWidth, newHeight);
                        newBmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

                        newBmp.Dispose();
                        newGraphic.Dispose();

                    }

                    upImage.Dispose();


                    // resize
                    // thumb
                    // medium


                    ProcessRender();

                } catch(Exception ex) {
                    lblUplderr1.Text = "<br><font color=red>ERROR: " + ex.Message.ToString() + "</font>";
                }
            } else {
                lblUplderr1.Text = "<br><font color=red>ERROR: You have not specified a file.</font>";
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e) {
            var fileName = (Server.MapPath(Folder) + "\\" + FileName + "." + Extension);
            File.Delete(fileName);
            if(CreateSmallThumbnail) {
                fileName = (Server.MapPath(Folder) + "\\" + SmallThumbnailPrefix + FileName + "." + Extension);
                File.Delete(fileName);
            }
            if(CreateMediumThumbnail) {
                fileName = (Server.MapPath(Folder) + "\\" + MediumThumbnailPrefix + FileName + "." + Extension);
                File.Delete(fileName);
            }
            ProcessRender();
        }

    }
}