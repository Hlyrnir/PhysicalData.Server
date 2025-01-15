using Mediator;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Passport.Abstraction.Result;
using Passport.Application.Query.HealthCheck;
using PhysicalData.Application.Query.HealthCheck;

namespace PhysicalData.Api.Health
{
    public class HealthCheck : IHealthCheck
    {
        public const string Name = "API";

        private readonly ISender mdtMediator;
        private readonly ILogger<HealthCheck> logLogger;


        public HealthCheck(ISender mdtMediator, ILogger<HealthCheck> logLogger)
        {
            this.mdtMediator = mdtMediator;
            this.logLogger = logLogger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext ctxHealthCheck, CancellationToken tknCancellation = default)
        {
            PassportHealthCheckQuery qryPassportHealth = new PassportHealthCheckQuery();
            PhysicalDataHealthCheckQuery qryPhysicalDataHealth = new PhysicalDataHealthCheckQuery();

            IMessageResult<bool> rstlPassport = await mdtMediator.Send(qryPassportHealth, tknCancellation);

            rstlPassport.Match(
                msgError =>
                {
                    logLogger.LogInformation("Health check failed - {}", msgError.Description);
                    return false;
                },
                bResult => true);

            IMessageResult<bool> rsltPhysicalData = await mdtMediator.Send(qryPhysicalDataHealth, tknCancellation);

            rsltPhysicalData.Match(
                msgError =>
                {
                    logLogger.LogInformation("Health check failed - {}", msgError.Description);
                    return false;
                },
                bResult => true);

            if (rstlPassport.IsFailed == true && rsltPhysicalData.IsFailed == true)
                return HealthCheckResult.Unhealthy("API is unhealthy.");

            if (rstlPassport.IsFailed == true ^ rsltPhysicalData.IsFailed == true)
                return HealthCheckResult.Degraded("API is degraded.");

            return HealthCheckResult.Healthy("API is available.");
        }
    }
}