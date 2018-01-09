using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement("button")]
    public class IsDisabledTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(IsDisabled)
            {
                var disabledAttribute = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(disabledAttribute);
            }
            base.Process(context, output);
        }
    }
}
