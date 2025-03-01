using Microsoft.AspNetCore.Cors.Infrastructure;

namespace PhysicalData.Api.Cors
{
    public static class DefaultCorsPolicy
    {
        public const string Name = "CORS_DEFAULT";

        public static CorsPolicy RestrictedOrigin(string sOrigin)
        {
            CorsPolicyBuilder plcyBuilder = new CorsPolicyBuilder();

            plcyBuilder.WithOrigins(sOrigin)
                .AllowAnyHeader()
                .AllowAnyMethod();

            return plcyBuilder.Build();
        }
    }
}
