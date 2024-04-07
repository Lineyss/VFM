using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VFM.Models;

namespace VFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<UserModel> signInManager;

        public AuthController(SignInManager<UserModel> signInManager)
        {
            this.signInManager = signInManager;
        }

/*        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok(); // Возвращаем успешный ответ
            }

            return Unauthorized(); // Возвращаем ошибку авторизации
        }*/
    }
}
