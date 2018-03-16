using CommonMark;
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
                    output.Content.SetHtmlContent(CommonMarkConverter.Convert(site.Footer));
                    break;
                case "metadescription":
                    output.TagName = string.Empty;
                    if (!string.IsNullOrEmpty(site.MetaDescription))
                    {
                        output.Content.AppendHtml(MetaName("description", site.MetaDescription));
                        output.Content.AppendHtml(Environment.NewLine);
                    }
                    break;
                case "dcmetadata":
                    output.TagName = string.Empty;
                    if (!string.IsNullOrEmpty(site.MetaDescription))
                    {
                        AddDcMetadata(output, site);
                    }
                    break;
                case "twittermetadata":
                    output.TagName = string.Empty;
                    if (site.TwitterLargeCard != null
                        && !string.IsNullOrEmpty(site.MetaDescription))
                    {
                        AddTwitterMetadata(output, site);
                    }
                    break;
                case "facebookmetadata":
                    output.TagName = string.Empty;
                    if (!string.IsNullOrEmpty(site.MetaDescription)
                        && !string.IsNullOrEmpty(site.FacebookAppId))
                    {
                        AddFacebookMetadata(output, site);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        private string GetSiteUrl(Site site)
        {
            string scheme = ViewContext.HttpContext.Request.Scheme;
            string host = ViewContext.HttpContext.Request.Host.Value;
            if(site.IsHttpsForced)
            {
                scheme = "https";
            }
            if (site.IsDefault)
            {
                return $"{scheme}://{host}";
            }
            else
            {
                return $"{scheme}://{host}/{site.Path}";
            }
        }
        private TagBuilder MetaName(string name, string content)
        {
            var metaTag = new TagBuilder("meta");
            metaTag.TagRenderMode = TagRenderMode.SelfClosing;
            metaTag.Attributes.Add("name", name);
            metaTag.Attributes.Add("content", content);
            return metaTag;
        }
    
        private TagBuilder MetaProperty(string property, string content)
        {
            var metaTag = new TagBuilder("meta");
            metaTag.TagRenderMode = TagRenderMode.SelfClosing;
            metaTag.Attributes.Add("property", property);
            metaTag.Attributes.Add("content", content);
            return metaTag;
        }

        private void AddDcMetadata(TagHelperOutput output, Site site)
        {
            output.Content.AppendHtml(MetaName("DC.Title", site.Name));
            output.Content.AppendHtml(Environment.NewLine);
            output.Content.AppendHtml(MetaName("DC.Description", site.MetaDescription));
            output.Content.AppendHtml(Environment.NewLine);
            output.Content.AppendHtml(MetaName("DC.Source", GetSiteUrl(site)));
            output.Content.AppendHtml(Environment.NewLine);
            output.Content.AppendHtml(MetaName("DC.Type", "InteractiveResource"));
            output.Content.AppendHtml(Environment.NewLine);
        }

        private void AddTwitterMetadata(TagHelperOutput output, Site site)
        {
            if (site.TwitterLargeCard == true)
            {
                output.Content.AppendHtml(MetaName("twitter:card", "summary_large_image"));
            }
            else
            {
                output.Content.AppendHtml(MetaName("twitter:card", "summary"));
            }
            output.Content.AppendHtml(Environment.NewLine);
            output.Content.AppendHtml(MetaName("twitter:title", site.Name));
            output.Content.AppendHtml(Environment.NewLine);
            output.Content.AppendHtml(MetaName("twitter:description", site.MetaDescription));
            output.Content.AppendHtml(Environment.NewLine);
            if (!string.IsNullOrEmpty(site.TwitterUsername))
            {
                output.Content.AppendHtml(MetaName("twitter:site", site.TwitterUsername));
                output.Content.AppendHtml(Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(site.TwitterCardImageUrl))
            {
                output.Content.AppendHtml(MetaName("twitter:image", site.TwitterCardImageUrl));
                output.Content.AppendHtml(Environment.NewLine);
            }
        }

        private void AddFacebookMetadata(TagHelperOutput output, Site site)
        {
            output.Content.AppendHtml(MetaProperty("fb:app_id", site.FacebookAppId));
            output.Content.AppendHtml(Environment.NewLine);
            output.Content.AppendHtml(MetaProperty("og:title", site.Name));
            output.Content.AppendHtml(Environment.NewLine);
            output.Content.AppendHtml(MetaProperty("og:type", "website"));
            output.Content.AppendHtml(Environment.NewLine);
            output.Content.AppendHtml(MetaProperty("og:description", site.MetaDescription));
            output.Content.AppendHtml(Environment.NewLine);
            output.Content.AppendHtml(MetaProperty("og:url", GetSiteUrl(site)));
            output.Content.AppendHtml(Environment.NewLine);
            if (!string.IsNullOrEmpty(site.FacebookImageUrl))
            {
                output.Content.AppendHtml(MetaProperty("og:image", site.FacebookImageUrl));
                output.Content.AppendHtml(Environment.NewLine);
            }
        }
    }
}
