using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
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
            switch (property.ToLower())
            {
                case "name":
                    output.TagName = "span";
                    output.Attributes.SetAttribute("class", "gra-sitename");
                    output.Content.SetHtmlContent(site.Name);
                    break;
                case "pagetitle":
                    output.TagName = "span";
                    output.Attributes.SetAttribute("class", "gra-pagetitle");
                    output.Content.SetHtmlContent(site.PageTitle);
                    break;
                case "footer":
                    output.TagName = "p";
                    output.Attributes.SetAttribute("class", "footer");
                    output.Content.SetHtmlContent(site.Footer);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
