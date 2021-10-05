using System;
using System.Globalization;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace GRA.Controllers.Helper
{
    [HtmlTargetElement("paginate", Attributes = "paginateModel")]
    public class PaginateTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PaginateTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));
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
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            IUrlHelper url = _urlHelperFactory.GetUrlHelper(ViewContextData);
            var ulTag = new TagBuilder("ul")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            ulTag.MergeAttribute("class", "pagination");

            var firstPage = PaginateModel.FirstPage == null
                               ? null
                               : QueryBuilder(url, PaginateModel.FirstPage, AsButtons);
            ulTag.InnerHtml.AppendHtml(PaginatorLi(firstPage, "fast-backward", AsButtons));

            var previousPage = PaginateModel.PreviousPage == null
                                  ? null
                                  : QueryBuilder(url, PaginateModel.PreviousPage, AsButtons);
            ulTag.InnerHtml.AppendHtml(PaginatorLi(previousPage, "backward", AsButtons));

            ulTag.InnerHtml.AppendHtml(PaginatorLi(PaginateModel
                .CurrentPage
                .ToString(CultureInfo.InvariantCulture), AsButtons));

            var nextPage = PaginateModel.NextPage == null
                              ? null
                              : QueryBuilder(url, PaginateModel.NextPage, AsButtons);

            ulTag.InnerHtml.AppendHtml(PaginatorLi(nextPage, "forward", AsButtons));

            var lastPage = PaginateModel.LastPage == null
                              ? null
                              : QueryBuilder(url, PaginateModel.LastPage, AsButtons);

            ulTag.InnerHtml.AppendHtml(PaginatorLi(lastPage, "fast-forward", AsButtons));

            var navTag = new TagBuilder("nav")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            navTag.InnerHtml.SetHtmlContent(ulTag);
            output.Content.SetHtmlContent(navTag);
        }

        private static TagBuilder PaginatorLi(string text, bool asButtons)
        {
            var liTag = new TagBuilder("li")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            liTag.MergeAttribute("class", "disabled");

            if (asButtons)
            {
                var buttonTag = new TagBuilder("button")
                {
                    TagRenderMode = TagRenderMode.Normal
                };
                buttonTag.InnerHtml.SetHtmlContent(text);
                buttonTag.MergeAttribute("class", "page-button disabled");
                buttonTag.MergeAttribute("type", "button");
                liTag.InnerHtml.SetHtmlContent(buttonTag);
            }
            else
            {
                var aTag = new TagBuilder("a");
                aTag.MergeAttribute("href", "#");
                aTag.MergeAttribute("onclick", "return false;");
                aTag.InnerHtml.SetHtmlContent(text);
                aTag.TagRenderMode = TagRenderMode.Normal;
                liTag.InnerHtml.SetHtmlContent(aTag);
            }

            return liTag;
        }

        private static TagBuilder PaginatorLi(string pageUrl, string glyph, bool asButtons)
        {
            var liTag = new TagBuilder("li")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            var spanTag = new TagBuilder("span")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            spanTag.MergeAttribute("class", $"fas fa-{glyph}");
            if (asButtons)
            {
                var buttonTag = new TagBuilder("button")
                {
                    TagRenderMode = TagRenderMode.Normal
                };
                buttonTag.MergeAttribute("class", "page-button");
                buttonTag.MergeAttribute("type", "button");
                if (pageUrl == null)
                {
                    buttonTag.AddCssClass("disabled");
                    liTag.MergeAttribute("class", "disabled");
                }
                else
                {
                    buttonTag.MergeAttribute("data-page", pageUrl);
                }
                buttonTag.InnerHtml.SetHtmlContent(spanTag);
                liTag.InnerHtml.SetHtmlContent(buttonTag);
            }
            else
            {
                var aTag = new TagBuilder("a")
                {
                    TagRenderMode = TagRenderMode.Normal
                };
                if (pageUrl == null)
                {
                    liTag.MergeAttribute("class", "disabled");
                    aTag.MergeAttribute("href", "#");
                    aTag.MergeAttribute("onclick", "return false;");
                }
                else
                {
                    aTag.MergeAttribute("href", pageUrl);
                }

                aTag.InnerHtml.SetHtmlContent(spanTag);
                liTag.InnerHtml.SetHtmlContent(aTag);
            }
            return liTag;
        }

        private static string QueryBuilder(IUrlHelper url, int? page, bool asButtons)
        {
            if (asButtons)
            {
                return page.HasValue ? page.ToString() : null;
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