﻿using Mediator;
using Passport.Abstraction.Result;
using Passport.Api;
using Passport.Application.Default;
using PhysicalData.Application.Query.TimePeriod.ById;
using PhysicalData.Contract.v01.Response.TimePeriod;

namespace PhysicalData.Api.Endpoint.TimePeriod
{
    public static class FindTimePeriodByIdEndpoint
    {
        public const string Name = "FindTimePeriodById";

        public static void AddFindTimePeriodByIdEndpoint(this IEndpointRouteBuilder epBuilder, params string[] sPolicyName)
        {
            epBuilder.MapGet(
                EndpointRoute.TimePeriod.GetById, FindTimePeriodById)
                .RequireAuthorization(sPolicyName)
                .WithName(Name)
                .WithTags("TimePeriod")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces<TimePeriodByIdResponse>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(EndpointVersion.VersionSet!)
                .HasApiVersion(1.0);
        }

        public static async Task<IResult> FindTimePeriodById(
            Guid guId,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            TimePeriodByIdQuery qryPhysicalDimension = MapToQuery(guId, guPassportId);

            IMessageResult<TimePeriodByIdResult> mdtResult = await mdtMediator.Send(qryPhysicalDimension, tknCancellation);

            return mdtResult.Match(
                msgError =>
                {
                    if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
                        return Results.Forbid();

                    return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
                },
                rsltTimePeriod => TypedResults.Ok(rsltTimePeriod.MapToResponse()));
        }

        private static TimePeriodByIdQuery MapToQuery(Guid guTimePeriodId, Guid guPassportId)
        {
            return new TimePeriodByIdQuery()
            {
                RestrictedPassportId = guPassportId,
                TimePeriodId = guTimePeriodId
            };
        }

        private static TimePeriodByIdResponse MapToResponse(this TimePeriodByIdResult rsltTimePeriod)
        {
            return new TimePeriodByIdResponse()
            {
                ConcurrencyStamp = rsltTimePeriod.TimePeriod.ConcurrencyStamp,
                Id = rsltTimePeriod.TimePeriod.Id,
                Magnitude = rsltTimePeriod.TimePeriod.Magnitude,
                Offset = rsltTimePeriod.TimePeriod.Offset,
                PhysicalDimensionId = rsltTimePeriod.TimePeriod.PhysicalDimensionId
            };
        }
    }
}
