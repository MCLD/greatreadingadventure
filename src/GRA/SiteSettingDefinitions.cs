using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA
{
    public static class SiteSettingDefinitions
    {
        public static Dictionary<string, SiteSettingDefinition> DefinitionDictionary
            = new Dictionary<string, SiteSettingDefinition>()
        {
            {
                SiteSettingKey.SecretCode.Disable,
                new SiteSettingDefinition()
                {
                    Name = "Disable",
                    Info = "Put any text here to disable secret code entry",
                    Category = typeof(SiteSettingKey.SecretCode).Name,
                    Format = SiteSettingFormat.Boolean
                }
            },
            {
                SiteSettingKey.Challenges.HideUntilRegistrationOpen,
                new SiteSettingDefinition()
                {
                    Name = "Hide until registration opens",
                    Info = "Put any text here to hide challenges until the program is open for registration",
                    Category = typeof(SiteSettingKey.Challenges).Name,
                    Format = SiteSettingFormat.Boolean
                }
            },
            {
                SiteSettingKey.Events.HideUntilRegistrationOpen,
                new SiteSettingDefinition()
                {
                    Name = "Hide until registration opens",
                    Info = "Put any text here to hide events until the program is open for registration",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.Boolean
                }
            },
            {
                SiteSettingKey.Events.RequireBadge,
                new SiteSettingDefinition()
                {
                    Name = "Require badges",
                    Info = "Put any text here to require all events to have a badge and secret code associated with them. With this set anyone who has the ManageEvents permission will need the ManageTriggers permission as well.",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.Boolean,
                }
            },
            {
                SiteSettingKey.Points.MaximumPermitted,
                new SiteSettingDefinition()
                {
                    Name = "Maximum points",
                    Info = "A number representing the maximum amount of points permitted for a participant to have through regular means (it may be able to be overidden from Mission Control).",
                    Category = typeof(SiteSettingKey.Points).Name,
                    Format = SiteSettingFormat.Integer
                }
            },
            {
                SiteSettingKey.Users.RestrictChangingSystemBranch,
                new SiteSettingDefinition()
                {
                    Name = "Restrict changing system and branch",
                    Info = "Put any text here to forbid participants from changing their system/branch after joining",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean,
                }
            },
            {
                SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup,
                new SiteSettingDefinition()
                {
                    Name = "Maximum household size before group",
                    Info = "A number that is the maximum size for a household before it is converted to a group. Group Types must be configured for this to work.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Integer
                }
            },
            {
                SiteSettingKey.Users.AskIfFirstTime,
                new SiteSettingDefinition()
                {
                    Name = "Ask if first time",
                    Info = "Put any text here to ask participants if it is their first time in the program",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                }
            },
            {
                SiteSettingKey.Users.AskPreregistrationReminder,
                new SiteSettingDefinition()
                {
                    Name = "Ask reminder email during pre-registraion",
                    Info = "Put any text here to ask participants during pre-registration if they want to receive an email when the program starts. This software does not send the email, it just collects the addresses.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                }
            },
            {
                SiteSettingKey.Users.DefaultDailyPersonalGoal,
                new SiteSettingDefinition()
                {
                    Name = "Default daily personal goal",
                    Info = "A number here will prompt participants if they want to use that as a daily personal activity goal. Requires the site to program start and program end dates configured.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Integer
                }
            }
        };
    }
}
