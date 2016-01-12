using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;


namespace GRA.SRP.Controls
{
    public partial class AudioUploadCtl : System.Web.UI.UserControl
    {

        public string Folder
        {
            get
            {
                if (null != ViewState["_Folder_" + this.ID + "_"])
                {
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
                if (null != ViewState["_FileName_" + this.ID + "_"])
                {
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
                if (null != ViewState["_Extension_" + this.ID + "_"])
                {
                    return ViewState["_Extension_" + this.ID + "_"] as string;
                }
                return "mp3";
            }
            set
            {
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
        

        public void ProcessRender()
        {
            lblUplderr.Text = lblUplderr1.Text= string.Empty;
            if (FileExists())
            {
                pnlExists.Visible = true;
                pnlNew.Visible = false;
                pnlReplace.Visible = false;
                //PreviewSound.NavigateUrl = Folder + "\\" + FileName + "." + Extension + "?" + DateTime.Now;
            }
            else
            {
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

        protected void btnUpload0_Click(object sender, EventArgs e)
        {
            if (flUpload.HasFile)
            {
                try
                {
                    Extension = flUpload.FileName.Substring(flUpload.FileName.LastIndexOf(".")+1);
                    if (Extension == "") Extension = "mp3";
                    var fileName = (Server.MapPath(Folder) + FileName + "." + Extension);

                    //flUpload.SaveAs(fileName);
                    //lblUplderr.Text = "<br>File size: " +
                    //        flUpload.PostedFile.ContentLength + " kb<br>" +
                    //        "Content type: " +
                    //        flUpload.PostedFile.ContentType;

                    flUpload.SaveAs(fileName);

                    
                    ProcessRender();

                }
                catch (Exception ex)
                {
                    lblUplderr.Text = "<br><font color=red>ERROR: " + ex.Message.ToString() + "</font>";
                }
            }
            else
            {
                lblUplderr.Text = "<br><font color=red>ERROR: You have not specified a file.</font>";
            }

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (flUploadReplace.HasFile)
            {
                try
                {
                    Extension = flUploadReplace.FileName.Substring(flUpload.FileName.LastIndexOf(".") + 1);
                    if (Extension == "") Extension = "mp3";
                    var fileName = (Server.MapPath(Folder) + FileName + "." + Extension);

                    //flUpload.SaveAs(fileName);
                    //lblUplderr.Text = "<br>File size: " +
                    //        flUploadReplace.PostedFile.ContentLength + " kb<br>" +
                    //        "Content type: " +
                    //        flUploadReplace.PostedFile.ContentType;

                    flUpload.SaveAs(fileName);
                    

                    ProcessRender();

                }
                catch (Exception ex)
                {
                    lblUplderr1.Text = "<br><font color=red>ERROR: " + ex.Message.ToString() + "</font>";
                }
            }
            else
            {
                lblUplderr1.Text = "<br><font color=red>ERROR: You have not specified a file.</font>";
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var fileName = (Server.MapPath(Folder) + "\\" + FileName + "." + Extension);
            File.Delete(fileName);
          
            ProcessRender();
        }


    }
}