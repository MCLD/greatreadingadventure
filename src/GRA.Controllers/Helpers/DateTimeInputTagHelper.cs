using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GRA.Controllers.Helpers
{
    [HtmlTargetElement(TagInput, Attributes = AttributeTdPicker)]
    public class DateTimeInputTagHelper : TagHelper
    {
        private const string AttributeClass = "class";
        private const string AttributeTdPicker = "datetimepicker-input";
        private const string DataTdTarget = "data-td-target";
        private const string TagInput = "input";
        private readonly HashSet<string> _classes;

        public DateTimeInputTagHelper()
        {
            _classes = new HashSet<string> { "form-control" };
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

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(output);

            output.Attributes.Add(new TagHelperAttribute(AttributeClass, Class));

            if (context.Items.TryGetValue(DateTimeContainerTagHelper.KeyPickerContainerId,
                out var value))
            {
                output.Attributes.Add(new TagHelperAttribute(DataTdTarget, $"#{value}"));
            }
            output.Attributes.Remove(output.Attributes[AttributeTdPicker]);
        }
    }
}
