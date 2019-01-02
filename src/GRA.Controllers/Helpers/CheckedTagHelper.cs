using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement(Attributes = "asp-checked")]
    public class CheckedTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-checked")]
        public bool IsChecked { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsChecked)
            {
                output.Attributes.Add("checked", "checked");
            }
        }
    }
}
