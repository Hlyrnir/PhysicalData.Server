using Mediator;
using Passport.Abstraction.Result;
using Passport.Api;
using Passport.Application.Default;
using PhysicalData.Application.Command.PhysicalDimension.Update;
using PhysicalData.Contract.v01.Request.PhysicalDimension;

namespace PhysicalData.Api.Endpoint.PhysicalDimension
{
    public static class UpdatePhysicalDimensionEndpoint
    {
        public const string Name = "UpdatePhysicalDimension";

        public static void AddUpdatePhysicalDimensionEndpoint(this IEndpointRouteBuilder epBuilder, string sCorsPolicyName, params string[] sAuthorizationPolicyName)
        {
            epBuilder.MapPut(
                EndpointRoute.PhysicalDimension.Update, UpdatePhysicalDimension)
                .RequireCors(sCorsPolicyName)
                .RequireAuthorization(sAuthorizationPolicyName)
                .WithName(Name)
                .WithTags("PhysicalDimension")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces<bool>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(EndpointVersion.VersionSet!)
                .HasApiVersion(1.0);
        }

        public static async Task<IResult> UpdatePhysicalDimension(
            UpdatePhysicalDimensionRequest rqstPhysicalDimension,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            UpdatePhysicalDimensionCommand cmdUpdate = rqstPhysicalDimension.MapToCommand(guPassportId);

            IMessageResult<bool> mdtResult = await mdtMediator.Send(cmdUpdate, tknCancellation);

            return mdtResult.Match(
                msgError =>
                {
                    if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
                        return Results.Forbid();

                    return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
                },
                bResult => TypedResults.Ok(bResult));
        }

        private static UpdatePhysicalDimensionCommand MapToCommand(this UpdatePhysicalDimensionRequest rqstPhysicalDimension, Guid guPassportId)
        {
            return new UpdatePhysicalDimensionCommand()
            {
                ConcurrencyStamp = rqstPhysicalDimension.ConcurrencyStamp,
                ConversionFactorToSI = rqstPhysicalDimension.ConversionFactorToSI,
                CultureName = rqstPhysicalDimension.CultureName,
                ExponentOfAmpere = rqstPhysicalDimension.ExponentOfAmpere,
                ExponentOfCandela = rqstPhysicalDimension.ExponentOfCandela,
                ExponentOfKelvin = rqstPhysicalDimension.ExponentOfKelvin,
                ExponentOfKilogram = rqstPhysicalDimension.ExponentOfKilogram,
                ExponentOfMetre = rqstPhysicalDimension.ExponentOfMetre,
                ExponentOfMole = rqstPhysicalDimension.ExponentOfMole,
                ExponentOfSecond = rqstPhysicalDimension.ExponentOfSecond,
                PhysicalDimensionId = rqstPhysicalDimension.PhysicalDimensionId,
                Name = rqstPhysicalDimension.Name,
                Symbol = rqstPhysicalDimension.Symbol,
                Unit = rqstPhysicalDimension.Unit,
                RestrictedPassportId = guPassportId
            };
        }
    }
}