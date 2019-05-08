using System.ComponentModel;
using GRA.Domain.Model; 
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Shared
{
    public abstract class SchoolSelectionViewModel
    {
        [DisplayName(DisplayNames.School)]
        public int? SchoolId { get; set; }

        public SelectList SchoolList { get; set; }

        public bool IsHomeschooled { get; set; }
        public bool SchoolNotListed { get; set; }
    }
}
