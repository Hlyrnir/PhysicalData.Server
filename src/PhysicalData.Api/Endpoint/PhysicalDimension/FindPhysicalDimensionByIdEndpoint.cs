using Mediator;
using Passport.Abstraction.Result;
using Passport.Api;
using Passport.Application.Default;
using PhysicalData.Application.Query.PhysicalDimension.ById;
using PhysicalData.Contract.v01.Response.PhysicalDimension;

namespace PhysicalData.Api.Endpoint.PhysicalDimension
{
    public static class FindPhysicalDimensionByIdEndpoint
    {
        public const string Name = "FindPhysicalDimensionById";

        public static void AddFindPhysicalDimensionByIdEndpoint(this IEndpointRouteBuilder epBuilder, params string[] sPolicyName)
        {
            epBuilder.MapGet(
                EndpointRoute.PhysicalDimension.GetById, FindPhysicalDimensionById)
                .RequireAuthorization(sPolicyName)
                .WithName(Name)
                .WithTags("PhysicalDimension")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces<PhysicalDimensionByIdResponse>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(EndpointVersion.VersionSet!)
                .HasApiVersion(1.0);
        }

        public static async Task<IResult> FindPhysicalDimensionById(
            Guid guId,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            PhysicalDimensionByIdQuery qryGetById = MapToQuery(guId, guPassportId);

            IMessageResult<PhysicalDimensionByIdResult> mdtResult = await mdtMediator.Send(qryGetById, tknCancellation);

            return mdtResult.Match(
                msgError =>
                {
                    if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
                        return Results.Forbid();

                    return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
                },
                rsltPhysicalDimension => TypedResults.Ok(rsltPhysicalDimension.MapToResponse()));
        }

        private static PhysicalDimensionByIdQuery MapToQuery(Guid guPhysicalDimensionId, Guid guPassportId)
        {
            return new PhysicalDimensionByIdQuery()
            {
                PhysicalDimensionId = guPhysicalDimensionId,
                RestrictedPassportId = guPassportId
            };
        }

        private static PhysicalDimensionByIdResponse MapToResponse(this PhysicalDimensionByIdResult rsltPhysicalDimension)
        {
            return new PhysicalDimensionByIdResponse()
            {
                ConcurrencyStamp = rsltPhysicalDimension.PhysicalDimension.ConcurrencyStamp,
                ConversionFactorToSI = rsltPhysicalDimension.PhysicalDimension.ConversionFactorToSI,
                CultureName = rsltPhysicalDimension.PhysicalDimension.CultureName,
                ExponentOfAmpere = rsltPhysicalDimension.PhysicalDimension.ExponentOfAmpere,
                ExponentOfCandela = rsltPhysicalDimension.PhysicalDimension.ExponentOfCandela,
                ExponentOfKelvin = rsltPhysicalDimension.PhysicalDimension.ExponentOfKelvin,
                ExponentOfKilogram = rsltPhysicalDimension.PhysicalDimension.ExponentOfKilogram,
                ExponentOfMetre = rsltPhysicalDimension.PhysicalDimension.ExponentOfMetre,
                ExponentOfMole = rsltPhysicalDimension.PhysicalDimension.ExponentOfMole,
                ExponentOfSecond = rsltPhysicalDimension.PhysicalDimension.ExponentOfSecond,
                Id = rsltPhysicalDimension.PhysicalDimension.Id,
                Name = rsltPhysicalDimension.PhysicalDimension.Name,
                Symbol = rsltPhysicalDimension.PhysicalDimension.Symbol,
                Unit = rsltPhysicalDimension.PhysicalDimension.Unit
            };
        }
    }
}
