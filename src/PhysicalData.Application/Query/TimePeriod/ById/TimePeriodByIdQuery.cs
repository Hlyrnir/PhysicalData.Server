using Mediator;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;

namespace PhysicalData.Application.Query.TimePeriod.ById
{
    public sealed class TimePeriodByIdQuery : IQuery<IMessageResult<TimePeriodByIdResult>>, IRestrictedAuthorization
    {
        public required Guid TimePeriodId { get; init; }

        public required Guid RestrictedPassportId { get; init; }
    }
}
