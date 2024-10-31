using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helper
{
    [HtmlTargetElement(Attributes = "ActiveBy, routeKey, value")]
    public class ActiveByTagHelper : TagHelper
    {
        private const string ActiveClass = "active";
        private const string ClassAttribute = "class";
        private readonly IUrlHelperFactory _urlHelperFactory;

        public ActiveByTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            ArgumentNullException.ThrowIfNull(urlHelperFactory);

            _urlHelperFactory = urlHelperFactory;
        }

        [HtmlAttributeName("appendClass")]
        public string AppendClass { get; set; }

        [HtmlAttributeName("inactiveClass")]
        public string InactiveClass { get; set; }

        [HtmlAttributeName("routeKey")]
        public string RouteKey { get; set; }

        [HtmlAttributeName("value")]
        public string Value { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContextData { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            var url = _urlHelperFactory.GetUrlHelper(ViewContextData);
            var routeData = url.ActionContext.RouteData.Values;
            var routeValue = routeData[RouteKey] as string
                ?? url.ActionContext.HttpContext.Request.Query[RouteKey].ToString();

            string addClass = null;

            if (Value.Split(',').Contains(routeValue))
            {
                addClass = string.IsNullOrEmpty(AppendClass) ? ActiveClass : AppendClass;
            }
            else if (!string.IsNullOrEmpty(InactiveClass))
            {
                addClass = InactiveClass;
            }

            if (!string.IsNullOrEmpty(addClass))
            {
                var cssClass = new StringBuilder();
                var existingClass = output.Attributes.FirstOrDefault(_ => _.Name == ClassAttribute);
                if (existingClass != null)
                {
                    cssClass.Append(existingClass.Value.ToString());
                    output.Attributes.Remove(existingClass);
                }
                cssClass.Append(' ').Append(addClass);
                output.Attributes.Add(new TagHelperAttribute(ClassAttribute, cssClass.ToString()));
            }

            output.Attributes.Remove(new TagHelperAttribute("ActiveBy"));
        }
    }
}
