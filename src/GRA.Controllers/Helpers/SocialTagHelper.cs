using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement("social", Attributes = "data")]
    public class SocialTagHelper : TagHelper
    {
        private const string SecureScheme = "https";

        private readonly IOptions<RequestLocalizationOptions> _localizationOptions;
        private readonly SiteLookupService _siteLookupService;
        private readonly IUserContextProvider _userContextProvider;

        public SocialTagHelper(
            IOptions<RequestLocalizationOptions> localizationOptions,
            SiteLookupService siteLookupService,
            IUserContextProvider userContextProvider)
        {
            _localizationOptions = localizationOptions
                ?? throw new ArgumentNullException(nameof(localizationOptions));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _userContextProvider = userContextProvider
                ?? throw new ArgumentNullException(nameof(userContextProvider));
        }

        [HtmlAttributeName("data")]
        public Social Data { get; set; }

        [HtmlAttributeName("description")]
        public string Description { get; set; }

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

            output.TagName = null;
            string description = null;

            if (!string.IsNullOrEmpty(Description))
            {
                description = Description.Contains("{sitename}",
                        StringComparison.OrdinalIgnoreCase)
                    ? Description.Replace("{sitename}", site.Name,
                        StringComparison.OrdinalIgnoreCase)
                    : Description;
                output.Content.AppendHtml(MetaName("description", description));
                output.Content.AppendHtml(Environment.NewLine);
            }
            else if (!string.IsNullOrEmpty(site.MetaDescription))
            {
                output.Content.AppendHtml(MetaName("description", site.MetaDescription));
                output.Content.AppendHtml(Environment.NewLine);
            }

            var title = !string.IsNullOrEmpty(Data?.Title)
                ? Data.Title
                : site.PageTitle;

            if (string.IsNullOrEmpty(description))
            {
                description = !string.IsNullOrEmpty(Data?.Title)
                    ? Data.Title
                    : site.PageTitle;
            }

            AddDublinCoreTags(output, site);

            if (!string.IsNullOrEmpty(Data?.ImageLink))
            {
                AddFacebookTags(output, site, title, description);
                AddTwitterTags(output);
            }
        }

        private static TagBuilder MetaName(string name, string content)
        {
            var metaTag = new TagBuilder("meta")
            {
                TagRenderMode = TagRenderMode.SelfClosing
            };
            metaTag.Attributes.Add("name", name);
            metaTag.Attributes.Add("content", content);
            return metaTag;
        }

        private static TagBuilder MetaProperty(string property, string content)
        {
            var metaTag = new TagBuilder("meta")
            {
                TagRenderMode = TagRenderMode.SelfClosing
            };
            metaTag.Attributes.Add("property", property);
            metaTag.Attributes.Add("content", content);
            return metaTag;
        }

        private void AddDublinCoreTags(TagHelperOutput output, Site site)
        {
            output.Content.AppendHtml(MetaName("DC.Title", site.Name));
            output.Content.AppendHtml(Environment.NewLine);
            if (!string.IsNullOrEmpty(site.MetaDescription))
            {
                output.Content.AppendHtml(MetaName("DC.Description", site.MetaDescription));
                output.Content.AppendHtml(Environment.NewLine);
            }
            output.Content.AppendHtml(MetaName("DC.Source", GetSiteLink(site, null)));
            output.Content.AppendHtml(Environment.NewLine);
            output.Content.AppendHtml(MetaName("DC.Type", "InteractiveResource"));
            output.Content.AppendHtml(Environment.NewLine);
        }

        private void AddFacebookTags(TagHelperOutput output,
            Site site,
            string title,
            string description)
        {
            var currentCulture = _userContextProvider.GetCurrentCulture();
            var ogLink = GetSiteLink(site, currentCulture);

            output.Content.AppendHtml(MetaProperty("og:url", ogLink));
            output.Content.AppendHtml(Environment.NewLine);

            output.Content.AppendHtml(MetaProperty("og:type", "website"));
            output.Content.AppendHtml(Environment.NewLine);

            output.Content.AppendHtml(MetaProperty("og:title", title));
            output.Content.AppendHtml(Environment.NewLine);

            if (!string.IsNullOrEmpty(description))
            {
                output.Content.AppendHtml(MetaProperty("og:description", description));
                output.Content.AppendHtml(Environment.NewLine);
            }

            if (currentCulture != null)
            {
                output.Content.AppendHtml(MetaProperty("og:locale",
                    currentCulture.Name.Replace('-', '_')));
                output.Content.AppendHtml(Environment.NewLine);

                if (_localizationOptions != null)
                {
                    foreach (var l10n in _localizationOptions.Value.SupportedUICultures)
                    {
                        if (l10n.Name != currentCulture.Name)
                        {
                            output.Content.AppendHtml(MetaProperty("og:locale:alternate",
                                l10n.Name.Replace('-', '_')));
                            output.Content.AppendHtml(Environment.NewLine);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(Data?.ImageLink) && !string.IsNullOrEmpty(Data?.ImageAlt))
            {
                var link = GetSiteLink(site, null);
                var cardLink = link.EndsWith('/')
                    ? link + Data.ImageLink.TrimStart('/')
                    : link + '/' + Data.ImageLink.TrimStart('/');

                output.Content.AppendHtml(MetaProperty("og:image", cardLink));
                output.Content.AppendHtml(Environment.NewLine);
                if (cardLink.StartsWith(SecureScheme))
                {
                    output.Content.AppendHtml(MetaProperty("og:image:secure_url", cardLink));
                    output.Content.AppendHtml(Environment.NewLine);
                }
                output.Content.AppendHtml(MetaProperty("og:image:alt", Data.ImageAlt));
                output.Content.AppendHtml(Environment.NewLine);
                if (Data.ImageWidth != default && Data.ImageHeight != default)
                {
                    output.Content.AppendHtml(MetaProperty("og:image:width",
                        Data.ImageWidth.ToString(CultureInfo.InvariantCulture)));
                    output.Content.AppendHtml(Environment.NewLine);
                    output.Content.AppendHtml(MetaProperty("og:image:height",
                        Data.ImageHeight.ToString(CultureInfo.InvariantCulture)));
                    output.Content.AppendHtml(Environment.NewLine);
                }
            }
        }

        private void AddTwitterTags(TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(Data?.TwitterUsername))
            {
                output.Content.AppendHtml(MetaName("twitter:site", Data.TwitterUsername));
                output.Content.AppendHtml(Environment.NewLine);
            }

            if (!string.IsNullOrEmpty(Data?.ImageLink))
            {
                output.Content.AppendHtml(MetaName("twitter:card", "summary_large_image"));
                output.Content.AppendHtml(Environment.NewLine);
            }
        }

        private string GetSiteLink(Site site, CultureInfo linkForCulture)
        {
            string scheme = site.IsHttpsForced
                ? SecureScheme
                : ViewContext.HttpContext.Request.Scheme;

            var link = new StringBuilder(scheme)
                .Append("://")
                .Append(ViewContext.HttpContext.Request.Host.Value)
                .Append('/');

            if (linkForCulture != null)
            {
                var defaultUiCulture = _localizationOptions
                    .Value
                    .DefaultRequestCulture
                    .UICulture;

                if (linkForCulture.Name != defaultUiCulture.Name)
                {
                    link.Append(linkForCulture.Name).Append('/');
                }
            }

            if (!site.IsDefault)
            {
                link.Append(site.Path).Append('/');
            }

            return link.ToString();
        }
    }
}