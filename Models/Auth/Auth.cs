using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;
using VFM.Models.Help;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using VFM.Models.Users;

namespace VFM.Models.Auth
{
    public class Auth : AuthorizeAttribute, IAuthorizationFilter
    {
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                ClaimsPrincipal user = context.HttpContext.User ?? throw new Exception(ErrorModel.WrongLoginOrPassword);

                var db = context.HttpContext
                                .RequestServices
                                .GetService(typeof(LiteDbContext)) as LiteDbContext;
                string sID = user.FindFirstValue("ID");


                if (string.IsNullOrWhiteSpace(sID)) throw new Exception();

                int ID = Convert.ToInt32(sID);
                var dbUser = db.GetCollection<User>("user").FindById(ID);

                if (dbUser == null)
                {
                    var AuthService = context.HttpContext
                                .RequestServices
                                .GetService(typeof(AuthManager)) as AuthManager;

                    AuthService.CookieLogOut(context.HttpContext);
                    AuthService.JwtLogOut(context.HttpContext);
                    throw new Exception();
                }

                if (string.IsNullOrEmpty(PropertyName) || string.IsNullOrEmpty(PropertyValue))
                {
                    if (user.Identity.IsAuthenticated) return;
                    else throw new Exception();
                }
                else
                {

                    PropertyInfo property = typeof(User).GetProperty(PropertyName) ?? throw new Exception();
                    object value = property.GetValue(dbUser);

                    if (value?.ToString() != PropertyValue) throw new Exception();
                }
            }
            catch
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
