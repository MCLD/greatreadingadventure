using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Shared
{
    public abstract class SchoolSelectionViewModel
    {
        [DisplayName("School")]
        public int? SchoolId { get; set; }
        public bool SchoolNotListed { get; set; }
        public bool IsHomeschooled { get; set; }

        public int? SchoolDistrictId { get; set; }
        public int? SchoolTypeId { get; set; }

        public bool ShowPrivateOption { get; set; }
        public bool ShowCharterOption { get; set; }
        public bool PublicSelected { get; set; }
        public bool PrivateSelected { get; set; }
        public bool CharterSelected { get; set; }
        public bool SetPublic { get; set; }
        public bool SetPrivate { get; set; }
        public bool SetCharter { get; set; }
        public bool SetHomeschool { get; set; }

        public string CategorySelectionAction { get; set; }

        public SelectList SchoolList { get; set; }
        public SelectList SchoolDistrictList { get; set; }
        public SelectList SchoolTypeList { get; set; }
    }
}
