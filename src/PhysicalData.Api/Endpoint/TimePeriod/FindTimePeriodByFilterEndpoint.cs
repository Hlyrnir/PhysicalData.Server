using Mediator;
using Passport.Abstraction.Result;
using Passport.Api;
using Passport.Application.Default;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Query.TimePeriod.ByFilter;
using PhysicalData.Application.Transfer;
using PhysicalData.Contract.v01.Request.TimePeriod;
using PhysicalData.Contract.v01.Response.TimePeriod;

namespace PhysicalData.Api.Endpoint.TimePeriod
{
    public static class FindTimePeriodByFilterEndpoint
    {
        public const string Name = "FindTimePeriodByFilter";

        public static void AddFindTimePeriodByFilterEndpoint(this IEndpointRouteBuilder epBuilder, string sCorsPolicyName, params string[] sAuthorizationPolicyName)
        {
            epBuilder.MapGet(
                EndpointRoute.TimePeriod.GetByFilter, FindTimePeriodByFilter)
                .RequireCors(sCorsPolicyName)
                .RequireAuthorization(sAuthorizationPolicyName)
                .WithName(Name)
                .WithTags("TimePeriod")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces<TimePeriodByFilterResponse>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(EndpointVersion.VersionSet!)
                .HasApiVersion(1.0);
        }

        private static async Task<IResult> FindTimePeriodByFilter(
            [AsParameters] FindTimePeriodByFilterRequest rqstTimePeriod,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            TimePeriodByFilterQuery qryFindByFilter = rqstTimePeriod.MapToQuery(guPassportId);

            IMessageResult<TimePeriodByFilterResult> mdtResult = await mdtMediator.Send(qryFindByFilter, tknCancellation);

            return mdtResult.Match(
                msgError =>
                {
                    if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
                        return Results.Forbid();

                    return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
                },
                rsltTimePeriod => TypedResults.Ok(rsltTimePeriod.MapToResponse(
                    qryFindByFilter.Filter.Page,
                    qryFindByFilter.Filter.PageSize)));
        }

        private static TimePeriodByFilterQuery MapToQuery(this FindTimePeriodByFilterRequest rqstTimePeriod, Guid guPassportId)
        {
            return new TimePeriodByFilterQuery()
            {
                RestrictedPassportId = guPassportId,
                Filter = new TimePeriodFilterOption()
                {
                    PhysicalDimensionId = rqstTimePeriod.PhysicalDimensionId,
                    Magnitude = rqstTimePeriod.Magnitude,
                    Offset = rqstTimePeriod.Offset,

                    Page = rqstTimePeriod.Page,
                    PageSize = rqstTimePeriod.PageSize
                }
            };
        }

        private static TimePeriodByFilterResponse MapToResponse(this TimePeriodByFilterResult rsltTimePeriod, int iPage, int iPageSize)
        {
            return new TimePeriodByFilterResponse()
            {
                TimePeriod = rsltTimePeriod.TimePeriod.MapToResponse(),
                Page = iPage,
                PageSize = iPageSize,
                ResultCount = rsltTimePeriod.MaximalNumberOfTimePeriod
            };
        }

        public static IEnumerable<TimePeriodByIdResponse> MapToResponse(this IEnumerable<TimePeriodTransferObject> enumTimePeriod)
        {
            foreach (TimePeriodTransferObject dtoTimePeriod in enumTimePeriod)
            {
                yield return new TimePeriodByIdResponse()
                {
                    ConcurrencyStamp = dtoTimePeriod.ConcurrencyStamp,
                    Id = dtoTimePeriod.Id,
                    Magnitude = dtoTimePeriod.Magnitude,
                    Offset = dtoTimePeriod.Offset,
                    PhysicalDimensionId = dtoTimePeriod.PhysicalDimensionId
                };
            }
        }
    }
}
