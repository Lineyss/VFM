using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace VFM.Models.Auth
{
    public class NoAuth : ResultFilterAttribute
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
