using Mediator;
using Passport.Abstraction.Result;
using Passport.Api;
using Passport.Application.Default;
using PhysicalData.Application.Filter;
using PhysicalData.Application.Query.PhysicalDimension.ByFilter;
using PhysicalData.Application.Transfer;
using PhysicalData.Contract.v01.Request.PhysicalDimension;
using PhysicalData.Contract.v01.Response.PhysicalDimension;

namespace PhysicalData.Api.Endpoint.PhysicalDimension
{
    public static class FindPhysicalDimensionByFilterEndpoint
    {
        public const string Name = "FindPhysicalDimensionByFilter";

        public static void AddFindPhysicalDimensionByFilterEndpoint(this IEndpointRouteBuilder epBuilder, string sCorsPolicyName, params string[] sAuthorizationPolicyName)
        {
            epBuilder.MapGet(
                EndpointRoute.PhysicalDimension.GetByFilter, FindPhysicalDimensionByFilter)
                .RequireCors(sCorsPolicyName)
                .RequireAuthorization(sAuthorizationPolicyName)
                .WithName(Name)
                .WithTags("PhysicalDimension")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces<PhysicalDimensionByFilterResponse>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(EndpointVersion.VersionSet!)
                .HasApiVersion(1.0);
        }

        private static async Task<IResult> FindPhysicalDimensionByFilter(
            [AsParameters] FindPhysicalDimensionByFilterRequest rqstPhysicalDimension,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            PhysicalDimensionByFilterQuery qryFindByFilter = rqstPhysicalDimension.MapToQuery(guPassportId);

            IMessageResult<PhysicalDimensionByFilterResult> mdtResult = await mdtMediator.Send(qryFindByFilter, tknCancellation);

            return mdtResult.Match(
                msgError =>
                {
                    if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
                        return Results.Forbid();

                    return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
                },
                rsltPhysicalDimension => TypedResults.Ok(rsltPhysicalDimension.MapToResponse(
                    iPage: qryFindByFilter.Filter.Page,
                    iPageSize: qryFindByFilter.Filter.PageSize)));
        }

        private static PhysicalDimensionByFilterQuery MapToQuery(this FindPhysicalDimensionByFilterRequest rqstPhysicalDimension, Guid guPassportId)
        {
            return new PhysicalDimensionByFilterQuery()
            {
                Filter = new PhysicalDimensionFilterOption
                {
                    ConversionFactorToSI = rqstPhysicalDimension.ConversionFactorToSI,
                    CultureName = rqstPhysicalDimension.CultureName,
                    ExponentOfAmpere = rqstPhysicalDimension.ExponentOfAmpere,
                    ExponentOfCandela = rqstPhysicalDimension.ExponentOfCandela,
                    ExponentOfKelvin = rqstPhysicalDimension.ExponentOfKelvin,
                    ExponentOfKilogram = rqstPhysicalDimension.ExponentOfKilogram,
                    ExponentOfMetre = rqstPhysicalDimension.ExponentOfMetre,
                    ExponentOfMole = rqstPhysicalDimension.ExponentOfMole,
                    ExponentOfSecond = rqstPhysicalDimension.ExponentOfSecond,
                    Name = rqstPhysicalDimension.Name,
                    Symbol = rqstPhysicalDimension.Symbol,
                    Unit = rqstPhysicalDimension.Unit,

                    Page = rqstPhysicalDimension.Page,
                    PageSize = rqstPhysicalDimension.PageSize
                },
                RestrictedPassportId = guPassportId
            };
        }

        private static PhysicalDimensionByFilterResponse MapToResponse(this PhysicalDimensionByFilterResult rsltPhysicalDimension, int iPage, int iPageSize)
        {
            return new PhysicalDimensionByFilterResponse()
            {
                PhysicalDimension = rsltPhysicalDimension.PhysicalDimension.MapToResponse(),
                Page = iPage,
                PageSize = iPageSize,
                ResultCount = rsltPhysicalDimension.MaximalNumberOfPhysicalDimension
            };
        }

        public static IEnumerable<PhysicalDimensionByIdResponse> MapToResponse(this IEnumerable<PhysicalDimensionTransferObject> enumPhysicalDimension)
        {
            foreach (PhysicalDimensionTransferObject dtoPhysicalDimension in enumPhysicalDimension)
            {
                yield return new PhysicalDimensionByIdResponse()
                {
                    ConcurrencyStamp = dtoPhysicalDimension.ConcurrencyStamp,
                    ConversionFactorToSI = dtoPhysicalDimension.ConversionFactorToSI,
                    CultureName = dtoPhysicalDimension.CultureName,
                    ExponentOfAmpere = dtoPhysicalDimension.ExponentOfAmpere,
                    ExponentOfCandela = dtoPhysicalDimension.ExponentOfCandela,
                    ExponentOfKelvin = dtoPhysicalDimension.ExponentOfKelvin,
                    ExponentOfKilogram = dtoPhysicalDimension.ExponentOfKilogram,
                    ExponentOfMetre = dtoPhysicalDimension.ExponentOfMetre,
                    ExponentOfMole = dtoPhysicalDimension.ExponentOfMole,
                    ExponentOfSecond = dtoPhysicalDimension.ExponentOfSecond,
                    Id = dtoPhysicalDimension.Id,
                    Name = dtoPhysicalDimension.Name,
                    Symbol = dtoPhysicalDimension.Symbol,
                    Unit = dtoPhysicalDimension.Unit
                };
            }
        }
    }
}
