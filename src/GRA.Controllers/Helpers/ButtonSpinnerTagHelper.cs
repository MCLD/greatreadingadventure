using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helper
{
    [HtmlTargetElement(targetElement, Attributes = attributeName)]
    public class ButtonSpinnerTagHelper : TagHelper
    {
        private const string targetElement = "button";
        private const string attributeName = "button-spinner";
        private const string ignoreValidationAttributeName = "ignore-validation";
        private const string classAttribute = "class";
        private const string spinnerClass = "btn-spinner";
        private const string ignoreValidationClass = "spinner-ignore-validation";
        private const string spanTag = "span";
        private const string spanClass = "fa fa-spinner fa-pulse fa-fw hidden";

        [HtmlAttributeName(ignoreValidationAttributeName)]
        public bool IgnoreValidation { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var existingClasses = output.Attributes.FirstOrDefault(f => f.Name == classAttribute);
            var buttonClasses = string.Empty;
            if (existingClasses != null)
            {
                buttonClasses = existingClasses.Value.ToString();
                output.Attributes.Remove(existingClasses);
            }
            buttonClasses += $" {spinnerClass}";
            if (IgnoreValidation)
            {
                buttonClasses += $" {ignoreValidationClass}";
            }

            var buttonClassAttribute = new TagHelperAttribute(classAttribute, buttonClasses);
            output.Attributes.Add(buttonClassAttribute);

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
