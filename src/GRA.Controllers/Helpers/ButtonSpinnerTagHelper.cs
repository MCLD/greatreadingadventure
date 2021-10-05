using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helper
{
    [HtmlTargetElement(targetElement, Attributes = attributeName)]
    public class ButtonSpinnerTagHelper : TagHelper
    {
        private const string attributeName = "button-spinner";
        private const string classAttribute = "class";
        private const string ignoreValidationAttributeName = "ignore-validation";
        private const string ignoreValidationClass = "spinner-ignore-validation";
        private const string spanClass = "fas fa-spinner fa-pulse fa-fw hidden";
        private const string spanTag = "span";
        private const string spinnerClass = "btn-spinner";
        private const string targetElement = "button";

        [HtmlAttributeName(ignoreValidationAttributeName)]
        public bool IgnoreValidation { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var existingClasses = output.Attributes.FirstOrDefault(f => f.Name == classAttribute);
            var buttonClasses = new StringBuilder();
            if (existingClasses != null)
            {
                buttonClasses.Append(existingClasses.Value).Append(' ');
                output.Attributes.Remove(existingClasses);
            }

            buttonClasses.Append(spinnerClass).Append(' ');
            if (IgnoreValidation)
            {
                buttonClasses.Append(ignoreValidationClass);
            }

            output.Attributes.Add(new TagHelperAttribute(classAttribute,
                buttonClasses.ToString().Trim()));

            var spinnerSpan = new TagBuilder(spanTag)
            {
                TagRenderMode = TagRenderMode.Normal
            };
            spinnerSpan.AddCssClass(spanClass);

            output.PostContent.SetHtmlContent(spinnerSpan);
            output.Attributes.Remove(new TagHelperAttribute(attributeName));
        }
    }
}