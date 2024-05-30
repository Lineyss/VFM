using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using VFM.Models.Help;
using VFM.Models.Auth;
using VFM.Models.Users;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace VFM.Controllers.Main
{
    public class MainController : Controller
    {
        private readonly LiteDbContext db;
        public MainController(LiteDbContext db)
        {
            this.db = db;
        }

        [HttpGet("Auth/Login.html")]
        [NoAuth(RedirectPath = "/VirtualFileManager")]
        public IActionResult Auth() => View();      

        [Auth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("VirtualFileManager")]
        public IActionResult Index()
        {
            User userModel;
            try
            {
                string? idUser = User.FindFirst("ID")?.Value ?? throw new Exception();

                int ID = Convert.ToInt32(idUser);
                userModel = GetUser(ID);
            }
            catch
            {
                userModel = new User();
            }
            return View(userModel);
        }

        [Auth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("VirtualFileManager/OpenFile")]
        public IActionResult ViewFile() => View();

        [HttpGet("VirtualFileManager/Admin")]
        [Auth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, PropertyName = "isAdmin", PropertyValue = "True")]
        public IActionResult Admin() => View();

        private User GetUser(int ID)
        {
            return db.user.Find(ID) ?? new User();
        }
    }
}
