using Microsoft.AspNetCore.Mvc;

namespace VFM.Controllers.Main
{
    public class MainController : Controller
    {
        [HttpGet("Auth/Login.html")]
        public IActionResult Auth() => View();

        [HttpGet("Auth/Exit")]
        public IActionResult Exit() => View();

        [HttpGet("VirtualFileManager")]
        public IActionResult Index() => View();

        [HttpGet("VirtualFileManager/Admin")]
        public IActionResult Admin() => View();

    }
}
