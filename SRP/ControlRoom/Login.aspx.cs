using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using GRA.SRP.Core.Utilities;
using GRA.SRP.Utilities;
using SRPApp.Classes;

namespace GRA.SRP.ControlRoom {
    public partial class Login : Page {
        protected void Page_Load(object sender, EventArgs e) {
            if(Session[CRSessionKey.TenantID] != null) {
                SRPUser currentUser = Session[SessionData.UserProfile.ToString()] as SRPUser;
                string loggedInAs = string.Empty;
                if(currentUser != null) {
                    loggedInAs = string.Format(" as {0}", currentUser.Username);
                }
                Session[CRSessionKey.CRMessage] = string.Format("You are already logged in{0}. If you wish to log in as another user, please select Logoff first.",
                                                                loggedInAs);
                Response.Redirect("~/ControlRoom/Default.aspx", true);
                return;
            }

            uxLogin.Focus();
            if(Page.IsPostBack) {

                uxLogin.PasswordRequiredErrorMessage = GRA.SRP.ControlRoom.SRPResources.PasswordRequired;
                Page.Validate("uxLogin");
                if(!Page.IsValid) {
                    uxMessageBox.Visible = true;
                }
            } else {
                if(Request.Cookies["ControlRoomUsername"] != null) {
                    var cookie = Request.Cookies["ControlRoomUsername"];
                    if(!string.IsNullOrEmpty(cookie.Value)) {
                        this.uxLogin.UserName = cookie.Value;
                        this.uxLogin.RememberMeSet = true;

                    }
                }
                this.SystemName.Text = StringResources.getString("system-name");
            }
        }

        public void OnAuthenticate(object sender, AuthenticateEventArgs e) {
            if(Page.IsValid) {
                SRPUser user = new SRPUser();

                bool auth = SRPUser.Login(uxLogin.UserName,
                                          uxLogin.Password, Session.SessionID,
                                          Request.UserHostAddress == "::1" ? "127.0.0.1" : Request.UserHostAddress,
                                          Request.UserHostName == "::1" ? "localhost" : Request.UserHostName,
                                          Request.Browser.Browser + " - v" + Request.Browser.MajorVersion + Request.Browser.MinorVersionString);
                if(!auth) {
                    uxMessageBox.Visible = true;
                    FailureText.Text = SRPResources.BadUserPass;
                    //Account Inactive
                    //
                    e.Authenticated = false;
                } else {
                    e.Authenticated = true;
                }


                if(e.Authenticated) {
                    // handle remember me
                    if(uxLogin.RememberMeSet == true) {
                        var rememberMe = new HttpCookie("ControlRoomUsername", uxLogin.UserName);
                        rememberMe.Expires = DateTime.Now.AddDays(14);
                        Response.Cookies.Set(rememberMe);
                    } else {
                        var rememberMe = new HttpCookie("ControlRoomUsername", string.Empty);
                        rememberMe.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Set(rememberMe);
                    }

                    // Put User Profile into Session.
                    // Put Security roles into session
                    // = ConfigurationManager.AppSettings["ApplicationName"];
                    user = SRPUser.FetchByUsername(uxLogin.UserName);
                    Session[SessionData.IsLoggedIn.ToString()] = true;
                    Session[SessionData.UserProfile.ToString()] = user;

                    List<SRPPermission> perms = user.EffectiveUserPermissions();
                    //Session[SessionData.PermissionList.ToString()] = perms;
                    string permList= string.Empty;
                    foreach(SRPPermission perm in perms)
                        permList += String.Format("#{0}", perm.Permission);
                    Session[SessionData.StringPermissionList.ToString()] = permList;

                    Session["TenantID"] = user.TenID;
                    Session[CRSessionKey.TenantID] = user.TenID;
                    var tenant = Tenant.FetchObject(user.TenID);
                    Session[CRSessionKey.IsMaster] = tenant.isMasterFlag;


                    if(user.MustResetPassword) {
                        this.Log().Info("Redirecting {0} to mandatory password reset.",
                                        user.Username);
                        Response.Redirect("~/ControlRoom/PasswordReset.aspx");
                    }
                    //List<CMSFolder> folders = user.EffectiveUserFolders();
                    //Session[SessionData.FoldersList.ToString()] = folders;
                    //string foldersList= string.Empty;
                    //foreach (CMSFolder folder in folders)
                    //    foldersList += string.Format("#{0}", folder.Folder);
                    //Session[SessionData.StringFoldersList.ToString()] = foldersList;


                    ////// to do - make sure these are in the settings module/ complete the settings module
                    ////string[] HideFolders =  new string[] { ".svn", "CVS", "app_data", "properties", "bin", "obj", "controls", "core", "controlroom", "app_themes" };
                    ////CMSSettings.SetSetting("HideFolders", HideFolders, ",");

                    ////string[]  HideFiles =   new string[] { ".*" };
                    ////CMSSettings.SetSetting("HideFiles", HideFiles, ",");

                    ////string[] AllowedExtensions = new string[] { };
                    ////CMSSettings.SetSetting("AllowedExtensions", AllowedExtensions, ",");

                    ////string[] DeniedExtensions = new string[] { };
                    ////CMSSettings.SetSetting("DeniedExtensions", DeniedExtensions, ",");
                    ////// end to do

                    FormsAuthentication.RedirectFromLoginPage(uxLogin.UserName, false);
                }
            } else {
                uxMessageBox.Visible = true;
            }
        }

        protected void Page_PreInit(object sender, EventArgs e) {
            ////IsSecure = false;
            //string crRestrictions = CMSSettings.GetSetting("CRRestrictions");
            //if (crRestrictions.Equals("1"))
            //{
            //    string allow = CMSSettings.GetSetting("CRAllowList");
            //    bool match = CheckIPListForMatch(allow);

            //    if (!match)
            //        Response.Redirect("~/");
            //}
            //else if (crRestrictions.Equals("2"))
            //{
            //    string deny = CMSSettings.GetSetting("CRDenyList");
            //    bool match = CheckIPListForMatch(deny);

            //    if (match)
            //        Response.Redirect("~/");
            //}
        }

        protected bool CheckIPListForMatch(string List) {
            string browserIP = Request.UserHostAddress;
            List = string.Format("|{0}|", List);
            if(List.Contains(browserIP))
                return true;
            string[] browserIPArray = browserIP.Split('.');
            string[] listIPArray = List.Split('|');
            foreach(var checkIP in listIPArray) {
                if(checkIP.Length > 0) {
                    string[] checkIPArray = checkIP.Split('.');
                    bool CompleteMatch = true;
                    try {
                        for(int i = 0; i < (browserIPArray.Length); i++) {
                            if(browserIPArray[i] != checkIPArray[i]) {
                                if(checkIPArray[i].ToLower() != "x" && checkIPArray[i] != "*") {
                                    CompleteMatch = false;
                                    break;
                                }
                            }
                        }

                    } catch(Exception) {
                        CompleteMatch = false;
                    }
                    if(CompleteMatch)
                        return true;
                }
            }
            return false;
        }

        protected void LinkButton1_Click(object sender, EventArgs e) {
            uxLogin.PasswordRequiredErrorMessage= string.Empty;
            Page.Validate("uxLogin");

            if(Page.IsValid || (uxLogin.UserName.Length > 0 && !Page.IsValid)) {
                SRPUser u = SRPUser.FetchByUsername(uxLogin.UserName);
                if(u != null) {

                    // send email

                }
                uxMessageBox.Visible = true;
                FailureText.Text = SRPResources.PasswordEmailed;
            }
        }

    }
}