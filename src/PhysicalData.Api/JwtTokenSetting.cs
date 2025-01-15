namespace PhysicalData.Api
{
    public class JwtTokenSetting
    {
        public static string SectionName = "JwtSetting";

        public string Type { get; } = "jwtAuthentication";
        public string Audience { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string SecretKey { get; init; } = string.Empty;

        public int LifetimeInMinutes { get; init; } = 5;
    }
}
