namespace GRA.SiteSettingKey
{
    public static class Avatars
    {
        public static readonly string DisableSharing = "Avatars.DisableSharing";
    }

    public static class Badges
    {
        public static readonly string EnableBadgeMaker = "Badges.EnableMaker";
        public static readonly string MaxDimension = "Badges.MaxDimension";
        public static readonly string MaxFileSize = "Badges.MaxFileSize";
    }

    public static class Challenges
    {
        public static readonly string HideUntilRegistrationOpen
            = "Challenges.HideUntilRegistrationOpen";

        public static readonly string MaxPointsPerChallengeTask
            = "Challenges.MaxPointsPerChallengeTask";
    }

    public static class Email
    {
        public static readonly string MaximumWelcomeEmailSendBlock
            = "Email.MaximumWelcomeEmailSendBlock";

        public static readonly string UnsubscribeBase = "Email.UnsubscribeBase";
        public static readonly string WelcomeDelayHours = "Email.WelcomeEmailDelayHours";
        public static readonly string WelcomeTemplateId = "Email.WelcomeEmailId";
    }

    public static class Events
    {
        public static readonly string CommunityExperienceDescription
            = "Events.CommunityExperienceDescription";

        public static readonly string GoogleMapsAPIKey = "Events.GoogleMapsAPIKey";

        public static readonly string HideUntilProgramOpen
            = "Events.HideUntilProgramOpen";

        public static readonly string HideUntilRegistrationOpen
                        = "Events.HideUntilRegistrationOpen";

        public static readonly string RequireBadge = "Events.RequireBadge";

        public static readonly string StreamingShowCode = "Events.StreamingShowCode";
    }

    public static class Mail
    {
        public static readonly string Disable = "Mail.Enabled";
    }

    public static class Performer
    {
        public static readonly string BackToBackInterval = "Performers.PerformerBackToBackInterval";
        public static readonly string EnableInsuranceQuestion = "Performer.EnableInsuranceQuestion";

        public static readonly string EnableLivestreamQuestions
            = "Performer.EnableLivestreamQuestions";
    }

    public static class Points
    {
        public static readonly string MaximumPermitted = "Points.MaximumPermitted";
    }

    public static class SecretCode
    {
        // TODO make this truly disable secret codes for the site
        public static readonly string Disable = "SecretCode.Disable";
    }

    public static class Site
    {
        public static readonly string AskToAddFamilyMembers = "Site.AskToAddFamilyMembers";
        public static readonly string BackgroundColor = "Site.BackgroundColor";
        public static readonly string ReadingGoalInMinutes = "Site.ReadingGoalInMinutes";
        public static readonly string WelcomeMessage = "Site.WelcomeMessage";
    }

    public static class Triggers
    {
        public static readonly string DisallowTriggersBelowPoints = "Triggers.DisallowTriggersBelowPoints";
        public static readonly string LowPointThreshold = "Triggers.LowPointThreshold";
        public static readonly string MaxPointsPerTrigger = "Triggers.MaxPointsPerTrigger";
    }

    public static class Users
    {
        public static readonly string AskEmailSubPermission = "Users.AskEmailSubPermission";

        public static readonly string AskIfFirstTime = "Users.AskIfFirstTime";

        public static readonly string AskPersonalPointGoal = "Users.AskPersonalPointGoal";

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

        public static readonly string RestrictChangingProgram
            = "Users.RestrictChangingProgram";

        public static readonly string RestrictChangingSystemBranch
            = "Users.RestrictChangingSystemBranch";

        public static readonly string ShowLinkToParticipantsLibrary
            = "Users.ShowLinkToParticipantsLibrary";

        public static readonly string ShowLinkToParticipatingLibraries
            = "Users.ShowLinkToParticipatingLibraries";

        public static readonly string SurveyUrl = "Users.SurveyUrl";
    }

    public static class VendorCodes
    {
        public static readonly string ShowPackingSlip = "VendorCodes.ShowPackingSlip";
    }

    public static class Web
    {
        public static readonly string CacheSiteCustomizationsMinutes
            = "Web.CacheSiteCustomizationsMinutes";
    }
}
