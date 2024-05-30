using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VFM.Models.Auth;
using VFM.Models.Help;
using VFM.Models.Users;

namespace VFM.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LiteDbContext db;
        private readonly AuthManager authenticationManager;

        public AuthController(LiteDbContext db, AuthManager authenticationManager)
        {
            this.db = db;
            this.authenticationManager = authenticationManager;
        }

        [HttpPost("Login/Cookie")]
        [NoAuth]
        public async Task<IActionResult> LoginCookie([FromForm] UserAuth model)
        {
            try
            {
                if (model == null) throw new Exception(ErrorModel.AllFieldsMostBeFields);

                var users = db.user.ToList();
                var user = db.user.FirstOrDefault(user => user.login == model.login);

                if (user == null) throw new Exception(ErrorModel.WrongLoginOrPassword);

                if (!HashPassword.ComparePasswords(user.password, model.password)) throw new Exception(ErrorModel.WrongLoginOrPassword);

                await authenticationManager.CookieLogIn(HttpContext, user);
                return Ok();


            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("Login/Jwt")]
        [NoAuth]
        public async Task<IActionResult> LoginJWT([FromBody] UserAuth model)
        {
            try
            {
                if (model == null) throw new Exception(ErrorModel.AllFieldsMostBeFields);

                var users = db.user.ToList();
                var user = db.user.FirstOrDefault(user => user.login == model.login);

                if (user == null) throw new Exception(ErrorModel.WrongLoginOrPassword);

                if (!HashPassword.ComparePasswords(user.password, model.password)) throw new Exception(ErrorModel.WrongLoginOrPassword);

                return Ok(new JwtModel
                {
                    token = authenticationManager.JwtLogIn(user)
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("Logout")]
        [Auth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                char[] authSchemes = HttpContext.User.Identities.SelectMany(i => i.AuthenticationType).ToArray();
                string _authScheme = string.Join("", authSchemes);

                if (CookieAuthenticationDefaults.AuthenticationScheme == _authScheme)  await authenticationManager.CookieLogOut(HttpContext);
                else await authenticationManager.JwtLogOut(HttpContext);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
