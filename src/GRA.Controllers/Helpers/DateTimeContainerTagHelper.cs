using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement(TagDiv, Attributes = AttributeTdContainer)]
    public class DateTimeContainerTagHelper : TagHelper
    {
        public static readonly string KeyPickerContainerId = "DateTimePickerContainerId";
        private const string AttributeClass = "class";
        private const string AttributeCurrentValue = "current-value";
        private const string AttributeId = "id";
        private const string AttributeTdContainer = "datetimepicker-container";
        private const string ClassInputGroup = "input-group";
        private const string ClassInputGroupText = "input-group-text";
        private const string DataCurrentValue = "data-current-value";
        private const string DataTdTarget = "data-td-target";
        private const string DataTdTargetInput = "data-td-target-input";
        private const string DataTdTargetToggle = "data-td-target-toggle";
        private const string DataTdToggle = "data-td-toggle";
        private const string TagDiv = "div";
        private const string TagSpan = "span";
        private const string ValueCalendarIcon = "far fa-calendar";
        private const string ValueNearest = "nearest";
        private const string ValueTdToggle = "datetimepicker";
        private readonly HashSet<string> _classes;

        public DateTimeContainerTagHelper()
        {
            _classes = new HashSet<string> { "input-group" };
        }

        [HtmlAttributeName(AttributeClass)]
        public string Class
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    foreach (var className in value.Split(' '))
                    {
                        _classes.Add(className);
                    }
                }
            }
            get
            {
                return string.Join(' ', _classes);
            }
        }

        [HtmlAttributeName(AttributeCurrentValue)]
        public DateTime? CurrentValue { get; set; }

        private bool IsInputGroup
        {
            get
            {
                return _classes.Contains(ClassInputGroup);
            }
        }

        public override void Init(TagHelperContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            var idAttribute = context.AllAttributes[AttributeId];
            context.Items.Add(KeyPickerContainerId,
                idAttribute != null ? idAttribute.Value : context.UniqueId);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(output);

            if (IsInputGroup)
            {
                var spanTag = new TagBuilder(TagSpan);
                spanTag.AddCssClass(ClassInputGroupText);
                spanTag.Attributes.Add(DataTdTarget, $"#{context.Items[KeyPickerContainerId]}");
                spanTag.Attributes.Add(DataTdToggle, ValueTdToggle);

                var spanIconTag = new TagBuilder(TagSpan);
                spanIconTag.AddCssClass(ValueCalendarIcon);
                spanTag.InnerHtml.AppendHtml(spanIconTag);
                output.PostContent.AppendHtml(spanTag);
            }

            output.Attributes.Add(new TagHelperAttribute(AttributeClass, Class));
            output.Attributes.Add(new TagHelperAttribute(DataTdTargetInput, ValueNearest));
            output.Attributes.Add(new TagHelperAttribute(DataTdTargetToggle, ValueNearest));
            if (CurrentValue.HasValue)
            {
                output.Attributes.Add(new TagHelperAttribute(DataCurrentValue,
                    CurrentValue.Value.ToString("o",
                        System.Globalization.CultureInfo.InvariantCulture)));
            }

            output.Attributes.Remove(output.Attributes[AttributeTdContainer]);
        }
    }
}
