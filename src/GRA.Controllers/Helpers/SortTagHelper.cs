using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement(Attributes = "sort")]
    public class SortTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        [HtmlAttributeName("asp-route-Sort")]
        public string SortValue { get; set; }

        [HtmlAttributeName("sortColumn")]
        public bool SortColumn { get; set; }

        public SortTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory 
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContextData { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper url = _urlHelperFactory.GetUrlHelper(ViewContextData);
            string sortBy = url.ActionContext.HttpContext.Request.Query["Sort"].ToString();
            string ascDesc = url.ActionContext.HttpContext.Request.Query["Order"].ToString();

            if (SortColumn || string.Equals(sortBy, SortValue, StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(ascDesc, "Descending", StringComparison.OrdinalIgnoreCase))
                {
                    output.PostContent.AppendHtml("<span class='fas fa-lg fa-sort-down sort-down'></span>");
                }
                else
                {
                    output.PostContent.AppendHtml("<span class='fas fa-lg fa-sort-up sort-up'></span>");
                }
            }
            else
            {
                output.PostContent.AppendHtml("<span class='fas fa-lg fa-sort-up sort-hide'></span>");
            }
            output.Attributes.Remove(new TagHelperAttribute("sort"));
        }
    }
}
