using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using GRA.SRP.Core.Utilities;
using System.Web.Configuration;
using System.Configuration;
using System.Net.Configuration;
using GRA.Communications;
using GRA.Tools;

namespace GRA.SRP.ControlRoom {
    public partial class DBCreate : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                if(string.IsNullOrEmpty(Request.QueryString["setup"])) {
                    Response.Redirect("~/ControlRoom/Configure.aspx");
                }
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
                    this.Log().Info("Error selecting programs, forwarding to setup.");
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
                int queryCount = 0;
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
                            queryCount++;
                        } catch(Exception ex) {
                            string error = string.Format("Error: {0} data: {1} on query: {2}",
                                                         ex.Message,
                                                         ex.Data,
                                                         sb);
                            issues.Add(LogAndReturnError(error));
                        }

                    }
                    sr.Close();
                    this.Log().Info("Finished processing {0}: {1} queries and {2} issues.",
                                    path.Substring(path.LastIndexOf("/") + 1),
                                    queryCount,
                                    issues.Count);
                }
                if(issues.Count == 0) {
                    try {
                        transaction.Commit();
                    } catch(Exception ex) {
                        string error = string.Format("Error committing transaction: {0}, data: {1}",
                                                     ex.Message,
                                                     ex.Data);
                        issues.Add(LogAndReturnError(error));
                    }
                } else {
                    try {
                        transaction.Rollback();
                    } catch(Exception ex) {
                        string error = string.Format("Error rolling back transaction: {0}, data: {1}",
                                                     ex.Message,
                                                     ex.Data);
                        issues.Add(LogAndReturnError(error));
                    }
                }
            }
            return issues;
        }

        private string LogAndReturnError(string errorMessage) {
            this.Log().Error(errorMessage);
            return errorMessage;
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

            this.Log().Info("GRA setup started, using LocalDb is: {0}", localDbMode);

            // test writing to Web.config before we go further
            if(!localDbMode) {
                if(!this.IsValid) {
                    return;
                }

                try {
                    webConfig = WebConfigurationManager.OpenWebConfiguration("~");
                } catch(Exception ex) {
                    this.Log().Error("There was an error reading the Web.config: {0}", ex.Message);
                    FailureText.Text = "There was an error when trying to read the Web.config file, see below:";
                    errorLabel.Text = ex.Message;
                    return;
                }
                try {
                    webConfig.Save();
                } catch(Exception ex) {
                    this.Log().Error("There was an error writing the Web.config file: {0}",
                                     ex.Message);
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
                    this.Log().Error("There was an error when trying to connect to LocalDb: {0}",
                                     ex.Message);
                    FailureText.Text = "There was an error when trying to connect to LocalDb, see below:";
                    errorLabel.Text = ex.Message;
                    return;
                }

                if(result == null) {
                    string createDb = string.Format("CREATE DATABASE [{0}]", dbName);
                    try {
                        SqlHelper.ExecuteNonQuery(localDbCs, CommandType.Text, createDb);
                    } catch(Exception ex) {
                        this.Log().Error("There was an error creating the database: {0}",
                                         ex.Message);
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
                this.Log().Error("There was an error when trying to connect with the SA account: {0}",
                                 ex.Message);
                FailureText.Text = "There was an error when trying to connect with the SA account, see below:";
                errorLabel.Text = ex.Message;
                return;
            }

            try {
                SqlHelper.ExecuteNonQuery(rcon, CommandType.Text, "SELECT 1");
            } catch(Exception ex) {
                this.Log().Error("There was an error when trying to connect with the runtime account: {0}",
                                 ex.Message);
                FailureText.Text = "There was an error when trying to connect with the runtime account, see below:";
                errorLabel.Text = ex.Message;
                return;
            }

            ////////////////////////////////////////
            ////////////////////////////////////////
            ////////////////////////////////////////
            List<string> issues = new List<string>();

            this.Log().Info("Executing the queries to create the database schema.");
            issues = ExecuteSqlFile(createSchemaFile, conn);

            if(issues.Count == 0) {
                this.Log().Info("Executing the queries to insert the initial data.");
                issues = ExecuteSqlFile(initialDataFile, conn);
                if(issues.Count != 0) {
                    issues.Add(LogAndReturnError("Could not insert initial data. GRA will not work until the data will insert properly. Please resolve the issue, recreate the database, and run this process again."));
                }
            } else {
                // schema create didn't work
                issues.Add(LogAndReturnError("Not inserting initial data due to schema issue. Please resolve the issue, recreate the database, and run this process again."));
            }

            if(issues.Count == 0) {
                // update email address with what the user entered
                this.Log().Info("Updating the administrative email addresses to: {0}",
                                Mailaddress);
                using(var connection = new SqlConnection(conn)) {
                    try {
                        connection.Open();
                        try {
                            // update the sysadmin user's email
                            SqlCommand updateEmail = new SqlCommand("UPDATE [SRPUser] SET [EmailAddress] = @emailAddress WHERE [Username] = 'sysadmin';",
                                                                    connection);
                            updateEmail.Parameters.AddWithValue("@emailAddress", Mailaddress.Text);
                            updateEmail.ExecuteNonQuery();
                        } catch(Exception ex) {
                            issues.Add(LogAndReturnError(string.Format("Unable to update sysadmin email: {0}",
                                                         ex.Message)));
                        }
                        try {
                            // update the sysadmin contact email and mail from address
                            // TODO email - provide better setup for email
                            SqlCommand updateEmail = new SqlCommand("UPDATE [SRPSettings] SET [Value] = @emailAddress WHERE [Name] IN ('ContactEmail', 'FromEmailAddress');",
                                                                    connection);
                            updateEmail.Parameters.AddWithValue("@emailAddress", Mailaddress.Text);
                            updateEmail.ExecuteNonQuery();
                        } catch(Exception ex) {
                            issues.Add(LogAndReturnError(string.Format("Unable to update settings emails: {0}", ex.Message)));
                        }
                    } catch(Exception ex) {
                        issues.Add(LogAndReturnError(string.Format("Error connecting to update email address: {0}", ex.Message)));
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
                this.Log().Info(() => "Great Reading Adventure setup complete!");
                try {
                    // TODO email - move this template out to the database
                    var values = new {
                        SystemName = "The Great Reading Adventure",
                        ControlRoomLink = string.Format("{0}{1}",
                                                        WebTools.GetBaseUrl(Request),
                                                        "/ControlRoom/"),
                    };

                    StringBuilder body = new StringBuilder();
                    body.Append("<p>Congratulations! You have successfully configured ");
                    body.Append("{SystemName}!</p><p>You may now ");
                    body.Append("<a href=\"{ControlRoomLink}\">log in</a> using the default ");
                    body.Append("system administrator credentials.</p><p>For more information on ");
                    body.Append("setting up and using the {SystemName} software, feel free to ");
                    body.Append("visit the <a href=\"http://manual.greatreadingadventure.com/\">manual</a>");
                    body.Append("and <a href=\"http://forum.greatreadingadventure.com/\">forum</a>.");
                    body.Append("</p>");

                    new EmailService().SendEmail(Mailaddress.Text,
                                                 "{SystemName} - Setup complete!"
                                                 .FormatWith(values),
                                                 body.ToString().FormatWith(values));
                    this.Log().Info(() => "Welcome email sent.");
                } catch(Exception ex) {
                    this.Log().Error(() => "Welcome email sending failure: {Message}"
                                           .FormatWith(ex));
                }

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