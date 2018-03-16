namespace GRA
{
    public struct SiteSettingKey
    {
        public struct SecretCode
        {
            // TODO make this truly disable secret codes for the site
            public const string Disable = "SecretCode.Disable";
        }

        public struct Challenges
        {
            public const string HideUntilRegistrationOpen = "Challenges.HideUntilRegistrationOpen";
        }

        public struct Events
        {
            public const string HideUntilRegistrationOpen = "Events.HideUntilRegistrationOpen";
            public const string RequireBadge = "Events.RequireBadge";
        }

        public struct Points
        {
            public const string MaximumPermitted = "Points.MaximumPermitted";
        }

        public struct Users
        {
            public const string RestrictChangingSystemBranch = "Users.RestrictChangingSystemBranch";
            public const string MaximumHouseholdSizeBeforeGroup = "Users.MaximumHouseholdSizeBeforeGroup";
            public const string AskIfFirstTime = "Users.AskIfFirstTime";
            public const string AskPreregistrationReminder = "Users.AskPreregistrationReminder";
            public const string DefaultDailyPersonalGoal = "Users.DefaultDailyPersonalGoal";
        }
    }
}