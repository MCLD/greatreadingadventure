using System;
using System.Threading.Tasks;
using CommonMark;
using GRA.Domain.Model;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement("grasite", Attributes = "property")]
    public class SiteTagHelper : TagHelper
    {
        private readonly SiteLookupService _siteLookupService;
        private readonly IUserContextProvider _userContextProvider;

        public SiteTagHelper(IUserContextProvider userContextProvider,
            SiteLookupService siteLookupService)
        {
            _userContextProvider = userContextProvider
                ?? throw new ArgumentNullException(nameof(userContextProvider));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
        }

        [HtmlAttributeName("property")]
        public string property { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null || output == null)
            {
                output?.SuppressOutput();
                return;
            }

            var userContext = _userContextProvider.GetContext();
            int siteId = userContext.SiteId;
            Site site = await _siteLookupService.GetByIdAsync(siteId);
            switch (property.ToUpperInvariant())
            {
                case "NAME":
                    output.TagName = "span";
                    output.Attributes.SetAttribute("class", "gra-sitename");
                    output.Content.SetHtmlContent(site.Name);
                    break;

                case "PAGETITLE":
                    output.TagName = "span";
                    output.Attributes.SetAttribute("class", "gra-pagetitle");
                    output.Content.SetHtmlContent(site.PageTitle);
                    break;

                case "FOOTER":
                    output.TagName = string.Empty;
                    output.Content.AppendHtml(CommonMarkConverter.Convert(site.Footer));
                    break;

                default:
                    output.SuppressOutput();
                    break;
            }
        }
    }
}