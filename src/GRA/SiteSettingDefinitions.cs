using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA
{
    public static class SiteSettingDefinitions
    {
        public static Dictionary<string, SiteSettingDefinition> DefinitionDictionary
            = new Dictionary<string, SiteSettingDefinition>()
            {
                [SiteSettingKey.SecretCode.Disable] =
                new SiteSettingDefinition
                {
                    Name = "Disable",
                    Info = "Put any text here to disable secret code entry",
                    Category = typeof(SiteSettingKey.SecretCode).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Challenges.HideUntilRegistrationOpen] =
                new SiteSettingDefinition
                {
                    Name = "Hide until registration opens",
                    Info = "Put any text here to hide challenges until the program is open for registration",
                    Category = typeof(SiteSettingKey.Challenges).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Events.CommunityExperienceDescription] =
                new SiteSettingDefinition
                {
                    Name = "Community experience definition",
                    Info = "Set the text to add as a tool tip explaining community experiences to participants",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.Events.GoogleMapsAPIKey] =
                new SiteSettingDefinition
                {
                    Name = "Google Maps API key",
                    Info = "Put the Google Maps API key here to enable proximity-based event searching",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.Events.HideUntilRegistrationOpen] =
                new SiteSettingDefinition
                {
                    Name = "Hide until registration opens",
                    Info = "Put any text here to hide events until the program is open for registration",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Events.HideUntilProgramOpen] =
                new SiteSettingDefinition
                {
                    Name = "Hide until program opens",
                    Info = "Put any text here to hide events until the program is open",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Events.RequireBadge] =
                new SiteSettingDefinition
                {
                    Name = "Require badges",
                    Info = "Put any text here to require all events to have a badge and secret code associated with them. With this set anyone who has the ManageEvents permission will need the ManageTriggers permission as well.",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.Boolean,
                },
                [SiteSettingKey.Points.MaximumPermitted] =
                new SiteSettingDefinition
                {
                    Name = "Maximum points",
                    Info = "A number representing the maximum amount of points permitted for a participant to have through regular means (it may be able to be overidden from Mission Control).",
                    Category = typeof(SiteSettingKey.Points).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Users.RestrictChangingSystemBranch] =
                new SiteSettingDefinition
                {
                    Name = "Restrict changing system and branch",
                    Info = "Put any text here to forbid participants from changing their system/branch after joining",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean,
                },
                [SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup] =
                new SiteSettingDefinition
                {
                    Name = "Maximum household size before group",
                    Info = "A number that is the maximum size for a household before it is converted to a group. Group Types must be configured for this to work.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Users.AskEmailSubPermission] =
                new SiteSettingDefinition
                {
                    Name = "Ask email subscription permission",
                    Info = "Set the text to be displayed when a user signs up to ask if they want to subscribe to emails.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.Users.AskIfFirstTime] =
                new SiteSettingDefinition
                {
                    Name = "Ask if first time",
                    Info = "Put any text here to ask participants if it is their first time in the program.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Users.CollectAccessClosedEmails] =
                new SiteSettingDefinition
                {
                    Name = "Collect emails after access has closed",
                    Info = "Put any text here to add email collection on the homepage after the program has closed. This software does not send the email, it just collects the addresses.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Users.CollectPreregistrationEmails] =
                new SiteSettingDefinition
                {
                    Name = "Collect emails before registration has opened",
                    Info = "Put any text here to add email collection on the homepage before registration has opened. This software does not send the email, it just collects the addresses.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                },
                [SiteSettingKey.Users.DefaultDailyPersonalGoal] =
                new SiteSettingDefinition
                {
                    Name = "Default daily personal goal",
                    Info = "A number here will prompt participants if they want to use that as a daily personal activity goal. Requires the site to program start and program end dates configured.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Users.MaximumActivityPermitted] =
                new SiteSettingDefinition
                {
                    Name = "Maximum activity",
                    Info = "A number representing the maximum amount of activity permitted for a participant.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Users.SurveyUrl] =
                new SiteSettingDefinition
                {
                    Name = "Survey URL",
                    Info = "Set a link for {Your Website}/Survey to redirect to.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.Users.FirstTimeSurveyUrl] =
                new SiteSettingDefinition
                {
                    Name = "First time survey URL",
                    Info = "Set a link for {Your Website}/Survey to redirect to for first time users. Requires Ask if first time and Survey URL to be configured.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.String
                },
                [SiteSettingKey.Web.CacheSiteCustomizationsMinutes] =
                new SiteSettingDefinition
                {
                    Name = "Check for site.css and site.js changes on disk",
                    Info = "How often (in minutes) To check if site.css and site.js have changed on disk. Set to 0 when working on the files so that they will reload on each page load, set to a higher number when you are editing them infrequently. An empty setting will default to 60 minutes",
                    Category = typeof(SiteSettingKey.Web).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Badges.MaxFileSize] =
                new SiteSettingDefinition
                {
                    Name = "Max File Size (KB)",
                    Info = "Maximum file size of a badge in kilobytes allowed to be uploaded.",
                    Category = typeof(SiteSettingKey.Badges).Name,
                    Format = SiteSettingFormat.Integer
                },
                [SiteSettingKey.Badges.MaxDimension] =
                new SiteSettingDefinition
                {
                    Name = "Max Dimension",
                    Info = "Maximum size in pixels (width or height) allowed for a badge.",
                    Category = typeof(SiteSettingKey.Badges).Name,
                    Format = SiteSettingFormat.Integer
                }
            };
    }
}
