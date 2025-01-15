using Mediator;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;

namespace PhysicalData.Application.Query.PhysicalDimension.ById
{
    public sealed class PhysicalDimensionByIdQuery : IQuery<IMessageResult<PhysicalDimensionByIdResult>>, IRestrictedAuthorization
    {
        public required Guid PhysicalDimensionId { get; init; }

        public required Guid RestrictedPassportId { get; init; }
    }
}
