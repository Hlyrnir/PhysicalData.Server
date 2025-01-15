using Mediator;
using Passport.Abstraction.Result;
using Passport.Api;
using Passport.Application.Default;
using PhysicalData.Application.Command.PhysicalDimension.Create;
using PhysicalData.Contract.v01.Request.PhysicalDimension;
using PhysicalData.Contract.v01.Response.PhysicalDimension;

namespace PhysicalData.Api.Endpoint.PhysicalDimension
{
    public static class CreatePhysicalDimensionEndpoint
    {
        public const string Name = "CreatePhysicalDimension";

        public static void AddCreatePhysicalDimensionEndpoint(this IEndpointRouteBuilder epBuilder, params string[] sPolicyName)
        {
            epBuilder.MapPost(
                EndpointRoute.PhysicalDimension.Create, CreatePhysicalDimension)
                .RequireAuthorization(sPolicyName)
                .WithName(Name)
                .WithTags("PhysicalDimension")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces<PhysicalDimensionByIdResponse>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(EndpointVersion.VersionSet!)
                .HasApiVersion(1.0);
        }

        public static async Task<IResult> CreatePhysicalDimension(
            CreatePhysicalDimensionRequest rqstPhysicalDimension,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            CreatePhysicalDimensionCommand cmdInsert = rqstPhysicalDimension.MapToCommand(guPassportId);

            IMessageResult<Guid> mdtResult = await mdtMediator.Send(cmdInsert, tknCancellation);

            return mdtResult.Match(
                msgError =>
                {
                    if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
                        return Results.Forbid();

                    return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
                },
                guPhysicalDimensionId => TypedResults.CreatedAtRoute(FindPhysicalDimensionByIdEndpoint.Name, new { guId = guPhysicalDimensionId }));
        }

        private static CreatePhysicalDimensionCommand MapToCommand(this CreatePhysicalDimensionRequest cmdRequest, Guid guPassportId)
        {
            return new CreatePhysicalDimensionCommand()
            {
                ConversionFactorToSI = cmdRequest.ConversionFactorToSI,
                CultureName = cmdRequest.CultureName,
                ExponentOfAmpere = cmdRequest.ExponentOfAmpere,
                ExponentOfCandela = cmdRequest.ExponentOfCandela,
                ExponentOfKelvin = cmdRequest.ExponentOfKelvin,
                ExponentOfKilogram = cmdRequest.ExponentOfKilogram,
                ExponentOfMetre = cmdRequest.ExponentOfMetre,
                ExponentOfMole = cmdRequest.ExponentOfMole,
                ExponentOfSecond = cmdRequest.ExponentOfSecond,
                Name = cmdRequest.Name,
                Symbol = cmdRequest.Symbol,
                Unit = cmdRequest.Unit,
                RestrictedPassportId = guPassportId
            };
        }
    }
}
