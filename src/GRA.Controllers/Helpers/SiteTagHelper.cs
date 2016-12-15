using GRA.Domain.Model;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
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
        private readonly SiteLookupService _siteLookupService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUserContextProvider _userContextProvider;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("property")]
        public string property { get; set; }

        public SiteTagHelper(IUrlHelperFactory urlHelperFactory,
            IUserContextProvider userContextProvider,
            SiteLookupService siteLookupService)
        {
            _urlHelperFactory = Require.IsNotNull(urlHelperFactory, nameof(urlHelperFactory));
            _userContextProvider = Require.IsNotNull(userContextProvider,
                nameof(userContextProvider));
            _siteLookupService = Require.IsNotNull(siteLookupService, nameof(siteLookupService));
        }

        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper url = _urlHelperFactory.GetUrlHelper(ViewContext);
            var routeData = url.ActionContext.RouteData.Values;
            var userContext = _userContextProvider.GetContext();
            int siteId = userContext.SiteId;
            Site site = await _siteLookupService.GetByIdAsync((int)siteId);
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
