﻿using Mediator;
using Passport.Abstraction.Result;
using Passport.Api;
using Passport.Application.Default;
using PhysicalData.Application.Command.PhysicalDimension.Delete;

namespace PhysicalData.Api.Endpoint.PhysicalDimension
{
    public static class DeletePhysicalDimensionEndpoint
    {
        public const string Name = "DeletePhysicalDimension";

        public static void AddDeletePhysicalDimensionEndpoint(this IEndpointRouteBuilder epBuilder, params string[] sPolicyName)
        {
            epBuilder.MapDelete(
                EndpointRoute.PhysicalDimension.Delete, DeletePhysicalDimension)
                .RequireAuthorization(sPolicyName)
                .WithName(Name)
                .WithTags("PhysicalDimension")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces<bool>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(EndpointVersion.VersionSet!)
                .HasApiVersion(1.0);
        }

        public static async Task<IResult> DeletePhysicalDimension(
            Guid guPhysicalDimensionId,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            DeletePhysicalDimensionCommand cmdDelete = MapToCommand(guPassportId, guPhysicalDimensionId);

            IMessageResult<bool> mdtResult = await mdtMediator.Send(cmdDelete, tknCancellation);

            return mdtResult.Match(
                msgError =>
                {
                    if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
                        return Results.Forbid();

                    return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
                },
                bResult => Results.Ok(bResult));
        }

        private static DeletePhysicalDimensionCommand MapToCommand(Guid guPassportId, Guid guPhysicalDimensionId)
        {
            return new DeletePhysicalDimensionCommand()
            {
                PhysicalDimensionId = guPhysicalDimensionId,
                RestrictedPassportId = guPassportId
            };
        }
    }
}