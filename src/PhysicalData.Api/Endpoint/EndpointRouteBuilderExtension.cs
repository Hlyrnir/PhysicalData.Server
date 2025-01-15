using PhysicalData.Api.Endpoint.PhysicalDimension;
using PhysicalData.Api.Endpoint.TimePeriod;

namespace PhysicalData.Api.Endpoint
{
    public static class EndpointRouteBuilderExtension
    {
        public static void AddPhysicalDimensionEndpoint(this IEndpointRouteBuilder epBuilder, params string[] sPolicyName)
        {
            epBuilder.AddCreatePhysicalDimensionEndpoint(sPolicyName: sPolicyName);
            epBuilder.AddDeletePhysicalDimensionEndpoint(sPolicyName: sPolicyName);
            epBuilder.AddFindPhysicalDimensionByFilterEndpoint(sPolicyName: sPolicyName);
            epBuilder.AddFindPhysicalDimensionByIdEndpoint(sPolicyName: sPolicyName);
            epBuilder.AddUpdatePhysicalDimensionEndpoint(sPolicyName: sPolicyName);
        }

        public static void AddTimePeriodEndpoint(this IEndpointRouteBuilder epBuilder, params string[] sPolicyName)
        {
            epBuilder.AddCreateTimePeriodEndpoint(sPolicyName: sPolicyName);
            epBuilder.AddDeleteTimePeriodEndpoint(sPolicyName: sPolicyName);
            epBuilder.AddFindTimePeriodByFilterEndpoint(sPolicyName: sPolicyName);
            epBuilder.AddFindTimePeriodByIdEndpoint(sPolicyName: sPolicyName);
            epBuilder.AddUpdateTimePeriodEndpoint(sPolicyName: sPolicyName);
        }
    }
}