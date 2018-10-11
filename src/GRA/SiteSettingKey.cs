namespace GRA
{
    public struct SiteSettingKey
    {
        public struct SecretCode
        {
            // TODO make this truly disable secret codes for the site
            public static readonly string Disable = "SecretCode.Disable";
        }

        public struct Challenges
        {
            public static readonly string HideUntilRegistrationOpen
                = "Challenges.HideUntilRegistrationOpen";
        }

        public struct Events
        {
            public static readonly string HideUntilRegistrationOpen
                = "Events.HideUntilRegistrationOpen";
            public static readonly string RequireBadge = "Events.RequireBadge";
        }

        public struct Points
        {
            public static readonly string MaximumPermitted = "Points.MaximumPermitted";
        }

        public struct Users
        {
            public static readonly string RestrictChangingSystemBranch
                = "Users.RestrictChangingSystemBranch";
            public static readonly string MaximumHouseholdSizeBeforeGroup
                = "Users.MaximumHouseholdSizeBeforeGroup";
            public static readonly string AskIfFirstTime = "Users.AskIfFirstTime";
            public static readonly string AskPreregistrationReminder
                = "Users.AskPreregistrationReminder";
            public static readonly string CollectAccessClosedEmails
                = "Users.CollectAccessClosedEmails";
            public static readonly string CollectPreregistrationEmails
                = "User.CollectPreregistrationEmails";
            public static readonly string DefaultDailyPersonalGoal
                = "Users.DefaultDailyPersonalGoal";
            public static readonly string SurveyUrl = "Users.SurveyUrl";
            public static readonly string FirstTimeSurveyUrl = "Users.FirstTimeSurveyUrl";
        }
    }
}