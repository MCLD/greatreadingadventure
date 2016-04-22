using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eSpares.Levity;
using GRA.SRP.Core.Utilities;

namespace GRA.SRP.ControlRoom.Modules.About
{
    public partial class Default : BaseControlRoomPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MasterPage.IsSecure = true;
                MasterPage.PageTitle = "About";
                SetPageRibbon(StandardModuleRibbons.AboutRibbon());

                StringBuilder sb = new StringBuilder("<tr>");

                string appVersion = null;
                string debug = "release";
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.GetExecutingAssembly();
                    try
                    {
                        appVersion = ApplicationAssemblyUtility.GetApplicationVersionNumber();
                        if (ApplicationAssemblyUtility.ApplicationIsDebugBuild())
                        {
                            debug = "debug";
                        }
                    }
                    catch (Exception) { }

                    sb.AppendFormat("<td>{0}</td>", assembly.GetName().Name);
                    sb.Append("<td>");
                    if (!string.IsNullOrEmpty(appVersion))
                    {
                        sb.AppendFormat("{0} ({1})",
                            appVersion,
                            assembly.GetName().Version);
                    }
                    else
                    {
                        sb.AppendFormat("{0}", assembly.GetName().Version);
                    }
                    sb.AppendFormat("</td><td>{0}</td>", debug);
                    sb.Append("</tr>");
                }
                catch (Exception)
                {
                    sb.Append("<td colspan=\"3\" class=\"danger\">{0} - could not determine version of executing assembly</td>");
                }

                Type[] types = new Type[]
                {
                    this.GetType(),
                    typeof(GRA.Communications.EmailService),
                    typeof(GRA.Logic.Code),
                    typeof(GRA.Tools.DisplayHelper),
                    typeof(Microsoft.ApplicationBlocks.Data.SqlHelper),
                    typeof(SRP.DAL.Patron),
                    typeof(GRA.SRP.Core.Utilities.GlobalUtilities)
                };

                bool first = true;

                foreach (var type in types)
                {
                    if (first)
                    {
                        sb.Append("<tr class=\"info\">");
                        first = false;
                    }
                    else
                    {
                        sb.Append("<tr>");
                    }

                    appVersion = null;
                    debug = "release";
                    assembly = null;
                    try
                    {
                        assembly = Assembly.GetAssembly(type);
                        if (assembly == null)
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception)
                    {
                        sb.AppendFormat("<td colspan=\"3\" class=\"danger\">{0} - could not determine version</td>", type.ToString());
                        continue;
                    }

                    try
                    {
                        appVersion = ApplicationAssemblyUtility.GetAssemblyVersionFromType(type);
                        if (ApplicationAssemblyUtility.AssemblyIsDebugBuild(assembly))
                        {
                            debug = "debug";
                        }
                    }
                    catch (Exception) { }
                    sb.AppendFormat("<td>{0}</td>", assembly.GetName().Name);
                    sb.Append("<td>");
                    if (!string.IsNullOrEmpty(appVersion))
                    {
                        sb.AppendFormat("{0} ({1})",
                            appVersion,
                            assembly.GetName().Version);
                    }
                    else
                    {
                        sb.AppendFormat("{0}", assembly.GetName().Version);
                    }
                    sb.AppendFormat("</td><td>{0}</td>", debug);
                    sb.Append("</tr>");
                }
                VersionInformation.Text = sb.ToString();
            }

        }
    }
}