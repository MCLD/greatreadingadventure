using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GRA.Controllers.ViewModel.MissionControl.Social
{
    public class SocialItemViewModel
    {
        [Required]
        [MaxLength(200)]
        [Display(Name = "OpenGraph/Twitter Card description",
            Description = "Requird, maximum 200 characters")]
        public string Description { get; set; }

        public string Filename { get; set; }
        public int HeaderId { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Description of the image content",
            Description = "Required, for accessibility, maximum 255 characters")]
        public string ImageAlt { get; set; }

        public string ImageDimensions { get; set; }
        public string ImageLink { get; set; }

        [Display(Name = "Language for this social record",
                            Description = "Required, once created you may add additional languages")]
        public int LanguageId { get; set; }

        public SelectList LanguageList { get; set; }

        public string LanguageName { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Administrative name",
            Description = "Required, this is only shown in Mission Control")]
        public string Name { get; set; }

        public bool SocialIsNew { get; set; }

        [Required]
        [Display(Name = "Start date",
            Description = "Required")]
        public DateTime StartDate { get; set; }

        [Required]
        [MaxLength(70)]
        [Display(Name = "OpenGraph/Twitter Card title",
            Description = "Required, maximum 70 characters")]
        public string Title { get; set; }

        [MaxLength(255)]
        [Display(Name = "Associated Twitter username", Description = "Optional")]
        public string TwitterUsername { get; set; }

        [Required]
        [Display(Name = "Select an image file",
            Description = "Required, compressed for the Web, 1200x630 pixels or similar")]
        public IFormFile UploadedImage { get; set; }

        public static string ValidationClass(ViewDataDictionary<SocialItemViewModel> viewData,
            string field)
        {
            return viewData?.ModelState.GetValidationState(field) == ModelValidationState.Invalid
                ? "input-validation-error"
                : "";
        }
    }
}