using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement("return", Attributes = ActionAttributeName)]
    [HtmlTargetElement("return", Attributes = ControllerAttributeName)]
    [HtmlTargetElement("return", Attributes = AreaAttributeName)]
    [HtmlTargetElement("return", Attributes = FragmentAttributeName)]
    [HtmlTargetElement("return", Attributes = HostAttributeName)]
    [HtmlTargetElement("return", Attributes = ProtocolAttributeName)]
    [HtmlTargetElement("return", Attributes = RouteAttributeName)]
    [HtmlTargetElement("return", Attributes = RouteValuesDictionaryName)]
    [HtmlTargetElement("return", Attributes = RouteValuesPrefix + "*")]
    public class ReturnTagHelper : AnchorTagHelper
    {
        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string AreaAttributeName = "asp-area";
        private const string FragmentAttributeName = "asp-fragment";
        private const string HostAttributeName = "asp-host";
        private const string ProtocolAttributeName = "asp-protocol";
        private const string RouteAttributeName = "asp-route";
        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContextData { get; set; }

        public ReturnTagHelper(IHtmlGenerator generator) : base(generator)
        {
            
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var session = ViewContextData.HttpContext.Session;
            var page = session.GetInt32(SessionKey.ChallengePage);
            var search = session.GetString(SessionKey.ChallengeSearch);
            if (!string.IsNullOrWhiteSpace(search))
            {
                base.RouteValues.Add("Search", search);
            }
            if (page > 1)
            {
                base.RouteValues.Add("page", page.ToString());
            }

            base.Process(context, output);

            output.TagName = "a";
        }
    }
}
