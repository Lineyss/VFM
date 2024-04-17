using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VFM.Models
{
    public class UserAuthorization : AuthorizeAttribute, IAuthorizationFilter
    {
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                ClaimsPrincipal user = context.HttpContext.User;
                if (user == null) throw new Exception();

                if (string.IsNullOrEmpty(PropertyName) || string.IsNullOrEmpty(PropertyValue))
                {
                    if (user.Identity.IsAuthenticated) return;
                    else throw new Exception();
                }
                else
                {
                    var db = context.HttpContext
                                    .RequestServices
                                    .GetService(typeof(LiteDbContext)) as LiteDbContext;
                    string sID = user.FindFirstValue("ID");

                    if (string.IsNullOrWhiteSpace(sID)) throw new Exception();

                    int ID = Convert.ToInt32(sID);

                    var dbUser = db.GetCollection<UserModel>("user").FindById(ID);

                    PropertyInfo property = typeof(UserModel).GetProperty(PropertyName) ?? throw new Exception();
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