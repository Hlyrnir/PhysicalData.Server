using PhysicalData.Application;
using PhysicalData.Infrastructure;

namespace PhysicalData.Api
{
    public static class ServiceCollectionExtension
    {
        public static PhysicalDataServiceCollectionBuilder AddPhysicalDataServiceCollection(this IServiceCollection cltService)
        {
            cltService.AddApplicationServiceCollection();
            cltService.AddInfrastructureServiceCollection();

            return new PhysicalDataServiceCollectionBuilder(cltService);
        }
    }
}