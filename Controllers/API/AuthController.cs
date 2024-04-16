using LiteDB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VFM.Models;
using VFM.Services;

namespace VFM.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LiteDbContext db;
        private readonly AuthenticationManager authenticationManager = new AuthenticationManager();

        public AuthController(LiteDbContext db)
        {
            this.db = db;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromForm] AuthUserModel model)
        {
            try
            {
                if (model == null) throw new Exception(ErrorModel.AllFieldsMostBeFields);

                var user = db.GetCollection<UserModel>("user").Find(element => element.login == model.login).FirstOrDefault();

                if (user == null) throw new Exception(ErrorModel.WrongLoginAndPassword);

                if (!HashPassword.ComparePasswords(user.password, model.password)) throw new Exception(ErrorModel.WrongLoginAndPassword);

                return Ok(new
                {
                    token = authenticationManager.JwtLogIn(user)
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                authenticationManager.JwtLogOut(HttpContext);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
