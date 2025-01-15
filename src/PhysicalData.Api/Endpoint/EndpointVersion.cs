using Asp.Versioning;
using Asp.Versioning.Builder;

namespace PhysicalData.Api.Endpoint
{
    public static class EndpointVersion
    {
        public static ApiVersionSet? VersionSet { get; private set; }

        public static IEndpointRouteBuilder AddPhysicalDataEndpointVersionSet(this IEndpointRouteBuilder epBuilder)
        {
            VersionSet = epBuilder.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1.0))
                .ReportApiVersions()
                .Build();

            return epBuilder;
        }
    }
}
