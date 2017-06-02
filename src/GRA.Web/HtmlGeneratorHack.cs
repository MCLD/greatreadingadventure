using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Text.Encodings.Web;

namespace GRA.Web
{

    // To be removed after .net core 2.0 is added
    // https://github.com/aspnet/Antiforgery/issues/116
    public class HtmlGeneratorHack : DefaultHtmlGenerator
    {
        public HtmlGeneratorHack(
            IAntiforgery antiforgery,
            IOptions<MvcViewOptions> optionsAccessor,
            IModelMetadataProvider metadataProvider,
            IUrlHelperFactory urlHelperFactory,
            HtmlEncoder htmlEncoder,
            ClientValidatorCache clientValidatorCache,
            ValidationHtmlAttributeProvider validationAttributeProvider)
            : base(
                antiforgery,
                optionsAccessor,
                metadataProvider,
                urlHelperFactory,
                htmlEncoder,
                clientValidatorCache,
                validationAttributeProvider)
        {
        }

        public override IHtmlContent GenerateAntiforgery(ViewContext viewContext)
        {
            var result = base.GenerateAntiforgery(viewContext);

            // disable caching for the browser back button                         
            viewContext
                .HttpContext
                .Response
                .Headers[HeaderNames.CacheControl]
                    = "no-cache, max-age=0, must-revalidate, no-store";

            return result;
        }
    }
}
