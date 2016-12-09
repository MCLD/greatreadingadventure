using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement(Attributes = "sort")]
    public class SortTagHelper : TagHelper
    {
        private IUrlHelperFactory _urlHelperFactory;

        [HtmlAttributeName("asp-route-Sort")]
        public string sortValue { get; set; }

        public SortTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = Require.IsNotNull(urlHelperFactory, nameof(urlHelperFactory));
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContextData { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper url = _urlHelperFactory.GetUrlHelper(ViewContextData);
            string sortBy = url.ActionContext.HttpContext.Request.Query["Sort"].ToString();
            string ascDesc = url.ActionContext.HttpContext.Request.Query["Order"].ToString();

            if (String.Equals(sortBy, sortValue, StringComparison.OrdinalIgnoreCase))
            {
                if (String.Equals(ascDesc, "Descending", StringComparison.OrdinalIgnoreCase))
                {
                    output.PostContent.AppendHtml("<span class='fa fa-lg fa-sort-desc sort-desc'></span>");
                }
                else
                {
                    output.PostContent.AppendHtml("<span class='fa fa-lg fa-sort-asc sort-asc'></span>");
                }
            }
            else
            {
                output.PostContent.AppendHtml("<span class='fa fa-lg fa-sort-asc sort-hide'></span>");
            }
            output.Attributes.Remove(new TagHelperAttribute("sort"));
        }
    }
}
