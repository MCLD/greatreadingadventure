using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA
{
    public static class SiteSettingDefinitions
    {
        public static readonly Dictionary<string, SiteSettingDefinition> DefinitionDictionary
            = new()
            {
                [SiteSettingKey.Avatars.DisableSharing] = new SiteSettingDefinition
                {
                    Name = "Disable Sharing",
                    Info = "Hide the avatar share button and disable sharing options",
                    Category = typeof(SiteSettingKey.Avatars).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Badges.EnableBadgeMaker] = new SiteSettingDefinition
                {
                    Name = "Enable badgemaker",
                    Info = "Enable the openbadges.me badge maker",
                    Category = typeof(SiteSettingKey.Badges).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Badges.MaxDimension] = new SiteSettingDefinition
                {
                    Name = "Max Dimension",
                    Info = "Maximum size in pixels (width or height) allowed for a badge.",
                    Category = typeof(SiteSettingKey.Badges).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Badges.MaxFileSize] = new SiteSettingDefinition
                {
                    Name = "Max File Size (KB)",
                    Info = "Maximum file size of a badge in kilobytes allowed to be uploaded.",
                    Category = typeof(SiteSettingKey.Badges).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Challenges.HideUntilRegistrationOpen] = new SiteSettingDefinition
                {
                    Name = "Hide until registration opens",
                    Info = "Put any text here to hide challenges until the program is open for registration",
                    Category = typeof(SiteSettingKey.Challenges).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Challenges.MaxPointsPerChallengeTask] = new SiteSettingDefinition
                {
                    Name = "Maximum points per challenge task",
                    Info = "A number representing the maximum amount of points permitted per challenge task (it may be able to be overidden from Mission Control).",
                    Category = typeof(SiteSettingKey.Challenges).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Email.MaximumWelcomeEmailSendBlock] = new SiteSettingDefinition
                {
                    Name = "Welcome email maximum sending block",
                    Info = "The number of welcome emails to send in one block, sending occurs every time the job runner executes - defaults to 20",
                    Category = typeof(SiteSettingKey.Email).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Email.UnsubscribeBase] = new SiteSettingDefinition
                {
                    Name = "Unsubscribe base",
                    Info = "Base part of the email unsubscribe url",
                    Category = typeof(SiteSettingKey.Email).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.Email.WelcomeDelayHours] = new SiteSettingDefinition
                {
                    Name = "Welcome email delay hours",
                    Info = "The delay, in hours, to wait after someone signs up before sending them the welcome email. If set to 0 the email is sent as soon as possible",
                    Category = typeof(SiteSettingKey.Email).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Email.WelcomeTemplateId] = new SiteSettingDefinition
                {
                    Name = "Welcome email ID",
                    Info = "The ID of the welcome email to send, if set to 0 then welcome emails are disabled",
                    Category = typeof(SiteSettingKey.Email).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Events.CommunityExperienceDescription] = new SiteSettingDefinition
                {
                    Name = "Community experience definition",
                    Info = "Set the text to add as a tool tip explaining community experiences to participants",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.Events.GoogleMapsAPIKey] = new SiteSettingDefinition
                {
                    Name = "Google Maps API key",
                    Info = "Put the Google Maps API key here to enable proximity-based event searching",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.Events.HideUntilProgramOpen] = new SiteSettingDefinition
                {
                    Name = "Hide until program opens",
                    Info = "Put any text here to hide events until the program is open",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Events.HideUntilRegistrationOpen] = new SiteSettingDefinition
                {
                    Name = "Hide until registration opens",
                    Info = "Put any text here to hide events until the program is open for registration",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Events.RequireBadge] = new SiteSettingDefinition
                {
                    Name = "Require badges",
                    Info = "Put any text here to require all events to have a badge and secret code associated with them. With this set anyone who has the ManageEvents permission will need the ManageTriggers permission as well.",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.Boolean,
                },
                [SiteSettingKey.Events.StreamingShowCode] = new SiteSettingDefinition
                {
                    Name = "Show code for streaming events",
                    Info = "Put any text here to show on all streaming event pages their associated secret code to participants.",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.Boolean,
                },
                [SiteSettingKey.Mail.Disable] = new SiteSettingDefinition
                {
                    Name = "Disable Mail",
                    Info = "Disable the in-software mail setting so that participants and administrators cannot exchange mail",
                    Category = typeof(SiteSettingKey.Mail).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Performer.BackToBackInterval] = new SiteSettingDefinition
                {
                    Name = "Intervals Between Performer Back-To-Back Programs",
                    Info = "(Optional) Input a comma separated list of values for performers to select during registration (ie: 20, 25, 30)",
                    Category = typeof(SiteSettingKey.Performer).Name,
                    Format = SiteSettingFormat.IntegerCsv
                },
                [SiteSettingKey.Performer.EnableInsuranceQuestion] = new SiteSettingDefinition
                {
                    Name = "Enable Performer Insurance Question",
                    Info = "If enabled, performers will be asked if they have liability insurance during registration",
                    Category = typeof(SiteSettingKey.Performer).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Performer.EnableLivestreamQuestions] = new SiteSettingDefinition
                {
                    Name = "Enable Performer Livestream Questions",
                    Info = "If enabled, performers will be asked for livestream and archive consent during registration",
                    Category = typeof(SiteSettingKey.Performer).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Points.MaximumPermitted] = new SiteSettingDefinition
                {
                    Name = "Maximum points",
                    Info = "A number representing the maximum amount of points permitted for a participant to have through regular means (it may be able to be overidden from Mission Control).",
                    Category = typeof(SiteSettingKey.Points).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.SecretCode.Disable] = new SiteSettingDefinition
                {
                    Name = "Disable",
                    Info = "Put any text here to disable secret code entry",
                    Category = typeof(SiteSettingKey.SecretCode).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Site.AskToAddFamilyMembers] = new SiteSettingDefinition
                {
                    Name = "Ask New Users to Add Family Members",
                    Info = "Upon joining, new users will see a one-time notification asking if they would like to add any family members.",
                    Category = typeof(SiteSettingKey.Site).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Site.BackgroundColor] = new SiteSettingDefinition
                {
                    Name = "Background color",
                    Info = "Set a background color for the site: must start with # and end with ;",
                    Category = typeof(SiteSettingKey.Site).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.Site.ReadingGoalInMinutes] = new SiteSettingDefinition
                {
                    Name = "Site Reading Goal",
                    Info = "Sets a reading goal (minutes) for the entire site and duration",
                    Category = typeof(SiteSettingKey.Site).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Site.WelcomeMessage] = new SiteSettingDefinition
                {
                    Name = "Welcome message",
                    Info = "Message and instructions to display to participants at the first join step",
                    Category = typeof(SiteSettingKey.Site).Name,
                    Format = SiteSettingFormat.SegmentId
                },
                [SiteSettingKey.Triggers.DisallowTriggersBelowPoints] = new SiteSettingDefinition
                {
                    Name = "Trigger minimum activation point level",
                    Info = "Do not allow triggers which activate at point values below this value (overridable with IgnorePointLimits permission)",
                    Category = typeof(SiteSettingKey.Triggers).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Triggers.LowPointThreshold] = new SiteSettingDefinition
                {
                    Name = "Low point activation threshold for triggers",
                    Info = "Triggers activated with this many points or fewer are considered to be low point activated.",
                    Category = typeof(SiteSettingKey.Triggers).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Triggers.MaxPointsPerTrigger] = new SiteSettingDefinition
                {
                    Name = "Maximum points awardable per trigger",
                    Info = "Do not allow triggers to award more than this many points (overridable with IgnorePointLimits permission)",
                    Category = typeof(SiteSettingKey.Triggers).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Users.AskEmailSubPermission] = new SiteSettingDefinition
                {
                    Name = "Ask email subscription permission",
                    Info = "Set the text to be displayed when a user signs up to ask if they want to subscribe to emails.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.Users.AskIfFirstTime] = new SiteSettingDefinition
                {
                    Name = "Ask if first time",
                    Info = "Put any text here to ask participants if it is their first time in the program.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Users.AskPersonalPointGoal] = new SiteSettingDefinition
                {
                    Name = "Ask personal point goal",
                    Info = "Put any text here to ask participants to optionally set a personal point goal.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Users.CollectAccessClosedEmails] = new SiteSettingDefinition
                {
                    Name = "Collect emails after access has closed",
                    Info = "Put any text here to add email collection on the homepage after the program has closed. This software does not send the email, it just collects the addresses.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Users.CollectPreregistrationEmails] = new SiteSettingDefinition
                {
                    Name = "Collect emails before registration has opened",
                    Info = "Put any text here to add email collection on the homepage before registration has opened. This software does not send the email, it just collects the addresses.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Users.DefaultDailyPersonalGoal] = new SiteSettingDefinition
                {
                    Name = "Default daily personal goal",
                    Info = "A number here will prompt participants if they want to use that as a daily personal activity goal. Requires the site to program start and program end dates configured.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Users.FirstTimeSurveyUrl] = new SiteSettingDefinition
                {
                    Name = "First time survey URL",
                    Info = "Set a link for {Your Website}/Survey to redirect to for first time users. Requires Ask if first time and Survey URL to be configured.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.Users.MaximumActivityPermitted] = new SiteSettingDefinition
                {
                    Name = "Maximum activity",
                    Info = "A number representing the maximum amount of activity permitted for a participant.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup] = new SiteSettingDefinition
                {
                    Name = "Maximum household size before group",
                    Info = "A number that is the maximum size for a household before it is converted to a group. Group Types must be configured for this to work.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Users.RestrictChangingProgram] = new SiteSettingDefinition
                {
                    Name = "Restrict changing program",
                    Info = "Put any text here to forbid participants from changing their program after joining",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean,
                },
                [SiteSettingKey.Users.RestrictChangingSystemBranch] = new SiteSettingDefinition
                {
                    Name = "Restrict changing system and branch",
                    Info = "Put any text here to forbid participants from changing their system/branch after joining",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean,
                },
                [SiteSettingKey.Users.ShowLinkToParticipantsLibrary] = new SiteSettingDefinition
                {
                    Name = "Show link to participant's library",
                    Info = "Shows the link to the participant's library in the footer.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Users.ShowLinkToParticipatingLibraries] = new SiteSettingDefinition
                {
                    Name = "Show link to all participating libraries",
                    Info = "Shows the link to all participating libraries in the footer.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Users.SurveyUrl] = new SiteSettingDefinition
                {
                    Name = "Survey URL",
                    Info = "Set a link for {Your Website}/Survey to redirect to.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.VendorCodes.ShowPackingSlip] = new SiteSettingDefinition
                {
                    Name = "Show Packing Slips",
                    Info = "Show the Packing Slip navigation in Mission Control",
                    Category = typeof(SiteSettingKey.VendorCodes).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Web.CacheSiteCustomizationsMinutes] = new SiteSettingDefinition
                {
                    Name = "Check for site.css and site.js changes on disk",
                    Info = "How often (in minutes) To check if site.css and site.js have changed on disk. Set to 0 when working on the files so that they will reload on each page load, set to a higher number when you are editing them infrequently. An empty setting will default to 60 minutes",
                    Category = typeof(SiteSettingKey.Web).Name,
                    Format = SiteSettingFormat.Integer
                },
            };
    }
}
