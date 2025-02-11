using System;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement("paginate", Attributes = "paginateModel")]
    public class PaginateTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PaginateTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            ArgumentNullException.ThrowIfNull(urlHelperFactory);

            _urlHelperFactory = urlHelperFactory;
        }

        [HtmlAttributeName("asButtons")]
        public bool AsButtons { get; set; }

        [HtmlAttributeName("paginateModel")]
        public PaginateViewModel PaginateModel { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContextData { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(output);

            IUrlHelper url = _urlHelperFactory.GetUrlHelper(ViewContextData);

            if (PaginateModel.MaxPage <= 1)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "nav";
            output.Attributes.Add("class", "btn-group");
            output.Attributes.Add("role", "group");
            output.Attributes.Add("aria-label", "Pagination");

            var firstPage = PaginateModel.FirstPage == null
                               ? null
                               : QueryBuilder(url, PaginateModel.FirstPage, AsButtons);
            output.Content.AppendHtml(PaginatorTag(firstPage, "fast-backward", AsButtons));

            var previousPage = PaginateModel.PreviousPage == null
                                  ? null
                                  : QueryBuilder(url, PaginateModel.PreviousPage, AsButtons);
            output.Content.AppendHtml(PaginatorTag(previousPage, "backward", AsButtons));

            output.Content.AppendHtml(
                PaginatorText($"{PaginateModel.CurrentPage:n0} / {PaginateModel.MaxPage:n0}",
                    $"{PaginateModel.ItemCount:n0} total items"));

            var nextPage = PaginateModel.NextPage == null
                              ? null
                              : QueryBuilder(url, PaginateModel.NextPage, AsButtons);

            output.Content.AppendHtml(PaginatorTag(nextPage, "forward", AsButtons));

            var lastPage = PaginateModel.LastPage == null
                              ? null
                              : QueryBuilder(url, PaginateModel.LastPage, AsButtons);

            output.Content.AppendHtml(PaginatorTag(lastPage, "fast-forward", AsButtons));
        }

        private static TagBuilder PaginatorTag(string pageUrl, string glyph, bool asButtons)
        {
            var baseTag = asButtons ? new TagBuilder("button") : new TagBuilder("a");

            if (asButtons)
            {
                baseTag.MergeAttribute("class", "btn btn-outline-primary page-button");
                baseTag.MergeAttribute("type", "button");
                if (pageUrl == null)
                {
                    baseTag.MergeAttribute("class", "disabled");
                }
                else
                {
                    baseTag.MergeAttribute("data-page", pageUrl);
                }
            }
            else
            {
                baseTag.MergeAttribute("class", "btn btn-outline-primary");
                if (pageUrl == null)
                {
                    baseTag.MergeAttribute("href", "#");
                    baseTag.MergeAttribute("onclick", "return false;");
                }
                else
                {
                    baseTag.MergeAttribute("href", pageUrl);
                }
            }

            var spanTag = new TagBuilder("span")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            spanTag.MergeAttribute("class", $"fas fa-{glyph}");
            baseTag.InnerHtml.SetHtmlContent(spanTag);

            return baseTag;
        }

        private static TagBuilder PaginatorText(string text, string title)
        {
            var baseTag = new TagBuilder("button");
            baseTag.MergeAttribute("class", "btn btn-outline-primary");
            baseTag.MergeAttribute("type", "button");
            if (!string.IsNullOrEmpty(title))
            {
                baseTag.MergeAttribute("title", title);
            }
            baseTag.InnerHtml.SetContent(text);
            return baseTag;
        }

        private static string QueryBuilder(IUrlHelper url, int? page, bool asButtons)
        {
            if (asButtons)
            {
                return page.HasValue ? $"{page}" : null;
            }
            else
            {
                var routeValues = new RouteValueDictionary();
                foreach (var query in url.ActionContext.HttpContext.Request.Query)
                {
                    if (!string.Equals(query.Key, "page", StringComparison.OrdinalIgnoreCase))
                    {
                        routeValues.Add(query.Key, query.Value);
                    }
                }
                routeValues.Add("page", page);
                return url.RouteUrl(routeValues);
            }
        }
    }
}
