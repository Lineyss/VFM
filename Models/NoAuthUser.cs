using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Web.Http;

namespace VFM.Models
{
    public class NoAuthUser : ResultFilterAttribute
    {
        public string RedirectPath { get; set; }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
                context.Result = new RedirectResult(RedirectPath);

            base.OnResultExecuting(context);
        }
    }
}
