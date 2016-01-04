using GRA.Communications;
using GRA.SRP.Core.Utilities;
using GRA.Tools;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GRA.SRP.ControlRoom {

    public partial class Configure : System.Web.UI.Page {
        private const string DbOwnerKey = "DbOwnerKey";
        private const string DbUserKey = "DbUserKey";
        private const string MailPwKey = "MailPwKey";
        private const string StepKey = "Step";

        private const short BlankPasswordLength = 16;

        private const string CreateSchemaPath = "~/ControlRoom/Modules/Install/CreateSchema.sql";
        private const string Insert1ProgramPath = "~/ControlRoom/Modules/Install/InsertInitialData.sql";
        private const string Insert4ProgramsPath = "~/ControlRoom/Modules/Install/InsertInitialData-MultiplePrograms.sql";

        protected void Page_Load(object sender, EventArgs e) {
            try {
                var programs = DAL.Programs.GetAll();
                // if this worked then we have a database and shouldn't be in configuration
                Response.Redirect("~/ControlRoom/");
            } catch(Exception ex) {
                // got an error querying the database, need to do configuration
                this.Log().Info("Error {0} selecting programs, proceeding with configuration.",
                                ex.Message);
            }

            if(Page.IsPostBack) {
                DatabaseIssuePanel.Visible = false;
                MailIssuePanel.Visible = false;
                MailSuccessPanel.Visible = false;
                FinalSuccessPanel.Visible = false;
                FinalIssuePanel.Visible = false;
                FinalProgressBar.Visible = false;
            } else {
                string connectionString = GlobalUtilities.SRPDB;
                try {
                    var builder = new SqlConnectionStringBuilder(connectionString);
                    if(!string.IsNullOrEmpty(builder.DataSource)) {
                        DatabaseServer.Text = builder.DataSource;
                    }
                    if(!string.IsNullOrEmpty(builder.InitialCatalog)) {
                        DatabaseCatalog.Text = builder.InitialCatalog;
                    }
                    if(!string.IsNullOrEmpty(builder.UserID)) {
                        DatabaseUserUser.Text = builder.UserID;
                    }
                    if(!string.IsNullOrEmpty(builder.Password)) {
                        ViewState[DbUserKey] = builder.Password;
                        DatabaseUserPassword.Text = MakeBlankPassword();
                    }
                } catch(Exception ex) {
                    this.Log().Error("Problem reading connection string from Web.config: {0}",
                                     ex.Message);
                }
            }
        }

        protected void HandlePasswordFields(TextBox field, string key) {
            if(!string.IsNullOrEmpty(field.Text)) {
                // there's text in the password blank
                if(ViewState[key] == null) {
                    // if we don't have a password, save it
                    ViewState[key] = field.Text;
                } else {
                    // we do have a password. update it?
                    if(!field.Text.Equals(MakeBlankPassword(),
                                          StringComparison.OrdinalIgnoreCase)) {
                        // it's different, save it
                        ViewState[key] = field.Text;
                        // it's saved, swap out for values in the field
                        field.Attributes.Remove("value");
                        field.Attributes.Add("value", MakeBlankPassword());
                    }
                }
            } else {
                // no text in the password blank, fill it with asterisks if it's visible
                if(field.Visible && ViewState[key] != null) {
                    field.Attributes.Add("value", MakeBlankPassword());
                }
            }

        }

        protected string MakeBlankPassword() {
            return new string('*', BlankPasswordLength);
        }

        protected void ShowStep(int stepNumber, bool skipValidation) {
            CurrentStep.Text = stepNumber.ToString();
            step1.Visible = false;
            step2.Visible = false;
            step3.Visible = false;
            step4.Visible = false;
            step5.Visible = false;
            step6.Visible = false;
            NavigationBack.Enabled = true;
            NavigationNext.Enabled = true;

            switch(stepNumber) {
                case 1:
                    // instructions
                    NavigationBack.Enabled = false;
                    step1.Visible = true;
                    break;
                case 2:
                    // database
                    step2.Visible = true;
                    break;
                case 3:
                    // mail
                    if(!skipValidation) {
                        List<string> databaseStatus = null;
                        try {
                            // save passwords
                            HandlePasswordFields(DatabaseOwnerPassword, DbOwnerKey);
                            HandlePasswordFields(DatabaseUserPassword, DbUserKey);
                            databaseStatus = ValidateDatabase();
                        } catch(Exception ex) {
                            // back to step 2
                            this.Log().Error("Error in database configuration: {0}", ex.Message);
                            DatabaseIssuePanel.Visible = true;
                            DatabaseIssueMessage.Text = ex.Message;
                            SendToStep(2);
                            break;
                        }
                        if(databaseStatus != null && databaseStatus.Count > 0) {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("Database configuration issue{0} found:<ul>",
                                            databaseStatus.Count == 1 ? string.Empty : "s");
                            foreach(var issue in databaseStatus) {
                                sb.AppendFormat("<li>{0}</li>", issue);
                                this.Log().Error("Database configuration problem: {0}", issue);
                            }
                            sb.Append("</ul>");
                            var ignoreError = ConfigurationManager.AppSettings[AppSettingKeys.IgnoreMissingDatabaseGroups.ToString()];
                            if(string.IsNullOrEmpty(ignoreError)
                               || !ignoreError.Equals("true", StringComparison.OrdinalIgnoreCase)) {
                                DatabaseIssuePanel.Visible = true;
                                DatabaseIssueMessage.Text = sb.ToString();
                                SendToStep(2);
                                break;
                            }
                        }
                    }
                    step3.Visible = true;
                    break;
                case 4:
                    // program
                    HandlePasswordFields(MailPassword, MailPwKey);
                    step4.Visible = true;
                    break;
                case 5:
                    // confirmation
                    DatabaseNameLabel.Text = DatabaseCatalog.Text;
                    DatabaseServerLabel.Text = DatabaseServer.Text;
                    MailServerLabel.Text = MailServer.Text;
                    MailAddressLabel.Text = MailAddress.Text;
                    ReadingProgramLabel.Text = ReadingProgram.Value;
                    step5.Visible = true;
                    break;
                case 6:
                    var configureResults = ConfigureTheGRA();
                    var success = configureResults == null || configureResults.Count == 0;

                    FinalProgressBar.Visible = true;
                    FinalProgressBar.Attributes.Remove("class");
                    if(success) {
                        FinalSuccessPanel.Visible = true;
                        FinalIssuePanel.Visible = false;
                        FinalProgressBar.Attributes.Add("class", "progress-bar progress-bar-success progress-bar-striped");
                        FinalProgressBarMessage.Text = "Configuration successful!";
                        NavigationBack.Enabled = false;
                    } else {
                        // make them perform step 6 again
                        FinalSuccessPanel.Visible = false;
                        FinalIssuePanel.Visible = true;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("The following issue{0} occurred:<ul>",
                                        configureResults.Count == 1 ? string.Empty : "s");
                        foreach(string issue in configureResults) {
                            this.Log().Error("Final configuration error: {0}", issue);
                            sb.AppendFormat("<li>{0}</li>", issue);
                        }
                        sb.Append("</ul>");
                        FinalIssueMessage.Text = sb.ToString();
                        FinalProgressBar.Visible = true;
                        FinalProgressBar.Attributes.Remove("class");
                        FinalProgressBar.Attributes.Add("class", "progress-bar progress-bar-danger progress-bar-striped");
                        FinalProgressBarMessage.Text = "Configuration failed, see below.";
                        NavigationBack.Enabled = true;
                        ViewState[StepKey] = 5;
                    }
                    step6.Visible = true;
                    break;
                case 7:
                    Response.Redirect("~/ControlRoom/");
                    break;
            }

            HandlePasswordFields(DatabaseOwnerPassword, DbOwnerKey);
            HandlePasswordFields(DatabaseUserPassword, DbUserKey);
            HandlePasswordFields(MailPassword, MailPwKey);
        }

        protected void NavigationNextClick(object sender, EventArgs e) {
            if(Page.IsValid) {
                int currentStep = ViewState[StepKey] as int? ?? 1;
                currentStep++;
                SendToStep(currentStep);
            }
        }

        protected void NavigationBackClick(object sender, EventArgs e) {
            int currentStep = ViewState[StepKey] as int? ?? 1;
            if(currentStep > 1) {
                currentStep--;
            }
            SendToStep(currentStep, skipValidation: true);
        }

        private void SendToStep(int step, bool skipValidation = false) {
            ViewState[StepKey] = step;
            ShowStep(step, skipValidation);
        }

        protected void SendTestMail(object sender, EventArgs e) {
            if(Page.IsValid) {
                HandlePasswordFields(MailPassword, MailPwKey);

                var service = new EmailService {
                    TestEmailDuringSetup = true
                };

                if(!string.IsNullOrEmpty(MailServer.Text)) {
                    service.Server = MailServer.Text;
                }

                if(!string.IsNullOrEmpty(MailPort.Text)) {
                    int port = 0;
                    if(int.TryParse(MailPort.Text, out port)) {
                        service.Port = port;
                    }
                }

                if(!string.IsNullOrEmpty(MailLogin.Text)) {
                    service.Login = MailLogin.Text;
                }

                if(!string.IsNullOrEmpty(MailPassword.Text)) {
                    service.Password = MailPassword.Text;
                }

                if(service.SendEmail(MailAddress.Text,
                                     MailAddress.Text,
                                     "Test email during Great Reading Adventure setup",
                                     "This is an email sent during the setup of the Great Reading Adventure software to ensure the mail settings are correct!")) {
                    this.MailSuccessPanel.Visible = true;
                } else {
                    this.MailIssuePanel.Visible = true;
                    this.MailIssueMessage.Text = string.Format("There was a problem sending the test message: {0}", service.Error);
                }
                ShowStep((int)ViewState[StepKey], true);
            }
        }

        protected List<string> ValidateLocalDb() {
            var issues = new List<string>();

            var localdbCs = new SqlConnectionStringBuilder();
            localdbCs.DataSource = DatabaseServer.Text;

            string existsQuery = string.Format("SELECT [database_id] FROM [sys].[databases] "
                                               + "WHERE [Name] = '{0}'",
                                               DatabaseCatalog.Text);
            object result = null;
            using(var sqlConnection = new SqlConnection(localdbCs.ConnectionString)) {
                try {
                    result = SqlHelper.ExecuteScalar(sqlConnection, CommandType.Text, existsQuery);
                } catch(Exception ex) {
                    string error = string.Format("Unable to check if the database exists: {0}", ex.Message);
                    issues.Add(error);
                    this.Log().Error(error);
                    return issues;
                }

                if(result == null) {
                    string createDb = string.Format("CREATE DATABASE [{0}]", DatabaseCatalog.Text);
                    try {
                        SqlHelper.ExecuteNonQuery(sqlConnection, CommandType.Text, createDb);
                    } catch(Exception ex) {
                        string error = string.Format("Unable to create exists: {0}", ex.Message);
                        issues.Add(error);
                        this.Log().Error(error);
                        return issues;
                    }
                }
            }
            return null;
        }

        protected List<string> ValidateDatabase() {
            if(DatabaseServer.Text.Contains("(localdb)")) {
                return ValidateLocalDb();
            }

            var issues = new List<string>();

            try {
                var ownerBuilder = new SqlConnectionStringBuilder();
                ownerBuilder.DataSource = DatabaseServer.Text;
                ownerBuilder.InitialCatalog = DatabaseCatalog.Text;
                ownerBuilder.UserID = DatabaseOwnerUser.Text;
                ownerBuilder.Password = DatabaseOwnerPassword.Text;
                if(ownerBuilder.Password.Equals(MakeBlankPassword(),
                                                StringComparison.OrdinalIgnoreCase)) {
                    ownerBuilder.Password = ViewState[DbOwnerKey].ToString();
                }

                using(var sqlConnection = new SqlConnection(ownerBuilder.ConnectionString)) {
                    var isDbOwner = SqlHelper.ExecuteScalar(sqlConnection, CommandType.Text, "SELECT IS_MEMBER('db_owner')");
                    if(isDbOwner as int? == null || isDbOwner as int? != 1) {
                        issues.Add(string.Format("The '{0}' user is not in the db_owner role for {1}.",
                                                 DatabaseOwnerUser.Text,
                                                 DatabaseCatalog.Text));
                    }
                }

            } catch(Exception ex) {
                throw new Exception(string.Format("An error occurred accessing as the <strong>database owner user ({0})</strong>: {1}",
                                     DatabaseOwnerUser.Text,
                                     ex.Message), ex);
            }

            try {
                var userBuilder = new SqlConnectionStringBuilder();
                userBuilder.DataSource = DatabaseServer.Text;
                userBuilder.InitialCatalog = DatabaseCatalog.Text;
                userBuilder.UserID = DatabaseUserUser.Text;
                userBuilder.Password = DatabaseUserPassword.Text;
                if(userBuilder.Password.Equals(MakeBlankPassword(),
                                               StringComparison.OrdinalIgnoreCase)) {
                    userBuilder.Password = ViewState[DbUserKey].ToString();
                }


                using(var sqlConnection = new SqlConnection(userBuilder.ConnectionString)) {
                    var isDbRole = SqlHelper.ExecuteScalar(sqlConnection, CommandType.Text, "SELECT IS_MEMBER('db_datareader')");
                    if(isDbRole as int? == null || isDbRole as int? != 1) {
                        issues.Add(string.Format("The '{0}' user is not in the db_datareader role for {1}.",
                                                 DatabaseUserUser.Text,
                                                 DatabaseCatalog.Text));
                    }

                    isDbRole = SqlHelper.ExecuteScalar(sqlConnection, CommandType.Text, "SELECT IS_MEMBER('db_datawriter')");
                    if(isDbRole as int? == null || isDbRole as int? != 1) {
                        issues.Add(string.Format("The '{0}' user is not in the db_datawriter role for {1}.",
                                                 DatabaseUserUser.Text,
                                                 DatabaseCatalog.Text));
                    }

                }

            } catch(Exception ex) {
                throw new Exception(string.Format("An error occurred trying to connect as the <strong>regular database user ({0})</strong>: {1}",
                                    DatabaseUserUser.Text,
                                    ex.Message), ex);
            }

            if(issues.Count > 0) {
                return issues;
            } else {
                return null;
            }
        }

        protected List<string> ConfigureTheGRA() {
            var issues = new List<string>();
            Configuration webConfig = null;

            try {
                webConfig = WebConfigurationManager.OpenWebConfiguration("~");
            } catch(Exception ex) {
                issues.Add(string.Format("Could not read the Web.config file on the disk (this is probably due to permissions): {0}", ex.Message));
                return issues;
            }
            try {
                webConfig.Save(ConfigurationSaveMode.Minimal);
            } catch(Exception ex) {
                issues.Add(string.Format("Could not write to the Web.config file on the disk (this is probably due to permissions): {0}", ex.Message));
                return issues;
            }

            var ownerBuilder = new SqlConnectionStringBuilder();
            ownerBuilder.DataSource = DatabaseServer.Text;
            ownerBuilder.InitialCatalog = DatabaseCatalog.Text;
            if(!DatabaseServer.Text.Contains("(localdb)")) {
                ownerBuilder.UserID = DatabaseOwnerUser.Text;
                ownerBuilder.Password = ViewState[DbOwnerKey].ToString();
            }

            using(var sqlConnection = new SqlConnection(ownerBuilder.ConnectionString)) {
                // do database script run
                var path = CreateSchemaPath;
                this.Log().Info("Creating database schema");
                var schemaIssues = ExecuteSqlFile(path, sqlConnection);

                if(schemaIssues.Count > 0) {
                    return schemaIssues;
                }

                if(ReadingProgram.SelectedIndex == 0) {
                    // single program
                    path = Insert1ProgramPath;
                } else {
                    // multiple programs
                    path = Insert4ProgramsPath;
                }
                var insertIssues = ExecuteSqlFile(path, sqlConnection);

                if(insertIssues.Count > 0) {
                    return insertIssues;
                }

                // update email address
                sqlConnection.Open();
                try {
                    // update the sysadmin user's email
                    SqlCommand updateEmail = new SqlCommand("UPDATE [SRPUser] SET [EmailAddress] = @emailAddress WHERE [Username] = 'sysadmin';",
                                                            sqlConnection);
                    updateEmail.Parameters.AddWithValue("@emailAddress", MailAddress.Text);
                    updateEmail.ExecuteNonQuery();
                } catch(Exception ex) {
                    string error = string.Format("Unable to update administrative email address: {0}",
                                                 ex);
                    this.Log().Error(error);
                }
                try {
                    // update the sysadmin contact email and mail from address
                    // TODO email - provide better setup for email
                    SqlCommand updateEmail = new SqlCommand("UPDATE [SRPSettings] SET [Value] = @emailAddress WHERE [Name] IN ('ContactEmail', 'FromEmailAddress');",
                                                            sqlConnection);
                    updateEmail.Parameters.AddWithValue("@emailAddress", MailAddress.Text);
                    updateEmail.ExecuteNonQuery();
                } catch(Exception ex) {
                    string error = string.Format("Unable to update administrative email address: {0}",
                                                 ex);
                    this.Log().Error(error);
                }
                sqlConnection.Close();
            }

            // data inserted, update Web.config
            try {
                var userBuilder = new SqlConnectionStringBuilder();
                userBuilder.DataSource = DatabaseServer.Text;
                userBuilder.InitialCatalog = DatabaseCatalog.Text;
                if(!DatabaseServer.Text.Contains("(localdb)")) {
                    userBuilder.UserID = DatabaseUserUser.Text;
                    userBuilder.Password = ViewState[DbUserKey].ToString();
                }

                var csSection = (ConnectionStringsSection)webConfig.GetSection("connectionStrings");
                csSection.ConnectionStrings[GlobalUtilities.SRPDBConnectionStringName].ConnectionString = userBuilder.ConnectionString;

                var mailSection = (MailSettingsSectionGroup)webConfig.GetSectionGroup("system.net/mailSettings");
                var network = mailSection.Smtp.Network;

                if(!string.IsNullOrEmpty(MailServer.Text)) {
                    network.Host = MailServer.Text;
                    mailSection.Smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                }

                mailSection.Smtp.From = null;


                if(!string.IsNullOrEmpty(MailPort.Text)) {
                    int port = 0;
                    if(int.TryParse(MailPort.Text, out port)) {
                        network.Port = port;
                    }
                }

                if(!string.IsNullOrEmpty(MailLogin.Text)) {
                    network.UserName = MailLogin.Text;
                }

                if(ViewState[MailPwKey] != null && !string.IsNullOrEmpty(ViewState[MailPwKey].ToString())) {
                    network.Password = ViewState[MailPwKey].ToString();
                }

                webConfig.Save(ConfigurationSaveMode.Minimal);
            } catch(Exception ex) {
                string error = string.Format("Couldn't update Web.config with new connection string: {0}", ex.Message);
                this.Log().Error(error);
                issues.Add(error);
            }



            if(issues.Count == 0) {
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

                    new EmailService().SendEmail(MailAddress.Text,
                                                 "{SystemName} - Setup complete!"
                                                 .FormatWith(values),
                                                 body.ToString().FormatWith(values));
                    this.Log().Info(() => "Welcome email sent.");
                } catch(Exception ex) {
                    this.Log().Error(() => "Welcome email sending failure: {Message}"
                                           .FormatWith(ex));
                }

                return null;
            } else {
                return issues;
            }
        }

        // from old setup
        private List<string> ExecuteSqlFile(string path, SqlConnection sqlConnection) {
            sqlConnection.Open();
            try {
                List<string> issues = new List<string>();
                using(SqlTransaction transaction = sqlConnection.BeginTransaction()) {
                    using(SqlCommand command = sqlConnection.CreateCommand()) {
                        command.CommandTimeout = 600;
                        command.Transaction = transaction;
                        int queryCount = 0;

                        using(var sr = new StreamReader(Server.MapPath(path))) {
                            while(!sr.EndOfStream) {
                                var sb = new StringBuilder();
                                while(!sr.EndOfStream) {
                                    var s = sr.ReadLine();
                                    if(!string.IsNullOrEmpty(s)
                                       && (s.ToUpper().Trim().Equals("GO")
                                           || s.ToUpper().Trim().StartsWith("GO"))) {
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
                                    this.Log().Error(error);
                                    issues.Add(string.Format("Database error occurred, aborting: {0}",
                                                             error));
                                    try {
                                        transaction.Rollback();
                                    } catch(Exception rex) {
                                        string rollbackError = string.Format("Rollback error: {0} data: {1} on query: {2}",
                                                                             rex.Message,
                                                                             rex.Data,
                                                                             sb);
                                        this.Log().Error(error);
                                        issues.Add("Rolling back the database script failed, the database may be partially deployed. Best to clear it and start over.");
                                    }
                                    break;
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
                                this.Log().Error(error);
                                issues.Add(error);
                            }
                        }
                        return issues;
                    }
                }
            } finally {
                sqlConnection.Close();
            }
        }

    }
}