using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Web;
using GRA.SRP.DAL;
using GRA.SRP.Core.Utilities;
using GRA;
using System.Net;

namespace GRA.Communications {
    public class EmailService {
        // sends email
        // Configuration:
        //      Log email to email log - Yes/No
        //      Use Template - Yes/No
        //          If Yes and no template given, use the Default Template (web.config)

        // To figure Out -- "mail merging"

        public string Server { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Error { get; set; }
        public bool TestEmailDuringSetup { get; set; }
        public EmailService() {
            // default port for SmtpClient (https://msdn.microsoft.com/en-us/library/system.net.mail.smtpclient.port%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396)
            this.Port = 25;
        }

        private static bool _logEmails = bool.Parse(ConfigurationManager.AppSettings[AppSettingKeys.LogEmails.ToString()] ?? "false");
        private static bool _useTemplates = bool.Parse(ConfigurationManager.AppSettings[AppSettingKeys.UseEmailTemplates.ToString()] ?? "false");
        public static bool UseTemplates {
            get {
                return _useTemplates;
            }
            set {
                _useTemplates = value;
            }
        }

        private static string _emailFrom
            = ConfigurationManager.AppSettings[AppSettingKeys.DefaultEmailFrom.ToString()];
        public static string EmailFrom {
            get {
                var em1 = _emailFrom;  // webconfig
                var em2 = SRPSettings.GetSettingValue("FromEmailAddress");

                if(em2.Length == 0) {
                    return _emailFrom;
                } else {
                    _emailFrom = em2;
                    return em2;
                }

            }
            set {
                _emailFrom = value;
            }
        }

        private static string _defaultTemplate
            = ConfigurationManager.AppSettings[AppSettingKeys.DefaultEmailTemplate.ToString()] ?? "";

        public static string EmailTemplate {
            get {
                try {
                    string templatePath = HttpContext.Current.Server.MapPath(_defaultTemplate);
                    string myString = null;
                    using(var myFile = new System.IO.StreamReader(templatePath)) {
                        myString = myFile.ReadToEnd();

                        myFile.Close();
                    }
                    return myString;
                } catch //(Exception ex)
                  {
                    return "{Content}";
                }
            }
        }


        public bool SendEmail(string fromAddress,
                              string toAddress,
                              string subject,
                              string body) {
            this.Error = null;
            if(this.TestEmailDuringSetup) {
                UseTemplates = false;
                _logEmails = false;
            }
            try {
                string mailBody;
                using(var mm = new MailMessage(fromAddress, toAddress)) {
                    if(UseTemplates) {
                        mailBody = EmailTemplate.FormatWith(new {
                            Subject = subject,
                            Content = body
                        });
                    } else {
                        mailBody = body;
                    }

                    mm.Subject = subject;
                    mm.Body = mailBody;
                    mm.IsBodyHtml = true;

                    NetworkCredential credentials = null;
                    if(!string.IsNullOrEmpty(this.Login)
                       && !string.IsNullOrEmpty(this.Password)) {
                        //smtp.Credentials
                        credentials = new NetworkCredential(this.Login, this.Password);
                    }


                    if(string.IsNullOrEmpty(this.Server)) {
                        using(var smtp = new SmtpClient()) {
                            if(credentials != null) {
                                smtp.Credentials = credentials;
                            }
                            smtp.Send(mm);
                        }
                    } else {
                        using(var smtp = new SmtpClient(this.Server, this.Port)) {
                            if(credentials != null) {
                                smtp.Credentials = credentials;
                            }
                            smtp.Send(mm);
                        }
                    }
                }
                if(_logEmails) {

                    var l = new EmailLog {
                        SentDateTime = DateTime.Now,
                        SentFrom = fromAddress,
                        SentTo = toAddress,
                        Subject = subject,
                        Body = mailBody
                    };
                    l.Insert();
                }
            } catch(Exception ex) {
                this.Log().Error("Unable to send email: {0}", ex.Message);
                this.Error = ex.Message;
                return false;
            }
            return true;
        }

        public bool SendEmail(string toAddress, string subject, string body) {
            return SendEmail(EmailFrom, toAddress, subject, body);
        }

        public bool SendEmail(string fromAddress,
                                      List<string> toAddress,
                                      string subject,
                                      string body) {
            foreach(string address in toAddress) {
                SendEmail(fromAddress, address, subject, body);
            }
            return true;
        }

        public bool SendEmail(List<string> toAddress,
                                      string subject,
                                      string body) {
            foreach(string address in toAddress) {
                SendEmail(EmailFrom, address, subject, body);
            }
            return true;
        }
    }
}