using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;

namespace PhysicalData.Application.Query.HealthCheck
{
    internal sealed class PhysicalDataHealthCheckQueryHandler : IQueryHandler<PhysicalDataHealthCheckQuery, IMessageResult<bool>>
    {
        private readonly IUnitOfWork uowUnitOfWork;

        public PhysicalDataHealthCheckQueryHandler([FromKeyedServices(DefaultKeyedServiceName.UnitOfWork)] IUnitOfWork uowUnitOfWork)
        {
            this.uowUnitOfWork = uowUnitOfWork;
        }

        public async ValueTask<IMessageResult<bool>> Handle(PhysicalDataHealthCheckQuery qryQuery, CancellationToken tknCancellation)
        {
            bool bIsHealthy = false;

            try
            {
                await uowUnitOfWork.TransactionAsync(() =>
                {
                    bIsHealthy = uowUnitOfWork.TryRollback();

                    return Task.CompletedTask;
                });
            }
            catch(Exception exException)
            {
                return new MessageResult<bool>(new MessageError() { Code = "EXCEPTION", Description = exException.Message });
            }

            return new MessageResult<bool>(bIsHealthy);
        }
    }
}