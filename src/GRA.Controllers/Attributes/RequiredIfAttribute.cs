using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace GRA.Controllers.Attributes
{
    // Source - https://stackoverflow.com/a/70835172
    // Posted by Neil
    // Retrieved 2026-03-30, License - CC BY-SA 4.0

    /// <summary>
    /// Provides conditional validation based on related property value.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
    /// </remarks>
    /// <param name="otherProperty">The other property.</param>
    /// <param name="otherPropertyValue">The other property value.</param>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RequiredIfAttribute(string otherProperty, object otherPropertyValue)
        : ValidationAttribute, IClientModelValidator
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether other property's value should match or differ
        /// from provided other property's value (default is <c>false</c>).
        /// </summary>
        /// <value>
        ///   <c>true</c> if other property's value validation should be inverted; otherwise,
        ///   <c>false</c>.
        /// </value>
        /// <remarks>
        /// How this works
        /// - true: validated property is required when other property doesn't equal provided value
        /// - false: validated property is required when other property matches provided value
        /// </remarks>
        public bool IsInverted { get; set; }

        /// <summary>
        /// Gets or sets the other property name that will be used during validation.
        /// </summary>
        /// <value>
        /// The other property name.
        /// </value>
        public string OtherProperty { get; } = otherProperty;

        /// <summary>
        /// Gets or sets the display name of the other property.
        /// </summary>
        /// <value>
        /// The display name of the other property.
        /// </value>
        public string OtherPropertyDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the other property value that will be relevant for validation.
        /// </summary>
        /// <value>
        /// The other property value.
        /// </value>
        public object OtherPropertyValue { get; } = otherPropertyValue;

        /// <summary>
        /// Gets a value that indicates whether the attribute requires validation context.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the attribute requires validation context; otherwise, <c>false</c>.
        /// </returns>
        public override bool RequiresValidationContext
        {
            get
            {
                return true;
            }
        }

        #endregion Properties

        /// <summary>
        /// Perform client-side validation
        /// </summary>
        /// <param name="context">
        /// The <see cref="ClientModelValidationContext"/> context information about the validation
        /// operation.
        /// </param>
        public void AddValidation(ClientModelValidationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var viewContext = context.ActionContext as ViewContext;
            var modelType = context.ModelMetadata.ContainerType;
            var instance = viewContext?.ViewData.Model;
            var model = instance?.GetType().Name == modelType.Name
                ? instance
                : instance?.GetType()?
                    .GetProperties()
                    .First(_ => _.PropertyType.Name == modelType.Name)
                    .GetValue(instance, null);
            object otherValue = modelType.GetProperty(OtherProperty)?.GetValue(model, null);
            object value = modelType.GetProperty(context.ModelMetadata.Name)?.GetValue(model, null);

            // check if this value is actually required and validate it
            if ((IsInverted ^ object.Equals(otherValue, OtherPropertyValue))
                && (value == null || (value as string)?.Trim().Length == 0))
            {
                var localizer = GetLocalizer(viewContext.HttpContext.RequestServices,
                    context.ModelMetadata.ModelType);

                var displayName = context.ModelMetadata.DisplayName ?? context.ModelMetadata.Name;

                context.Attributes.Add("data-val", "true");
                context.Attributes.Add("data-val-required", localizer[ErrorMessage, displayName]);
            }
        }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">
        /// The <see cref="ValidationContext"/> context information about the validation operation.
        /// </param>
        /// <returns>
        /// An instance of the
        /// <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.
        /// </returns>
        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            ArgumentNullException.ThrowIfNull(validationContext);

            var otherProp = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherProp == null)
            {
                return new ValidationResult($"Could not find a property named '{OtherProperty}'");
            }

            var otherValue = otherProp.GetValue(validationContext.ObjectInstance);

            // check if this value is actually required and validate it
            return (IsInverted ^ object.Equals(otherValue, OtherPropertyValue))
                && (value == null || (value as string)?.Trim().Length == 0)
                ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
                : ValidationResult.Success;
        }

        private static IStringLocalizer GetLocalizer(IServiceProvider serviceProvider, Type type)
        {
            var factory = serviceProvider.GetRequiredService<IStringLocalizerFactory>();
            var annotationOptions = serviceProvider
                .GetRequiredService<IOptions<MvcDataAnnotationsLocalizationOptions>>();
            return annotationOptions.Value.DataAnnotationLocalizerProvider(type, factory);
        }
    }
}
