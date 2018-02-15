
namespace GRA
{
    public struct SiteSettingKey
    {
        public struct SecretCode
        {
            // Currently: if this is set (i.e. not null), hide the secret code entry
            // TODO make this truly disable secret codes for the site
            public const string Disable = "SecretCode.Disable";
        }

        public struct Challenges
        {
            // If this is set (i.e. not null) hide challenges until the registration period is open
            public const string HideUntilRegistrationOpen = "Challenges.HideUntilRegistrationOpen";
        }

        public struct Events
        {
            // If this is set (i.e. not null) hide events until the registration period is open
            public const string HideUntilRegistrationOpen = "Events.HideUntilRegistrationOpen";

            // If this is set (i.e. not null) require all events to be created with a badge.
            // With this set anyone who has the ManageEvents permission will need the ManageTriggers
            // permission as well.
            public const string RequireBadge = "Events.RequireBadge";
        }

        public struct Points
        {
            // If this is set to an integer, it represents the maximum amount of points permitted
            // for a participant to have through regular means (it may be able to be overidden from
            // Mission Control.
            public const string MaximumPermitted = "Points.MaximumPermitted";
        }

        public struct Users
        {
            // If this is set (i.e. not null) do not allow users to change their system/branch after
            // joining
            public const string RestrictChangingSystemBranch = "Users.RestrictChangingSystemBranch";

            // If this is set to an integer, when the household count exceeds this number the
            // household head will be forced to enter a group name and select a group type.
            public const string MaximumHouseholdSizeBeforeGroup = "Users.MaximumHouseholdSizeBeforeGroup";

            // If this is set (i.e. not null) do not allow users to sign up without selecting if
            // it's their first time in the program
            public const string AskIfFirstTime = "Users.AskIfFirstTime";

            // If this is set to an integer ask users on sign up for a daily personal activity goal
            // with this value being the default option. Requires the site to have dates set for
            // ProgramStarst and ProgramEnds.
            public const string DefaultDailyPersonalGoal = "Users.DefaultDailyPersonalGoal";
        }
    }
}
