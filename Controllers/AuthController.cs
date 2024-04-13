using LiteDB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VFM.Models;
using VFM.Services;

namespace VFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LiteDbContext db;

        public AuthController(LiteDbContext db)
        {
            this.db = db;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromForm] AuthUserModel model)
        {
            try
            {
                if (model == null) throw new Exception("Все поля должны быть заполненны");

                var user = db.GetCollection<UserModel>("user").Find(element => element.login == model.login).FirstOrDefault();
                
                if (user == null) throw new Exception("Не верный логин или пароль");

                if(!HashPassword.ComparePasswords(user.password, model.password)) throw new Exception("Не верный логин или пароль");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.login),
                    new Claim("isAdmin", user.isAdmin.ToString()),
                    new Claim("createF", user.createF.ToString()),
                    new Claim("deleteF", user.deleteF.ToString()),
                    new Claim("updateNameF", user.updateNameF.ToString()),
                    new Claim("downloadF", user.downloadF.ToString()),
                    new Claim("uploadF", user.uploadF.ToString())
                };

                var jwt = new JwtSecurityToken(
                        issuer: Jwt.ValidIssuer,
                        audience: Jwt.ValidAudience,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromHours(2)), // время действия 2 минуты
                        signingCredentials: new SigningCredentials(Jwt.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

                return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
