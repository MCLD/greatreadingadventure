using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using Microsoft.ApplicationBlocks.Data;
using SRP_DAL;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using GRA.Communications;

namespace GRA.SRP.ControlRoom {
    public class TenantInitialize {
        public string Version { get { return "2.0"; } }
        private static readonly string conn = GRA.SRP.Core.Utilities.GlobalUtilities.SRPDB;


        public static void InitializeSecurity(SRPUser u, int TID, string newPassword) {
            var MTID = Core.Utilities.Tenant.GetMasterID();
            u.TenID = TID;
            u.MustResetPassword = true;
            u.Insert();

            var g = new SRPGroup();
            g.GID = 0;
            g.GroupName = "Superuser group";
            g.GroupDescription = "All permissions enabled.";
            g.TenID = TID;
            g.Insert();

            var PermissionID_LIST = "1000,2000,2100,2200,3000,4000,4100,4200,4300,4400,4500,4600,4700,4800,4900,5000,5100,5200,5300,8000";
            SRPGroup.UpdatePermissions(g.GID, PermissionID_LIST, ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username);
            SRPGroup.UpdateMemberUsers(g.GID, u.Uid.ToString(), ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username);

            var Message = "Summer Reading Program - Your account has been created";

            // TODO security - this should not email the password in cleartext

            string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
            var EmailBody =
                "<h1>Dear " + u.FirstName + ",</h1><br><br>Your account has been created and has full administrative access to your organization's reading rogram. <br>This is your current account information. Please make sure you reset your password as soon as you are able to log back in.<br><br>" +
                "Username: " + u.Username + "<br>Password: " + newPassword + "<br><br>If you have any questions regarding your account please contact " + SRPSettings.GetSettingValue("ContactName") +
                " at " + SRPSettings.GetSettingValue("ContactEmail") + "." +
                "<br><br><br><a href='" + baseUrl + "'>" + baseUrl + "</a> <br> ";

            new EmailService().SendEmail(u.EmailAddress, Message, EmailBody);

        }

        public static void InitializeData(int TID) {
            var MTID = Core.Utilities.Tenant.GetMasterID();

            InitializeSettings(TID, MTID);
            InitializeCodeTypes(TID, MTID);
            //InitializeCodes(TID, MTID);   -- don;t copy codes, they are unique
            InitializeRegSettings(TID, MTID);
            InitializeCustFields(TID, MTID);

            InitializeAvatars(TID, MTID);
            InitializeBadges(TID, MTID);
            InitializeEvents(TID, MTID);
            InitializeMinigames(TID, MTID);

            InitializeBoardGame(TID, MTID);
            InitializeSurveys(TID, MTID);

            InitializePrograms(TID, MTID);

            InitializeOffers(TID, MTID);
            InitializeAwards(TID, MTID);
            InitializeBookLists(TID, MTID);
        }

        public static void ReInitializeMissingData(int TID) {
            var MTID = Core.Utilities.Tenant.GetMasterID();

            InitializeSettings(TID, MTID);
            InitializeCodeTypes(TID, MTID);
            InitializeRegSettings(TID, MTID);

            InitializeAvatars(TID, MTID);
            InitializeBadges(TID, MTID);
            InitializeEvents(TID, MTID);
            InitializeMinigames(TID, MTID);

            InitializeBoardGame(TID, MTID);
            InitializePrograms(TID, MTID);

            InitializeOffers(TID, MTID);
            InitializeAwards(TID, MTID);
            InitializeBookLists(TID, MTID);
        }

        public static void InitializeSettings(int TID, int MTID) {
            var ds = SRPSettings.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["SID"]);

                var MappedPK = GetMappedPKbyOriginalPK("setting", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = SRPSettings.FetchObject(SrcPK);
                    /* ------------------------------------*/
                    srcObj.SID = 0;
                    srcObj.TenID = TID;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    InsertInitializationTrackingRecord("setting", TID, srcObj.SID, SrcPK);
                }
            }
        }

        public static void InitializeCodeTypes(int TID, int MTID) {
            var ds = CodeType.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["CTID"]);

                var MappedPK = GetMappedPKbyOriginalPK("codetype", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = CodeType.FetchObject(SrcPK);
                    /* ------------------------------------*/
                    srcObj.CTID = 0;
                    srcObj.TenID = TID;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    InsertInitializationTrackingRecord("codetype", TID, srcObj.CTID, SrcPK);
                }
            }
        }

        public static void InitializeCodes(int TID, int MTID) {
            var ds = Codes.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["CID"]);

                var MappedPK = GetMappedPKbyOriginalPK("code", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = Codes.FetchObject(SrcPK);

                    var MappedCTID = GetMappedPKbyOriginalPK("codetype", TID, srcObj.CTID);
                    if(MappedCTID > 0 && CodeType.FetchObject(MappedCTID) != null) {
                        /* ------------------------------------*/
                        srcObj.CID = 0;
                        srcObj.TenID = TID;
                        /* ------------------------------------*/

                        /* ------------------------------------*/
                        srcObj.Insert();

                        InsertInitializationTrackingRecord("code", TID, srcObj.CID, SrcPK);
                    }
                }
            }
        }

        public static void InitializeRegSettings(int TID, int MTID) {
            var ds = RegistrationSettings.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["RID"]);

                var MappedPK = GetMappedPKbyOriginalPK("regsetting", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = RegistrationSettings.FetchObject(MTID);
                    /* ------------------------------------*/
                    srcObj.RID = 0;
                    srcObj.TenID = TID;
                    srcObj.AddedDate = srcObj.LastModDate = DateTime.Now;
                    srcObj.AddedUser = srcObj.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                    srcObj.Custom1_Edit = srcObj.Custom2_Edit = srcObj.Custom3_Edit = srcObj.Custom4_Edit =
                                                                                      srcObj.Custom1_Prompt =
                                                                                      srcObj.Custom2_Prompt =
                                                                                      srcObj.Custom3_Prompt =
                                                                                      srcObj.Custom4_Prompt =
                                                                                      srcObj.Custom1_Req =
                                                                                      srcObj.Custom2_Req =
                                                                                      srcObj.Custom3_Req =
                                                                                      srcObj.Custom4_Req =
                                                                                      srcObj.Custom1_Show =
                                                                                      srcObj.Custom2_Show =
                                                                                      srcObj.Custom3_Show =
                                                                                      srcObj.Custom4_Show = false;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    InsertInitializationTrackingRecord("regsetting", TID, srcObj.RID, SrcPK);
                }
            }
        }

        public static void InitializeCustFields(int TID, int MTID) {
            var srcObj = new CustomEventFields();
            /* ------------------------------------*/
            srcObj.TenID = TID;
            srcObj.AddedDate = srcObj.LastModDate = DateTime.Now;
            srcObj.AddedUser = srcObj.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
            /* ------------------------------------*/
            srcObj.Insert();


            var srcObj2 = new CustomRegistrationFields();
            /* ------------------------------------*/
            srcObj2.TenID = TID;
            srcObj2.AddedDate = srcObj2.LastModDate = DateTime.Now;
            srcObj2.AddedUser = srcObj2.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
            /* ------------------------------------*/
            srcObj2.Insert();

        }

        public static void InitializeAvatars(int TID, int MTID) {
            var ds = Avatar.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["AID"]);

                var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Avatars/");

                var MappedPK = GetMappedPKbyOriginalPK("avatar", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = Avatar.FetchObject(SrcPK);
                    /* ------------------------------------*/
                    srcObj.AID = 0;
                    srcObj.TenID = TID;
                    /* ------------------------------------*/
                    srcObj.AddedDate = srcObj.LastModDate = DateTime.Now;
                    srcObj.AddedUser = srcObj.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", srcObj.AID, "png"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", srcObj.AID, "png"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", srcObj.AID, "png"), true);

                    InsertInitializationTrackingRecord("avatar", TID, srcObj.AID, SrcPK);
                }
            }
        }

        public static void InitializeBadges(int TID, int MTID) {
            var ds = Badge.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["BID"]);

                var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Badges/");

                var MappedPK = GetMappedPKbyOriginalPK("badge", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = Badge.FetchObject(SrcPK);
                    /* ------------------------------------*/
                    srcObj.BID = 0;
                    srcObj.TenID = TID;
                    /* ------------------------------------*/
                    srcObj.AddedDate = srcObj.LastModDate = DateTime.Now;
                    srcObj.AddedUser = srcObj.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", srcObj.BID, "png"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", srcObj.BID, "png"), true);
                    //if (File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", SrcPK, "png")))
                    //    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", SrcPK, "png"),
                    //                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", srcObj.BID, "png"), true);

                    InsertInitializationTrackingRecord("badge", TID, srcObj.BID, SrcPK);
                }
            }
        }

        public static void InitializeEvents(int TID, int MTID) {
            var evt = Event.GetAll(MTID);
            foreach(DataRow r in evt.Tables[0].Rows) {
                var SrcEID = Convert.ToInt32(r["EID"]);

                var MappedEID = GetMappedPKbyOriginalPK("event", TID, SrcEID);
                if(MappedEID < 0) {
                    var srcObj = Event.GetEvent(SrcEID);
                    /* ------------------------------------*/
                    srcObj.EID = 0;
                    srcObj.TenID = TID;

                    var MappedBID = GetMappedPKbyOriginalPK("badge", TID, srcObj.BadgeID);
                    srcObj.BadgeID = 0;
                    if(MappedBID > 0 && Badge.GetBadge(MappedBID) != null)
                        srcObj.BadgeID = MappedBID;

                    srcObj.BranchID = 0;
                    /* ------------------------------------*/
                    srcObj.AddedDate = srcObj.LastModDate = DateTime.Now;
                    srcObj.AddedUser = srcObj.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    InsertInitializationTrackingRecord("event", TID, srcObj.EID, SrcEID);
                }

            }
        }

        public static void InitializeMinigames(int TID, int MTID) {
            var ds = Minigame.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["MGID"]);

                var MappedPK = GetMappedPKbyOriginalPK("minigame", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = Minigame.FetchObject(SrcPK);
                    /* ------------------------------------*/
                    srcObj.MGID = 0;
                    srcObj.TenID = TID;
                    /* ------------------------------------*/
                    var MappedBID = GetMappedPKbyOriginalPK("badge", TID, srcObj.AwardedBadgeID);
                    srcObj.AwardedBadgeID = 0;
                    if(MappedBID > 0 && Badge.GetBadge(MappedBID) != null)
                        srcObj.AwardedBadgeID = MappedBID;
                    /* ------------------------------------*/
                    srcObj.AddedDate = srcObj.LastModDate = DateTime.Now;
                    srcObj.AddedUser = srcObj.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                    /* ------------------------------------*/
                    srcObj.InsertBaseOnly();


                    var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Games/");
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png"),
                            string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", srcObj.MGID, "png"), true);

                    InsertInitializationTrackingRecord("minigame", TID, srcObj.MGID, SrcPK);

                    switch(srcObj.MiniGameTypeName) {
                        case "Online Book":
                            InitOB(TID, MTID, srcObj.MGID, SrcPK);
                            break;
                        case "Hidden Picture":
                            InitHP(TID, MTID, srcObj.MGID, SrcPK);
                            break;
                        case "Mix-And-Match":
                            InitMM(TID, MTID, srcObj.MGID, SrcPK);
                            break;
                        case "Word Match":
                            InitWM(TID, MTID, srcObj.MGID, SrcPK);
                            break;
                        case "Matching Game":
                            InitMG(TID, MTID, srcObj.MGID, SrcPK);
                            break;
                        case "Code Breaker":
                            InitCB(TID, MTID, srcObj.MGID, SrcPK);
                            break;
                        case "Choose Your Adventure":
                            InitCYA(TID, MTID, srcObj.MGID, SrcPK);
                            break;
                    }

                }
            }
        }

        public static void InitOB(int dTenID, int sTenID, int dMGID, int sMGID) {
            var dst = MGOnlineBook.FetchObjectByParent(sMGID);
            dst.MGID = dMGID;
            dst.Insert();

            var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Games/Books/");
            var ds = MGOnlineBookPages.GetAll(sMGID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var sOBPGID = Convert.ToInt32(r["OBPGID"]);
                var p = MGOnlineBookPages.FetchObject(sOBPGID);
                p.OBID = dst.OBID;
                p.MGID = dMGID;
                p.Insert();


                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", sOBPGID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", sOBPGID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", p.OBPGID, "png"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", sOBPGID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", sOBPGID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", p.OBPGID, "png"), true);

                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "e_", sOBPGID, "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "e_", sOBPGID, "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "e_", p.OBPGID, "mp3"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "m_", sOBPGID, "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "m_", sOBPGID, "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "m_", p.OBPGID, "mp3"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "h_", sOBPGID, "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "h_", sOBPGID, "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "h_", p.OBPGID, "mp3"), true);
            }
        }

        public static void InitHP(int dTenID, int sTenID, int dMGID, int sMGID) {
            var dst = MGHiddenPic.FetchObjectByParent(sMGID);
            dst.MGID = dMGID;
            dst.Insert();

            var ds = MGHiddenPicBk.GetAll(sMGID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var sHPBID = Convert.ToInt32(r["HPBID"]);
                var p = MGHiddenPicBk.FetchObject(sHPBID);
                p.HPID = dst.HPID;
                p.MGID = dMGID;
                p.Insert();

                var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Games/HiddenPic/");
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", sHPBID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", sHPBID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", p.HPBID, "png"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", sHPBID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", sHPBID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", p.HPBID, "png"), true);
                //if (File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", sHPBID, "png")))
                //    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", sHPBID, "png"),
                //                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", p.HPBID, "png"), true);
            }
        }

        public static void InitMM(int dTenID, int sTenID, int dMGID, int sMGID) {
            var dst = MGMixAndMatch.FetchObjectByParent(sMGID);
            dst.MGID = dMGID;
            dst.Insert();

            var ds = MGMixAndMatchItems.GetAll(sMGID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var sMMIID = Convert.ToInt32(r["MMIID"]);
                var p = MGMixAndMatchItems.FetchObject(sMMIID);
                p.MMID = dst.MMID;
                p.MGID = dMGID;
                p.Insert();

                var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Games/MixMatch/");
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", sMMIID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", sMMIID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", p.MMIID, "png"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", sMMIID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", sMMIID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", p.MMIID, "png"), true);
                //if (File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", sMMIID, "png")))
                //    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", sMMIID, "png"),
                //                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", p.MMIID, "png"), true);


                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "e_", sMMIID, "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "e_", sMMIID, "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "e_", p.MMIID, "mp3"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "m_", sMMIID, "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "m_", sMMIID, "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "m_", p.MMIID, "mp3"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "h_", sMMIID, "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "h_", sMMIID, "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "h_", p.MMIID, "mp3"), true);
            }
        }

        public static void InitWM(int dTenID, int sTenID, int dMGID, int sMGID) {
            var dst = MGWordMatch.FetchObjectByParent(sMGID);
            dst.MGID = dMGID;
            dst.Insert();

            var ds = MGWordMatchItems.GetAll(sMGID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var sWMIID = Convert.ToInt32(r["WMIID"]);
                var p = MGWordMatchItems.FetchObject(sWMIID);
                p.WMID = dst.WMID;
                p.MGID = dMGID;
                p.Insert();

                var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Games/WordMatch/");
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", sWMIID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", sWMIID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", p.WMIID, "png"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", sWMIID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", sWMIID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", p.WMIID, "png"), true);
                //if (File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", sWMIID, "png")))
                //    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", sWMIID, "png"),
                //                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", p.WMIID, "png"), true);


                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "e_", sWMIID, "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "e_", sWMIID, "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "e_", p.WMIID, "mp3"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "m_", sWMIID, "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "m_", sWMIID, "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "m_", p.WMIID, "mp3"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "h_", sWMIID, "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "h_", sWMIID, "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "h_", p.WMIID, "mp3"), true);
            }
        }

        public static void InitMG(int dTenID, int sTenID, int dMGID, int sMGID) {
            var dst = MGMatchingGame.FetchObjectByParent(sMGID);
            dst.MGID = dMGID;
            dst.Insert();

            var ds = MGMatchingGameTiles.GetAll(sMGID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var sMAGTID = Convert.ToInt32(r["MAGTID"]);
                var p = MGMatchingGameTiles.FetchObject(sMAGTID);
                p.MAGID = dst.MAGID;
                p.MGID = dMGID;
                p.Insert();

                var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Games/MatchingGame/");
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "t1_", sMAGTID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "t1_", sMAGTID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "t1_", p.MAGID, "png"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_t1_", sMAGTID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_t1_", sMAGTID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_t1_", p.MAGID, "png"), true);
                //if (File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_t1_", sMAGTID, "png")))
                //    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_t1_", sMAGTID, "png"),
                //                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_t1_", p.MAGID, "png"), true);

                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "t2_", sMAGTID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "t2_", sMAGTID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "t2_", p.MAGID, "png"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_t2_", sMAGTID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_t2_", sMAGTID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_t2_", p.MAGID, "png"), true);
                //if (File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_t2_", sMAGTID, "png")))
                //    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_t2_", sMAGTID, "png"),
                //                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_t2_", p.MAGID, "png"), true);


                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "t3_", sMAGTID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "t3_", sMAGTID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "t3_", p.MAGID, "png"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_t3_", sMAGTID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_t3_", sMAGTID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_t3_", p.MAGID, "png"), true);
                //if (File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_t3_", sMAGTID, "png")))
                //    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_t3_", sMAGTID, "png"),
                //                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_t3_", p.MAGID, "png"), true);
            }
        }

        public static void InitCB(int dTenID, int sTenID, int dMGID, int sMGID) {
            var dst = MGCodeBreaker.FetchObjectByParent(sMGID);
            var sCBID = dst.CBID;
            dst.MGID = dMGID;
            dst.Insert();

            var chars = MGCodeBreaker.GetKeyCharacters(dst.CBID);
            //var chars = MGCodeBreakerKeySetup.GetKeyCharacters(dst.CBID);
            var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Games/CodeBreaker/");
            foreach(var keyItem in chars) {
                if(File.Exists(string.Format("{0}{1}{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png"),
                                    string.Format("{0}{1}{2}_{3}.{4}", MappedFolder, "\\", dst.CBID, keyItem.Character_Num, "png"), true);

                //if (File.Exists(string.Format("{0}{1}sm_{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png")))
                //    System.IO.File.Copy(string.Format("{0}{1}sm_{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png"),
                //                    string.Format("{0}{1}sm_{2}_{3}.{4}", MappedFolder, "\\", dst.CBID, keyItem.Character_Num, "png"), true);

                if(File.Exists(string.Format("{0}{1}{5}{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png", "m_")))
                    System.IO.File.Copy(string.Format("{0}{1}{5}{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png", "m_"),
                                    string.Format("{0}{1}{5}{2}_{3}.{4}", MappedFolder, "\\", dst.CBID, keyItem.Character_Num, "png", "m_"), true);

                //if (File.Exists(string.Format("{0}{1}sm_{5}{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png", "m_")))
                //    System.IO.File.Copy(string.Format("{0}{1}sm_{5}{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png", "m_"),
                //                    string.Format("{0}{1}sm_{5}{2}_{3}.{4}", MappedFolder, "\\", dst.CBID, keyItem.Character_Num, "png", "m_"), true);

                if(File.Exists(string.Format("{0}{1}{5}{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png", "h_")))
                    System.IO.File.Copy(string.Format("{0}{1}{5}{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png", "h_"),
                                    string.Format("{0}{1}{5}{2}_{3}.{4}", MappedFolder, "\\", dst.CBID, keyItem.Character_Num, "png", "h_"), true);

                //if (File.Exists(string.Format("{0}{1}sm_{5}{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png", "m_")))
                //    System.IO.File.Copy(string.Format("{0}{1}sm_{5}{2}_{3}.{4}", MappedFolder, "\\", sCBID, keyItem.Character_Num, "png", "h_"),
                //                    string.Format("{0}{1}sm_{5}{2}_{3}.{4}", MappedFolder, "\\", dst.CBID, keyItem.Character_Num, "png", "h_"), true);
            }
        }

        public static void InitCYA(int dTenID, int sTenID, int dMGID, int sMGID) {
            var dst = MGChooseAdv.FetchObjectByParent(sMGID);
            var sCAID = dst.CAID;
            dst.MGID = dMGID;
            dst.Insert();

            var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Games/ChooseAdv/");
            var ds = MGChooseAdvSlides.GetAll(sMGID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var sCASID = Convert.ToInt32(r["CASID"]);
                var p = MGChooseAdvSlides.FetchObject(sCASID);
                p.CAID = dst.CAID;
                p.MGID = dMGID;
                p.Insert();

                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "i1_", sCASID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "i1_", sCASID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "i1_", p.CASID, "png"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_i1_", sCASID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_i1_", sCASID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_i1_", p.CASID, "png"), true);
                //if (File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_i1_", sCASID, "png")))
                //    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_i1_", sCASID, "png"),
                //                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_i1_", p.CASID, "png"), true);

                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "i2_", sCASID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "i2_", sCASID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "i2_", p.CASID, "png"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_i2_", sCASID, "png")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_i2_", sCASID, "png"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_i2_", p.CASID, "png"), true);
                //if (File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_i2_", sCASID, "png")))
                //    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_i2_", sCASID, "png"),
                //                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_i2_", p.CASID, "png"), true);


                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", sCASID, "_1", "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", sCASID, "_1", "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", p.CASID, "_1", "mp3"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", sCASID, "_2", "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", sCASID, "_2", "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", p.CASID, "_2", "mp3"), true);
                if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", sCASID, "_3", "mp3")))
                    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", sCASID, "_3", "mp3"),
                                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", p.CASID, "_3", "mp3"), true);
            }

        }

        public static void InitializeBoardGame(int TID, int MTID) {
            var ds = ProgramGame.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["PGID"]);

                var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Games/Board/");

                var MappedPK = GetMappedPKbyOriginalPK("boardgame", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = ProgramGame.FetchObject(SrcPK);
                    /* ------------------------------------*/
                    srcObj.PGID = 0;
                    srcObj.TenID = TID;
                    /* ------------------------------------*/
                    srcObj.AddedDate = srcObj.LastModDate = DateTime.Now;
                    srcObj.AddedUser = srcObj.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", srcObj.PGID, "png"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "bonus_", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "bonus_", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "bonus_", srcObj.PGID, "png"), true);

                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "stamp_", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "stamp_", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "stamp_", srcObj.PGID, "png"), true);
                    //if (File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_stamp_", SrcPK, "png")))
                    //    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_stamp_", SrcPK, "png"),
                    //                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_stamp_", srcObj.PGID, "png"), true);
                    //if (File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_stamp_", SrcPK, "png")))
                    //    System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_stamp_", SrcPK, "png"),
                    //                    string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_stamp_", srcObj.PGID, "png"), true);

                    InsertInitializationTrackingRecord("boardgame", TID, srcObj.PGID, SrcPK);

                    // levels
                    var ds2 = ProgramGameLevel.GetAll(SrcPK);
                    foreach(DataRow r2 in ds2.Tables[0].Rows) {
                        var SrcPK2 = Convert.ToInt32(r2["PGLID"]);

                        var srcObj2 = ProgramGameLevel.FetchObject(SrcPK2);
                        /* ------------------------------------*/
                        srcObj2.PGLID = 0;
                        srcObj2.PGID = srcObj.PGID;
                        /* ------------------------------------*/
                        var MappedBID1 = GetMappedPKbyOriginalPK("badge", TID, srcObj2.AwardBadgeID);
                        if(MappedBID1 > 0 && Badge.FetchObject(MappedBID1) != null) {
                            srcObj2.AwardBadgeID = MappedBID1;
                        } else {
                            srcObj2.AwardBadgeID = 0;
                        }

                        var MappedBID2 = GetMappedPKbyOriginalPK("badge", TID, srcObj2.AwardBadgeIDBonus);
                        if(MappedBID2 > 0 && Badge.FetchObject(MappedBID2) != null) {
                            srcObj2.AwardBadgeIDBonus = MappedBID2;
                        } else {
                            srcObj2.AwardBadgeIDBonus = 0;
                        }

                        /* ------------------------------------*/

                        /* ------------------------------------*/
                        var MappedMGID1 = GetMappedPKbyOriginalPK("minigame", TID, srcObj2.Minigame1ID);
                        if(MappedMGID1 > 0 && Minigame.FetchObject(MappedMGID1) != null) {
                            srcObj2.Minigame1ID = MappedMGID1;
                        } else {
                            srcObj2.Minigame1ID = 0;
                        }
                        var MappedMGID2 = GetMappedPKbyOriginalPK("minigame", TID, srcObj2.Minigame2ID);
                        if(MappedMGID2 > 0 && Minigame.FetchObject(MappedMGID2) != null) {
                            srcObj2.Minigame2ID = MappedMGID2;
                        } else {
                            srcObj2.Minigame2ID = 0;
                        }

                        var MappedBMGID1 = GetMappedPKbyOriginalPK("minigame", TID, srcObj2.Minigame1IDBonus);
                        if(MappedBMGID1 > 0 && Minigame.FetchObject(MappedBMGID1) != null) {
                            srcObj2.Minigame1IDBonus = MappedBMGID1;
                        } else {
                            srcObj2.Minigame1IDBonus = 0;
                        }
                        var MappedBMGID2 = GetMappedPKbyOriginalPK("minigame", TID, srcObj2.Minigame2IDBonus);
                        if(MappedMGID2 > 0 && Minigame.FetchObject(MappedBMGID2) != null) {
                            srcObj2.Minigame2IDBonus = MappedBMGID2;
                        } else {
                            srcObj2.Minigame2IDBonus = 0;
                        }

                        /* ------------------------------------*/
                        srcObj2.AddedDate = srcObj2.LastModDate = DateTime.Now;
                        srcObj2.AddedUser = srcObj2.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                        /* ------------------------------------*/
                        srcObj2.Insert();

                    }

                }
            }
        }

        public static void InitializePrograms(int TID, int MTID) {
            var ds = Programs.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["PID"]);

                var MappedPK = GetMappedPKbyOriginalPK("program", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = Programs.FetchObject(SrcPK);
                    /* ------------------------------------*/
                    srcObj.PID = 0;
                    srcObj.TenID = TID;
                    /* ------------------------------------*/
                    var MappedPID = GetMappedPKbyOriginalPK("badge", TID, srcObj.RegistrationBadgeID);
                    if(MappedPID > 0 && Badge.FetchObject(MappedPID) != null) {
                        srcObj.RegistrationBadgeID = MappedPID;
                    } else {
                        srcObj.RegistrationBadgeID = 0;
                    }
                    /* ------------------------------------*/
                    var MappedPID2 = GetMappedPKbyOriginalPK("boardgame", TID, srcObj.ProgramGameID);
                    if(MappedPID2 > 0 && ProgramGame.FetchObject(MappedPID2) != null) {
                        srcObj.ProgramGameID = MappedPID2;
                    } else {
                        srcObj.ProgramGameID = 0;
                    }
                    /* ------------------------------------*/
                    var MappedT1 = GetMappedPKbyOriginalPK("survey", TID, srcObj.PreTestID);
                    if(MappedT1 > 0 && Survey.FetchObject(MappedT1) != null) {
                        srcObj.PreTestID = MappedT1;
                    } else {
                        srcObj.PreTestID = 0;
                    }
                    /* ------------------------------------*/
                    var MappedT2 = GetMappedPKbyOriginalPK("survey", TID, srcObj.PostTestID);
                    if(MappedT2 > 0 && Survey.FetchObject(MappedT2) != null) {
                        srcObj.PostTestID = MappedT2;
                    } else {
                        srcObj.PostTestID = 0;
                    }
                    /* ------------------------------------*/
                    srcObj.AddedDate = srcObj.LastModDate = DateTime.Now;
                    srcObj.AddedUser = srcObj.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Banners/");
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", srcObj.PID, "png"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}@2x.{4}", MappedFolder, "\\", "", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}@2x.{4}", MappedFolder, "\\", "", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}@2x.{4}", MappedFolder, "\\", "", srcObj.PID, "png"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", srcObj.PID, "png"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", srcObj.PID, "png"), true);

                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "jpg")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "jpg"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", srcObj.PID, "jpg"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}@2x.{4}", MappedFolder, "\\", "", SrcPK, "jpg")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}@2x.{4}", MappedFolder, "\\", "", SrcPK, "jpg"),
                                        string.Format("{0}{1}{2}{3}@2x.{4}", MappedFolder, "\\", "", srcObj.PID, "jpg"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", SrcPK, "jpg")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", SrcPK, "jpg"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", srcObj.PID, "jpg"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", SrcPK, "jpg")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", SrcPK, "jpg"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", srcObj.PID, "jpg"), true);

                    MappedFolder = HttpContext.Current.Server.MapPath("~/CSS/Program/");
                    if(File.Exists(string.Format("{0}{1}{2}.css", MappedFolder, "\\", SrcPK)))
                        System.IO.File.Copy(string.Format("{0}{1}{2}.css", MappedFolder, "\\", SrcPK),
                                        string.Format("{0}{1}{2}.css", MappedFolder, "\\", srcObj.PID), true);

                    MappedFolder = HttpContext.Current.Server.MapPath("~/Resources/");
                    if(File.Exists(string.Format("{0}{1}program.{2}.en-US.txt", MappedFolder, "\\", SrcPK)))
                        System.IO.File.Copy(string.Format("{0}{1}program.{2}.en-US.txt", MappedFolder, "\\", SrcPK),
                                        string.Format("{0}{1}program.{2}.en-US.txt", MappedFolder, "\\", srcObj.PID), true);

                    InsertInitializationTrackingRecord("program", TID, srcObj.PID, SrcPK);



                    var dsExists = ProgramGamePointConversion.GetAll(srcObj.PID);
                    if(dsExists.Tables[0].Rows.Count == 0) {
                        // no point conversions at all , copy them all ...
                        var ds2 = ProgramGamePointConversion.GetAll(SrcPK);
                        foreach(DataRow r2 in ds2.Tables[0].Rows) {
                            var SrcPK2 = Convert.ToInt32(r2["PGCID"]);
                            var obj2 = ProgramGamePointConversion.FetchObject(SrcPK2);
                            obj2.PGID = srcObj.PID;
                            obj2.AddedDate = obj2.LastModDate = DateTime.Now;
                            obj2.AddedUser = obj2.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                            obj2.Insert();
                        }
                    } else {
                        // leave them alone ...
                    }

                }
            }
        }

        public static void InitializeOffers(int TID, int MTID) {
            var ds = Offer.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["OID"]);

                var MappedFolder = HttpContext.Current.Server.MapPath("~/Images/Offers/");

                var MappedPK = GetMappedPKbyOriginalPK("offer", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = Offer.FetchObject(SrcPK);
                    /* ------------------------------------*/
                    srcObj.OID = 0;
                    srcObj.TenID = TID;
                    srcObj.BranchId = 0;
                    /* ------------------------------------*/
                    var MappedPID = GetMappedPKbyOriginalPK("program", TID, srcObj.ProgramId);
                    if(MappedPID > 0 && Programs.FetchObject(MappedPID) != null) {
                        srcObj.ProgramId = MappedPID;
                    } else {
                        srcObj.ProgramId = 0;
                    }
                    /* ------------------------------------*/
                    srcObj.AddedDate = srcObj.LastModDate = DateTime.Now;
                    srcObj.AddedUser = srcObj.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "", srcObj.OID, "png"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "sm_", srcObj.OID, "png"), true);
                    if(File.Exists(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", SrcPK, "png")))
                        System.IO.File.Copy(string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", SrcPK, "png"),
                                        string.Format("{0}{1}{2}{3}.{4}", MappedFolder, "\\", "md_", srcObj.OID, "png"), true);

                    InsertInitializationTrackingRecord("offer", TID, srcObj.OID, SrcPK);
                }
            }
        }

        public static void InitializeAwards(int TID, int MTID) {
            var ds = Award.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["AID"]);

                var MappedPK = GetMappedPKbyOriginalPK("award", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = Award.FetchObject(SrcPK);
                    /* ------------------------------------*/
                    srcObj.AID = 0;
                    srcObj.TenID = TID;
                    srcObj.BranchID = 0;
                    srcObj.SchoolName = "0";
                    srcObj.District = "0";
                    /* ------------------------------------*/
                    var MappedBID = GetMappedPKbyOriginalPK("badge", TID, srcObj.BadgeID);
                    if(MappedBID > 0 && Badge.FetchObject(MappedBID) != null) {
                        srcObj.BadgeID = MappedBID;
                    } else {
                        srcObj.BadgeID = 0;
                    }
                    /* ------------------------------------*/
                    var MappedPID = GetMappedPKbyOriginalPK("program", TID, srcObj.ProgramID);
                    if(MappedPID > 0 && Programs.FetchObject(MappedPID) != null) {
                        srcObj.ProgramID = MappedPID;
                    } else {
                        srcObj.ProgramID = 0;
                    }
                    /* ------------------------------------*/

                    if(srcObj.BadgeList.Length > 0) {
                        var b = srcObj.BadgeList.Split(',');
                        srcObj.BadgeList= string.Empty;
                        foreach(var i in b) {
                            var lBid = 0;
                            int.TryParse(i, out lBid);
                            var MappedBID2 = GetMappedPKbyOriginalPK("badge", TID, lBid);
                            if(MappedBID2 > 0 && Badge.FetchObject(MappedBID2) != null) {
                                srcObj.BadgeList = string.Format("{0}{1}{2}", srcObj.BadgeList, srcObj.BadgeList.Length > 0 ? "," : "", MappedBID2);
                            }
                        }
                    }
                    /* ------------------------------------*/
                    srcObj.AddedDate = srcObj.LastModDate = DateTime.Now;
                    srcObj.AddedUser = srcObj.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    InsertInitializationTrackingRecord("award", TID, srcObj.AID, SrcPK);
                }
            }
        }

        public static void InitializeBookLists(int TID, int MTID) {
            var ds = BookList.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["BLID"]);

                var MappedPK = GetMappedPKbyOriginalPK("booklist", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = BookList.FetchObject(SrcPK);
                    /* ------------------------------------*/
                    srcObj.BLID = 0;
                    srcObj.TenID = TID;
                    srcObj.LibraryID = 0;
                    /* ------------------------------------*/
                    var MappedPID = GetMappedPKbyOriginalPK("program", TID, srcObj.ProgID);
                    if(MappedPID > 0 && Programs.FetchObject(MappedPID) != null) {
                        srcObj.ProgID = MappedPID;
                    } else {
                        srcObj.ProgID = 0;
                    }
                    /* ------------------------------------*/
                    var MappedBID = GetMappedPKbyOriginalPK("badge", TID, srcObj.AwardBadgeID);
                    if(MappedBID > 0 && Badge.FetchObject(MappedBID) != null) {
                        srcObj.AwardBadgeID = MappedBID;
                    } else {
                        srcObj.AwardBadgeID = 0;
                    }
                    /* ------------------------------------*/
                    srcObj.AddedDate = srcObj.LastModDate = DateTime.Now;
                    srcObj.AddedUser = srcObj.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    InsertInitializationTrackingRecord("booklist", TID, srcObj.BLID, SrcPK);

                    var ds2 = BookListBooks.GetAll(SrcPK);
                    foreach(DataRow r2 in ds2.Tables[0].Rows) {
                        var SrcPK2 = Convert.ToInt32(r2["BLBID"]);

                        var srcObj2 = BookListBooks.FetchObject(SrcPK2);
                        /* ------------------------------------*/
                        srcObj2.BLBID = 0;
                        srcObj2.BLID = srcObj.BLID;
                        srcObj2.TenID = TID;

                        /* ------------------------------------*/
                        srcObj2.AddedDate = srcObj2.LastModDate = DateTime.Now;
                        srcObj2.AddedUser = srcObj2.LastModUser = ((SRPUser)HttpContext.Current.Session[SessionData.UserProfile.ToString()]).Username;
                        /* ------------------------------------*/
                        srcObj2.Insert();
                    }
                }
            }
        }

        public static void InitializeSurveys(int TID, int MTID) {
            var ds = Survey.GetAll(MTID);
            foreach(DataRow r in ds.Tables[0].Rows) {
                var SrcPK = Convert.ToInt32(r["SID"]);

                var MappedPK = GetMappedPKbyOriginalPK("survey", TID, SrcPK);
                if(MappedPK < 0) {
                    var srcObj = Survey.FetchObject(SrcPK);
                    /* ------------------------------------*/
                    srcObj.SID = 0;
                    srcObj.TenID = TID;
                    srcObj.TakenCount = srcObj.PatronCount = 0;
                    /* ------------------------------------*/
                    srcObj.Insert();

                    InsertInitializationTrackingRecord("survey", TID, srcObj.SID, SrcPK);

                    var ds2 = SurveyQuestion.GetAll(SrcPK);
                    foreach(DataRow r2 in ds2.Tables[0].Rows) {
                        var SrcPK2 = Convert.ToInt32(r2["QID"]);

                        var srcObj2 = SurveyQuestion.FetchObject(SrcPK2);
                        /* ------------------------------------*/
                        srcObj2.QID = 0;
                        srcObj2.SID = srcObj.SID;
                        srcObj2.Insert();
                        /* ------------------------------------*/

                        var ds3 = SQMatrixLines.GetAll(srcObj2.QID);
                        foreach(DataRow r3 in ds3.Tables[0].Rows) {
                            var SrcPK3 = Convert.ToInt32(r2["SQMLID"]);

                            var srcObj3 = SQMatrixLines.FetchObject(SrcPK3);
                            /* ------------------------------------*/
                            srcObj3.SQMLID = 0;
                            srcObj3.QID = srcObj2.QID;
                            srcObj3.Insert();
                        }
                        /* ------------------------------------*/

                        var ds4 = SQChoices.GetAll(srcObj2.QID);
                        foreach(DataRow r4 in ds4.Tables[0].Rows) {
                            var SrcPK4 = Convert.ToInt32(r2["SQCID"]);

                            var srcObj4 = SQChoices.FetchObject(SrcPK4);
                            /* ------------------------------------*/
                            srcObj4.SQCID = 0;
                            srcObj4.QID = srcObj2.QID;
                            srcObj4.Insert();
                        }
                        /* ------------------------------------*/
                    }
                }
            }
        }


        public static void CleanOrphanedAssets() {
            var Response = HttpContext.Current.Response;

            CleanOrphanedMinigameImages();

            if(Response != null) {
                Response.Buffer = false;
                Response.Write("<!--");
            }
            var WorkFolder = HttpContext.Current.Server.MapPath("~/Images/Avatars/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "sm_*.png")) {
                var sID = f.Replace(WorkFolder, "").Replace(".png", "").Replace("sm_", "");
                var ID = -1;
                int.TryParse(sID, out ID);
                if(ID > 0) {
                    var mg = Avatar.FetchObject(ID);
                    if(mg == null) {
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "", ID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "sm_", ID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_", ID, "png"));
                    }
                }
                Response.Write(".");
            }

            WorkFolder = HttpContext.Current.Server.MapPath("~/Images/Badges/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "sm_*.png")) {
                var sID = f.Replace(WorkFolder, "").Replace(".png", "").Replace("sm_", "");
                var ID = -1;
                int.TryParse(sID, out ID);
                if(ID > 0) {
                    var mg = Badge.FetchObject(ID);
                    if(mg == null) {
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "", ID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "sm_", ID, "png"));
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_", ID, "png"));
                    }
                }
                Response.Write(".");
            }

            WorkFolder = HttpContext.Current.Server.MapPath("~/Images/Banners/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "*.png")) {
                var sID = f.Replace(WorkFolder, "").Replace(".png", "");
                var ID = -1;
                int.TryParse(sID, out ID);
                if(ID > 0) {
                    var mg = Programs.FetchObject(ID);
                    if(mg == null)
                        File.Delete(f);
                }
                Response.Write(".");
            }
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "*.jpg")) {
                var sID = f.Replace(WorkFolder, "").Replace(".jpg", "");
                var ID = -1;
                int.TryParse(sID, out ID);
                if(ID > 0) {
                    var mg = Programs.FetchObject(ID);
                    if(mg == null)
                        File.Delete(f);
                }
                Response.Write(".");
            }


            WorkFolder = HttpContext.Current.Server.MapPath("~/CSS/Programs/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "*.css")) {
                var sID = f.Replace(WorkFolder, "").Replace(".css", "");
                var ID = -1;
                int.TryParse(sID, out ID);
                if(ID > 0) {
                    var mg = Programs.FetchObject(ID);
                    if(mg == null)
                        File.Delete(f);
                }
                Response.Write(".");
            }

            WorkFolder = HttpContext.Current.Server.MapPath("~/Resources/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "program.*.en-US.txt")) {
                var sID = f.Replace(WorkFolder, "").Replace(".en-US.txt", "").Replace("program.", "");
                var ID = -1;
                int.TryParse(sID, out ID);
                if(ID > 0) {
                    var mg = Programs.FetchObject(ID);
                    if(mg == null)
                        File.Delete(f);
                }
                Response.Write(".");
            }

            if(Response != null) {
                Response.Buffer = false;
                Response.Write("-->");
            }
        }
        public static void CleanOrphanedMinigameImages() {

            var Response = HttpContext.Current.Response;
            if(Response != null) {
                Response.Buffer = false;
                Response.Write("<!--");
            }



            var WorkFolder = HttpContext.Current.Server.MapPath("~/Images/Games/");

            // Minigame - mini image
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder)) {
                var sMGID = f.Replace(WorkFolder, "").Replace(".png", "");
                var MGID = -1;
                int.TryParse(sMGID, out MGID);
                if(MGID > 0) {
                    var mg = Minigame.FetchObject(MGID);
                    if(mg == null)
                        File.Delete(f);
                }
                Response.Write(".");
            }

            //  Board Games

            // Books
            WorkFolder = HttpContext.Current.Server.MapPath("~/Images/Games/Books/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "sm_*.png")) {
                var sOPBGID = f.Replace(WorkFolder, "").Replace(".png", "").Replace("sm_", "");
                var OPBGID = -1;
                int.TryParse(sOPBGID, out OPBGID);
                if(OPBGID > 0) {
                    var mg = MGOnlineBookPages.FetchObject(OPBGID);
                    if(mg == null) {
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "", OPBGID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "sm_", OPBGID, "png"));
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_", OPBGID, "png"));

                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "e_", OPBGID, "mp3"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "m_", OPBGID, "mp3"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "h_", OPBGID, "mp3"));
                    }
                }
                Response.Write(".");
            }

            //Choose Adv
            WorkFolder = HttpContext.Current.Server.MapPath("~/Images/Games/ChooseAdv/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "sm_i1_*.png")) {
                var sCASID = f.Replace(WorkFolder, "").Replace(".png", "").Replace("sm_i1", "");
                var CASID = -1;
                int.TryParse(sCASID, out CASID);
                if(CASID > 0) {
                    var mg = MGChooseAdvSlides.FetchObject(CASID);
                    if(mg == null) {
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "i1_", CASID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "i2_", CASID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "sm_i1_", CASID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "sm_i2_", CASID, "png"));
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_i1_", CASID, "png"));
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_i2_", CASID, "png")); 
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_t1_", CASID, "png"));

                        File.Delete(string.Format(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", CASID, "_1", "mp3")));
                        File.Delete(string.Format(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", CASID, "_2", "mp3")));
                        File.Delete(string.Format(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", CASID, "_3", "mp3")));
                    }
                }
                Response.Write(".");
            }

            //MGCodeBreaker
            WorkFolder = HttpContext.Current.Server.MapPath("~/Images/Games/CodeBreaker/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "sm_*_*.png")) {
                var sCBID = f.Replace(WorkFolder, "").Replace(".png", "").Replace("sm_", "").Split('_')[0];
                var CBID = -1;
                int.TryParse(sCBID, out CBID);
                if(CBID > 0) {
                    var mg = MGCodeBreaker.FetchObject(CBID);
                    if(mg == null) {
                        foreach(FileInfo fl in new DirectoryInfo(WorkFolder).GetFiles(CBID.ToString() + "_*.png")) {
                            fl.Delete();
                        }

                        //foreach (FileInfo fl in new DirectoryInfo(WorkFolder).GetFiles("sm_" + CBID.ToString() + "_*.png"))
                        //{
                        //    fl.Delete();
                        //}

                        foreach(FileInfo fl in new DirectoryInfo(WorkFolder).GetFiles("m_" + CBID.ToString() + "_*.png")) {
                            fl.Delete();
                        }

                        //foreach (FileInfo fl in new DirectoryInfo(WorkFolder).GetFiles("sm_m_" + CBID.ToString() + "_*.png"))
                        //{
                        //    fl.Delete();
                        //}

                        foreach(FileInfo fl in new DirectoryInfo(WorkFolder).GetFiles("h_" + CBID.ToString() + "_*.png")) {
                            fl.Delete();
                        }

                        //foreach (FileInfo fl in new DirectoryInfo(WorkFolder).GetFiles("sm_h_" + CBID.ToString() + "_*.png"))
                        //{
                        //    fl.Delete();
                        //}

                    }
                }
                Response.Write(".");
            }


            // HiddenPic
            WorkFolder = HttpContext.Current.Server.MapPath("~/Images/Games/HiddenPic/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "sm_*.png")) {
                var sHPBID = f.Replace(WorkFolder, "").Replace(".png", "").Replace("sm_", "");
                var HPBID = -1;
                int.TryParse(sHPBID, out HPBID);
                if(HPBID > 0) {
                    var mg = MGHiddenPicBk.FetchObject(HPBID);
                    if(mg == null) {
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "", HPBID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "sm_", HPBID, "png"));
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_", HPBID, "png"));
                    }
                }
                Response.Write(".");
            }


            // MatchingGame
            WorkFolder = HttpContext.Current.Server.MapPath("~/Images/Games/MatchingGame/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "sm_t1_*.png")) {
                var sMAGTID = f.Replace(WorkFolder, "").Replace(".png", "").Replace("sm_t1", "");
                var MAGTID = -1;
                int.TryParse(sMAGTID, out MAGTID);
                if(MAGTID > 0) {
                    var mg = MGMatchingGameTiles.FetchObject(MAGTID);
                    if(mg == null) {
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "t1_", MAGTID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "t2_", MAGTID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "t3_", MAGTID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "sm_t1_", MAGTID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "sm_t2_", MAGTID, "png"));
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "sm_t3_", MAGTID, "png"));
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_t1_", MAGTID, "png")); 
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_t2_", MAGTID, "png"));
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_t3_", MAGTID, "png"));
                    }
                }
                Response.Write(".");
            }

            // MixMatch
            WorkFolder = HttpContext.Current.Server.MapPath("~/Images/Games/MixMatch/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "sm_*.png")) {
                var sMMIID = f.Replace(WorkFolder, "").Replace(".png", "").Replace("sm_", "");
                var MMIID = -1;
                int.TryParse(sMMIID, out MMIID);
                if(MMIID > 0) {
                    var mg = MGMixAndMatchItems.FetchObject(MMIID);
                    if(mg == null) {
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "", MMIID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "sm_", MMIID, "png"));
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_", MMIID, "png"));
                    }
                }
                Response.Write(".");
            }

            // WordMatch
            WorkFolder = HttpContext.Current.Server.MapPath("~/Images/Games/WordMatch/");
            foreach(var f in System.IO.Directory.GetFiles(WorkFolder, "sm_*.png")) {
                var sWMIID = f.Replace(WorkFolder, "").Replace(".png", "").Replace("sm_", "");
                var WMIID = -1;
                int.TryParse(sWMIID, out WMIID);
                if(WMIID > 0) {
                    var mg = MGWordMatchItems.FetchObject(WMIID);
                    if(mg == null) {
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "", WMIID, "png"));
                        File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "sm_", WMIID, "png"));
                        //File.Delete(string.Format("{0}{1}{2}{3}.{4}", WorkFolder, "\\", "md_", WMIID, "png"));
                    }
                }
                Response.Write(".");
            }

            if(Response != null) {
                Response.Buffer = false;
                Response.Write("-->");
            }

        }

        public static void InsertInitializationTrackingRecord(string objType, int dst, int dstPK, int srcPK) {
            SqlParameter[] arrParams = new SqlParameter[6];

            arrParams[0] = new SqlParameter("@IntitType", objType);
            arrParams[1] = new SqlParameter("@DestTID", dst);
            arrParams[2] = new SqlParameter("@SrcPK", srcPK);
            arrParams[3] = new SqlParameter("@DateCreated", DateTime.Now);
            arrParams[4] = new SqlParameter("@DstPK", dstPK);
            arrParams[5] = new SqlParameter("@InitID", -1);
            arrParams[5].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "app_TenantInitData_Insert", arrParams);

            var PK = int.Parse(arrParams[5].Value.ToString());
        }

        public static int GetMappedPKbyOriginalPK(string objType, int dst, int srcPK) {
            var dstPK = -1;

            SqlParameter[] arrParams = new SqlParameter[3];

            arrParams[0] = new SqlParameter("@IntitType", objType);
            arrParams[1] = new SqlParameter("@DestTID", dst);
            arrParams[2] = new SqlParameter("@SrcPK", srcPK);

            var ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "app_TenantInitData_GetPKbyOriginalPK", arrParams);

            if(ds.Tables[0].Rows.Count > 0)
                dstPK = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

            return dstPK;
        }

    }
}