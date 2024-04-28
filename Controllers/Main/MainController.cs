using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using VFM.Models;
using VFM.Services;

namespace VFM.Controllers.Main
{
    public class MainController : Controller
    {
        private readonly LiteDbContext db;
        private readonly AuthenticationManager authenticationManager = new AuthenticationManager();
        public MainController(LiteDbContext db)
        {
            this.db = db;
        }

        [HttpGet("Auth/Login.html")]
        [NoAuthUser(RedirectPath= "/VirtualFileManager")]
        public IActionResult Auth() => View();

        [HttpPost("Auth/Login.html")]
        public async Task<IActionResult> Auth([FromForm] AuthUserModel model)
        {
            try
            {
                if(model == null) throw new Exception(ErrorModel.AllFieldsMostBeFields);

                var user = db.GetCollection<UserModel>("user").Find(element => element.login == model.login).FirstOrDefault();
                if (user == null) throw new Exception(ErrorModel.WrongLoginAndPassword);

                if (!HashPassword.ComparePasswords(user.password, model.password)) throw new Exception(ErrorModel.WrongLoginAndPassword);

                await authenticationManager.CookieLogIn(HttpContext, user);

                return Redirect("/VirtualFileManager");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [UserAuthorization(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("Auth/Exit")]
        public async Task<IActionResult> Exit()
        {
            try
            {
                await authenticationManager.CookieLogOut(HttpContext);

                return Redirect("/Auth/Login.html");
            }
            catch
            {
                return BadRequest();
            }
        }

        [UserAuthorization(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("VirtualFileManager")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("VirtualFileManager/Admin")]
        [UserAuthorization(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, PropertyName = "isAdmin", PropertyValue = "True")]
        public IActionResult Admin() => View();

    }
}
