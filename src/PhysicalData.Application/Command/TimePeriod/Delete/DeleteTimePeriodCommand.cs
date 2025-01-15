using Mediator;
using Passport.Abstraction.Authorization;
using Passport.Abstraction.Result;

namespace PhysicalData.Application.Command.TimePeriod.Delete
{
    public sealed class DeleteTimePeriodCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization
    {
        public required Guid TimePeriodId { get; init; }

        public required Guid RestrictedPassportId { get; init; }
    }
}
