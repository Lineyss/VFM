using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace VFM.Controllers.Main
{
    public class MainController : Controller
    {
        [HttpGet("Auth/Login.html")]
        public IActionResult Auth() => View();

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("Auth/Exit")]
        public IActionResult Exit() => View();

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("VirtualFileManager")]
        public IActionResult Index() => View();

        [HttpGet("VirtualFileManager/Admin")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Admin() => View();

    }
}
