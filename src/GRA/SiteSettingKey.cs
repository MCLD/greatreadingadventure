namespace GRA
{
    public struct SiteSettingKey : System.IEquatable<SiteSettingKey>
    {
        public struct Challenges : System.IEquatable<Challenges>
        {
            public static readonly string HideUntilRegistrationOpen
                = "Challenges.HideUntilRegistrationOpen";

            public bool Equals(Challenges other) { return true; }
        }

        public struct Events : System.IEquatable<Events>
        {
            public static readonly string CommunityExperienceDescription
                = "Events.CommunityExperienceDescription";

            public static readonly string GoogleMapsAPIKey = "Events.GoogleMapsAPIKey";

            public static readonly string HideUntilRegistrationOpen
                = "Events.HideUntilRegistrationOpen";

            public static readonly string RequireBadge = "Events.RequireBadge";
            public bool Equals(Events other) { return true; }
        }

        public struct Points : System.IEquatable<Points>
        {
            public static readonly string MaximumPermitted = "Points.MaximumPermitted";
            public bool Equals(Points other) { return true; }
        }

        public struct SecretCode : System.IEquatable<SecretCode>
        {
            // TODO make this truly disable secret codes for the site
            public static readonly string Disable = "SecretCode.Disable";
            public bool Equals(SecretCode other) { return true; }
        }

        public struct Users : System.IEquatable<Users>
        {
            public static readonly string RestrictChangingSystemBranch
                = "Users.RestrictChangingSystemBranch";

            public static readonly string MaximumHouseholdSizeBeforeGroup
                = "Users.MaximumHouseholdSizeBeforeGroup";

            public static readonly string AskEmailSubPermission = "Users.AskEmailSubPermission";

            public static readonly string AskIfFirstTime = "Users.AskIfFirstTime";

            public static readonly string CollectAccessClosedEmails
                = "Users.CollectAccessClosedEmails";

            public static readonly string CollectPreregistrationEmails
                = "User.CollectPreregistrationEmails";

            public static readonly string DefaultDailyPersonalGoal
                = "Users.DefaultDailyPersonalGoal";

            public static readonly string MaximumActivityPermitted
                = "Users.MaximumActivityPermitted";

            public static readonly string SurveyUrl = "Users.SurveyUrl";
            public static readonly string FirstTimeSurveyUrl = "Users.FirstTimeSurveyUrl";
            public bool Equals(Users other) { return true; }
        }

        public struct Web : System.IEquatable<Web>
        {
            public static readonly string CacheSiteCustomizationsMinutes
                = "Web.CacheSiteCustomizationsMinutes";

            public bool Equals(Web other) { return true; }
        }

        public bool Equals(SiteSettingKey other) { return true; }
    }
}