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

        private ILogger logger;

        public string? PropertyName { get; set; }
        public string? PropertyValue { get; set; }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<Auth>)) as ILogger<Auth>;

                ClaimsPrincipal user = context.HttpContext.User ?? throw new Exception(ErrorModel.WrongLoginOrPassword);

                var db = context.HttpContext
                                .RequestServices
                                .GetService(typeof(LiteDbContext)) as LiteDbContext ?? throw new Exception(ErrorModel.CanNotFoundServiceDataBase);

                logger.LogInformation("Был найден сервис бд", DateTime.Now.ToString());

                string sID = user.FindFirstValue("ID");

                if (string.IsNullOrWhiteSpace(sID)) throw new Exception(ErrorModel.NotValidFormatID);

                int ID = Convert.ToInt32(sID);
                var dbUser = db.user.Find(ID);

                if (dbUser == null)
                {
                    var AuthService = context.HttpContext
                                .RequestServices
                                .GetService(typeof(AuthManager)) as AuthManager ?? throw new Exception(ErrorModel.CanNotFoundServiceAuth);

                    logger.LogInformation("Был найден сервис авторизации", DateTime.Now.ToString());

                    await AuthService.CookieLogOut(context.HttpContext);
                    logger.LogInformation("Были удалены куки пользователя", DateTime.Now.ToString());
                    await AuthService.JwtLogOut(context.HttpContext);
                    logger.LogInformation("Были удаленн jwt токен пользователя", DateTime.Now.ToString());
                    throw new Exception(ErrorModel.AccountIsNotExist);
                }

                if (string.IsNullOrEmpty(PropertyName) || string.IsNullOrEmpty(PropertyValue))
                {
                    if (!user.Identity.IsAuthenticated) throw new Exception(ErrorModel.UserIsNotAuthenticated);
                }
                else
                {

                    PropertyInfo property = typeof(User).GetProperty(PropertyName) ?? throw new Exception(ErrorModel.UserModelHasNotThisProperty);
                    object value = property.GetValue(dbUser) ?? throw new Exception(ErrorModel.UserModelPropertyIsNull);

                    if (value.ToString() != PropertyValue) throw new Exception(ErrorModel.UserModelHasNotPropertyWithThisValue);
                }
                logger.LogInformation("Пользователь авторизован", DateTime.Now.ToString());
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, DateTime.Now.ToString());
                context.Result = new ForbidResult();
            }
        }
    }
}
