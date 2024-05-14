using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VFM.Models.Auth;
using VFM.Models.Users;

namespace VFM.Models.Auth
{
    public class AuthManager
    {
        public string JwtLogIn(User user)
        {
            var jwt = new JwtSecurityToken(
                       issuer: Jwt.ValidIssuer,
                       audience: Jwt.ValidAudience,
                       claims: GetClaims(user),
                       expires: DateTime.UtcNow.Add(TimeSpan.FromHours(2)),
                       signingCredentials: new SigningCredentials(Jwt.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt).ToString();
        }

        public async Task CookieLogIn(HttpContext context, User user)
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

        private List<Claim> GetClaims(User user)
        {
            return new List<Claim>
            {
                new Claim("ID", user.ID.ToString()),
            };
        }
    }
}
