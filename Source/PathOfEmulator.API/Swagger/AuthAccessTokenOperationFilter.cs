using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PathOfEmulator.API.Swagger
{
    public class AuthAccessTokenOperationFilter : IOperationFilter
    {
        private static bool HasAttribute(MemberInfo methodInfo, Type type, bool inherit)
        {
            // inherit = true also checks inherited attributes
            var actionAttributes = methodInfo.GetCustomAttributes(inherit);
            var controllerAttributes = methodInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes(inherit);
            var actionAndControllerAttributes = controllerAttributes?.Union(actionAttributes) ?? actionAttributes;

            return actionAndControllerAttributes.Any(attr => attr.GetType() == type);
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorizeAttribute = HasAttribute(context.MethodInfo, typeof(AuthorizeAttribute), true);
            var hasAnonymousAttribute = HasAttribute(context.MethodInfo, typeof(AllowAnonymousAttribute), true);

            var isAuthorized = hasAuthorizeAttribute && !hasAnonymousAttribute;

            if (!isAuthorized)
                return;

            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "access_token",
                Description = "Method requires authorization via Access Token obtained from OAuth",
                Required = true,
                In = ParameterLocation.Query,
                AllowEmptyValue = false
            });
        }
    }
}
