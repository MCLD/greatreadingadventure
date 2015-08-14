using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using SRPApp.Classes;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities;

namespace GRA.SRP.ControlRoom {
    public partial class DBCreate : BaseControlRoomPage {
        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) {
                try {
                    var p = DAL.Programs.GetAll();
                    Response.Redirect("~/ControlRoom/");
                    /*
                     * MasterPage.RequiredPermission = 3000;
                    MasterPage.IsSecure = true;

                    FailureText.Visible = true;
                    FailureText.Text =
                        "WARNING - IT APPEARS THAT THE APPLIACTION HAS ALREADY BEEN INSTALLED. CONTINUING WILL DELETE THE CURRENT INSTALL AND ALL ITS DATA AND REINSTALL.  ALL CURRENT DATA WILL BE LOST.";
                     */
                } catch {
                    // got an error, so there is no database ... continue with initialize ...
                }

            }

        }

        protected void InstallBtn_Click(object sender, EventArgs e) {
            ////////////////////////////////////////
            ////////////////////////////////////////
            ////////////////////////////////////////
            var InstallFile = "~/ControlRoom/Modules/Install/InstallScript.config";
            ////////////////////////////////////////
            ////////////////////////////////////////
            ////////////////////////////////////////

            string conn = null;
            string rcon = null;
            bool localDbMode = DBServer.Text.ToUpper() == "(LOCALDB)";

            // test writing to Web.config before we go further
            string config = null;
            if (!localDbMode) {
                if (!this.IsValid) {
                    return;
                }
                try {
                    config = System.IO.File.ReadAllText(Server.MapPath("~/Web.config"));
                } catch (Exception ex) {
                    FailureText.Text = "There was an error when trying to read the Web.config file, see below:";
                    errorLabel.Text = ex.Message;
                    return;
                }
                try {
                    System.IO.File.WriteAllText(Server.MapPath("~/Web.config"), config);
                } catch (Exception ex) {
                    FailureText.Text = "There was an error when trying to write to the Web.config file, see below:";
                    errorLabel.Text = ex.Message;
                    return;
                }
            }

            if (localDbMode) {
                conn = GlobalUtilities.SRPDB;
                rcon = GlobalUtilities.SRPDB;

                // set reasonable defaults
                string dataSource = @"(localdb)\ProjectsV12";
                string dbName = "SRP";

                // try to parse out data source and database name
                try {
                    var builder = new SqlConnectionStringBuilder(conn);
                    if (!string.IsNullOrEmpty(builder.DataSource)) {
                        dataSource = builder.DataSource;
                    }
                    if (!string.IsNullOrEmpty(builder.InitialCatalog)) {
                        dbName = builder.InitialCatalog;
                    }
                } catch (Exception) {
                    // if we can't parse the connection string, use defaults
                }

                string localDbCs = string.Format("server={0}", dataSource);

                string existsQuery = string.Format("SELECT [database_id] FROM [sys].[databases] "
                                                   + "WHERE [Name] = '{0}'",
                                                   dbName);

                var result = SqlHelper.ExecuteScalar(localDbCs, CommandType.Text, existsQuery);

                if (result == null) {
                    string createDb = string.Format("CREATE DATABASE [{0}]", dbName);

                    SqlHelper.ExecuteNonQuery(localDbCs, CommandType.Text, createDb);
                }
            } else {
                if (!this.IsValid) {
                    return;
                }

                conn = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}",
                                     DBServer.Text,
                                     DBName.Text,
                                     UserName.Text,
                                      Password.Text);
                rcon = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}",
                                     DBServer.Text,
                                     DBName.Text,
                                     RunUser.Text,
                                     RuntimePassword.Text);
            }
            var mailHost = Mailserver.Text;


            try {
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, "SELECT 1");
            } catch (Exception ex) {
                FailureText.Text = "There was an error when trying to connect with the SA account, see below:";
                errorLabel.Text = ex.Message;
                return;
            }

            try {
                SqlHelper.ExecuteNonQuery(rcon, CommandType.Text, "SELECT 1");
            } catch (Exception ex) {
                FailureText.Text = "There was an error when trying to connect with the runtime account, see below:";
                errorLabel.Text = ex.Message;
                return;
            }

            ////////////////////////////////////////
            ////////////////////////////////////////
            ////////////////////////////////////////

            var error = "";
            var sr = new StreamReader(Server.MapPath(InstallFile));
            var sb = new StringBuilder();

            using (SqlConnection connection = new SqlConnection(conn)) {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction = connection.BeginTransaction("InstallTransaction");
                command.Connection = connection;
                command.Transaction = transaction;

                while (!sr.EndOfStream) {
                    sb = new StringBuilder();
                    while (!sr.EndOfStream) {
                        var s = sr.ReadLine();
                        if (s != null && (s.ToUpper().Trim().Equals("GO") || s.ToUpper().Trim().StartsWith("GO ") || s.ToUpper().Trim().StartsWith("GO--"))) {
                            break;
                        }
                        sb.AppendLine(s);
                    }
                    try {
                        command.CommandText = sb.ToString();
                        command.ExecuteNonQuery();
                        //SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sb.ToString());
                    } catch (Exception ex) {
                        error = string.Format("{0}ERROR:{1}<br>DATA:{2}<br>SQL:<br>{3}<hr>", (error.Length == 0 ? "" : error),
                                              ex.Message, ex.Data, sb);
                    }

                }
                sr.Close();
                if (error.Length == 0) {
                    try {
                        transaction.Commit();
                    } catch (Exception ex) {
                        error = string.Format("{0}ERROR:{1}<br>DATA:{2}<br>SQL:<br>{3}<hr>", (error.Length == 0 ? "" : error),
                                              ex.Message, ex.Data, sb);
                    }
                }
                if (error.Length != 0) {
                    try {
                        transaction.Rollback();
                    } catch (Exception ex) {
                        error = string.Format("{0}ERROR:{1}<br>DATA:{2}<br>SQL:<br>{3}<hr>", (error.Length == 0 ? "" : error),
                                              ex.Message, ex.Data, sb);
                    }
                }

            }


            if (error.Length == 0 && !localDbMode) {
                if (string.IsNullOrEmpty(config)) {
                    config = System.IO.File.ReadAllText(Server.MapPath("~/Web.config"));
                }
                config =
                    config.Replace(
                        "connectionString=\"Data Source=(local);Initial Catalog=SRP;User ID=SRP;Password=SRP\"",
                        "connectionString=\"" + rcon + "\"");
                config =
                    config.Replace(
                        "<network host=\"relayServerHostname\" port=\"25\" userName=\"username\" password=\"password\" />",
                        string.Format("<network host=\"{0}\" port=\"25\"/>", mailHost));

                //Modify the web.config
                System.IO.File.WriteAllText(Server.MapPath("~/Web.config"), config);
            }

            if (error.Length == 0) {
                // Delete the Install File
                //System.IO.File.Delete(Server.MapPath(InstallFile));
                Response.Redirect("~/Default.aspx");
            } else {
                FailureText.Text = "There have been errors, see details below.";
                errorLabel.Text = error;
            }
        }



    }
}