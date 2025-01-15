using Microsoft.IdentityModel.Tokens;
using Passport.Abstraction.Authentication;
using Passport.Api;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PhysicalData.Api
{
    public class JwtTokenHandler<Guid> : IAuthenticationTokenHandler<Guid>
    {
        private readonly JwtTokenSetting jwtSetting;

        public JwtTokenHandler(JwtTokenSetting jwtSetting)
        {
            this.jwtSetting = jwtSetting;
        }

        public string Generate(Guid guPassportId, TimeProvider prvTime)
        {
            IDictionary<string, object> dictClaim = new Dictionary<string, object>();

            if (guPassportId is not null)
                dictClaim.Add(new KeyValuePair<string, object>(PassportClaim.Id, $"{guPassportId}"));

            SecurityTokenDescriptor tknDescriptor = new SecurityTokenDescriptor
            {
                Claims = dictClaim,
                IssuedAt = prvTime.GetUtcNow().UtcDateTime,
                Expires = prvTime.GetUtcNow().AddMinutes(jwtSetting.LifetimeInMinutes).UtcDateTime,
                NotBefore = prvTime.GetUtcNow().UtcDateTime,
                Issuer = jwtSetting.Issuer,
                Audience = jwtSetting.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecretKey)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler tknHandler = new JwtSecurityTokenHandler();
            SecurityToken tknToken = tknHandler.CreateToken(tknDescriptor);

            return tknHandler.WriteToken(tknToken);
        }
    }
}
