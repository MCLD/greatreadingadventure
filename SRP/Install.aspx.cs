using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;

namespace STG.SRP
{
    public partial class Install : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            var conn = txtDBO.Text;
            var rcon = txtConn.Text;
            var mailHost = txtEmailHost.Text;

            var error = "";
            var sr = new StreamReader(Server.MapPath("~/ControlRoom/Modules/Install/SQL2.txt"));
            while (!sr.EndOfStream)
            {
                var sb = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    var s = sr.ReadLine();
                    if (s != null && (s.ToUpper().Trim().Equals("GO") || s.ToUpper().Trim().StartsWith("GO ") || s.ToUpper().Trim().StartsWith("GO--")))
                    {
                        break;
                    }
                    sb.AppendLine(s);                    
                }
                try
                {
                    SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sb.ToString());
                }
                catch (Exception ex)
                {
                    error = string.Format("{0}ERROR:{1}<br>DATA:{2}<br>SQL:<br>{3}<hr>", (error.Length == 0 ? "" : error),
                                          ex.Message, ex.Data, sb);
                }

            }
            sr.Close();


            var config = System.IO.File.ReadAllText(Server.MapPath("~/web.config"));

            config = config.Replace("connectionString=\"Data Source=(local);Initial Catalog=SRP;User ID=SRP;Password=SRP\"",
                           "connectionString=\"" + rcon + "\"");
            config = config.Replace("<network host=\"relayServerHostname\" port=\"25\" userName=\"username\" password=\"password\" />",
                string.Format("<network host=\"{0}\" port=\"25\"/>", mailHost));

            //System.IO.File.WriteAllText(Server.MapPath("~/web.config"), config);

            if (error.Length == 0)
            {
                //System.IO.File.Delete(Server.MapPath("~/ControlRoom/Modules/Install/SQL.txt"));
                Response.Redirect("~/Default.aspx");                
            }
            else
            {
                Response.Write(string.Format("There have been errors<hr>{0}", error));
            }
        }
    }
}