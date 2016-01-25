using GRA.SRP.ControlRoom;
using GRA.SRP.Controls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.Tools;
using Newtonsoft.Json.Linq;
using SRPApp.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace GRA.SRP {
    public partial class RegisterILS : BaseSRPPage {
        protected void Page_Load(object sender, EventArgs e) {
            if(ConfigurationManager.AppSettings["ILSProxyEndpoint"] == null) {
                Response.Redirect("~/Register.aspx");
            }

            if(!Page.IsPostBack) {
                var patron = Session[SessionKey.Patron] as Patron;
                if(patron != null) {
                    Response.Redirect("~");
                }
                ((BaseSRPPage)Page).TranslateStrings(this);
            }

            this.MetaDescription = string.Format("Register now to join the fun! - {0}",
                                                 GetResourceString("system-name"));
        }

        protected void RegistrationStep(int currentStep) {
            if(!string.IsNullOrEmpty(MasterPID.Text)) {
                familyMemberPanel.Visible = true;
            } else {
                familyMemberPanel.Visible = false;
            }

            switch(currentStep) {
                case 1:
                    // library card, pin
                    step1.Visible = true;
                    step2.Visible = false;
                    step3.Visible = false;
                    step4.Visible = false;
                    instructionsPanel.Visible = true;
                    NavigationBack.Enabled = false;
                    break;
                case 2:
                    // name, branch
                    step1.Visible = false;
                    step2.Visible = true;
                    step3.Visible = false;
                    step4.Visible = false;
                    instructionsPanel.Visible = true;
                    NavigationBack.Enabled = true;
                    if(string.IsNullOrEmpty(FirstName.Text)
                       && !string.IsNullOrEmpty(LibraryCard.Text)
                       && !string.IsNullOrEmpty(LibraryPin.Text)) {
                        IlsPatronLookup();
                    }
                    break;
                case 3:
                    // username, password
                    step1.Visible = false;
                    step2.Visible = false;
                    step3.Visible = true;
                    step4.Visible = false;
                    instructionsPanel.Visible = true;
                    NavigationBack.Enabled = true;
                    break;
                case 4:
                    // confirm account, ask about family members
                    bool success = RegisterPatron();
                    if(success) {
                        // account created, allow family member creation or move on
                        step3.Visible = false;
                        step4.Visible = true;
                        instructionsPanel.Visible = false;
                        NavigationBack.Enabled = false;
                    } else {
                        // couldn't create, back to step 3
                        step3.Visible = true;
                        step4.Visible = false;
                        instructionsPanel.Visible = true;
                        NavigationBack.Enabled = true;
                        ViewState["CurrentStep"] = --currentStep;
                    }
                    step1.Visible = false;
                    step2.Visible = false;
                    break;
                case 5:
                    if(!string.IsNullOrEmpty(EarnedBadges.Text)) {
                        new SessionTools(Session).EarnedBadges(EarnedBadges.Text);
                    }
                    Response.Redirect("~");
                    break;
            }
        }

        protected void NavigationNextClick(object sender, EventArgs e) {
            if(Page.IsValid) {
                int currentStep = ViewState["CurrentStep"] as int? ?? 1;
                currentStep++;
                ViewState["CurrentStep"] = currentStep;
                RegistrationStep(currentStep);
            }
        }

        protected void NavigationBackClick(object sender, EventArgs e) {
            int currentStep = ViewState["CurrentStep"] as int? ?? 1;
            if(currentStep > 1) {
                currentStep--;
            }
            ViewState["CurrentStep"] = currentStep;
            RegistrationStep(currentStep);
        }

        protected void NavigationFamilyMember(object sender, EventArgs e) {
            LibraryCard.Text = string.Empty;
            LibraryPin.Text = string.Empty;
            FirstName.Text = string.Empty;
            EmailAddress.Text = string.Empty;
            Username.Text = string.Empty;
            Password.Text = string.Empty;
            Password2.Text = string.Empty;
            ViewState["CurrentStep"] = 1;
            RegistrationStep(1);
        }

        protected bool RegisterPatron() {
            var patron = new Patron();
            patron.ProgID = Programs.GetDefaultProgramForAgeAndGrade(0, 0);
            patron.Username = Username.Text;
            patron.NewPassword = Password.Text;
            // hardcoding over 18 because there's no facility to ask here
            patron.Over18Flag = true;
            patron.FirstName = FirstName.Text.Trim();
            patron.LastName = LastName.Text.Trim();
            patron.EmailAddress = EmailAddress.Text.Trim();
            patron.LibraryCard = LibraryCard.Text.Replace(" ", "");
            int primaryLibrary = 0;
            if(int.TryParse(PrimaryLibrary.SelectedValue, out primaryLibrary)) {
                patron.PrimaryLibrary = primaryLibrary;
            }
            int avatarId;
            if(int.TryParse(AvatarID.Value, out avatarId)) {
                patron.AvatarID = avatarId;
            }
            if(!string.IsNullOrEmpty(MasterPID.Text)) {
                int masterPid = 0;
                if(int.TryParse(MasterPID.Text, out masterPid)) {
                    var masterPatron = Patron.FetchObject(masterPid);
                    if(!masterPatron.IsMasterAccount) {
                        masterPatron.IsMasterAccount = true;
                        masterPatron.Update();
                        new SessionTools(Session).EstablishPatron(masterPatron);
                    }
                    patron.MasterAcctPID = masterPid;
                    patron.ParentGuardianFirstName = masterPatron.FirstName;
                    patron.ParentGuardianLastName = masterPatron.LastName;
                }
            }
            try {
                if(patron.IsValid(BusinessRulesValidationMode.INSERT)) {
                    patron.Insert();
                    this.Log().Info("New participant: {0} {1} ({2})",
                                    patron.FirstName,
                                    patron.LastName,
                                    patron.LibraryCard);
                } else {
                    StringBuilder message = new StringBuilder("<strong>");
                    message.AppendFormat(SRPResources.ApplicationError1, "<ul>");
                    foreach(BusinessRulesValidationMessage m in patron.ErrorCodes) {
                        this.Log().Warn("Business rule error creating patron: {0}", m.ErrorMessage);
                        message.AppendFormat("<li>{0}</li>", m.ErrorMessage);
                    }
                    message.Append("</ul></strong>");
                    new SessionTools(Session).AlertPatron(
                        message.ToString(),
                        PatronMessageLevels.Warning,
                        "exclamation-sign");
                    return false;
                }

                var prog = Programs.FetchObject(patron.ProgID);
                var earnedBadgesList = new List<Badge>();
                if(prog.RegistrationBadgeID != 0) {
                    AwardPoints.AwardBadgeToPatron(prog.RegistrationBadgeID,
                                                   patron,
                                                   ref earnedBadgesList);
                }
                AwardPoints.AwardBadgeToPatronViaMatchingAwards(patron, ref earnedBadgesList);

                if(string.IsNullOrEmpty(MasterPID.Text)) {
                    EarnedBadges.Text = string.Join("|", earnedBadgesList.Select(b => b.BID).Distinct());
                    MasterPID.Text = patron.PID.ToString();
                    new SessionTools(Session).EstablishPatron(patron);
                } else {
                    new SessionTools(Session).AlertPatron(
                        string.Format("Account created for: <strong>{0}</strong>",
                            DisplayHelper.FormatName(
                                patron.FirstName,
                                patron.LastName,
                                patron.Username)),
                        glyphicon: "thumbs-up");
                }
            } catch(Exception ex) {
                this.Log().Error(string.Format("A problem occurred during registration: {0}",
                                               ex.Message));
                new SessionTools(Session).AlertPatron(
                    string.Format("<strong>{0}</strong>", ex.Message),
                    PatronMessageLevels.Warning,
                    "exclamation-sign");
                return false;
            }
            return true;
        }

        protected void IlsPatronLookup() {
            try {
                var authJson = new {
                    Id = LibraryCard.Text.Replace(" ", ""),
                    Secret = LibraryPin.Text
                };
                string ilsUri = string.Format("{0}patron/get",
                                              ConfigurationManager.AppSettings["ILSProxyEndpoint"]);
                var webReq = (HttpWebRequest)WebRequest.Create(ilsUri);
                webReq.ContentType = "application/json; charset=utf-8";
                webReq.Method = "POST";
                webReq.Accept = "application/json; charset=utf-8";

                using(var writer = new StreamWriter(webReq.GetRequestStream())) {
                    string jsonInfo = new JavaScriptSerializer().Serialize(new {
                        Id = LibraryCard.Text.Trim(),
                        Secret = LibraryPin.Text
                    });
                    writer.Write(jsonInfo);
                    writer.Flush();
                    writer.Close();

                    var resp = (HttpWebResponse)webReq.GetResponse();
                    using(var reader = new StreamReader(resp.GetResponseStream())) {
                        var patron = JObject.Parse(reader.ReadToEnd());
                        if((bool)patron["Success"]) {
                            FirstName.Text = patron["FirstName"].ToString();
                            LastName.Text = patron["LastName"].ToString();
                            EmailAddress.Text = patron["Email"].ToString();
                            try {
                                if(PrimaryLibrary.Items.Count == 1) {
                                    PrimaryLibrary.DataBind();
                                }
                                PrimaryLibrary.SelectedValue = PrimaryLibrary.Items.FindByText(patron["Library"].ToString()).Value;
                            } catch (Exception ex) {
                                this.Log().Info("Couldn't find branch {0} in drop-down: {1}",
                                                patron["Library"].ToString(),
                                                ex.Message);
                            }
                        } else {
                            this.Log().Error(string.Format("ILS lookup on {0} was not successful: {1}",
                                             this.LibraryCard.Text,
                                             patron["Response"].ToString()));
                        }
                    }
                }
            } catch(Exception ex) {
                this.Log().Error(string.Format("Couldn't perform ILS lookup on {0}: {1}",
                                 this.LibraryCard.Text.Replace(" ", ""),
                                 ex.Message));
            }
        }
    }
}