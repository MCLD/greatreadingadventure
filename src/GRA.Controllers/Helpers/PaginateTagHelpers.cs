using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GRA.Controllers.Helper
{
    [HtmlTargetElement("paginate", Attributes = "paginateModel")]
    public class PaginateTagHelper : TagHelper
    {
        private IUrlHelperFactory _urlHelperFactory;

        public PaginateTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = Require.IsNotNull(urlHelperFactory, nameof(urlHelperFactory));
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContextData { get; set; }

        [HtmlAttributeName("paginateModel")]
        public PaginateViewModel paginateModel { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper url = _urlHelperFactory.GetUrlHelper(ViewContextData);
            TagBuilder ulTag = new TagBuilder("ul");
            ulTag.TagRenderMode = TagRenderMode.Normal;
            ulTag.MergeAttribute("class", "pagination");

            string firstPage = paginateModel.FirstPage == null
                               ? null
                               : QueryBuilder(url, paginateModel.FirstPage);
            ulTag.InnerHtml.AppendHtml(PaginatorLi(firstPage, "fast-backward"));

            string previousPage = paginateModel.PreviousPage == null
                                  ? null
                                  : QueryBuilder(url, paginateModel.PreviousPage);
            ulTag.InnerHtml.AppendHtml(PaginatorLi(previousPage, "backward"));

            ulTag.InnerHtml.AppendHtml(PaginatorLi(paginateModel.CurrentPage.ToString()));

            string nextPage = paginateModel.NextPage == null
                              ? null
                              : QueryBuilder(url, paginateModel.NextPage);

            ulTag.InnerHtml.AppendHtml(PaginatorLi(nextPage, "forward"));

            string lastPage = paginateModel.LastPage == null
                              ? null
                              : QueryBuilder(url, paginateModel.LastPage);

            ulTag.InnerHtml.AppendHtml(PaginatorLi(lastPage, "fast-forward"));

            TagBuilder navTag = new TagBuilder("nav");
            navTag.TagRenderMode = TagRenderMode.Normal;
            navTag.InnerHtml.SetHtmlContent(ulTag);
            output.Content.SetHtmlContent(navTag);
        }

        private static TagBuilder PaginatorLi(string text)
        {
            TagBuilder aTag = new TagBuilder("a");
            aTag.MergeAttribute("href", "#");
            aTag.MergeAttribute("onclick", "return false;");
            aTag.InnerHtml.SetHtmlContent(text);
            aTag.TagRenderMode = TagRenderMode.Normal;

            TagBuilder liTag = new TagBuilder("li");
            liTag.TagRenderMode = TagRenderMode.Normal;
            liTag.MergeAttribute("class", "disabled");
            liTag.InnerHtml.SetHtmlContent(aTag);

            return liTag;
        }

        private static TagBuilder PaginatorLi(string pageUrl, string glyph)
        {
            TagBuilder liTag = new TagBuilder("li");
            liTag.TagRenderMode = TagRenderMode.Normal;
            TagBuilder aTag = new TagBuilder("a");
            aTag.TagRenderMode = TagRenderMode.Normal;
            TagBuilder spanTag = new TagBuilder("span");
            spanTag.TagRenderMode = TagRenderMode.Normal;
            spanTag.MergeAttribute("class", string.Format("fa fa-{0}", glyph));
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
            return liTag;
        }

        private static string QueryBuilder(IUrlHelper url, int? page)
        {
            var routeValues = new RouteValueDictionary();
            foreach (var query in url.ActionContext.HttpContext.Request.Query)
            {
                if (!(String.Equals(query.Key, "page", StringComparison.OrdinalIgnoreCase)))
                {
                    routeValues.Add(query.Key, query.Value);
                }
            }
            routeValues.Add("page", page);
            return url.RouteUrl(routeValues);
        }
    }
}
