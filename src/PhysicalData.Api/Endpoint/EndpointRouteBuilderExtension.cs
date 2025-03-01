using PhysicalData.Api.Endpoint.PhysicalDimension;
using PhysicalData.Api.Endpoint.TimePeriod;

namespace PhysicalData.Api.Endpoint
{
    public static class EndpointRouteBuilderExtension
    {
        public static void AddPhysicalDimensionEndpoint(this IEndpointRouteBuilder epBuilder, string sCorsPolicyName, params string[] sAuthorizationPolicyName)
        {
            epBuilder.AddCreatePhysicalDimensionEndpoint(sCorsPolicyName: sCorsPolicyName, sAuthorizationPolicyName: sAuthorizationPolicyName);
            epBuilder.AddDeletePhysicalDimensionEndpoint(sCorsPolicyName: sCorsPolicyName, sAuthorizationPolicyName: sAuthorizationPolicyName);
            epBuilder.AddFindPhysicalDimensionByFilterEndpoint(sCorsPolicyName: sCorsPolicyName, sAuthorizationPolicyName: sAuthorizationPolicyName);
            epBuilder.AddFindPhysicalDimensionByIdEndpoint(sCorsPolicyName: sCorsPolicyName, sAuthorizationPolicyName: sAuthorizationPolicyName);
            epBuilder.AddUpdatePhysicalDimensionEndpoint(sCorsPolicyName: sCorsPolicyName, sAuthorizationPolicyName: sAuthorizationPolicyName);
        }

        public static void AddTimePeriodEndpoint(this IEndpointRouteBuilder epBuilder, string sCorsPolicyName, params string[] sAuthorizationPolicyName)
        {
            epBuilder.AddCreateTimePeriodEndpoint(sCorsPolicyName: sCorsPolicyName, sAuthorizationPolicyName: sAuthorizationPolicyName);
            epBuilder.AddDeleteTimePeriodEndpoint(sCorsPolicyName: sCorsPolicyName, sAuthorizationPolicyName: sAuthorizationPolicyName);
            epBuilder.AddFindTimePeriodByFilterEndpoint(sCorsPolicyName: sCorsPolicyName, sAuthorizationPolicyName: sAuthorizationPolicyName);
            epBuilder.AddFindTimePeriodByIdEndpoint(sCorsPolicyName: sCorsPolicyName, sAuthorizationPolicyName: sAuthorizationPolicyName);
            epBuilder.AddUpdateTimePeriodEndpoint(sCorsPolicyName: sCorsPolicyName, sAuthorizationPolicyName: sAuthorizationPolicyName);
        }
    }
}