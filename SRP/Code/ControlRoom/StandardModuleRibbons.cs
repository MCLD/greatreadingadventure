using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRA.SRP.Core.Utilities {
    public class StandardModuleRibbons {
        public static List<RibbonPanel> ReportsRibbon() {
            var returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel {
                Name = "Reports",
                ImageAlt = "Reports",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Reports.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Reports@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "New AdHoc Report", Url = "/ControlRoom/Modules/Reports/ReportAddEdit.aspx?RID=new" });
            pnl.Add(new RibbonLink { Name = "Existing Reports", Url = "/ControlRoom/Modules/Reports/ReportList.aspx" });
            returnList.Add(pnl);


            pnl = new RibbonPanel {
                Name = "Dashboard",
                ImageAlt = "Dashboard",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Dashboard.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Dashboard@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Stats Dashboard", Url = "/ControlRoom/Modules/Reports/DashboardStats.aspx" });
            pnl.Add(new RibbonLink { Name = "Registration Stats", Url = "/ControlRoom/Modules/Reports/RegStats.aspx" });
            pnl.Add(new RibbonLink { Name = "Finisher Stats", Url = "/ControlRoom/Modules/Reports/FinishStats.aspx" });
            returnList.Add(pnl);



            pnl = new RibbonPanel {
                Name = "Special Reports",
                ImageAlt = "Special Reports",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/SpecialReports.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/SpecialReports@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Game Level Stats", Url = "/ControlRoom/Modules/Reports/LevelStats.aspx" });
            pnl.Add(new RibbonLink { Name = "Prizes Stats", Url = "/ControlRoom/Modules/Reports/PrizesStats.aspx" });
            pnl.Add(new RibbonLink { Name = "MiniGame Play Stats", Url = "/ControlRoom/Modules/Reports/MiniGameStats.aspx" });
            returnList.Add(pnl);

            pnl = new RibbonPanel {
                Name = "Special Reports",
                ImageAlt = "Special Reports",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/SpecialReports.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/SpecialReports@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Reading Activity", Url = "/ControlRoom/Modules/Reports/ReadingActivityReport.aspx" });
            pnl.Add(new RibbonLink { Name = "Patron Activity", Url = "/ControlRoom/Modules/Reports/PatronActivityReport.aspx" });
            returnList.Add(pnl);


            return returnList;
        }

        public static List<RibbonPanel> DrawingsRibbon() {
            var returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel {
                Name = "Drawings",
                ImageAlt = "Drawings",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Drawings.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Drawings@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Drawings List", Url = "/ControlRoom/Modules/Drawings/Default.aspx" });
            pnl.Add(new RibbonLink { Name = "Add New Drawings", Url = "/ControlRoom/Modules/Drawings/PrizeDrawingAddEdit.aspx" });
            pnl.Add(new RibbonLink { Name = "Drawing Templates", Url = "/ControlRoom/Modules/Drawings/TemplateList.aspx" });
            returnList.Add(pnl);

            return returnList;
        }

        public static List<RibbonPanel> PatronRibbon() {
            var returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel {
                Name = "Patrons",
                ImageAlt = "Patrons",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Patrons.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Patrons@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Patron Search", Url = "/ControlRoom/Modules/Patrons/Default.aspx" });
            pnl.Add(new RibbonLink { Name = "By Program", Url = "/ControlRoom/Modules/Patrons/PatronsByProgram.aspx" });
            //pnl.Add(new RibbonLink { Name = "Search Results", Url = "/ControlRoom/Modules/Patrons/Default.aspx" });
            pnl.Add(new RibbonLink { Name = "Add Patron", Url = "/ControlRoom/Modules/Patrons/PatronAdd.aspx" });
            returnList.Add(pnl);

            return returnList;
        }

        public static List<RibbonPanel> SelectedPatronRibbon() {
            var returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel {
                Name = "Patron Info",
                ImageAlt = "Selected Patron Info",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/PatronInfo.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/PatronInfo@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Patron Details", Url = "/ControlRoom/Modules/Patrons/EditPatron.aspx" });

            //            pnl.Add(new RibbonLink { Name = "By Program", Url = "/ControlRoom/Modules/Drawings/PatronsByProgram.aspx" });
            //            pnl.Add(new RibbonLink { Name = "Search Results", Url = "/ControlRoom/Modules/Drawings/Search.aspx" });

            returnList.Add(pnl);

            return returnList;
        }

        public static List<RibbonPanel> SelectedPatronOtherRibbon() {
            var returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel {
                Name = "More Patron Info",
                ImageAlt = "More Selected Patron Info",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/MorePatronInfo.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/MorePatronInfo@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Patron Logs", Url = "/ControlRoom/Modules/Patrons/PatronLog.aspx" });
            pnl.Add(new RibbonLink { Name = "Patron Badges", Url = "/ControlRoom/Modules/Patrons/PatronBadges.aspx" });
            pnl.Add(new RibbonLink { Name = "Patron Mail", Url = "/ControlRoom/Modules/Patrons/PatronNotifications.aspx" });

            returnList.Add(pnl);

            pnl = new RibbonPanel {
                Name = "More Patron Info",
                ImageAlt = "More Selected Patron Info",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/MoreMorePatronInfo.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/MoreMorePatronInfo@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Patron Prizes", Url = "/ControlRoom/Modules/Patrons/PatronPrizes.aspx" });
            pnl.Add(new RibbonLink { Name = "Patron Reviews", Url = "/ControlRoom/Modules/Patrons/PatronReviews.aspx" });
            pnl.Add(new RibbonLink { Name = "Patron Tests/Surveys", Url = "/ControlRoom/Modules/Patrons/PatronSurveys.aspx" });

            returnList.Add(pnl);

            return returnList;
        }

        public static List<RibbonPanel> NotificationsRibbon() {
            var returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel {
                Name = "Mail",
                ImageAlt = "Mail",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Mail.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Mail@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Mail Queue", Url = "/ControlRoom/Modules/Notifications/NotificationList.aspx" });
            pnl.Add(new RibbonLink { Name = "Bulk Mail", Url = "/ControlRoom/Modules/Notifications/BulkNotification.aspx" });
            returnList.Add(pnl);

            return returnList;
        }

        public static List<RibbonPanel> ProgramRibbon() {
            var returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel {
                Name = "Program",
                ImageAlt = "Program",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Programs.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Programs@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Program List", Url = "/ControlRoom/Modules/Programs/ProgramList.aspx" });
            pnl.Add(new RibbonLink { Name = "Add New Program", Url = "/ControlRoom/Modules/Programs/ProgramsAddEdit.aspx" });
            pnl.Add(new RibbonLink { Name = "Display Order", Url = "/ControlRoom/Modules/Programs/ProgramOrder.aspx" });
            returnList.Add(pnl);

            pnl = new RibbonPanel {
                Name = "Styling",
                ImageAlt = "Program Style",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Styling.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Styling@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Static Text", Url = "/ControlRoom/Modules/Programs/ProgramText.aspx" });
            pnl.Add(new RibbonLink { Name = "CSS Styles", Url = "/ControlRoom/Modules/Programs/ProgramCSS.aspx" });

            returnList.Add(pnl);

            return returnList;
        }

        public static List<RibbonPanel> ManagementRibbon() {
            return SetupRibbon();
        }
        public static List<RibbonPanel> SetupRibbon() {
            var returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel {
                Name = "Avatars/Badges",
                ImageAlt = "Badges Setup",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/AvatarsBadges.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/AvatarsBadges@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Avatars", Url = "/ControlRoom/Modules/Setup/AvatarList.aspx" });
            pnl.Add(new RibbonLink { Name = "Badges", Url = "/ControlRoom/Modules/Setup/BadgeList.aspx" });
            returnList.Add(pnl);

            pnl = new RibbonPanel {
                Name = "Awards",
                ImageAlt = "Awards",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Awards.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Awards@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Badge Awards", Url = "/ControlRoom/Modules/Setup/AwardList.aspx" });
            pnl.Add(new RibbonLink { Name = "Manual Awards", Url = "/ControlRoom/Modules/Setup/AwardManual.aspx" });
            returnList.Add(pnl);

            pnl = new RibbonPanel {
                Name = "Adventures",
                ImageAlt = "Adventures",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Adventures.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Adventures@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Levels", Url = "/ControlRoom/Modules/Setup/BoardGameList.aspx" });
            pnl.Add(new RibbonLink { Name = "Adventures", Url = "/ControlRoom/Modules/Setup/MiniGameList.aspx" });
            returnList.Add(pnl);

            pnl = new RibbonPanel {
                Name = "Events/Offers",
                ImageAlt = "Events/Offers",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/EventsOffers.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/EventsOffers@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Events", Url = "/ControlRoom/Modules/Setup/EventList.aspx" });
            pnl.Add(new RibbonLink { Name = "Offers", Url = "/ControlRoom/Modules/Setup/OfferList.aspx" });
            returnList.Add(pnl);

            pnl = new RibbonPanel {
                Name = "Challenges",
                ImageAlt = "Challenges",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Challenges.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Challenges@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Challenges", Url = "/ControlRoom/Modules/Setup/BookListList.aspx" });
            returnList.Add(pnl);

            pnl = new RibbonPanel {
                Name = "Tests/Surveys",
                ImageAlt = "Tests/Surveys",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/TestsSurveys.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/TestsSurveys@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Test/Survey List", Url = "/ControlRoom/Modules/Setup/SurveyList.aspx" });
            pnl.Add(new RibbonLink { Name = "Test/Survey Results", Url = "/ControlRoom/Modules/Setup/SurveyResults.aspx" });
            returnList.Add(pnl);

            return returnList;
        }

        public static List<RibbonPanel> SettingsRibbon() {
            List<RibbonPanel> returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel {
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/SystemSettings.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/SystemSettings@2x.png")
            };
            pnl.Name = "System Settings";
            pnl.ImageAlt = "System Settings";
            pnl.Add(new RibbonLink { Name = "System Settings", Url = "/ControlRoom/Modules/Settings/SRPSettings.aspx" });
            pnl.Add(new RibbonLink { Name = "Registration Settings", Url = "/ControlRoom/Modules/Settings/RegistrationSettingsAddEdit.aspx" });
            returnList.Add(pnl);

            pnl = new RibbonPanel() {
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Codes.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Codes@2x.png")
            };
            pnl.Name = "Codes";
            pnl.ImageAlt = "System Codes";
            pnl.Add(new RibbonLink { Name = "System Codes", Url = "/ControlRoom/Modules/Settings/Codes.aspx" });
            pnl.Add(new RibbonLink { Name = "Library/District Setup", Url = "/ControlRoom/Modules/Settings/LibraryDistrict.aspx" });
            pnl.Add(new RibbonLink { Name = "School/District Setup", Url = "/ControlRoom/Modules/Settings/SchoolDistrict.aspx" });
            returnList.Add(pnl);

            pnl = new RibbonPanel() {
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/CustomFields.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/CustomFields@2x.png")
            };
            pnl.Name = "Custom Fields";
            pnl.ImageAlt = "Custom Fields";
            pnl.Add(new RibbonLink { Name = "Registration Fields", Url = "/ControlRoom/Modules/Settings/RegCustomFields.aspx" });
            pnl.Add(new RibbonLink { Name = "Event Fields", Url = "/ControlRoom/Modules/Settings/EvtCustomFields.aspx" });
            returnList.Add(pnl);

            return returnList;
        }

        public static List<RibbonPanel> MyAccountRibbon() {
            List<RibbonPanel> returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel() {
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Accounts.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/Accounts@2x.png")
            };
            pnl.Name = "Accounts";
            pnl.ImageAlt = "Accounts";
            pnl.Add(new RibbonLink { Name = "My Account", Url = "/ControlRoom/Modules/PortalUser/MyAccount.aspx" });
            pnl.Add(new RibbonLink { Name = "Reset Password", Url = "/ControlRoom/Modules/PortalUser/PasswordReset.aspx" });
            returnList.Add(pnl);

            return returnList;
        }

        public static List<RibbonPanel> MasterTenantRibbon() {
            List<RibbonPanel> returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel() {
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/OrganizationManagement.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/OrganizationManagement@2x.png")
            };
            pnl.Name = "Organization Management";
            pnl.ImageAlt = "Organization Management";
            pnl.Add(new RibbonLink { Name = "Organizations", Url = "/ControlRoom/Modules/Tenant/TenantList.aspx" });
            pnl.Add(new RibbonLink { Name = "Users", Url = "/ControlRoom/Modules/Tenant/TenantUserList.aspx" });
            pnl.Add(new RibbonLink { Name = "Groups", Url = "/ControlRoom/Modules/Tenant/TenantGroupList.aspx" });
            returnList.Add(pnl);

            pnl = new RibbonPanel {
                Name = "Tenant Reports",
                ImageAlt = "Tenant Reports",
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/TenantSummary.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/TenantSummary@2x.png")
            };
            pnl.Add(new RibbonLink { Name = "Tenant Summary", Url = "/ControlRoom/Modules/Tenant/TenantSummaryReport.aspx" });

            returnList.Add(pnl);


            return returnList;
        }

        public static List<RibbonPanel> SubTenantRibbon() {
            List<RibbonPanel> returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel {
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/TenantSummary.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/TenantSummary@2x.png")
            };
            pnl.Name = "Organization Account";
            pnl.ImageAlt = "Organization Account Management";
            pnl.Add(new RibbonLink { Name = "Org Account", Url = "/ControlRoom/Modules/Tenant/MyTenantAccount.aspx" });

            returnList.Add(pnl);

            return returnList;
        }

        public static List<RibbonPanel> SecurityRibbon() {
            List<RibbonPanel> returnList = new List<RibbonPanel>();
            var pnl = new RibbonPanel() {
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/UserSecurity.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/UserSecurity@2x.png")
            };
            pnl.Name = "User Security";
            pnl.ImageAlt = "User Security";
            pnl.Add(new RibbonLink { Name = "Control Room Users", Url = "/ControlRoom/Modules/Security/Default.aspx" });
            pnl.Add(new RibbonLink { Name = "Add New User", Url = "/ControlRoom/Modules/Security/UserAddEdit.aspx?type=new" });
            pnl.Add(new RibbonLink { Name = "Currently Logged In", Url = "/ControlRoom/Modules/Security/CurrentSessions.aspx" });
            returnList.Add(pnl);

            pnl = new RibbonPanel() {
                ImagePath = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/GroupSecurity.png"),
                ImagePath2x = VirtualPathUtility.ToAbsolute("~/ControlRoom/RibbonImages/GroupSecurity@2x.png")
            };
            pnl.Name = "Group Security";
            pnl.ImageAlt = "Group Security";
            pnl.Add(new RibbonLink { Name = "User Groups", Url = "/ControlRoom/Modules/Security/GroupsList.aspx" });
            pnl.Add(new RibbonLink { Name = "Add New Group", Url = "/ControlRoom/Modules/Security/GroupsAddEdit.aspx" });
            returnList.Add(pnl);

            return returnList;
        }
    }
}
