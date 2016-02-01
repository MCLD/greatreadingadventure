using System.ComponentModel;


namespace GRA.SRP.Core.Utilities
{
    public enum AppSettingKeys
    {
        [DescriptionAttribute("LogEmails")]
        LogEmails,
        [DescriptionAttribute("UseEmailTemplates")]
        UseEmailTemplates,
        [DescriptionAttribute("DefaultEmailTemplate")]
        DefaultEmailTemplate,
        [DescriptionAttribute("DefaultEmailFrom")]
        DefaultEmailFrom,
        [DescriptionAttribute("IgnoreMissingDatabaseGroups")]
        IgnoreMissingDatabaseGroups
    }
}
