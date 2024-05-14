using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace VFM.Models.Auth
{
    public class Jwt
    {
        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(IssuerSigningKey));

        public const bool ValidateIssuer = true;
        public const string ValidIssuer = "WFM";
        public const bool ValidateAudience = true;
        public const string ValidAudience = "Audience";
        public const bool ValidateLifetime = true;
        private const string IssuerSigningKey = "mysupersecret_secretsecretsecretkey!123";
        public const bool ValidateIssuerSigningKey = true;
    }
}
