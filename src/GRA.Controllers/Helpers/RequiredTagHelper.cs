using System;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helpers
{
    /// <summary>
    /// For input elements, if add-data-val-required is set to a validation message, change the
    /// property to data-val-required with the validation message. If the validation message is
    /// empty then do not set a data-val-required tag at all.
    /// 
    /// This allows a string property in the view model to determine whether or not JavaScript
    /// validation is activated for an input field.
    /// </summary>
    [HtmlTargetElement(HtmlTagInput, Attributes = AddRequiredTag)]
    public class RequiredTagHelper : TagHelper
    {
        private const string AddRequiredTag = "add-data-val-required";
        private const string AttributeDataValRequired = "data-val-required";
        private const string HtmlTagInput = "input";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            var requiredAtt = output.Attributes.SingleOrDefault(_ => _.Name == AddRequiredTag);
            if (requiredAtt != null)
            {
                output.Attributes.Add(AttributeDataValRequired, requiredAtt.Value);
                output.Attributes.Remove(requiredAtt);
            }
        }
    }
}
