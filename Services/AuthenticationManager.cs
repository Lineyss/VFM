using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VFM.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace VFM.Services
{
    public class AuthenticationManager
    {
        public string JwtLogIn(UserModel user)
        {
            var jwt = new JwtSecurityToken(
                       issuer: Jwt.ValidIssuer,
                       audience: Jwt.ValidAudience,
                       claims: GetClaims(user),
                       expires: DateTime.UtcNow.Add(TimeSpan.FromHours(2)), // время действия 2 минуты
                       signingCredentials: new SigningCredentials(Jwt.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt).ToString();
        }

        public async Task CookieLogIn(HttpContext context ,UserModel user)
        {
            var claimsIdentity = new ClaimsIdentity(GetClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);
           
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        public async Task JwtLogOut(HttpContext context)
        {
            await context.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
        }

        public async Task CookieLogOut(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private List<Claim> GetClaims(UserModel user)
        {
            return new List<Claim>
            {
                new Claim("ID", user.ID.ToString()),
            };
        }
    }
}
