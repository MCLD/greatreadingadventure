using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement(Attributes = "asp-readonly")]
    public class ReadOnlyTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-readonly")]
        public bool readOnly { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (readOnly)
            {
                output.Attributes.Add("readonly", "readonly");
            }
        }
    }
}
