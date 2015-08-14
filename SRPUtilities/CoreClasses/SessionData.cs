using System.ComponentModel;

namespace GRA.SRP.Core.Utilities
{
    public enum SessionData : short
    {
        [DescriptionAttribute("IsLoggedIn")]
        IsLoggedIn,
        [DescriptionAttribute("UserProfile")]
        UserProfile,
        [DescriptionAttribute("PermissionList")]
        PermissionList,
        [DescriptionAttribute("FoldersList")]
        FoldersList,
        [DescriptionAttribute("StringPermissionList")]
        StringPermissionList,
        [DescriptionAttribute("StringFoldersList")]
        StringFoldersList,
        [DescriptionAttribute("LastException")]
        LastException

    }
}
