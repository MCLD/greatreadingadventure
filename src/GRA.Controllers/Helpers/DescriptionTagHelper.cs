// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Sourced from the label tag helper, modified for the GRA to display "Description"

using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helpers
{
    /// <summary>
    /// <see cref="ITagHelper"/> implementation outputting the description attribute.
    /// </summary>
    [HtmlTargetElement("description", Attributes = ForAttributeName)]
    public class DescriptionTagHelper : TagHelper
    {
        private const string ForAttributeName = "gra-description-for";

        /// <summary>
        /// Creates a new <see cref="DescriptionTagHelper"/>.
        /// </summary>
        /// <param name="generator">The <see cref="IHtmlGenerator"/>.</param>
        public DescriptionTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        public override int Order => -1000;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected IHtmlGenerator Generator { get; }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <inheritdoc />
        /// <remarks>Does nothing if <see cref="For"/> is <c>null</c>.</remarks>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var description = For.ModelExplorer.Metadata.Description;

            if (string.IsNullOrEmpty(description))
            {
                output.SuppressOutput();
            }
            else
            {
                output.TagName = "span";
                output.Attributes.SetAttribute("class", "gra-form-description");
                output.Content.SetHtmlContent(description);
            }
        }
    }
}
