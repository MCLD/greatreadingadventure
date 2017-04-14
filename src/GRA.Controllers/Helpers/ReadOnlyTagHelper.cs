using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
