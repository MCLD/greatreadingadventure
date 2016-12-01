using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
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
    [HtmlTargetElement("grasite", Attributes = "property")]
    public class SiteTagHelper : TagHelper
    {
        private readonly SiteService _siteService;
        private readonly IUrlHelperFactory _urlHelperFactory;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("property")]
        public string property { get; set; }
 
        public SiteTagHelper(IUrlHelperFactory urlHelperFactory,
            SiteService siteService)
        {
            _urlHelperFactory = Require.IsNotNull(urlHelperFactory, nameof(urlHelperFactory));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
        }

        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper url = _urlHelperFactory.GetUrlHelper(ViewContext);
            var routeData = url.ActionContext.RouteData.Values;
            string sitePath = routeData["sitePath"] as string;
            int siteId = await new SiteHelper(_siteService)
                .GetSiteId(ViewContext.HttpContext, sitePath);
            Site site = await _siteService.GetById((int)siteId);
            TagBuilder result;
            switch(property)
            {
                case "name":
                    result = new TagBuilder("span");
                    result.InnerHtml.Append(site.Name);
                    break;
                case "footer":
                    result = new TagBuilder("p");
                    result.AddCssClass("footer");
                    result.InnerHtml.AppendHtml(site.Footer);
                    break;
                default:
                    throw new NotImplementedException();
            }

            output.Content.SetHtmlContent(result);
        }
    }
}
