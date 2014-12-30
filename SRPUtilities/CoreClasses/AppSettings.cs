using System.ComponentModel;


namespace STG.SRP.Core.Utilities
{
    public enum AppSettings
    {
        [DescriptionAttribute("LogEmails")]
        LogEmails,
        [DescriptionAttribute("UseEmailTemplates")]
        UseEmailTemplates,
        [DescriptionAttribute("DefaultEmailTemplate")]
        DefaultEmailTemplate,
        [DescriptionAttribute("DefaultEmailFrom")]
        DefaultEmailFrom
    }
}
