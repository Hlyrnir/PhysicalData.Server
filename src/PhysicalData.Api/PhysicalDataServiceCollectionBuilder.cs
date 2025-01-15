using Microsoft.Extensions.Options;
using Passport.Api;
using PhysicalData.Infrastructure;

namespace PhysicalData.Api
{
    public class PhysicalDataServiceCollectionBuilder
    {
        public readonly IServiceCollection cltService;
        public virtual IServiceCollection Services { get => cltService; }

        public PhysicalDataServiceCollectionBuilder(IServiceCollection cltService)
        {
            this.cltService = cltService;
        }

        public PhysicalDataServiceCollectionBuilder AddAuthorization()
        {
            this.Services.AddPassport(prvService =>
            {
                IOptions<JwtTokenSetting> optJwtSetting = prvService.GetRequiredService<IOptions<JwtTokenSetting>>();

                return new JwtTokenHandler<Guid>(optJwtSetting.Value);
            })
                .Configure(optPassport =>
                {
                    optPassport.MaximalAllowedAccessAttempt = 5;
                    optPassport.RefreshTokenExpiresAfterDuration = new TimeSpan(hours: 1, minutes: 0, seconds: 0);
                    optPassport.TwoFactorAuthentication = false;
                })
                .AddSqliteDatabase(sConnectionStringName: "Passport");

            return new PhysicalDataServiceCollectionBuilder(this.Services);
        }

        public PhysicalDataServiceCollectionBuilder AddSqliteDatabase(string sConnectionStringName)
        {
            cltService.AddSqliteDatabase(sConnectionStringName);

            return new PhysicalDataServiceCollectionBuilder(this.Services);
        }
    }
}