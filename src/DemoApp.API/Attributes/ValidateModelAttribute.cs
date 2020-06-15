using DemoApp.API.ApiModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace DemoApp.API.Attributes
{
    /// <summary>
    /// Enable validation for Model pass in action
    /// Also Enable validation for Action parameter based on DataAnnotation attributes on parameter
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        private static readonly ConcurrentDictionary<MethodInfo, ParameterInfo[]> MethodCache = new ConcurrentDictionary<MethodInfo, ParameterInfo[]>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ValidateParameters(context);
            if (!context.ModelState.IsValid)
            {
                context.Result = new ValidationFailedResult(context.ModelState);
            }
        }

        // Validating Parameters based on Attribute
        private void ValidateParameters(ActionExecutingContext context)
        {
            ControllerActionDescriptor descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor == null) return;

            foreach (var param in GetParameters(descriptor.MethodInfo))
            {
                object arg;
                context.ActionArguments.TryGetValue(param.Name, out arg);
                Validate(param, arg, context.ModelState);
            }
        }

        private void Validate(ParameterInfo parameter, object argument, ModelStateDictionary modelState)
        {
            var paramAttrs = parameter.CustomAttributes.Where(x => typeof(ValidationAttribute).IsAssignableFrom(x.AttributeType));

            foreach (var attr in paramAttrs)
            {
                ValidationAttribute validationAttribute = parameter.GetCustomAttribute(attr.AttributeType) as ValidationAttribute;
                if (validationAttribute == null) continue;
                if (validationAttribute.IsValid(argument)) continue;
                modelState.AddModelError(parameter.Name, validationAttribute.FormatErrorMessage(parameter.Name));
            }
        }

        private static IEnumerable<ParameterInfo> GetParameters(MethodInfo method) => MethodCache.GetOrAdd(method, x => x.GetParameters());
    }
}
