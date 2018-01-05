
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
        }

        public struct Users
        {
            // If this is set (i.e. not null) do not allow users to change their system/branch after
            // joining
            public const string RestrictChangingSystemBranch = "Users.RestrictChangingSystemBranch";
        }
    }
}
