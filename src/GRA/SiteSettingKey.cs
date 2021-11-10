namespace GRA
{
    public struct SiteSettingKey : System.IEquatable<SiteSettingKey>
    {
        public bool Equals(SiteSettingKey other)
        {
            return true;
        }

        public struct Avatars : System.IEquatable<Avatars>
        {
            public static readonly string DisableSharing = "Avatars.DisableSharing";

            public bool Equals(Avatars other)
            {
                return true;
            }
        }

        public struct Badges : System.IEquatable<Badges>
        {
            public static readonly string MaxDimension = "Badges.MaxDimension";
            public static readonly string MaxFileSize = "Badges.MaxFileSize";

            public bool Equals(Badges other)
            {
                return true;
            }
        }

        public struct Challenges : System.IEquatable<Challenges>
        {
            public static readonly string HideUntilRegistrationOpen
                = "Challenges.HideUntilRegistrationOpen";

            public static readonly string MaxPointsPerChallengeTask
                = "Challenges.MaxPointsPerChallengeTask";

            public bool Equals(Challenges other)
            {
                return true;
            }
        }

        public struct Events : System.IEquatable<Events>
        {
            public static readonly string CommunityExperienceDescription
                = "Events.CommunityExperienceDescription";

            public static readonly string GoogleMapsAPIKey = "Events.GoogleMapsAPIKey";

            public static readonly string HideUntilProgramOpen
                = "Events.HideUntilProgramOpen";

            public static readonly string HideUntilRegistrationOpen
                            = "Events.HideUntilRegistrationOpen";

            public static readonly string RequireBadge = "Events.RequireBadge";

            public bool Equals(Events other)
            {
                return true;
            }
        }

        public struct Points : System.IEquatable<Points>
        {
            public static readonly string MaximumPermitted = "Points.MaximumPermitted";

            public bool Equals(Points other)
            {
                return true;
            }
        }

        public struct SecretCode : System.IEquatable<SecretCode>
        {
            // TODO make this truly disable secret codes for the site
            public static readonly string Disable = "SecretCode.Disable";

            public bool Equals(SecretCode other)
            {
                return true;
            }
        }

        public struct Triggers : System.IEquatable<Triggers>
        {
            public static readonly string MaxPointsPerTrigger = "Triggers.MaxPointsPerTrigger";

            public bool Equals(Triggers other)
            {
                return true;
            }
        }

        public struct Users : System.IEquatable<Users>
        {
            public static readonly string AskEmailSubPermission = "Users.AskEmailSubPermission";

            public static readonly string AskIfFirstTime = "Users.AskIfFirstTime";

            public static readonly string CollectAccessClosedEmails
                = "Users.CollectAccessClosedEmails";

            public static readonly string CollectPreregistrationEmails
                = "User.CollectPreregistrationEmails";

            public static readonly string DefaultDailyPersonalGoal
                = "Users.DefaultDailyPersonalGoal";

            public static readonly string FirstTimeSurveyUrl = "Users.FirstTimeSurveyUrl";

            public static readonly string MaximumActivityPermitted
                = "Users.MaximumActivityPermitted";

            public static readonly string MaximumHouseholdSizeBeforeGroup
                = "Users.MaximumHouseholdSizeBeforeGroup";

            public static readonly string RestrictChangingSystemBranch
                                                                                                                = "Users.RestrictChangingSystemBranch";

            public static readonly string ShowLinkToParticipantsLibrary
                = "Users.ShowLinkToParticipantsLibrary";

            public static readonly string ShowLinkToParticipatingLibraries
                = "Users.ShowLinkToParticipatingLibraries";

            public static readonly string SurveyUrl = "Users.SurveyUrl";

            public bool Equals(Users other)
            {
                return true;
            }
        }

        public struct VendorCodes : System.IEquatable<VendorCodes>
        {
            public static readonly string ShowPackingSlip = "VendorCodes.ShowPackingSlip";

            public bool Equals(VendorCodes other)
            {
                return true;
            }
        }

        public struct Web : System.IEquatable<Web>
        {
            public static readonly string CacheSiteCustomizationsMinutes
                = "Web.CacheSiteCustomizationsMinutes";

            public bool Equals(Web other)
            {
                return true;
            }
        }
    }
}
