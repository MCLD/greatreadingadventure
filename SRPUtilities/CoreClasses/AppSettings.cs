using System.ComponentModel;


namespace GRA.SRP.Core.Utilities
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
