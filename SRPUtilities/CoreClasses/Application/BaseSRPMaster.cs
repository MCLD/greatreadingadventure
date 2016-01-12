using System;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace SRPApp.Classes
{
    public class BaseSRPMaster : System.Web.UI.MasterPage
    {
        #region Properties
        
        public int UID = 1;
        public string PageTitle { get; set; }

        public string SRPPageName= string.Empty;
        public int SRPPageID = -1;

        //public DataSet Template ;
        //public DataSet Widgets ;
        //public DataSet Settings;

        protected BaseSRPPage SRPPage;
        public ResourceManager rm = null;

        #endregion



        protected void PageLoad(object sender, EventArgs e)
        {
            try {SRPPage = (BaseSRPPage)this.Page;} catch {}

            //Template = SRPTemplateHelper.GetTemplate(UID);
            //Widgets = SRPTemplateHelper.GetTemplateWidgets(UID);
            //Settings = SRPTemplateHelper.GetTemplateWidgetSettings(UID);           
        }

        public void InitResFile(string strCulture)
        {
            //Session["strpgm"] = strCulture;
            //var objCi = new CultureInfo(strCulture);
            //Thread.CurrentThread.CurrentCulture = objCi;
            //Thread.CurrentThread.CurrentUICulture = objCi;
            
            //String strResourcesPath = Server.MapPath("~/Resources");
            //rm = ResourceManager.CreateFileBasedResourceManager("program.default", strResourcesPath, null);

        }

        public void InitResFile()
        {
            //var strpgm = "program.default";


            //if (Session["ProgramID"] == null || Session["ProgramID"].ToString() == "")
            //{

            //    strpgm = "program.default";
            //}
            //else
            //{
            //    strpgm = "program." + Session["ProgramID"].ToString();
            //}

            //Session["strpgm"] = strpgm ;
            //var objCI = new CultureInfo("en-US");
            //Thread.CurrentThread.CurrentCulture = objCI;
            //Thread.CurrentThread.CurrentUICulture = objCI;
            //String strResourcesPath = Server.MapPath("~/Resources");
            //rm = ResourceManager.CreateFileBasedResourceManager(strpgm, strResourcesPath, null);
             
        }

    }
}