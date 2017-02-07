using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step2ViewModel
    {
        [Required]
        [DisplayName("Program")]
        [Range(0, int.MaxValue, ErrorMessage = "The Branch field is required.")]
        public int? ProgramId { get; set; }

        public int? Age { get; set; }
        [DisplayName("School")]
        public int? SchoolId { get; set; }
        [DisplayName("School Name")]
        public string EnteredSchoolName { get; set; }

        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public bool NewEnteredSchool { get; set; }
        public int? SchoolDistrictId { get; set; }
        public int? SchoolTypeId { get; set; }
        public string ProgramJson { get; set; }

        public SelectList ProgramList { get; set; }
        public SelectList SchoolList { get; set; }
        public SelectList SchoolDistrictList { get; set; }
        public SelectList SchoolTypeList { get; set; }
    }
}
