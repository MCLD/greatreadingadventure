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
using System.Web.Configuration;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;

namespace GRA.SRP.ControlRoom {
    public partial class DBCreate : BaseControlRoomPage {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
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

        private List<string> ExecuteSqlFile(string path, string sqlConnection) {
            List<string> issues = new List<string>();
            using(SqlConnection connection = new SqlConnection(sqlConnection)) {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction = connection.BeginTransaction("InstallTransaction");
                command.Connection = connection;
                command.Transaction = transaction;
                using(var sr = new StreamReader(Server.MapPath(path))) {
                    while(!sr.EndOfStream) {
                        var sb = new StringBuilder();
                        while(!sr.EndOfStream) {
                            var s = sr.ReadLine();
                            if(s != null && (s.ToUpper().Trim().Equals("GO")
                               || s.ToUpper().Trim().StartsWith("GO ")
                               || s.ToUpper().Trim().StartsWith("GO--"))) {
                                break;
                            }
                            sb.AppendLine(s);
                        }
                        try {
                            command.CommandText = sb.ToString();
                            command.ExecuteNonQuery();
                        } catch(Exception ex) {
                            string error = string.Format("Error: {0} data: {1} on query: {2}",
                                                         ex.Message,
                                                         ex.Data,
                                                         sb);
                            this.Log().Error(() => error);
                            issues.Add(error);
                        }

                    }
                    sr.Close();
                }
                if(issues.Count == 0) {
                    try {
                        transaction.Commit();
                    } catch(Exception ex) {
                        string error = string.Format("Error committing transaction: {0}, data: {1}",
                                                     ex.Message,
                                                     ex.Data);
                        this.Log().Error(() => error);
                        issues.Add(error);
                    }
                } else {
                    try {
                        transaction.Rollback();
                    } catch(Exception ex) {
                        string error = string.Format("Error rolling back transaction: {0}, data: {1}",
                                                     ex.Message,
                                                     ex.Data);
                        this.Log().Error(() => error);
                        issues.Add(error);
                    }
                }
            }
            return issues;
        }
        protected void InstallBtn_Click(object sender, EventArgs e) {
            ////////////////////////////////////////
            ////////////////////////////////////////
            ////////////////////////////////////////
            string createSchemaFile = "~/ControlRoom/Modules/Install/CreateSchema.sql";
            string initialDataFile = "~/ControlRoom/Modules/Install/InsertInitialData.sql";
            ////////////////////////////////////////
            ////////////////////////////////////////
            ////////////////////////////////////////

            string conn = null;
            string rcon = null;
            bool localDbMode = DBServer.Text.ToUpper() == "(LOCALDB)";
            Configuration webConfig = null;

            if(string.IsNullOrEmpty(Mailaddress.Text)) {
                return;
            }

            this.Log().Info(() => "Initial GRA configuration, using LocalDb is {LocalDbMode}"
                                  .FormatWith(new { LocalDbMode = localDbMode }));

            // test writing to Web.config before we go further
            if(!localDbMode) {
                if(!this.IsValid) {
                    return;
                }

                try {
                    webConfig = WebConfigurationManager.OpenWebConfiguration("~");
                } catch(Exception ex) {
                    this.Log().Error(() => "There was an error reading the Web.config: {Message}"
                                           .FormatWith(ex));
                    FailureText.Text = "There was an error when trying to read the Web.config file, see below:";
                    errorLabel.Text = ex.Message;
                    return;
                }
                try {
                    webConfig.Save();
                } catch(Exception ex) {
                    this.Log().Error(() => "There was an error writing the Web.config file: {Message}"
                                           .FormatWith(ex));
                    FailureText.Text = "There was an error when trying to write to the Web.config file, see below:";
                    errorLabel.Text = ex.Message;
                    return;
                }
            }

            if(localDbMode) {
                conn = GlobalUtilities.SRPDB;
                rcon = GlobalUtilities.SRPDB;

                // set reasonable defaults
                string dataSource = @"(localdb)\ProjectsV12";
                string dbName = "SRP";

                // try to parse out data source and database name
                try {
                    var builder = new SqlConnectionStringBuilder(conn);
                    if(!string.IsNullOrEmpty(builder.DataSource)) {
                        dataSource = builder.DataSource;
                    }
                    if(!string.IsNullOrEmpty(builder.InitialCatalog)) {
                        dbName = builder.InitialCatalog;
                    }
                } catch(Exception) {
                    // if we can't parse the connection string, use defaults
                }

                string localDbCs = string.Format("server={0}", dataSource);

                string existsQuery = string.Format("SELECT [database_id] FROM [sys].[databases] "
                                                   + "WHERE [Name] = '{0}'",
                                                   dbName);
                object result = null;
                try {
                    result = SqlHelper.ExecuteScalar(localDbCs, CommandType.Text, existsQuery);
                } catch(Exception ex) {
                    this.Log().Error(() => "There was an error when trying to connect to LocalDb: {Message}"
                                           .FormatWith(ex));
                    FailureText.Text = "There was an error when trying to connect to LocalDb, see below:";
                    errorLabel.Text = ex.Message;
                    return;
                }

                if(result == null) {
                    string createDb = string.Format("CREATE DATABASE [{0}]", dbName);
                    try {
                        SqlHelper.ExecuteNonQuery(localDbCs, CommandType.Text, createDb);
                    } catch(Exception ex) {
                        this.Log().Error(() => "There was an error creating the database: {Message}"
                                         .FormatWith(ex));
                        FailureText.Text = "There was an error creating the database, see below:";
                        errorLabel.Text = ex.Message;
                    }
                }
            } else {
                if(!this.IsValid) {
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
            } catch(Exception ex) {
                this.Log().Error(() => "There was an error when trying to connect with the SA account: {Message}"
                                       .FormatWith(ex));
                FailureText.Text = "There was an error when trying to connect with the SA account, see below:";
                errorLabel.Text = ex.Message;
                return;
            }

            try {
                SqlHelper.ExecuteNonQuery(rcon, CommandType.Text, "SELECT 1");
            } catch(Exception ex) {
                this.Log().Error(() => "There was an error when trying to connect with the runtime account: {Message}"
                                       .FormatWith(ex));
                FailureText.Text = "There was an error when trying to connect with the runtime account, see below:";
                errorLabel.Text = ex.Message;
                return;
            }

            ////////////////////////////////////////
            ////////////////////////////////////////
            ////////////////////////////////////////
            List<string> issues = new List<string>();

            this.Log().Info(() => "Executing the queries to create the database schema.");
            issues = ExecuteSqlFile(createSchemaFile, conn);

            if(issues.Count == 0) {
                this.Log().Info(() => "Executing the queries to insert the initial data.");
                issues = ExecuteSqlFile(initialDataFile, conn);
                if(issues.Count != 0) {
                    issues.Add("Could not insert initial data. GRA will not work until the data will insert properly. Please resolve the issue, recreate the database, and run this process again.");
                    this.Log().Error(() => "Could not insert initial data. GRA will not work until the data will insert properly. Please resolve the issue, recreate the database, and run this process again.");

                }
            } else {
                // schema create didn't work
                issues.Add("Not inserting initial data due to schema issue. Please resolve the issue, recreate the database, and run this process again.");
                this.Log().Error(() => "Not inserting initial data due to schema issue. Please resolve the issue, recreate the database, and run this process again.");
            }

            if(issues.Count == 0) {
                // update email address with what the user entered
                this.Log().Info(() => "Updating the administrative email addresses to: {Text}"
                                      .FormatWith(Mailaddress));
                using(var connection = new SqlConnection(conn)) {
                    try {
                        connection.Open();
                        try {
                            SqlCommand updateEmail = new SqlCommand("UPDATE [SRPUser] SET [EmailAddress] = @emailAddress WHERE [Username] = 'sysadmin';",
                                                                    connection);
                            updateEmail.Parameters.AddWithValue("@emailAddress", Mailaddress.Text);
                            updateEmail.ExecuteNonQuery();
                        } catch(Exception ex) {
                            this.Log().Error(() => "Unable to update sysadmin email: {Message}".FormatWith(ex));
                            issues.Add("Unable to update sysadmin email: {Message}".FormatWith(ex));
                        }
                        try {
                            SqlCommand updateEmail = new SqlCommand("UPDATE [SRPSettings] SET [Value] = @emailAddress WHERE [Name] IN ('ContactEmail', 'FromEmailAddress');",
                                                                    connection);
                            updateEmail.Parameters.AddWithValue("@emailAddress", Mailaddress.Text);
                            updateEmail.ExecuteNonQuery();
                        } catch(Exception ex) {
                            this.Log().Error(() => "Unable to update settings emails: {Message}".FormatWith(ex));
                            issues.Add("Unable to update settings emails: {Message}".FormatWith(ex));
                        }
                    } catch(Exception ex) {
                        this.Log().Error(() => "Error connecting to update email address: {Message}".FormatWith(ex));
                        issues.Add("Error connecting to update email address: {Message}".FormatWith(ex));
                    } finally {
                        connection.Close();
                    }
                }
            }

            if(issues.Count == 0 && !localDbMode) {
                // modify the Web.config
                this.Log().Info(() => "Updating the Web.config file with the provided settings");

                webConfig = WebConfigurationManager.OpenWebConfiguration("~");

                var csSection = (ConnectionStringsSection)webConfig.GetSection("connectionStrings");
                csSection.ConnectionStrings[GlobalUtilities.SRPDBConnectionStringName].ConnectionString = rcon;

                if(mailHost != "(localhost)") {
                    var mailSection = (MailSettingsSectionGroup)webConfig.GetSectionGroup("system.net/mailSettings");
                    mailSection.Smtp.Network.Host = mailHost;
                }
                webConfig.Save();
            }

            if(issues.Count == 0) {
                // Delete the Install File
                //System.IO.File.Delete(Server.MapPath(InstallFile));
                this.Log().Info(() => "GRA initial setup complete!");

                Response.Redirect("~/ControlRoom/");
            } else {
                FailureText.Text = "There have been errors, see details below.";
                StringBuilder errorText = new StringBuilder();
                foreach(var issue in issues) {
                    errorText.Append(issue);
                    errorText.AppendLine("<br>");
                }
                errorLabel.Text = errorText.ToString();
            }
        }



    }
}