using Mediator;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Filter;

namespace PhysicalData.Application.Query.PhysicalDimension.ByFilter
{
    public sealed class PhysicalDimensionByFilterQuery : IQuery<IMessageResult<PhysicalDimensionByFilterResult>>, IRestrictedAuthorization
    {
        public required PhysicalDimensionByFilterOption Filter { get; init; }

        public required Guid RestrictedPassportId { get; init; }
    }
}
