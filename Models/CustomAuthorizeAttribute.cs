using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace CustomAuthorization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public string AuthenticationScheme { get; set; }
        public string RequiredClaim { get; }

        public CustomAuthorizationAttribute(string requiredClaim)
        {
            RequiredClaim = requiredClaim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!string.IsNullOrEmpty(AuthenticationScheme))
            {
                context.HttpContext.Request.Headers["Authorization"] = AuthenticationScheme;
            }
            var hasRequiredClaim = context.HttpContext.User.HasClaim(c => c.Type == RequiredClaim);

            if (!hasRequiredClaim)
            {
                context.Result = new ForbidResult();
            }
            else
            {
               context.HttpContext.User.HasClaim(c => c.Type == RequiredClaim);
                if (hasRequiredClaim)
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}