using Mediator;
using Passport.Abstraction.Result;
using Passport.Api;
using Passport.Application.Default;
using PhysicalData.Application.Command.TimePeriod.Create;
using PhysicalData.Contract.v01.Request.TimePeriod;
using PhysicalData.Contract.v01.Response.TimePeriod;

namespace PhysicalData.Api.Endpoint.TimePeriod
{
    public static class CreateTimePeriodEndpoint
    {
        public const string Name = "CreateTimePeriod";

        public static void AddCreateTimePeriodEndpoint(this IEndpointRouteBuilder epBuilder, params string[] sPolicyName)
        {
            epBuilder.MapPost(
                EndpointRoute.TimePeriod.Create, CreateTimePeriod)
                .RequireAuthorization(sPolicyName)
                .WithName(Name)
                .WithTags("TimePeriod")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces<TimePeriodByIdResponse>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(EndpointVersion.VersionSet!)
                .HasApiVersion(1.0);
        }

        public static async Task<IResult> CreateTimePeriod(
            CreateTimePeriodRequest rqstTimePeriod,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            CreateTimePeriodCommand cmdInsert = rqstTimePeriod.MapToCommand(guPassportId);

            IMessageResult<Guid> mdtResult = await mdtMediator.Send(cmdInsert, tknCancellation);

            return mdtResult.Match(
                msgError =>
                {
                    if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
                        return Results.Forbid();

                    return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
                },
                guTimePeriodId => TypedResults.CreatedAtRoute(FindTimePeriodByIdEndpoint.Name, new { guId = guTimePeriodId }));
        }

        private static CreateTimePeriodCommand MapToCommand(this CreateTimePeriodRequest cmdRequest, Guid guPassportId)
        {
            return new CreateTimePeriodCommand()
            {
                Magnitude = cmdRequest.Magnitude,
                Offset = cmdRequest.Offset,
                PhysicalDimensionId = cmdRequest.PhysicalDimensionId,
                RestrictedPassportId = guPassportId
            };
        }
    }
}
