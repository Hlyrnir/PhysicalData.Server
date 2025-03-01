using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Passport.Api;

namespace PhysicalData.Api.Authorization
{
    public static class PassportAuthorizationPolicy
    {
        public const string Name = "AUTHORIZATION_PASSPORT";

        public static AuthorizationPolicy WithPassport()
        {
            var plcyBuilder = new AuthorizationPolicyBuilder();

            plcyBuilder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
            plcyBuilder.RequireAuthenticatedUser();
            plcyBuilder.RequireClaim(PassportClaim.Id);

            return plcyBuilder.Build();
        }
    }
}
