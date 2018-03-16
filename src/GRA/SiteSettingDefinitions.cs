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
                    Info = "Currently: if this is set (i.e. not null), hide the secret code entry",
                    Category = typeof(SiteSettingKey.SecretCode).Name,
                    Format = SiteSettingFormat.Boolean
                }
            },
            {
                SiteSettingKey.Challenges.HideUntilRegistrationOpen,
                new SiteSettingDefinition()
                {
                    Name = "Hide until registration opens",
                    Info = "If this is set (i.e. not null) hide challenges until the " +
                        "registration period is open",
                    Category = typeof(SiteSettingKey.Challenges).Name,
                    Format = SiteSettingFormat.Boolean
                }
            },
            {
                SiteSettingKey.Events.HideUntilRegistrationOpen,
                new SiteSettingDefinition()
                {
                    Name = "Hide until registration opens",
                    Info = "If this is set (i.e. not null) hide events until the registration " +
                        "period is open",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.Boolean
                }
            },
            {
                SiteSettingKey.Events.RequireBadge,
                new SiteSettingDefinition()
                {
                    Name = "Require badges",
                    Info = "If this is set (i.e. not null) require all events to be created with " +
                        "a badge. With this set anyone who has the ManageEvents permission will " +
                        "need the ManageTriggers permission as well.",
                    Category = typeof(SiteSettingKey.Events).Name,
                    Format = SiteSettingFormat.Boolean,
                }
            },
            {
                SiteSettingKey.Points.MaximumPermitted,
                new SiteSettingDefinition()
                {
                    Name = "Maximum points",
                    Info = "If this is set to an integer, it represents the maximum amount of " +
                        "points permitted for a participant to have through regular means " +
                        "(it may be able to be overidden from Mission Control).",
                    Category = typeof(SiteSettingKey.Points).Name,
                    Format = SiteSettingFormat.Integer
                }
            },
            {
                SiteSettingKey.Users.RestrictChangingSystemBranch,
                new SiteSettingDefinition()
                {
                    Name = "Restrict changing system and branch",
                    Info = "If this is set (i.e. not null) do not allow users to change their " +
                        "system/branch after joining",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean,
                }
            },
            {
                SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup,
                new SiteSettingDefinition()
                {
                    Name = "Maximum household size before group",
                    Info = "If this is set to an integer, when the household count exceeds this " +
                        "number the household head will be forced to enter a group name and " +
                        "select a group type.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Integer
                }
            },
            {
                SiteSettingKey.Users.AskIfFirstTime,
                new SiteSettingDefinition()
                {
                    Name = "Ask if first time",
                    Info = "If this is set (i.e. not null) do not allow users to sign up without " +
                        "selecting if it's their first time in the program",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                }
            },
            {
                SiteSettingKey.Users.AskPreregistrationReminder,
                new SiteSettingDefinition()
                {
                    Name = "Ask reminder email during preregistraion",
                    Info = "If this is set (i.e. not null) ask users on sign up during " +
                        "preregistration if they want a reminder email when the program starts",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Boolean
                }
            },
            {
                SiteSettingKey.Users.DefaultDailyPersonalGoal,
                new SiteSettingDefinition()
                {
                    Name = "Default daily personal goal",
                    Info = "If this is set to an integer ask users on sign up for a daily " +
                        "personal activity goal with this value being the default option. " +
                        "Requires the site to have dates set for ProgramStarst and ProgramEnds.",
                    Category = typeof(SiteSettingKey.Users).Name,
                    Format = SiteSettingFormat.Integer
                }
            }
        };
    }
}
