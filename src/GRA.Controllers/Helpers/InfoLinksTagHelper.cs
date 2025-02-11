using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement("infolinks", Attributes = "navPages")]
    public class InfoLinksTagHelper : TagHelper
    {
        private readonly PageService _pageService;
        private readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;
        private readonly SiteLookupService _siteLookupService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly UserService _userService;

        public InfoLinksTagHelper(IStringLocalizer<Resources.Shared> sharedLocalizer,
            IUrlHelperFactory urlHelperFactory,
            PageService pageService,
            SiteLookupService siteLookupService,
            UserService userService)
        {
            ArgumentNullException.ThrowIfNull(pageService);
            ArgumentNullException.ThrowIfNull(sharedLocalizer);
            ArgumentNullException.ThrowIfNull(siteLookupService);
            ArgumentNullException.ThrowIfNull(urlHelperFactory);
            ArgumentNullException.ThrowIfNull(userService);

            _pageService = pageService;
            _sharedLocalizer = sharedLocalizer;
            _siteLookupService = siteLookupService;
            _urlHelperFactory = urlHelperFactory;
            _userService = userService;
        }

        /// <summary>
        /// If this attribute is true, render the items in list elements for the navbar at the top,
        /// otherwise they are links at the bottom and should be pipe-delimited links
        /// </summary>
        [HtmlAttributeName("navPages")]
        public bool NavPages { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            var pages = await _pageService.GetAreaPagesAsync(NavPages);

            if (!NavPages)
            {
                output.AddClass("infolinks", HtmlEncoder.Default);
                output.TagName = "div";
            }
            else
            {
                output.TagName = "";
            }

            if (pages.Any())
            {
                var divTag = new TagBuilder("div");
                divTag.AddCssClass("info-page-links");

                IUrlHelper url = _urlHelperFactory.GetUrlHelper(ViewContext);
                string activeStub = url.ActionContext.RouteData.Values["id"] as string;
                var first = true;

                foreach (var page in pages)
                {
                    var link = url.Action(nameof(InfoController.Index),
                        InfoController.Name,
                        new { id = page.PageStub });

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        divTag.InnerHtml.Append(NavPages ? " " : " | ");
                    }

                    TagBuilder outputTag;

                    var aTag = new TagBuilder("a");
                    aTag.Attributes.Add("href", link);
                    if (NavPages)
                    {
                        aTag.AddCssClass("nav-link");
                    }
                    aTag.InnerHtml.AppendHtml(NavPages ? page.NavText : page.FooterText);
                    if (NavPages)
                    {
                        outputTag = new TagBuilder("li");
                        outputTag.AddCssClass("nav-item");
                        outputTag.InnerHtml.AppendHtml(aTag);
                    }
                    else
                    {
                        outputTag = aTag;
                    }

                    if (page.PageStub == activeStub)
                    {
                        outputTag.AddCssClass("active");
                    }

                    divTag.InnerHtml.AppendHtml(outputTag);
                }

                output.Content.AppendHtml(divTag);
            }

            var siteId = (int)ViewContext
                .HttpContext
                .Items[ItemKey.SiteId];

            var userClaim = ViewContext
                .HttpContext
                .User
                .Claims
                .SingleOrDefault(_ => _.Type == ClaimType.UserId);

            if (!NavPages && siteId != 0)
            {
                bool linkToLibrary = await _siteLookupService.GetSiteSettingBoolAsync(siteId,
                        SiteSettingKey.Users.ShowLinkToParticipantsLibrary);

                bool showParticipatingLibraries = await _siteLookupService
                    .GetSiteSettingBoolAsync(siteId,
                        SiteSettingKey.Users.ShowLinkToParticipatingLibraries);

                if (linkToLibrary || showParticipatingLibraries)
                {
                    var divTag = new TagBuilder("div");
                    divTag.AddCssClass("locations-page-link");

                    if (showParticipatingLibraries)
                    {
                        var allBranchesLink = new TagBuilder("a");
                        allBranchesLink.InnerHtml.AppendHtml(_sharedLocalizer[Annotations
                            .Interface
                            .AllParticipatingLibraries]);
                        IUrlHelper url = _urlHelperFactory.GetUrlHelper(ViewContext);
                        allBranchesLink.MergeAttribute("href",
                            url.Action(nameof(ParticipatingLibrariesController.Index),
                                ParticipatingLibrariesController.Name));
                        divTag.InnerHtml.AppendHtml(allBranchesLink);
                    }

                    if (linkToLibrary
                        && userClaim != null
                        && int.TryParse(userClaim.Value, out int userId))
                    {
                        var branch = await _userService.GetUsersBranch(userId);
                        if (!string.IsNullOrEmpty(branch.Url))
                        {
                            var branchLink = new TagBuilder("a");
                            branchLink
                                .InnerHtml
                                .AppendHtml(_sharedLocalizer[Annotations.Interface.Explore,
                                    branch.Name]);
                            branchLink.MergeAttribute("href", branch.Url);

                            if (showParticipatingLibraries)
                            {
                                divTag.InnerHtml.AppendHtml(" | ");
                            }

                            divTag.InnerHtml.AppendHtml(branchLink);
                        }
                    }

                    output.Content.AppendHtml(divTag);
                }
            }
        }
    }
}
