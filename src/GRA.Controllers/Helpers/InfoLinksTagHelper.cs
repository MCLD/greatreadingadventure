﻿using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
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
            _urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));
            _sharedLocalizer = sharedLocalizer
                ?? throw new ArgumentNullException(nameof(sharedLocalizer));
            _pageService = pageService
                ?? throw new ArgumentNullException(nameof(pageService));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
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
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var pages = await _pageService.GetAreaPagesAsync(NavPages);
            if (pages.Any())
            {
                IUrlHelper url = _urlHelperFactory.GetUrlHelper(ViewContext);
                string activeStub = url.ActionContext.RouteData.Values["id"] as string;
                var first = true;

                if (!NavPages)
                {
                    output.Attributes.Add("class", "infolinks");
                    output.TagName = "div";
                }
                else
                {
                    output.TagName = "";
                }

                foreach (var page in pages)
                {
                    var link = url.Action("Index", "Info", new { id = page.PageStub });

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        output.Content.Append(NavPages ? " " : " | ");
                    }

                    TagBuilder outputTag;

                    var aTag = new TagBuilder("a");
                    aTag.Attributes.Add("href", link);
                    aTag.InnerHtml.AppendHtml(NavPages ? page.NavText : page.FooterText);
                    if (NavPages)
                    {
                        outputTag = new TagBuilder("li");
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

                    output.Content.AppendHtml(outputTag);
                }
            }
            else
            {
                output.TagName = "";
            }

            var siteClaim = ViewContext
                .HttpContext
                .User
                .Claims
                .SingleOrDefault(_ => _.Type == ClaimType.SiteId);

            var userClaim = ViewContext
                .HttpContext
                .User
                .Claims
                .SingleOrDefault(_ => _.Type == ClaimType.UserId);

            if (!NavPages
                && siteClaim != null
                && userClaim != null
                && int.TryParse(siteClaim.Value, out int siteId)
                && int.TryParse(userClaim.Value, out int userId)
                && await _siteLookupService.GetSiteSettingBoolAsync(siteId,
                    SiteSettingKey.Users.ShowLinkToParticipantsLibrary))
            {
                var branch = await _userService.GetUsersBranch(userId);
                if (!string.IsNullOrEmpty(branch.Url))
                {
                    var branchLink = new TagBuilder("a");
                    branchLink.InnerHtml.AppendHtml(_sharedLocalizer[Annotations.Interface.Explore,
                        branch.Name]);
                    branchLink.MergeAttribute("href", branch.Url);
                    var divTag = new TagBuilder("div");
                    divTag.AddCssClass("locations");
                    divTag.InnerHtml.AppendHtml(branchLink);
                    output.Content.AppendHtml(divTag);
                }
            }
        }
    }
}
