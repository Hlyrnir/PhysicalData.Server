using Mediator;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;
using PhysicalData.Application.Filter;

namespace PhysicalData.Application.Query.TimePeriod.ByFilter
{
    public sealed class TimePeriodByFilterQuery : IQuery<IMessageResult<TimePeriodByFilterResult>>, IRestrictedAuthorization
    {
        public required TimePeriodFilterOption Filter { get; init; }

        public required Guid RestrictedPassportId { get; init; }
    }
}
